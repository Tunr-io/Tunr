var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PlayingHelper = (function (_super) {
    __extends(PlayingHelper, _super);
    function PlayingHelper() {
        _super.apply(this, arguments);
    }
    PlayingHelper.prototype.init = function () {
        var _this = this;
        this.title_element = this.element.getElementsByClassName("title")[0];
        this.controls_element = this.element.getElementsByClassName("controls")[0];
        this.canvas_element = this.element.getElementsByTagName("canvas")[0];
        this.visualizer = new Visualizer(this, this.canvas_element);

        // Set up event handlers for control buttons...
        TiltEffect.addTilt(this.controls_element.getElementsByClassName("play")[0]);
        TiltEffect.addTilt(this.controls_element.getElementsByClassName("pause")[0]);
        TiltEffect.addTilt(this.controls_element.getElementsByClassName("next")[0]);
        TiltEffect.addTilt(this.controls_element.getElementsByClassName("prev")[0]);
        this.controls_element.getElementsByClassName("play")[0].addEventListener("click", function () {
            _this.parent.getHelper("PlaylistHelper").play();
        });
        this.controls_element.getElementsByClassName("pause")[0].addEventListener("click", function () {
            _this.parent.getHelper("PlaylistHelper").pause();
        });
        this.controls_element.getElementsByClassName("next")[0].addEventListener("click", function () {
            _this.parent.getHelper("PlaylistHelper").next();
        });
        this.controls_element.getElementsByClassName("prev")[0].addEventListener("click", function () {
            _this.parent.getHelper("PlaylistHelper").prev();
        });

        setTimeout(function () {
            // Initialize the canvas.
            _this.visualizer.init();
        }, 300);
    };

    PlayingHelper.prototype.getSong = function () {
        return this.song;
    };

    PlayingHelper.prototype.changeSong = function (song) {
        var _this = this;
        // Keep a reference of the song
        this.song = song;

        // Set up the new title
        var title = document.createElement("div");
        title.classList.add("title");
        title.classList.add("animated");
        title.classList.add("anim_playingtitle_in");
        title.innerHTML = '<h1>' + htmlEscape(song.tagTitle) + '</h1><h2>' + htmlEscape(song.tagPerformers[0]) + '</h2>';

        // Title animations
        var title = this.element.insertBefore(title, this.title_element);
        this.title_element.classList.remove("anim_playingtitle_in");
        this.title_element.classList.add("anim_playingtitle_out");

        // Remove the old title after the animation completes.
        setTimeout(function () {
            _this.element.removeChild(_this.title_element);
            _this.title_element = title;
        }, 300);

        var oldArt = (this.element.getElementsByTagName("img")[0]);
        oldArt.classList.add("animated");
        oldArt.classList.add("anim_albumart_out");

        // Set up the new art
        var art = document.createElement("img");
        art.src = '/api/LibraryData/' + urlEscape(song.tagPerformers[0]) + '/' + urlEscape(song.tagAlbum) + '/art';
        art.alt = song.tagAlbum;
        art.classList.add("animated");
        art.classList.add("anim_albumart_in");

        oldArt.parentElement.appendChild(art);

        // Remove the old art after the animation completes.
        setTimeout(function () {
            oldArt.parentElement.removeChild(oldArt);
        }, 300);
    };

    PlayingHelper.prototype.play = function () {
        var _this = this;
        clearInterval(this.playtime_timer);
        this.playtime_timer = setInterval(function () {
            _this.update_playtime();
        }, 500);
        if (!this.controls_element.classList.contains("playing")) {
            this.controls_element.classList.add("playing");
        }
        this.visualizer.start(this.song);
    };

    PlayingHelper.prototype.pause = function () {
        clearInterval(this.playtime_timer);
        if (this.controls_element.classList.contains("playing")) {
            this.controls_element.classList.remove("playing");
        }
        this.visualizer.stop();
    };

    // Updates play timer displayed on the UI.
    PlayingHelper.prototype.update_playtime = function () {
        var seconds = this.parent.getHelper("PlaylistHelper").getSongTime();
        this.element.getElementsByClassName("playtimer")[0].innerHTML = Math.floor(seconds / 60) + ":" + ("0" + Math.floor(seconds % 60)).slice(-2);
    };
    return PlayingHelper;
})(Helper);
//# sourceMappingURL=PlayingHelper.js.map
