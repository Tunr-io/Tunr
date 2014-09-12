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
using System.Runtime.Serialization;

namespace Microsoft.Xbox.Music.Platform.Contract.DataModel
{
    public interface IPaginatedList<out T>
    {
        IEnumerable<T> ReadOnlyItems { get; }
        string ContinuationToken { get; set; }
        int TotalItemCount { get; set; }
    }

    [DataContract(Namespace = Constants.Xmlns)]
    public class PaginatedList<T> : IPaginatedList<T>
    {
        [DataMember(EmitDefaultValue = false)]
        public List<T> Items { get; set; }

        public IEnumerable<T> ReadOnlyItems { get { return Items; } }

        [DataMember(EmitDefaultValue = false)]
        public string ContinuationToken { get; set; }

        /// <summary>
        /// An estimate count of the total number of items available in the list
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int TotalItemCount { get; set; }

        public PaginatedList()
        {
            Items = new List<T>();
        }
    }
}
