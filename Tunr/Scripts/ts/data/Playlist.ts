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

	public moveIndex(src: number, target: number): void {
		if (target >= this.songs.length) {
			var k = target - this.songs.length;
			while ((k--) + 1) {
				this.songs.push(undefined);
			}
		}
		this.songs.splice(target, 0, this.songs.splice(src, 1)[0]);
	}

	public removeIndex(index: number): void {
		this.songs.splice(index, 1);
	}
}