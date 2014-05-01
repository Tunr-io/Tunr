var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var LibraryPane = (function (_super) {
    __extends(LibraryPane, _super);
    function LibraryPane(tunr) {
        _super.call(this, tunr, "LibraryPane");
        this.load();
    }
    LibraryPane.prototype.load = function () {
        var _this = this;
        this.getTunr().library.load().then(function () {
            _this.loadArtists();
        }, function () {
            console.error("failed to load library.");
        });
    };

    LibraryPane.prototype.loadArtists = function () {
        var artists = this.getTunr().library.artists();
        var artist_element = this.getElement().getElementsByClassName("artists")[0];
        artist_element.innerHTML = ''; // clear existing entries
        for (var i = 0; i < artists.length; i++) {
            var li = document.createElement("li");
            li.innerText = artists[i];
            artist_element.appendChild(li);
        }
    };
    return LibraryPane;
})(Component);
//# sourceMappingURL=LibraryPane.js.map
