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
using SnapGate.Framework.Log;
using Timer = System.Timers.Timer;

#endregion

namespace SnapGate.Framework.Engine
{
    public abstract class LockSlimQueueEngine<BubblingObject> : ConcurrentQueue<BubblingObject>
        where BubblingObject : class
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
        ///     Initializes a new instance of the <see cref="LockSlimQueueEngine{T}" /> class.
        /// </summary>
        protected LockSlimQueueEngine()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LockSlimQueueEngine{T}" /> class.
        /// </summary>
        /// <param name="capLimit">
        ///     TODO The cap limit.
        /// </param>
        /// <param name="timeLimit">
        ///     TODO The time limit.
        /// </param>
        protected LockSlimQueueEngine(int capLimit, int timeLimit)
        {
            Init(capLimit, timeLimit);
        }

        /// <summary>
        ///     TODO The on publish.
        /// </summary>
        public event Action<List<BubblingObject>> OnPublish = delegate { };

        /// <summary>
        ///     TODO The enqueue.
        /// </summary>
        /// <param name="item">
        ///     TODO The item.
        /// </param>
        public new virtual void Enqueue(BubblingObject item)
        {
            base.Enqueue(item);
            if (Count >= CapLimit)
            {
                //4321 Trace.WriteLine($"Queue limit of {CapLimit} raised message published.");
                Publish();
            }
        }

        /// <summary>
        ///     TODO The init.
        /// </summary>
        /// <param name="capLimit">
        ///     TODO The cap limit.
        /// </param>
        /// <param name="timeLimit">
        ///     TODO The time limit.
        /// </param>
        private void Init(int capLimit, int timeLimit)
        {
            CapLimit = capLimit;
            TimeLimit = timeLimit;
            Locker = new ReaderWriterLockSlim();
            InitTimer();
        }

        /// <summary>
        ///     TODO The init timer.
        /// </summary>
        protected virtual void InitTimer()
        {
            InternalTimer = new Timer();
            InternalTimer.AutoReset = false;
            InternalTimer.Interval = TimeLimit * 1000;
            InternalTimer.Elapsed += (s, e) => { Publish(); };
            InternalTimer.Start();
        }

        /// <summary>
        ///     TODO The publish.
        /// </summary>
        protected virtual void Publish()
        {
            var task = new Task(
                () =>
                {
                    var itemsToPublish = new List<BubblingObject>();
                    try
                    {
                        if (IsPublishing())
                        {
                            return;
                        }

                        StartPublishing();
                        //Trace.WriteLine($"Log start dequeue {this.Count} items!");
                        BubblingObject item;
                        while (TryDequeue(out item))
                        {
                            itemsToPublish.Add(item);
                        }
                    }
                    catch (ThreadAbortException tex)
                    {
                        Trace.WriteLine($"Dequeue items failed > {tex.Message}");

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
                        OnPublish(itemsToPublish);
                        CompletePublishing();
                    }
                });
            task.Start();
        }

        /// <summary>
        ///     TODO The is publishing.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool IsPublishing()
        {
            return Interlocked.CompareExchange(ref OnPublishExecuted, 1, 0) > 0;
        }

        /// <summary>
        ///     TODO The start publishing.
        /// </summary>
        private void StartPublishing()
        {
            InternalTimer.Stop();
        }

        /// <summary>
        ///     TODO The complete publishing.
        /// </summary>
        private void CompletePublishing()
        {
            InternalTimer.Start();
            Interlocked.Decrement(ref OnPublishExecuted);
        }
    }
}