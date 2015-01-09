class PlaylistPane extends Component {
	//private playlist: Playlist;
	private audio: HTMLAudioElement = new Audio();
	private songlist_element: HTMLUListElement;

	private playstate: PlayState = PlayState.STOPPED;
	private repeat: boolean = false;

	private controls_element: HTMLElement;
	private controls_index: number;
	private controls_timeout: number;

	constructor(tunr: Tunr) {
		super(tunr, "PlaylistPane");
		this.songlist_element = <HTMLUListElement>this.getElement().getElementsByClassName("songlist")[0];
		this.audio.onended = (e) => {
			this.trackEnded();
		};
	}

	private _setPlayState(state: PlayState): void {
		this.playstate = state;
		if (this.playstate == PlayState.PLAYING) {
			//this.getTunr().playingpane.play();
		} else if (this.playstate == PlayState.PAUSED || this.playstate == PlayState.STOPPED) {
			//this.getTunr().playingpane.pause();
		}
	}

	/**
	 * Retreives the song object at the specified index of the playlist.
	 */
	public get_song_at(index: number): Song {
		var element = this.get_element_at(index);
		var songID = element.attributes["data-song-id"];
		return this.getTunr().library.get_song_by_id(songID);
	}

	/**
	 * Gets the playlist HTML element at the specified playlist index.
	 */
	public get_element_at(index: number): HTMLElement {
		return <HTMLElement>this.songlist_element.getElementsByClassName("playlistitem")[index];
	}

	/**
	 * Returns the number of items in the playlist.
	 */
	public count(): number {
		return this.songlist_element.getElementsByClassName("playlistitem").length;
	}

	/**
	 * Gets the index of the specified playlist element.
	 */
	private _find_index_of_element(element: HTMLElement): number {
		var song_elements = this.songlist_element.getElementsByClassName("playlistitem");
		var i;
		for (i = 0; i < song_elements.length; i++) {
			if (<HTMLElement>song_elements[i] == element) {
				break;
			}
		}
		return i;
	}

	/**
	 * Gets the index of the currently playing playlist item
	 */
	public get_playing_index(): number {
		var playing = this.songlist_element.getElementsByClassName("playing");
		if (playing.length <= 0) {
			return -1;
		}
		var playing_element = <HTMLElement>playing[0];
		var i;
		for (i = 0; i < this.count(); i++) {
			if (this.get_element_at(i) == playing_element) {
				return i;
			}
		}
		return -1;
	}

	// Gets how many seconds have elapsed since the song started.
	public getSongTime(): number {
		return this.audio.currentTime;
	}

	// Audio controls
	public play(): void {
		if (this.playstate == PlayState.PAUSED) {
			// If paused, just resume ...
			this.audio.play();
			this._setPlayState(PlayState.PLAYING);
		} else if (this.playstate == PlayState.STOPPED) {
			if (this.count() > 0) {
				// If stopped, start from the beginning
				this.playIndex(0);
			}
			// If we have no tracks, do nothing.
		}
	}

	public pause(): void {
		if (this.playstate == PlayState.PLAYING) {
			this.audio.pause();
			this._setPlayState(PlayState.PAUSED);
		}
	}

	public stop(): void {
		if (this.playstate != PlayState.STOPPED) {
			if (this.playstate == PlayState.PLAYING) {
				this.audio.pause();
			}
			this.audio = new Audio(); // Reset audio
			// Clear play markers on all list items.
			var song_elements = this.songlist_element.getElementsByClassName("playing");
			for (var i = 0; i < song_elements.length; i++) {
				(<HTMLLIElement>song_elements[i]).classList.remove("playing");
			}
			this._setPlayState(PlayState.STOPPED);
		}
	}

	public next(): void {
		var next = this.nextIndex();
		if (next >= 0) {
			this.playIndex(next);
		} else {
			this.stop();
		}
	}

	public prev(): void {
		var next = this.prevIndex();
		if (next >= 0) {
			this.playIndex(next);
		} else {
			this.stop();
		}
	}

	public playIndex(index: number) {
		var song: Song = this.get_song_at(index);

		// Change the audio source
		//this.audio.src = "/api/Library/" + song.songId;
		this.audio.src = "/stream/" + song.songId;

		// Change play listing
		//this.getTunr().playingpane.changeSong(song);

		// Play it
		this.audio.play();
		this._setPlayState(PlayState.PLAYING);

		// Clear play markers on all list items.
		var song_elements = this.songlist_element.getElementsByClassName("playing");
		for (var i = 0; i < song_elements.length; i++) {
			(<HTMLLIElement>song_elements[i]).classList.remove("playing");
		}

		// Add play marker to current song.
		var song_element = this.get_element_at(index);
		song_element.classList.add("playing");
	}

	private trackEnded(): void {
		this.next();
	}

	private _renderElement(song: Song): HTMLLIElement {
		var song_element: HTMLLIElement = document.createElement("li");
		song_element.classList.add("playlistitem");
		song_element.classList.add("animated");
		song_element.classList.add("anim_playlistitem_in");
		song_element.attributes["data-song-id"] = song.songId;
		song_element.innerHTML = '<div class="listing"><span class="title">' + htmlEscape(song.title) + '</span><br /><span class="artist">' + htmlEscape(song.artist) + '</span></div>';
		song_element = <HTMLLIElement>this.songlist_element.appendChild(song_element);
		song_element.getElementsByClassName("listing")[0].addEventListener("click", (e) => {
			var clicked_element = (<HTMLElement>e.target);
			while (clicked_element.tagName.toLowerCase() != "li") {
				clicked_element = clicked_element.parentElement;
				if (clicked_element == null) {
					return;
				}
			}
			var i = this._find_index_of_element(clicked_element);
			//this.playIndex(i);
			this.show_controls_at(i);
		});
		return song_element;
	}

	public set_controls_timeout() {
		clearTimeout(this.controls_timeout);
		this.controls_timeout = setTimeout(() => {
			this.hide_controls();
		}, 4000);
	}

	public hide_controls() {
		if (this.controls_element !== undefined && this.controls_element.parentElement !== undefined && this.controls_element.parentElement != null) {
			this.controls_element.parentElement.classList.remove("dim");
			this.controls_element.parentElement.removeChild(this.controls_element);
		}
	}

	public show_controls_at(index: number) {
		this.hide_controls();
		var song_element = this.get_element_at(index);

		// Make us some controls ...
		var itemControls = document.createElement("ul");
		itemControls.classList.add("controls");

		// Play button
		var b_play = document.createElement("li");
		b_play.classList.add("play");
		b_play.addEventListener("click", () => {
			this.playIndex(this.controls_index);
		});
		itemControls.appendChild(b_play);
		
		// Remove button
		var b_remove = document.createElement("li");
		b_remove.classList.add("remove");
		itemControls.appendChild(b_remove);
		b_remove.addEventListener("click", () => {
			this.remove_by_index(this.controls_index);
		});

		// Up button
		var b_up = document.createElement("li");
		b_up.classList.add("up");
		itemControls.appendChild(b_up);
		b_up.addEventListener("click", () => {
			var targetIndex = this.controls_index - 1;
			if (targetIndex < 0) {
				targetIndex = this.count() - 1;
			}
			this.move_song(this.controls_index, targetIndex);
			this.controls_index = targetIndex;
			this.set_controls_timeout();
		});

		// Down button
		var b_down = document.createElement("li");
		b_down.classList.add("down");
		itemControls.appendChild(b_down);
		b_down.addEventListener("click", () => {
			var targetIndex = this.controls_index + 1;
			if (targetIndex >= this.count()) {
				targetIndex = 0;
			}
			this.move_song(this.controls_index, targetIndex);
			this.controls_index = targetIndex;
			this.set_controls_timeout();
		});

		song_element.classList.add("dim");
		this.controls_element = <HTMLElement>song_element.appendChild(itemControls);
		this.controls_index = index;
		
		if (!window.hasOwnProperty("chrome")) {
			TiltEffect.addTilt(this.controls_element);
		}
		this.set_controls_timeout();
	}

	public addSong(song: Song) {
		var song_element: HTMLLIElement = this._renderElement(song);

		if (this.playstate == PlayState.STOPPED) {
			this.playIndex(this.count() - 1);
		}
	}

	public move_song(srcIndex: number, targetIndex: number) {
		if (srcIndex == targetIndex) {
			// If they're equal why are we moving ...?
			return;
		}
		var item = this.get_element_at(srcIndex);
		
		if (targetIndex > srcIndex) {
			// Moving to a higher index
			if (targetIndex == this.count() - 1) {
				//if next element is the last one, we hit the end.
				item = <HTMLElement>item.parentElement.removeChild(item);
				this.songlist_element.appendChild(item);
			} else {
				// Insert before the element after the next element
				var target = this.get_element_at(targetIndex + 1);
				item = <HTMLElement>item.parentElement.removeChild(item);
				this.songlist_element.insertBefore(item, target);
			}
		} else {
			// Moving to a lower index
			// Insert before the element
			var target = this.get_element_at(targetIndex);
			item = <HTMLElement>item.parentElement.removeChild(item);
			this.songlist_element.insertBefore(item, target);
		}
	}

	public remove_by_index(index: number) {
		if (this.get_playing_index() == index) {
			if (this.count() == 1) {
				this.stop();
			} else {
				this.next();
			}
		}
		var item = this.get_element_at(index);
		item = <HTMLElement>item.parentElement.removeChild(item);
	}

	public nextIndex(): number {
		var nextIndex = this.get_playing_index() + 1;
		if (nextIndex >= this.count()) {
			nextIndex = 0;
		}
		return nextIndex;
	}

	public prevIndex(): number {
		var prevIndex = this.get_playing_index() - 1;
		if (prevIndex < 0) {
			prevIndex = this.count() - 1;
		}
		return prevIndex;
	}
} 