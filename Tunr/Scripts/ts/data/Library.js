var Library = (function () {
    function Library(tunr) {
        this.songs = new Array();
        this.tunr = tunr;
    }
    Library.prototype.load = function () {
        var _this = this;
        var retval = $.Deferred();
        this.tunr.api.get("Library").then(function (songs) {
            _this.songs = songs;
            retval.resolve();
        }, function () {
            console.error("failed to retrieve library.");
            retval.reject();
        });
        return retval;
    };

    Library.prototype.artists = function () {
        var artists = new Array();
        for (var i = 0; i < this.songs.length; i++) {
            if (artists.indexOf(this.songs[i].artist) < 0) {
                artists.push(this.songs[i].artist);
            }
        }
        return artists.sort();
    };

    Library.prototype.albumsin = function (artist) {
        var albums = new Array();
        for (var i = 0; i < this.songs.length; i++) {
            if (this.songs[i].artist == artist && albums.indexOf(this.songs[i].album) < 0) {
                albums.push(this.songs[i].album);
            }
        }
        return albums.sort();
    };

    Library.prototype.albums = function () {
        var albums = new Array();
        for (var i = 0; i < this.songs.length; i++) {
            if (albums.indexOf(this.songs[i].album) < 0) {
                albums.push(this.songs[i].album);
            }
        }
        return albums;
    };

    Library.prototype.songsin = function (album, artist) {
        var songs = new Array();
        for (var i = 0; i < this.songs.length; i++) {
            if (this.songs[i].album == album) {
                if (typeof artist === undefined || this.songs[i].artist == artist) {
                    songs.push(this.songs[i]);
                }
            }
        }
        return songs.sort(function (a, b) {
            return a.trackNumber - b.trackNumber;
        });
    };
    return Library;
})();

var Song = (function () {
    function Song() {
    }
    return Song;
})();
//# sourceMappingURL=Library.js.map
