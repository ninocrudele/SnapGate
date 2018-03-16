#region License
//-----------------------------------------------------------------------
// <copyright file="CreationModeEnum.cs" company="Nino Crudele">
//   Copyright (c) Nino Crudele. All Rights Reserved.
// </copyright>
// <license>
//   This work is registered with the UK Copyright Service.
//   Registration No:284695248
//   Licensed under the Reciprocal Public License 1.5 (RPL1.5) 
//   See License.txt in the project root for license information.
// </license>
//-----------------------------------------------------------------------
#endregion

namespace Framework
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Principal;

    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// Wraps Windows API to impersonate a user.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class Impersonation : IDisposable
    {
        /// <summary>
        /// Holds a reference to the handle.
        /// </summary>
        private readonly SafeTokenHandle handle;

        /// <summary>
        /// Holds a reference to the impersonation context.
        /// </summary>
        private readonly WindowsImpersonationContext context;

        /// <summary>
        /// The Windows API code for logon type.
        /// </summary>
        private const int Logon32LogonNewCredentials = 9;

        /// <summary>
        /// Initializes a new instance of the <see cref="Impersonation"/> class.
        /// </summary>
        /// <param name="domain">The domain that the user belongs.</param>
        /// <param name="username">The user name of the login credentials for the identity to impersonate.</param>
        /// <param name="password">The password of the login credentials for the identity to impersonate.</param>
        /// <exception cref="System.ApplicationException">Exception thrown if failed to impersonate a user.</exception>
        public Impersonation(string domain, string username, string password)
        {
            bool ok = LogonUser(username, domain, password,
                           Logon32LogonNewCredentials, 0, out this.handle);
            if (!ok)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(string.Format("Could not impersonate the elevated user.  LogonUser returned error code {0}.", errorCode));
            }

            this.context = WindowsIdentity.Impersonate(this.handle.DangerousGetHandle());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.context.Dispose();
            this.handle.Dispose();
        }

        /// <summary>
        /// Wraps the Windows API to logon user.
        /// </summary>
        /// <param name="lpszUsername">The user name of the identity to log in.</param>
        /// <param name="lpszDomain">The domain of the identity to log in.</param>
        /// <param name="lpszPassword">The password of the identity to log in.</param>
        /// <param name="dwLogonType">Type of the logon.</param>
        /// <param name="dwLogonProvider">The logon provider.</param>
        /// <param name="phToken">The handle to the token.</param>
        /// <returns>bool</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        /// <summary>
        /// Wraps a token handle.
        /// </summary>
        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            /// <summary>
            /// Prevents a default instance of the <see cref="SafeTokenHandle"/> class from being created.
            /// </summary>
            private SafeTokenHandle()
                : base(true)
            { }

            /// <summary>
            /// Closes the handle.
            /// </summary>
            /// <param name="handle">The handle to close.</param>
            /// <returns>pointer</returns>
            [DllImport("kernel32.dll")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);

            /// <summary>
            /// When overridden in a derived class, executes the code required to free the handle.
            /// </summary>
            /// <returns>
            /// <c>true</c> if the handle is released successfully; otherwise, in the event of a catastrophic failure, <c>false</c>. 
            /// In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.
            /// </returns>
            protected override bool ReleaseHandle()
            {
                return CloseHandle(this.handle);
            }
        }
    }
}
