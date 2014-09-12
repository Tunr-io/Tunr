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

using System.Runtime.Serialization;

namespace Microsoft.Xbox.Music.Platform.BackendClients.ADM
{
    [DataContract]
    public class AzureDataMarketAuthenticationResponse
    {
        [DataMember(Name = "token_type", EmitDefaultValue = false)]
        public string TokenType { get; set; }
        [DataMember(Name = "access_token", EmitDefaultValue = false)]
        public string AccessToken { get; set; }
        [DataMember(Name = "expires_in", EmitDefaultValue = false)]
        public double ExpiresIn { get; set; }
        [DataMember(Name = "scope", EmitDefaultValue = false)]
        public string Scope { get; set; }
        [DataMember(Name = "error", EmitDefaultValue = false)]
        public string Error { get; set; }
        [DataMember(Name = "error_description", EmitDefaultValue = false)]
        public string ErrorDescription { get; set; }
    }
}