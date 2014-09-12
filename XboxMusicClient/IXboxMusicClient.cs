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
using System.Threading.Tasks;
using Microsoft.Xbox.Music.Platform.Contract.AuthenticationDataModel;
using Microsoft.Xbox.Music.Platform.Contract.DataModel;
using Microsoft.Xbox.Music.Platform.Contract.DataModel.CollectionEdit;

namespace Microsoft.Xbox.Music.Platform.Client
{
    /// <summary>
    /// Client interface for the Xbox Music API. See http://music.xbox.com/developer
    /// </summary>
    public interface IXboxMusicClient : IDisposable
    {
        /// <summary>
        /// Timeout applied to all backend service calls.
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Access to user authenticated requests is restricted under the terms of the Xbox Music API Pilot program (http://music.xbox.com/developer/pilot).
        /// </summary>
        IXToken XToken { get; }

        /// <summary>
        /// Get an instance of a user specific API client.
        /// Access to user authenticated requests is restricted under the terms of the Xbox Music API Pilot program (http://music.xbox.com/developer/pilot).
        /// </summary>
        /// <param name="xToken">The user's authentication token.</param>
        /// <returns>The corresponding API client.</returns>
        IXboxMusicClient CreateUserAuthenticatedClient(IXToken xToken);

        /// <summary>
        /// Performs a media search.
        /// </summary>
        /// <param name="mediaNamespace">"music" only for now.</param>
        /// <param name="query">Query string to search.</param>
        /// <param name="source">The content source: Catalog, Collection or both</param>
        /// <param name="filter">Filters the response items. Can be "Artists", "Albums", "Tracks" or any combination.</param>
        /// <param name="language">ISO 2 letter code.</param>
        /// <param name="country">ISO 2 letter code.</param>
        /// <param name="maxItems">Max items per category in the response, between 1 and 25. Default value is 25.</param>
        /// <returns>Content response with lists of media items </returns>
        Task<ContentResponse> SearchAsync(Namespace mediaNamespace, string query, ContentSource? source = null, SearchFilter filter = SearchFilter.Default,
            string language = null, string country = null, int? maxItems = null);


        /// <summary>
        /// Request the continuation of an incomplete list of content from the service.
        /// </summary>
        /// <param name="mediaNamespace">Must be the same as in the original request.</param>
        /// <param name="continuationToken">A Continuation Token provided in an earlier service response.</param>
        /// <returns>Content response with lists of media items.</returns>
        Task<ContentResponse> SearchContinuationAsync(Namespace mediaNamespace, string continuationToken);

        /// <summary>
        /// Lookup an item and get details about it.
        /// </summary>
        /// <param name="itemIds">Ids to look up, each of which is prefixed by a namespace: {namespaces.id}.</param>
        /// <param name="source">The content source: Catalog, Collection or both</param>
        /// <param name="language">ISO 2 letter code.</param>
        /// <param name="country">ISO 2 letter code.</param>
        /// <param name="extras">Enumeration of extra details.</param>
        /// <returns>Content response with details about one or more items.</returns>
        Task<ContentResponse> LookupAsync(List<string>itemIds, ContentSource? source = null, string language = null,
            string country = null, ExtraDetails extras = ExtraDetails.None);

        /// <summary>
        /// Request the continuation of an incomplete list of content from the service. The relative URL (i.e. the ids list) must be the same as in the original request.
        /// </summary>
        /// <param name="itemIds">Ids to look up, each of which is prefixed by a namespace: {namespaces.id}.</param>
        /// <param name="continuationToken">A Continuation Token provided in an earlier service response.</param>
        /// <returns>Content response with details about one or more items.</returns>
        Task<ContentResponse> LookupContinuationAsync(List<string>itemIds, string continuationToken);

        /// <summary>
        /// Browse the catalog or your collection
        /// </summary>
        /// <param name="mediaNamespace">"music" only for now.</param>
        /// <param name="source">A ContentSource value.</param>
        /// <param name="type">The item type you want to browse</param>
        /// <param name="genre">Filter to a specific genre.</param>
        /// <param name="orderBy">Specify how results are ordered.</param>
        /// <param name="maxItems">Max items per category in the response, between 1 and 25. Default value is 25.</param>
        /// <param name="page">Go directly to a given page. Page size is maxItems.</param>
        /// <param name="country">ISO 2 letter code.</param>
        /// <param name="language">ISO 2 letter code.</param>
        /// <returns>Content response with the items corresponding to the browse request.</returns>
        Task<ContentResponse> BrowseAsync(Namespace mediaNamespace, ContentSource source, ItemType type,
            string genre = null, OrderBy? orderBy = null, int? maxItems = null, int? page = null,
            string language = null, string country = null);

        /// <summary>
        /// Request the continuation of an incomplete browse response. The relative URL (i.e. mediaNamespace, source and type) must be the same as in the original request.
        /// </summary>
        /// <param name="mediaNamespace">"music" only for now.</param>
        /// <param name="source">A ContentSource value.</param>
        /// <param name="type">The item type you want to browse</param>
        /// <param name="continuationToken">A Continuation Token provided in an earlier service response.</param>
        /// <returns>Content response with the items corresponding to the browse request.</returns>
        Task<ContentResponse> BrowseContinuationAsync(Namespace mediaNamespace, ContentSource source, ItemType type,
            string continuationToken);

        /// <summary>
        /// Browse sub elements of the catalog or your collection
        /// </summary>
        /// <param name="id">Id of the parent item to browse.</param>
        /// <param name="source">A ContentSource value. Only Collection for now</param>
        /// <param name="browseType">The item type you want to browse</param>
        /// <param name="extra">The extra details to browse.</param>
        /// <param name="orderBy">Specify how results are ordered.</param>
        /// <param name="maxItems">Max items per category in the response, between 1 and 25. Default value is 25.</param>
        /// <param name="page">Go directly to a given page. Page size is maxItems.</param>
        /// <param name="language">ISO 2 letter code.</param>
        /// <param name="country">ISO 2 letter code.</param>
        /// <returns>Content response with the items corresponding to the sub-browse request.</returns>
        Task<ContentResponse> SubBrowseAsync(string id, ContentSource source, BrowseItemType browseType, ExtraDetails extra,
            OrderBy? orderBy = null, int? maxItems = null, int? page = null, string language = null,
            string country = null);

        /// <summary>
        /// Request the continuation of an incomplete sub browse
        /// </summary>
        /// <param name="id">Id of the parent item to browse.</param>
        /// <param name="source">A ContentSource value. Only Collection for now</param>
        /// <param name="browseType">The item type you want to browse</param>
        /// <param name="extra">The extra details to browse.</param>
        /// <param name="continuationToken">A Continuation Token provided in an earlier service response.</param>
        /// <returns>Content response with the items corresponding to the sub-browse request.</returns>
        Task<ContentResponse> SubBrowseContinuationAsync(string id, ContentSource source, BrowseItemType browseType,
            ExtraDetails extra, string continuationToken);


        /// <summary>
        /// Get spotlight items of the moment
        /// </summary>
        /// <param name="mediaNamespace">"music" only for now.</param>
        /// <param name="language">ISO 2 letter code.</param>
        /// <param name="country">ISO 2 letter code.</param>
        /// <returns>Content response with spotlight items in order in the Results element of the response.</returns>
        Task<ContentResponse> SpotlightAsync(Namespace mediaNamespace, string language = null,
            string country = null);

        /// <summary>
        /// Get new releases of the moment.
        /// </summary>
        /// <param name="mediaNamespace">"music" only for now.</param>
        /// <param name="genre">A valid genre coherent with the locale to get specific new releases. If null, new releases are from all genres.</param>
        /// <param name="language">ISO 2 letter code.</param>
        /// <param name="country">ISO 2 letter code.</param>
        /// <returns>Content response with spotlight items in order in the Results element of the response.</returns>
        Task<ContentResponse> NewReleasesAsync(Namespace mediaNamespace,
            string genre = null, string language = null, string country = null);

        /// <summary>
        /// Get a list of all posible genres for a given locale
        /// </summary>
        /// <param name="mediaNamespace">"music" only for now</param>
        /// <param name="language">ISO 2 letter code</param>
        /// <param name="country">ISO 2 letter code</param>
        /// <returns>Response.Genres contains the list of genres for your locale</returns>
        Task<ContentResponse> BrowseGenresAsync(Namespace mediaNamespace,
            string language = null, string country = null);

        /// <summary>
        /// Edit your collection
        /// Access to this API is restricted under the terms of the Xbox Music API Pilot program (http://music.xbox.com/developer/pilot).
        /// </summary>
        /// <param name="mediaNamespace">"music" only for now.</param>
        /// <param name="operation">Operation to be done on the collection. Possible values are "add" and "delete".</param>
        /// <param name="trackActionRequest">List of track IDs to be processed.</param>
        /// <returns>List of TrackActionResults corresponding to the result for each track action. This shows which operations did fail and why</returns>
        Task<TrackActionResponse> CollectionOperationAsync(Namespace mediaNamespace, TrackActionType operation,
            TrackActionRequest trackActionRequest);

        /// <summary>
        /// Edit a playlist
        /// Access to this API is restricted under the terms of the Xbox Music API Pilot program (http://music.xbox.com/developer/pilot).
        /// </summary>
        /// <param name="mediaNamespace">"music" only for now.</param>
        /// <param name="operation">Operation to be done on the playlist. Possible values are "create", "update" and "delete".</param>
        /// <param name="playlistAction">Playlist Id and List of TrackActions. A trackAction is a track ID and an operation (add/delete) to apply on the playlist.</param>
        /// <returns>PlaylistActionResult giving details on the playlist action and a list of TrackActionResults corresponding to the result for each track action. This shows which operations did fail and why</returns>
        Task<PlaylistActionResponse> PlaylistOperationAsync(Namespace mediaNamespace, PlaylistActionType operation,
            PlaylistAction playlistAction);

        /// <summary>
        /// Stream a media
        /// Access to this API is restricted under the terms of the Xbox Music API Pilot program (http://music.xbox.com/developer/pilot).
        /// </summary>
        /// <param name="id">Id of the media to be streamed</param>
        /// <param name="clientInstanceId">Client instance Id</param>
        /// <returns>Stream response containing the url, expiration date and content type</returns>
        Task<StreamResponse> StreamAsync(string id, string clientInstanceId);

        /// <summary>
        /// Get a 30s preview of a media
        /// </summary>
        /// <param name="id">Id of the media to be streamed</param>
        /// <param name="clientInstanceId">Client instance Id</param>
        /// <param name="country">ISO 2 letter code.</param>
        /// <returns>Stream response containing the url, expiration date and content type</returns>
        Task<StreamResponse> PreviewAsync(string id, string clientInstanceId, string country = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaNamespace">"music" only for now.</param>
        /// <param name="language">ISO 2 letter code</param>
        /// <param name="country">ISO 2 letter code</param>
        /// <returns></returns>
        Task<UserProfileResponse> GetUserProfileAsync(Namespace mediaNamespace, string language = null, string country = null);
    }
}
