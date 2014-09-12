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

namespace Microsoft.Xbox.Music.Platform.Contract.DataModel
{
    [DataContract(Namespace = Constants.Xmlns)]
    public class UserProfileResponse : BaseResponse
    {
        /// <summary>
        /// If the request is authenticated, provides the user's subscription state.
        /// Will be null if the request is unauthenticated.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool? HasSubscription { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public bool IsSubscriptionAvailableForPurchase { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Culture { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CollectionState Collection { get; set; }
    }
}
