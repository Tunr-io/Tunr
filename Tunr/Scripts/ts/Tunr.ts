/**
 * Tunr
 * This is the class that initializes the web-app and starts everything rolling.
 */
class Tunr {
	public static instance: Tunr;
	public api: API;
	public library: Library;
	public librarypane: LibraryPane;
	public playlistpane: PlaylistPane;
	public playingpane: PlayingPane;
	constructor() {
		Tunr.instance = this; // Set the current running instance...
		this.api = new API(); // Instantiate the API.
		var l = new Login(this); // Just show a log in for now.
		l.show();
	}

	initialize(): void {
		this.library = new Library(this);
		this.librarypane = new LibraryPane(this);
		this.librarypane.show();
		this.playlistpane = new PlaylistPane(this);
		this.playlistpane.show();
		this.playingpane = new PlayingPane(this);
		this.playingpane.show();
	}
}

// Start 'er up.
new Tunr();