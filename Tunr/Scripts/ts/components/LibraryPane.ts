class LibraryPane extends Component {
	constructor(tunr: Tunr) {
		super(tunr, "LibraryPane");
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
		var artist_element: HTMLUListElement = <HTMLUListElement>this.getElement().getElementsByClassName("artists")[0];
		artist_element.innerHTML = ''; // clear existing entries
		for (var i = 0; i < artists.length; i++) {
			var li = document.createElement("li");
			li.innerText = artists[i];
			artist_element.appendChild(li);
		}
	}
} 