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
    [DataContract(Namespace = Constants.Xmlns)]
    public class ContentResponse : BaseResponse
    {
        [DataMember(EmitDefaultValue = false)]
        public PaginatedList<Artist> Artists { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public PaginatedList<Album> Albums { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public PaginatedList<Track> Tracks { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public PaginatedList<Playlist> Playlists { get; set; } 

        [DataMember(EmitDefaultValue = false)]
        public PaginatedList<ContentItem> Results { get; set; } 

        [DataMember(EmitDefaultValue = false)]
        public GenreList Genres { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Culture { get; set; }

        private void GenericAddPieceOfContent<T>(Content content, PaginatedList<T> container, Func<PaginatedList<T>> containerInstanciator)
            where T : Content
        {
            if (container == null)
            {
                container = containerInstanciator();
            }
            if (container.Items == null)
            {
                container.Items = new List<T>();
            }
            container.Items.Add(content as T);
        }

        public void AddPieceOfContent(Content content)
        {
            if (content is Artist)
            {
                GenericAddPieceOfContent(content, Artists, () => Artists = new PaginatedList<Artist>());
            }
            else if (content is Album)
            {
                GenericAddPieceOfContent(content, Albums, () => Albums = new PaginatedList<Album>());
            }
            else if (content is Track)
            {
                GenericAddPieceOfContent(content, Tracks, () => Tracks = new PaginatedList<Track>());
            }
            else if (content is Playlist)
            {
                GenericAddPieceOfContent(content, Playlists, () => Playlists = new PaginatedList<Playlist>());
            }
            else
            {
                throw new ArgumentException("Unknown content type:" + content.GetType().ToString());
            }
        }

        public IEnumerable<Content> GetAllTopLevelContent()
        {
            return GetAllContentLists()
                .Where(c => c != null && c.ReadOnlyItems != null)
                .SelectMany(x => x.ReadOnlyItems);
        }

        public IEnumerable<IPaginatedList<Content>> GetAllContentLists()
        {
            yield return Artists;
            yield return Albums;
            yield return Tracks;
            yield return Playlists;
        }
    }
}
