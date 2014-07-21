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
        //private playlist: Playlist;
        this.audio = new Audio();
        this.playstate = 2 /* STOPPED */;
        this.repeat = false;
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

    /**
    * Retreives the song object at the specified index of the playlist.
    */
    PlaylistPane.prototype.get_song_at = function (index) {
        var element = this.get_element_at(index);
        var songID = element.attributes["data-song-id"];
        return this.getTunr().library.get_song_by_id(songID);
    };

    /**
    * Gets the playlist HTML element at the specified playlist index.
    */
    PlaylistPane.prototype.get_element_at = function (index) {
        return this.songlist_element.getElementsByClassName("playlistitem")[index];
    };

    /**
    * Returns the number of items in the playlist.
    */
    PlaylistPane.prototype.count = function () {
        return this.songlist_element.getElementsByClassName("playlistitem").length;
    };

    /**
    * Gets the index of the specified playlist element.
    */
    PlaylistPane.prototype._find_index_of_element = function (element) {
        var song_elements = this.songlist_element.getElementsByClassName("playlistitem");
        var i;
        for (i = 0; i < song_elements.length; i++) {
            if (song_elements[i] == element) {
                break;
            }
        }
        return i;
    };

    /**
    * Gets the index of the currently playing playlist item
    */
    PlaylistPane.prototype.get_playing_index = function () {
        var playing = this.songlist_element.getElementsByClassName("playing");
        if (playing.length <= 0) {
            return -1;
        }
        var playing_element = playing[0];
        var i;
        for (i = 0; i < this.count(); i++) {
            if (this.get_element_at(i) == playing_element) {
                return i;
            }
        }
        return -1;
    };

    // Gets how many seconds have elapsed since the song started.
    PlaylistPane.prototype.getSongTime = function () {
        return this.audio.currentTime;
    };

    // Audio controls
    PlaylistPane.prototype.play = function () {
        if (this.playstate == 1 /* PAUSED */) {
            // If paused, just resume ...
            this.audio.play();
            this._setPlayState(0 /* PLAYING */);
        } else if (this.playstate == 2 /* STOPPED */) {
            if (this.count() > 0) {
                // If stopped, start from the beginning
                this.playIndex(0);
            }
            // If we have no tracks, do nothing.
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
        var song = this.get_song_at(index);

        // Change the audio source
        this.audio.src = "/api/Library/" + song.songId;

        // Change play listing
        this.getTunr().playingpane.changeSong(song);

        // Play it
        this.audio.play();
        this._setPlayState(0 /* PLAYING */);

        // Clear play markers on all list items.
        var song_elements = this.songlist_element.getElementsByClassName("playing");
        for (var i = 0; i < song_elements.length; i++) {
            song_elements[i].classList.remove("playing");
        }

        // Add play marker to current song.
        var song_element = this.get_element_at(index);
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
        song_element.attributes["data-song-id"] = song.songId;
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
            var i = _this._find_index_of_element(clicked_element);

            //this.playIndex(i);
            _this.show_controls_at(i);
        });
        return song_element;
    };

    PlaylistPane.prototype.set_controls_timeout = function () {
        var _this = this;
        clearTimeout(this.controls_timeout);
        this.controls_timeout = setTimeout(function () {
            _this.hide_controls();
        }, 4000);
    };

    PlaylistPane.prototype.hide_controls = function () {
        if (this.controls_element !== undefined && this.controls_element.parentElement !== undefined && this.controls_element.parentElement != null) {
            this.controls_element.parentElement.classList.remove("dim");
            this.controls_element.parentElement.removeChild(this.controls_element);
        }
    };

    PlaylistPane.prototype.show_controls_at = function (index) {
        var _this = this;
        this.hide_controls();
        var song_element = this.get_element_at(index);

        // Make us some controls ...
        var itemControls = document.createElement("ul");
        itemControls.classList.add("controls");

        // Play button
        var b_play = document.createElement("li");
        b_play.classList.add("play");
        b_play.addEventListener("click", function () {
            _this.playIndex(_this.controls_index);
        });
        itemControls.appendChild(b_play);

        // Remove button
        var b_remove = document.createElement("li");
        b_remove.classList.add("remove");
        itemControls.appendChild(b_remove);
        b_remove.addEventListener("click", function () {
            _this.remove_by_index(_this.controls_index);
        });

        // Up button
        var b_up = document.createElement("li");
        b_up.classList.add("up");
        itemControls.appendChild(b_up);
        b_up.addEventListener("click", function () {
            var targetIndex = _this.controls_index - 1;
            if (targetIndex < 0) {
                targetIndex = _this.count() - 1;
            }
            _this.move_song(_this.controls_index, targetIndex);
            _this.controls_index = targetIndex;
            _this.set_controls_timeout();
        });

        // Down button
        var b_down = document.createElement("li");
        b_down.classList.add("down");
        itemControls.appendChild(b_down);
        b_down.addEventListener("click", function () {
            var targetIndex = _this.controls_index + 1;
            if (targetIndex >= _this.count()) {
                targetIndex = 0;
            }
            _this.move_song(_this.controls_index, targetIndex);
            _this.controls_index = targetIndex;
            _this.set_controls_timeout();
        });

        song_element.classList.add("dim");
        this.controls_element = song_element.appendChild(itemControls);
        this.controls_index = index;

        TiltEffect.addTilt(this.controls_element);
        this.set_controls_timeout();
    };

    PlaylistPane.prototype.addSong = function (song) {
        var song_element = this._renderElement(song);

        if (this.playstate == 2 /* STOPPED */) {
            this.playIndex(this.count() - 1);
        }
    };

    PlaylistPane.prototype.move_song = function (srcIndex, targetIndex) {
        if (srcIndex == targetIndex) {
            // If they're equal why are we moving ...?
            return;
        }
        var item = this.get_element_at(srcIndex);

        if (targetIndex > srcIndex) {
            // Moving to a higher index
            if (targetIndex == this.count() - 1) {
                //if next element is the last one, we hit the end.
                item = item.parentElement.removeChild(item);
                this.songlist_element.appendChild(item);
            } else {
                // Insert before the element after the next element
                var target = this.get_element_at(targetIndex + 1);
                item = item.parentElement.removeChild(item);
                this.songlist_element.insertBefore(item, target);
            }
        } else {
            // Moving to a lower index
            // Insert before the element
            var target = this.get_element_at(targetIndex);
            item = item.parentElement.removeChild(item);
            this.songlist_element.insertBefore(item, target);
        }
    };

    PlaylistPane.prototype.remove_by_index = function (index) {
        if (this.get_playing_index() == index) {
            if (this.count() == 1) {
                this.stop();
            } else {
                this.next();
            }
        }
        var item = this.get_element_at(index);
        item = item.parentElement.removeChild(item);
    };

    PlaylistPane.prototype.nextIndex = function () {
        var nextIndex = this.get_playing_index() + 1;
        if (nextIndex >= this.count()) {
            nextIndex = 0;
        }
        return nextIndex;
    };

    PlaylistPane.prototype.prevIndex = function () {
        var prevIndex = this.get_playing_index() - 1;
        if (prevIndex < 0) {
            prevIndex = this.count() - 1;
        }
        return prevIndex;
    };
    return PlaylistPane;
})(Component);
//# sourceMappingURL=PlaylistPane.js.map
