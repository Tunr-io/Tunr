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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Xbox.Music.Platform.Contract.DataModel
{
    [DataContract(Namespace = Constants.Xmlns)]
    [Flags]
    public enum ContentSource : byte
    {
        Invalid = 0, // to avoid EmitDefaultValue=false "forgetting" the value
        
        [EnumMember]
        Catalog,
        [EnumMember]
        Collection
    }

    [DataContract(Namespace = Constants.Xmlns)]
    public abstract class Content
    {
        // The following 4 properties are always available even as content of a sub-element, not the main element
        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ImageUrl { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Link { get; set; }

        // Other items are available when this is the main element of the query or if an extra details parameter has been specified for that sub-element
        [DataMember(EmitDefaultValue = false)]
        public IdDictionary OtherIds { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentSource Source { get; set; }
    }

}
