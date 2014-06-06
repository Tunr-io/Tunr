var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PlayState;
(function (PlayState) {
    PlayState[PlayState["PLAYING"] = 0] = "PLAYING";
    PlayState[PlayState["PAUSED"] = 1] = "PAUSED";
    PlayState[PlayState["STOPPED"] = 2] = "STOPPED";
})(PlayState || (PlayState = {}));
;

var PlaylistPane = (function (_super) {
    __extends(PlaylistPane, _super);
    function PlaylistPane(tunr) {
        var _this = this;
        _super.call(this, tunr, "PlaylistPane");
        this.audio = new Audio();
        this.playstate = 2 /* STOPPED */;
        this.repeat = false;
        this.playlist = new Playlist("Playlist");
        this.songlist_element = this.getElement().getElementsByClassName("songlist")[0];
        this.audio.onended = function (e) {
            _this.trackEnded();
        };
    }
    PlaylistPane.prototype._setPlayState = function (state) {
        this.playstate = state;
        if (this.playstate == 0 /* PLAYING */) {
            this.getTunr().playingpane.play();
        } else if (this.playstate == 1 /* PAUSED */ || this.playstate == 2 /* STOPPED */) {
            this.getTunr().playingpane.pause();
        }
    };

    // Audio controls
    PlaylistPane.prototype.play = function () {
        if (this.playstate == 1 /* PAUSED */) {
            // If paused, just resume ...
            this.audio.play();
            this._setPlayState(0 /* PLAYING */);
        } else if (this.playstate == 2 /* STOPPED */) {
            if (this.playlist.getCount() > 0) {
                // If stopped, start from the beginning
                this.playIndex(0);
            }
        }
    };

    PlaylistPane.prototype.pause = function () {
        if (this.playstate == 0 /* PLAYING */) {
            this.audio.pause();
            this._setPlayState(1 /* PAUSED */);
        }
    };

    PlaylistPane.prototype.stop = function () {
        if (this.playstate != 2 /* STOPPED */) {
            if (this.playstate == 0 /* PLAYING */) {
                this.audio.pause();
            }
            this.audio = new Audio(); // Reset audio

            // Clear play markers on all list items.
            var song_elements = this.songlist_element.getElementsByClassName("playing");
            for (var i = 0; i < song_elements.length; i++) {
                song_elements[i].classList.remove("playing");
            }
            this._setPlayState(2 /* STOPPED */);
        }
    };

    PlaylistPane.prototype.next = function () {
        var next = this.nextIndex();
        if (next >= 0) {
            this.playIndex(next);
        } else {
            this.stop();
        }
    };

    PlaylistPane.prototype.prev = function () {
        var next = this.prevIndex();
        if (next >= 0) {
            this.playIndex(next);
        } else {
            this.stop();
        }
    };

    PlaylistPane.prototype.playIndex = function (index) {
        var song = this.playlist.getSong(index);

        // Change the audio source
        this.audio.src = "/api/Library/" + song.songID;

        // Change play listing
        this.getTunr().playingpane.changeSong(song);

        // Play it
        this.audio.play();
        this._setPlayState(0 /* PLAYING */);

        // Set index of current track
        this.playingIndex = index;

        // Clear play markers on all list items.
        var song_elements = this.songlist_element.getElementsByClassName("playing");
        for (var i = 0; i < song_elements.length; i++) {
            song_elements[i].classList.remove("playing");
        }

        // Add play marker to current song.
        var song_element = this.songlist_element.getElementsByClassName("playlistitem")[index];
        song_element.classList.add("playing");
    };

    PlaylistPane.prototype.trackEnded = function () {
        this.next();
    };

    PlaylistPane.prototype._renderElement = function (song) {
        var _this = this;
        var song_element = document.createElement("li");
        song_element.classList.add("playlistitem");
        song_element.classList.add("animated");
        song_element.classList.add("anim_playlistitem_in");
        song_element.innerHTML = '<div class="listing"><span class="title">' + htmlEscape(song.title) + '</span><br /><span class="artist">' + htmlEscape(song.artist) + '</span></div>';
        song_element = this.songlist_element.appendChild(song_element);
        song_element.getElementsByClassName("listing")[0].addEventListener("click", function (e) {
            var clicked_element = e.target;
            while (clicked_element.tagName.toLowerCase() != "li") {
                clicked_element = clicked_element.parentElement;
                if (clicked_element == null) {
                    return;
                }
            }
            var song_elements = _this.songlist_element.getElementsByClassName("playlistitem");
            var i;
            for (i = 0; i < song_elements.length; i++) {
                if (song_elements[i] == clicked_element) {
                    break;
                }
            }

            //this.playIndex(i);
            _this.showControls(i);
        });
        return song_element;
    };

    PlaylistPane.prototype.hideAllControls = function () {
        var playlistitems = (this.songlist_element.getElementsByClassName("playlistitem"));
        for (var i = 0; i < playlistitems.length; i++) {
            var controls = playlistitems[i].getElementsByClassName("controls");
            if (controls.length > 0) {
                this.hideControlsAt(i);
            }
        }
    };

    PlaylistPane.prototype.hideControlsAt = function (index) {
        var controls_element = this.songlist_element.getElementsByClassName("playlistitem")[index].getElementsByClassName("controls")[0];
        this.hideControls(controls_element);
    };

    PlaylistPane.prototype.hideControls = function (element) {
        element.parentElement.classList.remove("dim");
        element.parentElement.removeChild(element);
    };

    PlaylistPane.prototype.showControls = function (index) {
        var _this = this;
        this.hideAllControls();

        var song_element = this.songlist_element.getElementsByClassName("playlistitem")[index];

        // Make us some controls ...
        var itemControls = document.createElement("ul");
        itemControls.classList.add("controls");

        // Play button
        var b_play = document.createElement("li");
        b_play.classList.add("play");
        b_play.addEventListener("click", function () {
            _this.playIndex(index);
            _this.hideControls(b_play.parentElement);
        });
        itemControls.appendChild(b_play);

        // Remove button
        var b_remove = document.createElement("li");
        b_remove.classList.add("remove");
        itemControls.appendChild(b_remove);

        // Up button
        var b_up = document.createElement("li");
        b_up.classList.add("up");
        itemControls.appendChild(b_up);

        // Down button
        var b_down = document.createElement("li");
        b_down.classList.add("down");
        itemControls.appendChild(b_down);

        song_element.classList.add("dim");
        song_element.appendChild(itemControls);
    };

    PlaylistPane.prototype.addSong = function (song) {
        this.playlist.addSong(song);
        var song_element = this._renderElement(song);

        if (this.playstate == 2 /* STOPPED */) {
            this.playIndex(this.playlist.getCount() - 1);
        }
    };

    PlaylistPane.prototype.nextIndex = function () {
        var nextIndex = this.playingIndex + 1;
        if (nextIndex >= this.playlist.getCount()) {
            nextIndex = 0;
        }
        return nextIndex;
    };

    PlaylistPane.prototype.prevIndex = function () {
        var prevIndex = this.playingIndex - 1;
        if (prevIndex < 0) {
            prevIndex = this.playlist.getCount() - 1;
        }
        return prevIndex;
    };
    return PlaylistPane;
})(Component);
//# sourceMappingURL=PlaylistPane.js.map
