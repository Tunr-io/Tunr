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
    return Playlist;
})();
//# sourceMappingURL=Playlist.js.map
