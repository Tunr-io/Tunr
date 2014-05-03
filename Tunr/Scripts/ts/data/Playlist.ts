class Playlist {
	private name: string;
	private songs: Array<Song>;
	constructor(name: string, songs?: Array<Song>) {
		this.name = name;
		if (songs) {
			this.songs = songs;
		} else {
			this.songs = new Array<Song>();
		}
	}

	public addSong(song: Song) {
		this.songs.push(song);
	}

	public getSong(index: number) {
		return this.songs[index];
	}

	public getCount(): number {
		return this.songs.length;
	}
}