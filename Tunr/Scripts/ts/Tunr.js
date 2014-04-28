/**
* Tunr
* This is the class that initializes the web-app and starts everything rolling.
*/
var Tunr = (function () {
    function Tunr() {
        Tunr.instance = this; // Set the current running instance...
        var l = new Login();
        l.show();
    }
    Tunr.prototype.initialize = function () {
        this.librarypane = new LibraryPane();
        this.librarypane.show();
        this.playlistpane = new PlaylistPane();
        this.playlistpane.show();
        this.playingpane = new PlayingPane();
        this.playingpane.show();
    };
    return Tunr;
})();

// Start 'er up.
new Tunr();
//# sourceMappingURL=Tunr.js.map
