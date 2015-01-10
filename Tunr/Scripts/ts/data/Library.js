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
                // Find all the set properties of the condition
                if (typeof conditions[prop] !== 'undefined' && conditions[prop] != "") {
                    if (Array.isArray(conditions[prop])) {
                        for (var j in conditions[prop]) {
                            if (this.songs[i][prop].indexOf(conditions[prop][j]) < 0) {
                                add = false;
                                break;
                            }
                        }
                        if (!add) {
                            break;
                        }
                    } else {
                        // Otherwise just check that the one property matches.
                        if (conditions[prop] != this.songs[i][prop]) {
                            add = false;
                            break;
                        }
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
    * Fetches all of the unique values of the specified property in the library.
    */
    Library.prototype.fetchUniquePropertyValues = function (conditions, property) {
        var uniquePropertyValues = new Array();
        for (var i = 0; i < this.songs.length; i++) {
            var add = true;
            for (var prop in conditions) {
                // Find all the set properties of the condition
                if (typeof conditions[prop] !== 'undefined' && conditions[prop] != "") {
                    if (Array.isArray(conditions[prop])) {
                        for (var j in conditions[prop]) {
                            if (this.songs[i][prop].indexOf(conditions[prop][j]) < 0) {
                                add = false;
                                break;
                            }
                        }
                        if (!add) {
                            break;
                        }
                    } else {
                        // Otherwise just check that the one property matches.
                        if (conditions[prop] != this.songs[i][prop]) {
                            add = false;
                            break;
                        }
                    }
                }
            }
            if (add) {
                if (Array.isArray(this.songs[i][property])) {
                    var pushed = false;
                    for (var j in this.songs[i][property]) {
                        if (uniquePropertyValues.indexOf(this.songs[i][property][j]) < 0) {
                            uniquePropertyValues.push(this.songs[i][property][j]);
                        }
                    }
                } else {
                    if (uniquePropertyValues.indexOf(this.songs[i][property]) < 0) {
                        uniquePropertyValues.push(this.songs[i][property]);
                    }
                }
            }
        }
        return uniquePropertyValues;
    };

    /**
    * Filter the library and return songs that match the specified conditions,
    * whilst each having a unique value for the given property.
    */
    Library.prototype.filterUniqueProperty = function (conditions, property) {
        var uniquePropertyValues = new Array();
        var results = new Array();
        for (var i = 0; i < this.songs.length; i++) {
            var add = true;
            for (var prop in conditions) {
                // Find all the set properties of the condition
                if (typeof conditions[prop] !== 'undefined' && conditions[prop] != "") {
                    if (Array.isArray(conditions[prop])) {
                        for (var j in conditions[prop]) {
                            if (this.songs[i][prop].indexOf(conditions[prop][j]) < 0) {
                                add = false;
                                break;
                            }
                        }
                        if (!add) {
                            break;
                        }
                    } else {
                        // Otherwise just check that the one property matches.
                        if (conditions[prop] != this.songs[i][prop]) {
                            add = false;
                            break;
                        }
                    }
                }
            }
            if (add) {
                if (Array.isArray(this.songs[i][property])) {
                    var pushed = false;
                    for (var j in this.songs[i][property]) {
                        if (uniquePropertyValues.indexOf(this.songs[i][property][j]) < 0) {
                            uniquePropertyValues.push(this.songs[i][property][j]);
                            if (!pushed) {
                                results.push(this.songs[i]);
                                pushed = true;
                            }
                        }
                    }
                } else {
                    if (uniquePropertyValues.indexOf(this.songs[i][property]) < 0) {
                        uniquePropertyValues.push(this.songs[i][property]);
                        results.push(this.songs[i]);
                    }
                }
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
