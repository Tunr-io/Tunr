/**
* Tunr
* This is the class that initializes the web-app and starts everything rolling.
*/
var Tunr = (function () {
    function Tunr() {
        Tunr.instance = this; // Set the current running instance...
        this.api = new API(); // Instantiate the API.
        var l = new Login(this);
        l.show();
    }
    Tunr.prototype.initialize = function () {
        this.librarypane = new LibraryPane(this);
        this.librarypane.show();
        this.playlistpane = new PlaylistPane(this);
        this.playlistpane.show();
        this.playingpane = new PlayingPane(this);
        this.playingpane.show();
    };
    return Tunr;
})();

// Start 'er up.
new Tunr();
//# sourceMappingURL=Tunr.js.map
