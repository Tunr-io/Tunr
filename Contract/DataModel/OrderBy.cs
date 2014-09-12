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
    // This enum contains all possible ordering of our backend services
    // However, we don't expose all of them to our end-user
    // In case of functionnally duplicated values (AlbumTitle and AlbumName for example), we will have both but expose only one
    [DataContract(Namespace = Constants.Xmlns)]
    public enum OrderBy : byte
    {
        None = 0,
        [EnumMember]
        AllTimePlayCount,               // Catalog
        [EnumMember]
        ReleaseDate,                    // Catalog & Collection
        [EnumMember]
        ArtistName,                     // Collection's artists, albums, tracks
        [EnumMember]
        AlbumTitle,                     // Collection's albums, tracks
        [EnumMember]
        TrackTitle,                     // Collection's tracks
        [EnumMember]
        GenreName,                      // Collection's albums, tracks
        [EnumMember]
        CollectionDate,                 // Collection : date added to the collection
        [EnumMember]
        TrackNumber,                    // Collection lookup of an album's tracks
        [EnumMember]
        MostPopular                     // Catalog
    }
}
