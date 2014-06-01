class LibraryPane extends Component {
	private nav_element: HTMLElement;
	private artists_element: HTMLUListElement;
	private albums_element: HTMLUListElement;
	private songs_element: HTMLUListElement;

	constructor(tunr: Tunr) {
		super(tunr, "LibraryPane");
		this.nav_element = <HTMLElement>this.getElement().getElementsByTagName("nav")[0];
		this.artists_element = <HTMLUListElement>this.getElement().getElementsByClassName("artists")[0];
		this.albums_element = <HTMLUListElement>this.getElement().getElementsByClassName("albums")[0];
		this.songs_element = <HTMLUListElement>this.getElement().getElementsByClassName("songs")[0];

		this.nav_element.getElementsByTagName("a")[0].addEventListener("click", () => {
			this.showArtists();
		});
		this.load();
	}

	public load(): void {
		this.getTunr().library.load().then(() => {
			this.loadArtists();
		}, () => {
			console.error("failed to load library.");
		});
	}

	public loadArtists(): void {
		var artists: Array<string> = this.getTunr().library.artists();
		this.artists_element.innerHTML = ''; // clear existing entries
		for (var i = 0; i < artists.length; i++) {
			var li = document.createElement("li");
			li.innerHTML = htmlEscape(artists[i]);
			((artist, element: HTMLElement) => {
				TiltEffect.addTilt(element);
				element.addEventListener("click", (e) => {
					this.loadAlbums(artist);
				});
			})(artists[i],li);
			this.artists_element.appendChild(li);
		}
	}

	public showArtists(): void {
		// Hide other things
		if (!this.albums_element.classList.contains("hidden")) {
			this.albums_element.classList.add("hidden");
		}
		if (!this.songs_element.classList.contains("hidden")) {
			this.songs_element.classList.add("hidden");
		}

		// Remove extraneous nav elements
		var navs = this.nav_element.getElementsByTagName("a");
		for (var i = navs.length-1; i > 0; i--) {
			this.nav_element.removeChild(navs[i]);
		}

		// This shouldn't change content.
		if (this.artists_element.classList.contains("hidden")) {
			this.artists_element.classList.remove("hidden");
		}
	}

	public showAlbums(): void {
		// Hide other things
		if (!this.artists_element.classList.contains("hidden")) {
			this.artists_element.classList.add("hidden");
		}
		if (!this.songs_element.classList.contains("hidden")) {
			this.songs_element.classList.add("hidden");
		}

		// Remove extraneous nav elements
		var navs = this.nav_element.getElementsByTagName("a");
		for (var i = navs.length - 1; i > 1; i--) {
			this.nav_element.removeChild(navs[i]);
		}

		// This shouldn't change content.
		if (this.albums_element.classList.contains("hidden")) {
			this.albums_element.classList.remove("hidden");
		}
	}

	public loadAlbums(artist: string): void {
		var albums: Array<string> = this.getTunr().library.albumsin(artist);
		this.albums_element.innerHTML = "";
		for (var i = 0; i < albums.length; i++) {
			var li = document.createElement("li");
			li.innerHTML = '<img src="/api/LibraryData/' + artist + '/' + albums[i] + '/art" alt="' + albums[i] + '" />';
			((album, element) => {
				TiltEffect.addTilt(element);
				element.addEventListener("click", () => {
					this.loadSongs(album);
				});
			})(albums[i], li);
			this.albums_element.appendChild(li);
		}
		// Add header
		var nav = document.createElement("a");
		nav.innerHTML = htmlEscape(artist);
		nav.addEventListener("click", () => {
			this.showAlbums();
		});
		this.nav_element.appendChild(nav);
		// Hide artists
		this.artists_element.classList.add("hidden");
		// Show albums
		this.albums_element.classList.remove("hidden");
	}

	public loadSongs(album: string): void {
		var songs: Array<Song> = this.getTunr().library.songsin(album);
		this.songs_element.innerHTML = "";
		for (var i = 0; i < songs.length; i++) {
			var li = document.createElement("li");
			li.innerHTML = '<span class="track">' + ('0' + songs[i].trackNumber).slice(-2) + '</span>' + htmlEscape(songs[i].title); //TODO: we need the track number
			((song: Song, element) => {
				TiltEffect.addTilt(element);
				element.addEventListener("click", () => {
					this.selectSong(song);
				});
			})(songs[i], li);
			this.songs_element.appendChild(li);
		}

		// Add header
		var nav = document.createElement("a");
		nav.innerHTML = htmlEscape(album);
		this.nav_element.appendChild(nav);
		// Hide albums
		this.albums_element.classList.add("hidden");
		// Show songs
		this.songs_element.classList.remove("hidden");
	}

	public selectSong(song: Song): void {
		this.getTunr().playlistpane.addSong(song);
	}
} 