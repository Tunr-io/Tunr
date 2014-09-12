// Copyright (c) Microsoft Corporation
// All rights reserved. 
//
// Licensed under the Apache License, Version 2.0 (the ""License""); you may
// not use this file except in compliance with the License. You may obtain a
// copy of the License at http://www.apache.org/licenses/LICENSE-2.0 
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT. 
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Music.Platform.Contract.AuthenticationDataModel
{
    public interface IXToken
    {
        /// <summary>
        /// This is what you need to use as value for the HTTP Authorization header when identifying users.
        /// </summary>
        string AuthorizationHeaderValue { get; set; }

        /// <summary>
        /// The token is invalid after this date.
        /// </summary>
        DateTime NotAfter { get; set; }

        /// <summary>
        /// The token was created at this date.
        /// </summary>
        DateTime IssueInstant { get; set; }

        /// <summary>
        /// Refresh the token.
        /// This is intended to be used mainly to react to authentication errors due to token expiry or revocation.
        /// It is recommended to run this proactively only if <see cref="NotAfter"/> &lt; <see cref="DateTime.UtcNow"/> .
        /// It is recommended not to throw exceptions on authentication errors when implementing this method.
        /// </summary>
        /// <returns>True if the token was refreshed succesfully.</returns>
        Task<bool> RefreshAsync(CancellationToken cancellationToken);
    }
}
