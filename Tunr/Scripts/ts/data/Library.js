var Library = (function () {
    function Library(tunr) {
        this.songs = new Array();
        this.song_index = {};
        this.tunr = tunr;
    }
    Library.prototype.get_song_by_id = function (songid) {
        return this.song_index[songid];
    };

    Library.prototype.load = function () {
        var _this = this;
        var retval = $.Deferred();
        this.tunr.api.get("Library").then(function (songs) {
            _this.songs = songs;
            for (var i = 0; i < _this.songs.length; i++) {
                _this.song_index[_this.songs[i].songId] = _this.songs[i];
            }
            retval.resolve();
        }, function () {
            console.error("failed to retrieve library.");
            retval.reject();
        });
        return retval;
    };

    Library.prototype.addSong = function (s) {
        this.songs.push(s);
        this.song_index[s.songId] = s;
    };

    Library.prototype.filter = function (conditions) {
        var results = new Array();
        for (var i = 0; i < this.songs.length; i++) {
            var add = true;
            for (var prop in conditions) {
                if (typeof conditions[prop] !== 'undefined' && conditions[prop] != "") {
                    if (conditions[prop] != this.songs[i][prop]) {
                        add = false;
                        break;
                    }
                }
            }
            if (add) {
                results.push(this.songs[i]);
            }
        }
        return results;
    };

    /**
    * Filter the library and return songs that match the specified conditions,
    * whilst each having a unique value for the given property.
    */
    Library.prototype.filterUniqueProperty = function (conditions, property) {
        var uniqueProps = new Array();
        var results = new Array();
        for (var i = 0; i < this.songs.length; i++) {
            var add = true;
            for (var prop in conditions) {
                if (typeof conditions[prop] !== 'undefined' && conditions[prop] != "") {
                    if (conditions[prop] != this.songs[i][prop]) {
                        add = false;
                        break;
                    }
                }
            }
            if (add && uniqueProps.indexOf(this.songs[i][property]) < 0) {
                uniqueProps.push(this.songs[i][property]);
                results.push(this.songs[i]);
            }
        }
        return results;
    };
    return Library;
})();

var Song = (function () {
    function Song() {
    }
    return Song;
})();
//# sourceMappingURL=Library.js.map
