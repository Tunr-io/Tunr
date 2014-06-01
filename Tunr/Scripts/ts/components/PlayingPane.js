var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PlayingPane = (function (_super) {
    __extends(PlayingPane, _super);
    function PlayingPane(tunr) {
        var _this = this;
        _super.call(this, tunr, "PlayingPane");
        this.title_element = this.getElement().getElementsByClassName("title")[0];
        this.controls_element = this.getElement().getElementsByClassName("controls")[0];

        // Set up event handlers for control buttons...
        TiltEffect.addTilt(this.controls_element.getElementsByClassName("play")[0]);
        TiltEffect.addTilt(this.controls_element.getElementsByClassName("pause")[0]);
        TiltEffect.addTilt(this.controls_element.getElementsByClassName("next")[0]);
        TiltEffect.addTilt(this.controls_element.getElementsByClassName("prev")[0]);
        this.controls_element.getElementsByClassName("play")[0].addEventListener("click", function () {
            _this.getTunr().playlistpane.play();
        });
        this.controls_element.getElementsByClassName("pause")[0].addEventListener("click", function () {
            _this.getTunr().playlistpane.pause();
        });
        this.controls_element.getElementsByClassName("next")[0].addEventListener("click", function () {
            _this.getTunr().playlistpane.next();
        });
        this.controls_element.getElementsByClassName("prev")[0].addEventListener("click", function () {
            _this.getTunr().playlistpane.prev();
        });
    }
    PlayingPane.prototype.changeSong = function (song) {
        var _this = this;
        // Set up the new title
        var title = document.createElement("div");
        title.classList.add("title");
        title.classList.add("animated");
        title.classList.add("anim_playingtitle_in");
        title.innerHTML = '<h1>' + htmlEscape(song.title) + '</h1><h2>' + htmlEscape(song.artist) + '</h2>';

        // Title animations
        var title = this.getElement().insertBefore(title, this.title_element);
        this.title_element.classList.remove("anim_playingtitle_in");
        this.title_element.classList.add("anim_playingtitle_out");

        // Remove the old title after the animation completes.
        setTimeout(function () {
            _this.getElement().removeChild(_this.title_element);
            _this.title_element = title;
        }, 300);

        var oldArt = (this.getElement().getElementsByTagName("img")[0]);
        oldArt.classList.add("animated");
        oldArt.classList.add("anim_albumart_out");

        // Set up the new art
        var art = document.createElement("img");
        art.src = '/api/LibraryData/' + urlEscape(song.artist) + '/' + urlEscape(song.album) + '/art';
        art.alt = song.album;
        art.classList.add("animated");
        art.classList.add("anim_albumart_in");

        oldArt.parentElement.appendChild(art);

        // Remove the old art after the animation completes.
        setTimeout(function () {
            oldArt.parentElement.removeChild(oldArt);
        }, 300);
    };

    PlayingPane.prototype.play = function () {
        if (!this.controls_element.classList.contains("playing")) {
            this.controls_element.classList.add("playing");
        }
    };

    PlayingPane.prototype.pause = function () {
        if (this.controls_element.classList.contains("playing")) {
            this.controls_element.classList.remove("playing");
        }
    };
    return PlayingPane;
})(Component);
//# sourceMappingURL=PlayingPane.js.map
