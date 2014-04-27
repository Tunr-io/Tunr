/**
 * Tunr
 * This is the class that initializes the web-app and starts everything rolling.
 */
class Tunr {
	public static instance: Tunr;
	constructor() {
		Tunr.instance = this; // Set the current running instance...
		var l = new Login(); // Just show a log in for now.
		l.show();
	}
}

// Start 'er up.
new Tunr();