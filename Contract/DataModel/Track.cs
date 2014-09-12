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
    public class Track : Content
    {
        // These items are available when this is the main element of the query or if an extra details parameter has been specified for that sub-element
        [DataMember(EmitDefaultValue = false)]
        public DateTime? ReleaseDate { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public TimeSpan? Duration { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? TrackNumber { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool? IsExplicit { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GenreList Genres { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GenreList Subgenres { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public RightList Rights { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Subtitle { get; set; }

        // This sub-element is null when this Track is queried as a sub-element of an album (to avoid looping), populated with just the minimal stuff by default when this Track is the main element, and extra details can obtained with a details parameter
        [DataMember(EmitDefaultValue = false)]
        public Album Album { get; set; }

        // This sub-element populated with just the minimal stuff by default when this Track is the main element, and extra details can obtained with a details parameter
        [DataMember(EmitDefaultValue = false)]
        public List<Contributor> Artists { get; set; }
    }
}
