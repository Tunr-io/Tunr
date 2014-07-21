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

	public artists(): Array<string> {
		var artists: Array<string> = new Array<string>();
		for (var i = 0; i < this.songs.length; i++) {
			if (artists.indexOf(this.songs[i].artist) < 0) {
				artists.push(this.songs[i].artist);
			}
		}
		return artists.sort();
	}

	public albumsin(artist: string) {
		var albums: Array<string> = new Array<string>();
		for (var i = 0; i < this.songs.length; i++) {
			if (this.songs[i].artist == artist && albums.indexOf(this.songs[i].album) < 0) {
				albums.push(this.songs[i].album);
			}
		}
		return albums.sort();
	}

	public albums(): Array<string> {
		var albums: Array<string> = new Array<string>();
		for (var i = 0; i < this.songs.length; i++) {
			if (albums.indexOf(this.songs[i].album) < 0) {
				albums.push(this.songs[i].album);
			}
		}
		return albums;
	}

	public songsin(album: string, artist?: string): Array<Song> {
		var songs: Array<Song> = new Array<Song>();
		for (var i = 0; i < this.songs.length; i++) {
			if (this.songs[i].album == album) {
				if (typeof artist === undefined || this.songs[i].artist == artist) {
					songs.push(this.songs[i]);
				}
			}
		}
		return songs.sort((a, b) => {
			return a.trackNumber - b.trackNumber;
		});
	}
}

class Song {
	public songId: string;
	public ownerId: string;
	//public songFingerPrint: string;
	public artist: string;
	public album: string;
	public title: string;
	public genre: string;
	public discNumber: string;
	public trackNumber: number;
	public year: number;
	public length: number;
}