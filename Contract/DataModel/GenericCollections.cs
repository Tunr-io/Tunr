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
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Xbox.Music.Platform.Contract.DataModel
{
    /// <summary>
    /// See http://msdn.microsoft.com/en-us/library/aa347850.aspx, paragraph "Customizing Collection Types"
    /// </summary>
    [CollectionDataContract(Namespace = Constants.Xmlns, ItemName = "Genre")]
    public class GenreList : List<string>
    {
        public GenreList()
            : base()
        {
        }

        public GenreList(IEnumerable<string> collection)
            : base(collection)
        {
        }
    }

    [CollectionDataContract(Namespace = Constants.Xmlns, ItemName = "Right")]
    public class RightList : List<string>
    {
        public RightList()
            : base()
        {
        }

        public RightList(IEnumerable<string> collection)
            : base(collection)
        {
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:Mark ISerializable types with SerializableAttribute", Justification = "SerializableAttribute does not exist in Portable .NET")]
    [CollectionDataContract(Namespace = Constants.Xmlns, ItemName = "OtherId", KeyName="Namespace", ValueName="Id")]
    public class IdDictionary : Dictionary<string, string>
    {
        public IdDictionary()
            : base()
        {
        }

        public IdDictionary(IDictionary<string, string> dictionary)
            : base(dictionary)
        {
        }
    }

    [CollectionDataContract(Namespace = Constants.Xmlns, ItemName = "TrackId")]
    public class TrackIdList : List<string>
    {
        public TrackIdList()
            : base()
        {
        }

        public TrackIdList(IEnumerable<string> collection)
            : base(collection)
        {
        }
    }
}
