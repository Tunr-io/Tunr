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
using System.Runtime.Serialization;

namespace Microsoft.Xbox.Music.Platform.Contract.DataModel
{
    [DataContract(Namespace = Constants.Xmlns)]
    public class Album : Content
    {
        // These items are available when this is the main element of the query or if an extra details parameter has been specified for that sub-element
        [DataMember(EmitDefaultValue = false)]
        public DateTime? ReleaseDate { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public TimeSpan? Duration { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? TrackCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool? IsExplicit { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LabelName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GenreList Genres { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GenreList Subgenres { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string AlbumType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Subtitle { get; set; }

        // The following sub-element will be provided with just the minimal stuff unless an extra details parameter is specified for additional sub-element details
        [DataMember(EmitDefaultValue = false)]
        public List<Contributor> Artists { get; set; }

        // The following list require a specific extra details parameter, otherwise it will be null
        [DataMember(EmitDefaultValue = false)]
        public PaginatedList<Track> Tracks { get; set; }

        public Album ShallowCopy()
        {
            return (Album)this.MemberwiseClone();
        }
    }
}
