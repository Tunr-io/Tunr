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
    public class Artist : Content
    {
        // These items are available when this is the main element of the query or if an extra details parameter has been specified for that sub-element

        [DataMember(EmitDefaultValue = false)]
        public GenreList Genres { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GenreList Subgenres { get; set; }

        // The following lists each require a specific extra details parameter, otherwise they will be null
        [DataMember(EmitDefaultValue = false)]
        public PaginatedList<Album> Albums { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public PaginatedList<Track> TopTracks { get; set; }
    }

    [DataContract(Namespace = Constants.Xmlns)]
    public class Contributor
    {
        public const string MainRole = "Main"; // What EDS shows for the primary artist
        public const string DefaultRole = "Other"; // Choice of a default fallback role name in case we can't find the artist's role in an album/track

        [DataMember(EmitDefaultValue = false)]
        public string Role { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Artist Artist { get; set; }

        public Contributor(string role, Artist artist)
        {
            this.Role = role;
            this.Artist = artist;
        }
    }
}
