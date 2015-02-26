var TunrHub = (function () {
    function TunrHub(tunr) {
        var _this = this;
        this.hub = $.connection.tunrHub;
        this.ready = false;
        this.tunr = tunr;
        this.hub.client.newSong = function (s) {
            _this.newSong(s);
        };
    }
    TunrHub.prototype.connect = function () {
        var _this = this;
        if (window.debug) {
            $.connection.hub.logging = true;
        }
        $.connection.hub.start().done(function () {
            _this.ready = true;
        });
    };

    TunrHub.prototype.newSong = function (s) {
        if (window.debug) {
            console.log("NEW SONG ADDED TO LIBRARY:");
            console.dir(s);
        }
        this.tunr.library.addSong(s);
        this.tunr.player.getHelper("LibraryHelper").list_helpers[0].init(new Song());
    };
    return TunrHub;
})();
//# sourceMappingURL=TunrHub.js.map
