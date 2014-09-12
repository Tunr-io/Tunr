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
    public class Playlist : Content
    {
        [DataMember(EmitDefaultValue = true)] // TrackCount=0 should be serialized too for empty playlists
        public int TrackCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Owner { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsReadOnly { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsPublished { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool UserIsOwner { get; set; }

        // This sub-element might not be populated in a browse playlists case, but in a lookup it will
        [DataMember(EmitDefaultValue = false)]
        public PaginatedList<Track> Tracks { get; set; }
    }
}
