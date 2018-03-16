// Copyright 2016-2040 Nino Crudele
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SnapGate.Framework.Base;
using Timer = System.Timers.Timer;

#endregion

namespace SnapGate.Framework.Log
{
    public abstract class LockSlimQueueLog<T> : ConcurrentQueue<T>
        where T : class
    {
        /// <summary>
        ///     The cap limit.
        /// </summary>
        protected int CapLimit;

        /// <summary>
        ///     The internal timer.
        /// </summary>
        protected Timer InternalTimer;

        /// <summary>
        ///     The locker.
        /// </summary>
        protected ReaderWriterLockSlim Locker;

        /// <summary>
        ///     The on publish executed.
        /// </summary>
        protected int OnPublishExecuted;

        /// <summary>
        ///     The time limit.
        /// </summary>
        protected int TimeLimit;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LockSlimQueueLog{T}" /> class.
        /// </summary>
        protected LockSlimQueueLog()
        {
        }

        protected LockSlimQueueLog(int capLimit, int timeLimit)
        {
            Init(capLimit, timeLimit);
        }

        public event Action<List<T>> OnPublish = delegate { };

        public new virtual void Enqueue(T item)
        {
            base.Enqueue(item);
            if (Count >= CapLimit)
            {
                Trace.WriteLine($"LOG CAPTURE LIMIT!!!!!: {CapLimit} > PUBLISH!");
                Publish();
            }
        }

        private void Init(int capLimit, int timeLimit)
        {
            CapLimit = capLimit;
            TimeLimit = timeLimit;
            Locker = new ReaderWriterLockSlim();
            InitTimer();
        }

        protected virtual void InitTimer()
        {
            InternalTimer = new Timer();
            InternalTimer.AutoReset = false;
            InternalTimer.Interval = TimeLimit * 1000;
            InternalTimer.Elapsed += (s, e) =>
            {
                //Trace.WriteLine($"Log time limit: {this.TimeLimit} > Publish!");
                Publish();
            };
            InternalTimer.Start();
        }

        /// <summary>
        ///     The publish.
        /// </summary>
        protected virtual void Publish()
        {
            var task = new Task(
                () =>
                {
                    var itemsToLog = new List<T>();
                    try
                    {
                        if (IsPublishing())
                        {
                            return;
                        }

                        StartPublishing();
                        //Trace.WriteLine($"Log start dequeue {this.Count} items.");
                        T item;
                        while (TryDequeue(out item))
                        {
                            itemsToLog.Add(item);
                        }
                    }
                    catch (ThreadAbortException tex)
                    {
                        Trace.WriteLine($"Warning-Dequeue items failed > {tex.Message}");

                        LogEngine.WriteLog(
                            ConfigurationBag.EngineName,
                            $"Error in {MethodBase.GetCurrentMethod().Name}",
                            Constant.LogLevelError,
                            Constant.TaskCategoriesError,
                            tex,
                            Constant.LogLevelError);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Dequeue items failed > {ex.Message}");

                        LogEngine.WriteLog(
                            ConfigurationBag.EngineName,
                            $"Error in {MethodBase.GetCurrentMethod().Name}",
                            Constant.LogLevelError,
                            Constant.TaskCategoriesError,
                            ex,
                            Constant.LogLevelError);
                    }
                    finally
                    {
                        //Trace.WriteLine($"Log dequeued {itemsToLog.Count} items");
                        OnPublish(itemsToLog);
                        CompletePublishing();
                    }
                });
            task.Start();
        }

        private bool IsPublishing()
        {
            return Interlocked.CompareExchange(ref OnPublishExecuted, 1, 0) > 0;
        }

        private void StartPublishing()
        {
            InternalTimer.Stop();
        }

        private void CompletePublishing()
        {
            InternalTimer.Start();
            Interlocked.Decrement(ref OnPublishExecuted);
        }
    }
}