var Playlist = (function () {
    function Playlist(name, songs) {
        this.name = name;
        if (songs) {
            this.songs = songs;
        } else {
            this.songs = new Array();
        }
    }
    Playlist.prototype.addSong = function (song) {
        this.songs.push(song);
    };

    Playlist.prototype.getSong = function (index) {
        return this.songs[index];
    };

    Playlist.prototype.getCount = function () {
        return this.songs.length;
    };

    Playlist.prototype.moveIndex = function (src, target) {
        if (target >= this.songs.length) {
            var k = target - this.songs.length;
            while ((k--) + 1) {
                this.songs.push(undefined);
            }
        }
        this.songs.splice(target, 0, this.songs.splice(src, 1)[0]);
    };
    return Playlist;
})();
//# sourceMappingURL=Playlist.js.map
