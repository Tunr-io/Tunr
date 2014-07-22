class PlayingPane extends Component {
	private song: Song;
	private title_element: HTMLDivElement;
	private controls_element: HTMLUListElement;
	private playtime_timer: number;

	constructor(tunr: Tunr) {
		super(tunr, "PlayingPane");
		this.title_element = <HTMLDivElement>this.getElement().getElementsByClassName("title")[0];
		this.controls_element = <HTMLUListElement>this.getElement().getElementsByClassName("controls")[0];
		// Set up event handlers for control buttons...
		TiltEffect.addTilt(<HTMLElement>this.controls_element.getElementsByClassName("play")[0]);
		TiltEffect.addTilt(<HTMLElement>this.controls_element.getElementsByClassName("pause")[0]);
		TiltEffect.addTilt(<HTMLElement>this.controls_element.getElementsByClassName("next")[0]);
		TiltEffect.addTilt(<HTMLElement>this.controls_element.getElementsByClassName("prev")[0]);
		this.controls_element.getElementsByClassName("play")[0].addEventListener("click", () => {
			this.getTunr().playlistpane.play();
		});
		this.controls_element.getElementsByClassName("pause")[0].addEventListener("click", () => {
			this.getTunr().playlistpane.pause();
		});
		this.controls_element.getElementsByClassName("next")[0].addEventListener("click", () => {
			this.getTunr().playlistpane.next();
		});
		this.controls_element.getElementsByClassName("prev")[0].addEventListener("click", () => {
			this.getTunr().playlistpane.prev();
		});
	}

	public changeSong(song: Song): void {
		// Set up the new title
		var title = document.createElement("div");
		title.classList.add("title");
		title.classList.add("animated");
		title.classList.add("anim_playingtitle_in");
		title.innerHTML = '<h1>' + htmlEscape(song.title) + '</h1><h2>' + htmlEscape(song.artist) + '</h2>';

		// Title animations
		var title = <HTMLDivElement>this.getElement().insertBefore(title, this.title_element);
		this.title_element.classList.remove("anim_playingtitle_in");
		this.title_element.classList.add("anim_playingtitle_out");

		// Remove the old title after the animation completes.
		setTimeout(() => {
			this.getElement().removeChild(this.title_element);
			this.title_element = title;
		}, 300);

		var oldArt = <HTMLImageElement>(this.getElement().getElementsByTagName("img")[0]);
		oldArt.classList.add("animated");
		oldArt.classList.add("anim_albumart_out");

		// Set up the new art
		var art = document.createElement("img");
		art.src = '/api/LibraryData/' + urlEscape(song.artist) + '/' + urlEscape(song.album) + '/art';
		art.alt = song.album;
		art.classList.add("animated");
		art.classList.add("anim_albumart_in");

		oldArt.parentElement.appendChild(art);

		// Remove the old art after the animation completes.
		setTimeout(() => {
			oldArt.parentElement.removeChild(oldArt);
		}, 300);
	}

	public play(): void {
		clearInterval(this.playtime_timer);
		this.playtime_timer = setInterval(() => {
			this.update_playtime();
		}, 500);
		if (!this.controls_element.classList.contains("playing")) {
			this.controls_element.classList.add("playing");
		}
	}

	public pause(): void {
		clearInterval(this.playtime_timer);
		if (this.controls_element.classList.contains("playing")) {
			this.controls_element.classList.remove("playing");
		}
	}

	// Updates play timer displayed on the UI.
	public update_playtime(): void {
		var seconds = this.getTunr().playlistpane.getSongTime();
		(<HTMLElement>this.getElement().getElementsByClassName("playtimer")[0]).innerHTML = Math.floor(seconds / 60) + ":" + ("0" + Math.floor(seconds % 60)).slice(-2);
	}
} 