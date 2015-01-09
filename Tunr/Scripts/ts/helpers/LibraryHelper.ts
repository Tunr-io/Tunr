class LibraryHelper extends Helper {
	private nav_element: HTMLElement;
	private list_filter_state: Song;
	private list_helpers: Array<LibraryListHelper>;
	private list_helper_classes: { [index: string]: { new(parent: LibraryHelper, element: HTMLElement): LibraryListHelper; } } = { "artist": ArtistListHelper, "album": AlbumListHelper, "title": SongListHelper };
	private tree_structure: Array<string>;
	private root_name: string;
	private uploading_count: number = 0;
	private drag_target;

	public init () {
		this.nav_element = <HTMLElement>this.element.getElementsByTagName("nav")[0];
		this.tree_structure = ["tagPerformers", "tagAlbum", "title"]; // hard-set for now. user-configurable later.
		this.list_helpers = new Array<LibraryListHelper>();
		this.root_name = "Music";
		this.list_filter_state = new Song();
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

	/**
	 * Loads user's library from the server, then triggers root list render.
	 */
	public load(): void {
		this.parent.getTunr().library.load().then(() => {
			this.loadLevel(0);
		}, () => {
				console.error("failed to load library.");
			});
	}

	public getFilterState(): Song {
		return this.list_filter_state;
	}

	public loadChild(value: string) {
		// Get current property name
		var prop = this.tree_structure[this.list_helpers.length - 1];
		// Set filter state
		this.list_filter_state[prop] = value;
		// Load the next list
		this.loadLevel(this.list_helpers.length);
	}

	private loadLevel(levelIndex: number): void {
		var value = this.root_name;
		if (levelIndex > 0) {
			// Get current property name
			var prop = this.tree_structure[this.list_helpers.length - 1];
			// Get new value
			if (Array.isArray(this.list_filter_state[prop])) {
				value = this.list_filter_state[prop][0];
			} else {
				value = this.list_filter_state[prop];
			}
		}

		// Add nav header
		var header = document.createElement("a");
		header.innerHTML = value;
		header.addEventListener("click", () => {
			this.backLevel(levelIndex);
		});
		this.nav_element.appendChild(header);

		// Prepare list helper
		var listElement = document.createElement("ul");
		listElement.classList.add(this.tree_structure[levelIndex]);
		listElement.style.top = this.nav_element.clientHeight + "px";
		listElement = <HTMLUListElement>this.element.appendChild(listElement);
		var listHelper = new this.list_helper_classes[this.tree_structure[levelIndex]](this, listElement);
		this.list_helpers.push(listHelper);
		listHelper.init();

		// Hide others
		for (var i = 0; i < this.list_helpers.length - 1; i++) {
			this.list_helpers[i].hide();
		}
	}

	private backLevel(levelIndex): void {
		if (levelIndex == this.list_helpers.length - 1) {
			return;
		}
		var headers = this.nav_element.getElementsByTagName("a");
		// Show the specified list level
		this.list_helpers[levelIndex].show();
		// Hide the rest
		for (var i = this.list_helpers.length - 1; i > levelIndex; i--) {
			// Remove property of this from the filter state
			var prop = this.tree_structure[i];
			this.list_filter_state[prop] = "";
			// Remove the navigation header
			this.nav_element.removeChild(headers[i]);
			// Destroy the list
			var helper = this.list_helpers.pop();
			helper.destroy();
		}
	}

	public selectSong(song: Song): void {
		var playlistHelper = <PlaylistHelper>this.parent.getHelper("PlaylistHelper");
		playlistHelper.addSong(song);
	}
}

class LibraryListHelper extends Helper {
	public library_helper: LibraryHelper;
	constructor(parent: LibraryHelper, element: HTMLElement) {
		this.library_helper = parent;
		super(parent.parent, element);
	}

	public hide(): void {
		this.element.classList.add("hidden");
	}

	public show(): void {
		this.element.classList.remove("hidden");
	}

	public destroy(): void {
		this.element.parentNode.removeChild(this.element);
	}
}

class ArtistListHelper extends LibraryListHelper {
	public init(): void {
		var artists: Array<Song> = this.parent.getTunr().library.filterUniqueProperty(this.library_helper.getFilterState(), "tagPerformers");
		this.element.innerHTML = ''; // clear existing entries
		for (var i = 0; i < artists.length; i++) {
			var li = document.createElement("li");
			li.innerHTML = htmlEscape(artists[i].tagPerformers[0]);
			((artist, element: HTMLElement) => {
				TiltEffect.addTilt(element);
				element.addEventListener("click", (e) => {
					this.library_helper.loadChild(artist);
				});
			})(artists[i].tagPerformers[0], li);
			this.element.appendChild(li);
		}
	}
}

class AlbumListHelper extends LibraryListHelper {
	public init(): void {
		var albums: Array<Song> = this.parent.getTunr().library.filterUniqueProperty(this.library_helper.getFilterState(), "tagAlbum");
		this.element.innerHTML = "";
		for (var i = 0; i < albums.length; i++) {
			var img = document.createElement("img");
			img.src = '/api/LibraryData/' + urlEscape(albums[i].tagPerformers[0]) + '/' + urlEscape(albums[i].tagAlbum) + '/art';
			img.alt = albums[i].tagAlbum;
			img.style.opacity = '0';
			((imgel: HTMLImageElement) => {
				imgel.addEventListener("load", (ev) => {
					imgel.style.opacity = '1';
				});
			})(img);
			var li = document.createElement("li");
			li.appendChild(img);
			((album, element) => {
				TiltEffect.addTilt(element);
				element.addEventListener("click", () => {
					this.library_helper.loadChild(album);
				});
			})(albums[i].tagAlbum, li);
			this.element.appendChild(li);
		}
	}
}

class SongListHelper extends LibraryListHelper {
	public init(): void {
		var songs: Array<Song> = this.parent.getTunr().library.filter(this.library_helper.getFilterState());
		this.element.innerHTML = "";
		for (var i = 0; i < songs.length; i++) {
			var li = document.createElement("li");
			li.innerHTML = '<span class="track">' + ('0' + songs[i].tagTrack).slice(-2) + '</span>' + htmlEscape(songs[i].tagTitle);
			((song: Song, element) => {
				TiltEffect.addTilt(element);
				element.addEventListener("click", () => {
					this.library_helper.selectSong(song);
				});
			})(songs[i], li);
			this.element.appendChild(li);
		}
	}
}