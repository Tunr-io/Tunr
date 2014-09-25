interface SignalR {
	tunrHub: any;
}

class TunrHub {
	private tunr: Tunr;
	private hub = $.connection.tunrHub;
	public ready: boolean = false;

	constructor(tunr: Tunr) {
		this.tunr = tunr;
		this.hub.client.newSong = (s: Song) => {
			this.newSong(s);
		};
	}

	public connect() {
		if (window.debug) {
			$.connection.hub.logging = true;
		}
		$.connection.hub.start().done(() => {
			this.ready = true;
		});
	}

	private newSong(s: Song) {
		if (window.debug) {
			console.log("NEW SONG ADDED TO LIBRARY:");
			console.dir(s);
		}
		this.tunr.library.addSong(s);
		//this.tunr.librarypane.loadArtists();
	}
}