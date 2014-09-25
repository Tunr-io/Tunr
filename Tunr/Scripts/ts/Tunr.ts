/**
 * Tunr
 * This is the class that initializes the web-app and starts everything rolling.
 */
class Tunr {
	public static instance: Tunr;
	public api: API;
	public hub: TunrHub;
	public library: Library;
	public player: Player;

	constructor() {
		Tunr.instance = this; // Set the current running instance...
		this.api = new API(); // Instantiate the API.
		var l = new Login(this); // Just show a log in for now.
		l.show();
	}

	initialize(): void {
		this.library = new Library(this);
		this.player = new Player(this);
		this.player.show();

		this.hub = new TunrHub(this); // Instantiate the SignalR Hub.
		this.hub.connect();

		window.onresize = () => {
			this.windowResize();
		};
	}

	windowResize() {
		if (this.player != null) {
			this.player.windowResize();
		}
	}
}

window.debug = false; // This can be overridden by Debug.ts

// Start 'er up.
new Tunr();