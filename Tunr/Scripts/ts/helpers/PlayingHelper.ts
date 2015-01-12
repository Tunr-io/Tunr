class PlayingHelper extends Helper {
	private song: Song;
	private title_element: HTMLDivElement;
	private controls_element: HTMLUListElement;
	private canvas_element: HTMLCanvasElement;
	private visualizer: Visualizer;
	private playtime_timer: number;

	public init(): void {
		this.title_element = <HTMLDivElement>this.element.getElementsByClassName("title")[0];
		this.controls_element = <HTMLUListElement>this.element.getElementsByClassName("controls")[0];
		this.canvas_element = <HTMLCanvasElement>this.element.getElementsByTagName("canvas")[0];
		this.visualizer = new Visualizer(this, this.canvas_element);
		// Set up event handlers for control buttons...
		TiltEffect.addTilt(<HTMLElement>this.controls_element.getElementsByClassName("play")[0]);
		TiltEffect.addTilt(<HTMLElement>this.controls_element.getElementsByClassName("pause")[0]);
		TiltEffect.addTilt(<HTMLElement>this.controls_element.getElementsByClassName("next")[0]);
		TiltEffect.addTilt(<HTMLElement>this.controls_element.getElementsByClassName("prev")[0]);
		this.controls_element.getElementsByClassName("play")[0].addEventListener("click", () => {
			this.parent.getHelper("PlaylistHelper").play();
		});
		this.controls_element.getElementsByClassName("pause")[0].addEventListener("click", () => {
			this.parent.getHelper("PlaylistHelper").pause();
		});
		this.controls_element.getElementsByClassName("next")[0].addEventListener("click", () => {
			this.parent.getHelper("PlaylistHelper").next();
		});
		this.controls_element.getElementsByClassName("prev")[0].addEventListener("click", () => {
			this.parent.getHelper("PlaylistHelper").prev();
		});

		setTimeout(() => {
			// Initialize the canvas.
			this.visualizer.init();
		}, 300);
	}

	public getSong(): Song {
		return this.song;
	}

	public changeSong(song: Song): void {
		// Keep a reference of the song
		this.song = song;

		// Set up the new title
		var title = document.createElement("div");
		title.classList.add("title");
		title.classList.add("animated");
		title.classList.add("anim_playingtitle_in");
		title.innerHTML = '<h1>' + htmlEscape(song.tagTitle) + '</h1><h2>' + htmlEscape(song.tagPerformers[0]) + '</h2>';

		// Title animations
		var title = <HTMLDivElement>this.element.insertBefore(title, this.title_element);
		this.title_element.classList.remove("anim_playingtitle_in");
		this.title_element.classList.add("anim_playingtitle_out");

		// Remove the old title after the animation completes.
		((el) => {
			setTimeout(() => {
				this.element.removeChild(el);
			}, 300);
		})(this.title_element);

		// Set the new title as the 'actual' title.
		this.title_element = title;

		var oldArt = this.element.querySelectorAll("img");
		for (var i = 0; i < oldArt.length; i++) {
			// animate out any old artwork
			var oldArtItem = (<HTMLElement>oldArt[i]);
			oldArtItem.classList.add("animated");
			oldArtItem.classList.add("anim_albumart_out");
		}

		// Set up the new art
		var art = document.createElement("img");
		art.src = '/api/Library/' + song.songId + '/AlbumArt';
		art.alt = song.tagAlbum;
		art.classList.add("animated");
		art.classList.add("anim_albumart_in");

		(<HTMLElement>oldArt[0]).parentElement.appendChild(art);

		// Remove the old art after the animation completes.
		((els) => {
			setTimeout(() => {
				for (var i = 0; i < els.length; i++) {
					els[i].parentElement.removeChild(els[i]);
				}
			}, 300);
		})(oldArt);
	}

	public play(): void {
		clearInterval(this.playtime_timer);
		this.playtime_timer = setInterval(() => {
			this.update_playtime();
		}, 500);
		if (!this.controls_element.classList.contains("playing")) {
			this.controls_element.classList.add("playing");
		}
		this.visualizer.start(this.song);
	}

	public pause(): void {
		clearInterval(this.playtime_timer);
		if (this.controls_element.classList.contains("playing")) {
			this.controls_element.classList.remove("playing");
		}
		this.visualizer.stop();
	}

	// Updates play timer displayed on the UI.
	public update_playtime(): void {
		var seconds = this.parent.getHelper("PlaylistHelper").getSongTime();
		(<HTMLElement>this.element.getElementsByClassName("playtimer")[0]).innerHTML = Math.floor(seconds / 60) + ":" + ("0" + Math.floor(seconds % 60)).slice(-2);
	}
} 