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
using Microsoft.Xbox.Music.Platform.BackendClients.ADM;

namespace Microsoft.Xbox.Music.Platform.Client
{
    /// <summary>
    /// Basic Azure Data Market (http://datamarket.azure.com/) authentication cache
    /// </summary>
    public class AzureDataMarketAuthenticationCache : IDisposable
    {
        public class AccessToken
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        }

        private readonly string clientId;
        private readonly string clientSecret;
        private AccessToken token;

        private readonly AzureDataMarketAuthenticationClient client = new AzureDataMarketAuthenticationClient();

        /// <summary>
        /// Cache an application's authentication token on Azure Data Market
        /// </summary>
        /// <param name="clientId">The application's client ID</param>
        /// <param name="clientSecret">The application's secret</param>
        public AzureDataMarketAuthenticationCache(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        public void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// Get the application's token. Renew it if needed.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AccessToken> CheckAndRenewTokenAsync(CancellationToken cancellationToken)
        {
            if (token == null || token.Expiration < DateTime.UtcNow)
            {
                // This is not thread safe. Unfortunately, portable class library requirements prevent use of
                // asynchronous locking mechanisms. The risk here is authenticating multiple times in parallel
                // which is bad from a performance standpoint but is transparent from a functional standpoint.
                AzureDataMarketAuthenticationResponse authenticationResponse =
                    await client.AuthenticateAsync(clientId, clientSecret, cancellationToken);
                if (authenticationResponse != null)
                {
                    token = new AccessToken
                    {
                        Token = authenticationResponse.AccessToken,
                        Expiration = DateTime.UtcNow.AddSeconds(authenticationResponse.ExpiresIn)
                    };
                }
            }

            return token;
        }
    }
}
