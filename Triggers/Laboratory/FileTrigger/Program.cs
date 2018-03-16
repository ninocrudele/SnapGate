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

namespace FileTrigger
{
    class Program
    {
        public string ReceiveFolder { get; set; }
        public string FileMask { get; set; }
        public bool RenameFileWhileReading { get; set; }
        public bool RenameFileAfterSent { get; set; }
        public string RenameFileAfterSentPattern { get; set; }
        public int PollingInterval { get; set; }
        public string RetryCountOnNetworkFailure { get; set; }
        public int RetryIntervalOnNetworkFailure { get; set; }
        public int RetryCountOnFileLocking { get; set; }
        public int RetryIntervalOnFileLocking { get; set; }
        public int MaxRetryIntervalOnFileLocking { get; set; }
        public string CredentialUsername { get; set; }
        public string CredentialPassword { get; set; }
        public int NumberOfMessagesInBatch { get; set; }
        public int MaximumBatchSizeByte { get; set; }

        static void Main(string[] args)
        {
        }
    }
}