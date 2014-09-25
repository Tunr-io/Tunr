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
        var _this = this;
        this.library = new Library(this);
        this.player = new Player(this);
        this.player.show();

        this.hub = new TunrHub(this); // Instantiate the SignalR Hub.
        this.hub.connect();

        window.onresize = function () {
            _this.windowResize();
        };
    };

    Tunr.prototype.windowResize = function () {
        if (this.player != null) {
            this.player.windowResize();
        }
    };
    return Tunr;
})();

window.debug = false; // This can be overridden by Debug.ts

// Start 'er up.
new Tunr();
//# sourceMappingURL=Tunr.js.map
