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
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xbox.Music.Platform.BackendClients.ADM;

namespace Microsoft.Xbox.Music.Platform.Client
{
    /// <summary>
    /// Basic Azure Data Market (http://datamarket.azure.com/) authentication client
    /// </summary>
    public class AzureDataMarketAuthenticationClient : SimpleServiceClient
    {
        private readonly Uri hostname = new Uri("https://datamarket.accesscontrol.windows.net");

        /// <summary>
        /// Authenticate an application on Azure Data Market
        /// </summary>
        /// <param name="clientId">The application's client ID</param>
        /// <param name="clientSecret">The application's secret</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AzureDataMarketAuthenticationResponse> AuthenticateAsync(string clientId, string clientSecret, CancellationToken cancellationToken)
        {
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"scope", "http://music.xboxlive.com/"},
                {"grant_type", "client_credentials"}
            };
            return PostAsync<AzureDataMarketAuthenticationResponse, Dictionary<string, string>>(hostname, "/v2/OAuth2-13", request, cancellationToken);
        }

        protected override HttpContent CreateHttpContent<TRequest>(TRequest requestPayload, StreamWriter writer, MemoryStream stream)
        {
            // We need the url-encoded data for Azure authentication
            return new FormUrlEncodedContent(requestPayload as Dictionary<string, string>);
        }
    }
}
