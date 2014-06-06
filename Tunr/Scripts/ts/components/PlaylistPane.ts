enum PlayState {
	PLAYING,
	PAUSED,
	STOPPED
};

class PlaylistPane extends Component {
	private playlist: Playlist;
	private audio: HTMLAudioElement = new Audio();
	private songlist_element: HTMLUListElement;
	private playingIndex: number;
	private playstate: PlayState = PlayState.STOPPED;
	private repeat: boolean = false;

	constructor(tunr: Tunr) {
		super(tunr, "PlaylistPane");
		this.playlist = new Playlist("Playlist");
		this.songlist_element = <HTMLUListElement>this.getElement().getElementsByClassName("songlist")[0];
		this.audio.onended = (e) => {
			this.trackEnded();
		};
	}

	private _setPlayState(state: PlayState): void {
		this.playstate = state;
		if (this.playstate == PlayState.PLAYING) {
			this.getTunr().playingpane.play();
		} else if (this.playstate == PlayState.PAUSED || this.playstate == PlayState.STOPPED) {
			this.getTunr().playingpane.pause();
		}
	}

	// Audio controls
	public play(): void {
		if (this.playstate == PlayState.PAUSED) {
			// If paused, just resume ...
			this.audio.play();
			this._setPlayState(PlayState.PLAYING);
		} else if (this.playstate == PlayState.STOPPED) {
			if (this.playlist.getCount() > 0) {
				// If stopped, start from the beginning
				this.playIndex(0);
			}
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
		var song: Song = this.playlist.getSong(index);

		// Change the audio source
		this.audio.src = "/api/Library/" + song.songID;

		// Change play listing
		this.getTunr().playingpane.changeSong(song);

		// Play it
		this.audio.play();
		this._setPlayState(PlayState.PLAYING);

		// Set index of current track
		this.playingIndex = index;

		// Clear play markers on all list items.
		var song_elements = this.songlist_element.getElementsByClassName("playing");
		for (var i = 0; i < song_elements.length; i++) {
			(<HTMLLIElement>song_elements[i]).classList.remove("playing");
		}

		// Add play marker to current song.
		var song_element = <HTMLLIElement>this.songlist_element.getElementsByClassName("playlistitem")[index];
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
			var song_elements = this.songlist_element.getElementsByClassName("playlistitem");
			var i;
			for (i = 0; i < song_elements.length; i++) {
				if (<HTMLElement>song_elements[i] == clicked_element) {
					break;
				}
			}
			//this.playIndex(i);
			this.showControls(i);
		});
		return song_element;
	}

	public hideAllControls() {
		var playlistitems = (this.songlist_element.getElementsByClassName("playlistitem"));
		for (var i = 0; i < playlistitems.length; i++) {
			var controls = (<HTMLElement>playlistitems[i]).getElementsByClassName("controls");
			if (controls.length > 0) {
				this.hideControlsAt(i);
			}
		}
	}

	public hideControlsAt(index: number) {
		var controls_element = <HTMLElement>(<HTMLLIElement>this.songlist_element.getElementsByClassName("playlistitem")[index]).getElementsByClassName("controls")[0];
		this.hideControls(controls_element);
	}

	public hideControls(element: HTMLElement) {
		element.parentElement.classList.remove("dim");
		element.parentElement.removeChild(element);
	}

	public showControls(index: number) {
		this.hideAllControls();

		var song_element = <HTMLLIElement>this.songlist_element.getElementsByClassName("playlistitem")[index];

		// Make us some controls ...
		var itemControls = document.createElement("ul");
		itemControls.classList.add("controls");

		// Play button
		var b_play = document.createElement("li");
		b_play.classList.add("play");
		b_play.addEventListener("click", () => {
			this.playIndex(index);
			this.hideControls(b_play.parentElement);
		});
		itemControls.appendChild(b_play);
		
		// Remove button
		var b_remove = document.createElement("li");
		b_remove.classList.add("remove");
		itemControls.appendChild(b_remove);

		// Up button
		var b_up = document.createElement("li");
		b_up.classList.add("up");
		itemControls.appendChild(b_up);

		// Down button
		var b_down = document.createElement("li");
		b_down.classList.add("down");
		itemControls.appendChild(b_down);

		song_element.classList.add("dim");
		song_element.appendChild(itemControls);
	}

	public addSong(song: Song) {
		this.playlist.addSong(song);
		var song_element: HTMLLIElement = this._renderElement(song);

		if (this.playstate == PlayState.STOPPED) {
			this.playIndex(this.playlist.getCount() - 1);
		}
	}

	public nextIndex(): number {
		var nextIndex = this.playingIndex + 1;
		if (nextIndex >= this.playlist.getCount()) {
			nextIndex = 0;
		}
		return nextIndex;
	}

	public prevIndex(): number {
		var prevIndex = this.playingIndex - 1;
		if (prevIndex < 0) {
			prevIndex = this.playlist.getCount() - 1;
		}
		return prevIndex;
	}
} 