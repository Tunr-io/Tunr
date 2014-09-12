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

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xbox.Music.Platform.Contract.DataModel;

namespace Microsoft.Xbox.Music.Platform.Client
{
    public static class XboxMusicClientExtensions
    {
        /// <summary>
        /// Lookup an item and get details about it.
        /// </summary>
        /// <param name="client">An IXboxMusicClient instance.</param>
        /// <param name="itemId">Id to look up, prefixed by a namespace: {namespace.id}.</param>
        /// <param name="source">The content source: Catalog, Collection or both</param>
        /// <param name="language">ISO 2 letter code.</param>
        /// <param name="country">ISO 2 letter code.</param>
        /// <param name="extras">Enumeration of extra details.</param>
        /// <returns> Content response with details about one or more items.</returns>
        public static Task<ContentResponse> LookupAsync(this IXboxMusicClient client, string itemId, ContentSource? source = null, string language = null,
            string country = null, ExtraDetails extras = ExtraDetails.None)
        {
            return client.LookupAsync(new List<string> { itemId }, source, language, country, extras);
        }

        /// <summary>
        /// Request the continuation of an incomplete list of content from the service. The relative URL (i.e. the ids list) must be the same as in the original request.
        /// </summary>
        /// <param name="client">An IXboxMusicClient instance.</param>
        /// <param name="itemId">Id to look up, prefixed by a namespace: {namespace.id}.</param>
        /// <param name="continuationToken">A Continuation Token provided in an earlier service response.</param>
        /// <returns> Content response with details about one or more items.</returns>
        public static Task<ContentResponse> LookupContinuationAsync(this IXboxMusicClient client, string itemId, string continuationToken)
        {
            return client.LookupContinuationAsync(new List<string> { itemId }, continuationToken);
        }
    }
}
