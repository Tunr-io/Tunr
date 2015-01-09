class Library {
	private tunr: Tunr;
	private songs: Array<Song> = new Array<Song>();
	private song_index = {};
	constructor(tunr: Tunr) {
		this.tunr = tunr;
	}

	public get_song_by_id(songid: string): Song {
		return this.song_index[songid];
	}

	public load(): JQueryPromise<any> {
		var retval: JQueryDeferred<any> = $.Deferred<any>();
		this.tunr.api.get("Library").then((songs) => {
			this.songs = <Array<Song>>songs;
			for (var i = 0; i < this.songs.length; i++) {
				this.song_index[this.songs[i].songId] = this.songs[i];
			}
			retval.resolve();
		}, () => {
			console.error("failed to retrieve library.");
			retval.reject();
		});
		return retval;
	}

	public addSong(s: Song): void {
		this.songs.push(s);
		this.song_index[s.songId] = s;
	}

	public filter(conditions: Song): Array<Song> {
		var results: Array<Song> = new Array<Song>();
		for (var i = 0; i < this.songs.length; i++) {
			var add = true;
			for (var prop in conditions) {
				if (typeof conditions[prop] !== 'undefined' && conditions[prop] != "") {
					if (conditions[prop] != this.songs[i][prop]) {
						add = false;
						break;
					}
				}
			}
			if (add) {
				results.push(this.songs[i]);
			}
		}
		return results;
	}

	/**
	 * Filter the library and return songs that match the specified conditions,
	 * whilst each having a unique value for the given property.
	 */
	public filterUniqueProperty(conditions: Song, property: string): Array<Song> {
		var uniqueProps: Array<string> = new Array<string>();
		var results: Array<Song> = new Array<Song>();
		for (var i = 0; i < this.songs.length; i++) {
			var add = true;
			for (var prop in conditions) {
				if (typeof conditions[prop] !== 'undefined' && conditions[prop] != "") {
					if (conditions[prop] != this.songs[i][prop]) {
						add = false;
						break;
					}
				}
			}
			if (add && uniqueProps.indexOf(this.songs[i][property]) < 0) {
				uniqueProps.push(this.songs[i][property]);
				results.push(this.songs[i]);
			}
		}
		return results;
	}

	//public artists(): Array<string> {
	//	var artists: Array<string> = new Array<string>();
	//	for (var i = 0; i < this.songs.length; i++) {
	//		if (artists.indexOf(this.songs[i].artist) < 0) {
	//			artists.push(this.songs[i].artist);
	//		}
	//	}
	//	return artists.sort();
	//}

	//public albumsin(artist: string) {
	//	var albums: Array<string> = new Array<string>();
	//	for (var i = 0; i < this.songs.length; i++) {
	//		if (this.songs[i].artist == artist && albums.indexOf(this.songs[i].album) < 0) {
	//			albums.push(this.songs[i].album);
	//		}
	//	}
	//	return albums.sort();
	//}

	//public albums(): Array<string> {
	//	var albums: Array<string> = new Array<string>();
	//	for (var i = 0; i < this.songs.length; i++) {
	//		if (albums.indexOf(this.songs[i].album) < 0) {
	//			albums.push(this.songs[i].album);
	//		}
	//	}
	//	return albums;
	//}

	//public songsin(album: string, artist?: string): Array<Song> {
	//	var songs: Array<Song> = new Array<Song>();
	//	for (var i = 0; i < this.songs.length; i++) {
	//		if (this.songs[i].album == album) {
	//			if (typeof artist === undefined || this.songs[i].artist == artist) {
	//				songs.push(this.songs[i]);
	//			}
	//		}
	//	}
	//	return songs.sort((a, b) => {
	//		return a.trackNumber - b.trackNumber;
	//	});
	//}
}

class Song {
	public songId: string;
	public md5Hash: string;
	public duration: number;
	public tagTitle: string;
	public tagAlbum: string;
	public tagPerformers: Array<string>;
	public tagAlbumArtists: Array<string>;
	public tagComposers: Array<string>;
	public tagGenres: Array<string>;
	public tagYear: number;
	public tagTrack: number;
	public tagTrackCount: number;
	public tagDisc: number;
	public tagDiscCount: number;
	public tagComment: string;
	public tagLyrics: string;
	public tagConductor: string;
	public tagBeatsPerMinute: number;
	public tagGrouping: string;
	public tagCopyright: string;
}