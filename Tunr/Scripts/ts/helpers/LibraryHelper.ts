class LibraryHelper extends Helper {
	private nav_element: HTMLElement;
	private artists_element: HTMLUListElement;
	private albums_element: HTMLUListElement;
	private songs_element: HTMLUListElement;
	private uploading_count: number = 0;
	private drag_target;

	public init () {
		this.nav_element = <HTMLElement>this.element.getElementsByTagName("nav")[0];
		this.artists_element = <HTMLUListElement>this.element.getElementsByClassName("artists")[0];
		this.albums_element = <HTMLUListElement>this.element.getElementsByClassName("albums")[0];
		this.songs_element = <HTMLUListElement>this.element.getElementsByClassName("songs")[0];

		this.nav_element.getElementsByTagName("a")[0].addEventListener("click", () => {
			this.showArtists();
		});
		this.nav_element.getElementsByTagName("a")[1].addEventListener("click", () => {
			this.showAlbums();
		});
		this.nav_element.getElementsByTagName("a")[2].addEventListener("click", () => {
			this.showSongs();
		});
		this.load();

		// Prepare drag and drop uploading
		this.element.addEventListener("dragenter", (e) => {
			this.drag_target = e.target;
			if (!this.element.classList.contains("dragging")) {
				this.element.classList.add("dragging");
			}
			e.stopPropagation();
			e.preventDefault();
		}, false);
		this.element.addEventListener("dragover", (e) => {
			e.stopPropagation();
			e.preventDefault();
		});
		this.element.addEventListener("dragleave", (e) => {
			if (e.target == this.drag_target) {
				if (this.element.classList.contains("dragging")) {
					this.element.classList.remove("dragging");
				}
			}
			e.stopPropagation();
			e.preventDefault();
		}, false);
		this.element.addEventListener("drop", (e) => {
			if (this.element.classList.contains("dragging")) {
				this.element.classList.remove("dragging");
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
		xhr.setRequestHeader("Authorization", "Bearer " + this.parent.getTunr().api.getAuthentication().get_access_token());
		xhr.onload = () => {
			// TODO: Report errors to user.
			this.uploading_count--;
			if (this.uploading_count <= 0) {
				if (this.element.classList.contains("uploading")) {
					this.element.classList.remove("uploading");
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
		if (!this.element.classList.contains("uploading")) {
			this.element.classList.add("uploading");
		}
		xhr.send(formData);
	}

	public load(): void {
		this.parent.getTunr().library.load().then(() => {
			this.loadArtists();
		}, () => {
			console.error("failed to load library.");
		});
	}

	public loadArtists(): void {
		var artists: Array<string> = this.parent.getTunr().library.artists();
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
		this.element.classList.remove("back");
		this.element.classList.add("forward");
		this.element.classList.add("showArtists");
		this.element.classList.remove("showAlbums");
		this.element.classList.remove("showSongs");
	}

	public showAlbums(): void {
		if (this.element.classList.contains("showArtists")) {
			this.element.classList.add("forward");
			this.element.classList.remove("back");
		} else {
			this.element.classList.remove("forward");
			this.element.classList.add("back");
		}
		this.element.classList.remove("showArtists");
		this.element.classList.add("showAlbums");
		this.element.classList.remove("showSongs");
	}

	public showSongs(): void {
		this.element.classList.remove("back");
		this.element.classList.add("forward");
		this.element.classList.remove("showArtists");
		this.element.classList.remove("showAlbums");
		this.element.classList.add("showSongs");
	}

	public loadAlbums(artist: string): void {
		var albums: Array<string> = this.parent.getTunr().library.albumsin(artist);
		this.nav_element.getElementsByTagName("a")[1].innerText = artist;
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
		this.showAlbums();
	}

	public loadSongs(album: string, artist?: string): void {
		var songs: Array<Song> = this.parent.getTunr().library.songsin(album, artist);
		this.nav_element.getElementsByTagName("a")[2].innerText = album;
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
		this.showSongs();
	}

	public songActions(element: HTMLElement, song: Song) {

	}

	public selectSong(song: Song): void {
		var playlistHelper = <PlaylistHelper>this.parent.getHelper("PlaylistHelper");
		playlistHelper.addSong(song);
	}
} 