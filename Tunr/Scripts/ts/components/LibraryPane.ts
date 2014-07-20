class LibraryPane extends Component {
	private nav_element: HTMLElement;
	private artists_element: HTMLUListElement;
	private albums_element: HTMLUListElement;
	private songs_element: HTMLUListElement;
	private uploading_count: number = 0;
	private drag_target;

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

		// Prepare drag and drop uploading
		this.getElement().addEventListener("dragenter", (e) => {
			this.drag_target = e.target;
			if (!this.getElement().classList.contains("dragging")) {
				this.getElement().classList.add("dragging");
			}
			e.stopPropagation();
			e.preventDefault();
		}, false);
		this.getElement().addEventListener("dragover", (e) => {
			e.stopPropagation();
			e.preventDefault();
		});
		this.getElement().addEventListener("dragleave", (e) => {
			if (e.target == this.drag_target) {
				if (this.getElement().classList.contains("dragging")) {
					this.getElement().classList.remove("dragging");
				}
			}
			e.stopPropagation();
			e.preventDefault();
		}, false);
		this.getElement().addEventListener("drop", (e) => {
			if (this.getElement().classList.contains("dragging")) {
				this.getElement().classList.remove("dragging");
			}
			this.readFiles(e.dataTransfer.files);
			this.drag_target = null;
			e.stopPropagation();
			e.preventDefault();
		}, false);
	}

	private readFiles(files) {
		var formData = new FormData();
		for (var i = 0; i < files.length; i++) {
			formData.append('file', files[i]);
		}

		// now post a new XHR request
		var xhr = new XMLHttpRequest();
		xhr.open('POST', '/api/Library');
		xhr.setRequestHeader("Authorization", "Bearer " + this.getTunr().api.getAuthentication().get_access_token());
		xhr.onload = () => {
			this.uploading_count--;
			if (this.uploading_count <= 0) {
				if (this.getElement().classList.contains("uploading")) {
					this.getElement().classList.remove("uploading");
				}
			}
		};

		xhr.upload.onprogress = function (event) {
			if (event.lengthComputable) {
				var complete = (event.loaded / event.total * 100 | 0);
				console.log("Uploading track: " + complete + "%");
			}
		}
		this.uploading_count++;
		if (!this.getElement().classList.contains("uploading")) {
			this.getElement().classList.add("uploading");
		}
		xhr.send(formData);
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
			
			var img = document.createElement("img");
			img.src = '/api/LibraryData/' + urlEscape(artist) + '/' + urlEscape(albums[i]) + '/art';
			img.alt = albums[i];
			img.style.opacity = '0';
			((imgel:HTMLImageElement) => {
				imgel.addEventListener("load", (ev) => {
					imgel.style.opacity = '1';
				});
			})(img);
			var li = document.createElement("li");
			li.appendChild(img);
			((album, element) => {
				TiltEffect.addTilt(element);
				element.addEventListener("click", () => {
					this.loadSongs(album,artist);
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

	public loadSongs(album: string, artist?: string): void {
		var songs: Array<Song> = this.getTunr().library.songsin(album,artist);
		this.songs_element.innerHTML = "";
		for (var i = 0; i < songs.length; i++) {
			var li = document.createElement("li");
			li.innerHTML = '<span class="track">' + ('0' + songs[i].trackNumber).slice(-2) + '</span>' + htmlEscape(songs[i].title);
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

	public songActions(element: HTMLElement, song: Song) {

	}

	public selectSong(song: Song): void {
		this.getTunr().playlistpane.addSong(song);
	}
} 