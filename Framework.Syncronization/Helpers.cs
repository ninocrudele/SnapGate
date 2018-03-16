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
using System.Reflection;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;
using SnapGate.Framework.Base;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.Syncronization
{
    /// <summary>
    ///     Syncronozation Class
    /// </summary>
    public static class Helpers
    {
        public static string syncFile = "SyncronizationStatus.gc";


        /// <summary>
        ///     Syncronize 2 folders
        /// </summary>
        /// <param name="SourceFolder"></param>
        /// <param name="DestinationFolder"></param>
        public static void SyncFolders(string SourceFolder, string DestinationFolder)
        {
            //SyncOrchestrator syncOrchestrator = new SyncOrchestrator();
            //syncOrchestrator.LocalProvider = new FileSyncProvider(Guid.NewGuid(), SourceFolder);
            //syncOrchestrator.RemoteProvider = new FileSyncProvider(Guid.NewGuid(), DestinationFolder);
            ////syncOrchestrator.Direction = SyncDirectionOrder.Upload;

            //syncOrchestrator.Synchronize();


            //FileSyncOptions options = FileSyncOptions.ExplicitDetectChanges |
            //                          FileSyncOptions.RecycleDeletedFiles | FileSyncOptions.RecyclePreviousFileOnUpdates |
            //                          FileSyncOptions.RecycleConflictLoserFiles;


            FileSyncOptions options = FileSyncOptions.ExplicitDetectChanges |
                                      FileSyncOptions.RecyclePreviousFileOnUpdates |
                                      FileSyncOptions.RecycleConflictLoserFiles | FileSyncOptions.RecycleDeletedFiles;
            FileSyncScopeFilter filter = new FileSyncScopeFilter();
            filter.FileNameExcludes.Add("*.metadata");

            // Create file system provider
            FileSyncProvider sourceprovider = new FileSyncProvider(Guid.NewGuid(), SourceFolder, filter, options);
            FileSyncProvider destinationprovider =
                new FileSyncProvider(Guid.NewGuid(), DestinationFolder, filter, options);

            sourceprovider.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.SourceWins;
            destinationprovider.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.SourceWins;

            // Ask providers to detect changes
            sourceprovider.DetectChanges();
            destinationprovider.DetectChanges();

            // Synchronization of 2 Folders
            SyncOrchestrator agent = new SyncOrchestrator();
            agent.LocalProvider = sourceprovider;
            agent.RemoteProvider = destinationprovider;
            agent.Direction = SyncDirectionOrder.Upload;
            agent.Synchronize();
        }


        //Less than zero t1 is earlier than t2.
        //Zero t1 is the same as t2.
        //Greater than zero t1 is later than t2.
        public static bool ToBeSyncronized(string sourceFolder, string restinationFolder, bool syncronize)
        {
            try
            {
                if (syncronize)
                {
                    SyncFolders(sourceFolder, restinationFolder);
                }

                return true;
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                return false;
            }
        }
    }
}