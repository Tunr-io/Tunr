var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var LibraryPane = (function (_super) {
    __extends(LibraryPane, _super);
    function LibraryPane(tunr) {
        var _this = this;
        _super.call(this, tunr, "LibraryPane");
        this.nav_element = this.getElement().getElementsByTagName("nav")[0];
        this.artists_element = this.getElement().getElementsByClassName("artists")[0];
        this.albums_element = this.getElement().getElementsByClassName("albums")[0];
        this.songs_element = this.getElement().getElementsByClassName("songs")[0];

        this.nav_element.getElementsByTagName("a")[0].addEventListener("click", function () {
            _this.showArtists();
        });
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
        var _this = this;
        var artists = this.getTunr().library.artists();
        this.artists_element.innerHTML = ''; // clear existing entries
        for (var i = 0; i < artists.length; i++) {
            var li = document.createElement("li");
            li.innerText = artists[i];
            (function (artist, element) {
                element.addEventListener("click", function () {
                    _this.loadAlbums(artist);
                });
            })(artists[i], li);
            this.artists_element.appendChild(li);
        }
    };

    LibraryPane.prototype.showArtists = function () {
        // Hide other things
        if (!this.albums_element.classList.contains("hidden")) {
            this.albums_element.classList.add("hidden");
        }
        if (!this.songs_element.classList.contains("hidden")) {
            this.songs_element.classList.add("hidden");
        }

        // Remove extraneous nav elements
        var navs = this.nav_element.getElementsByTagName("a");
        for (var i = navs.length - 1; i > 0; i--) {
            this.nav_element.removeChild(navs[i]);
        }

        // This shouldn't change content.
        if (this.artists_element.classList.contains("hidden")) {
            this.artists_element.classList.remove("hidden");
        }
    };

    LibraryPane.prototype.showAlbums = function () {
        // Hide other things
        if (!this.artists_element.classList.contains("hidden")) {
            this.artists_element.classList.add("hidden");
        }
        if (!this.songs_element.classList.contains("hidden")) {
            this.songs_element.classList.add("hidden");
        }

        // Remove extraneous nav elements
        var navs = this.nav_element.getElementsByTagName("a");
        for (var i = navs.length - 1; i > 1; i--) {
            this.nav_element.removeChild(navs[i]);
        }

        // This shouldn't change content.
        if (this.albums_element.classList.contains("hidden")) {
            this.albums_element.classList.remove("hidden");
        }
    };

    LibraryPane.prototype.loadAlbums = function (artist) {
        var _this = this;
        var albums = this.getTunr().library.albumsin(artist);
        this.albums_element.innerHTML = "";
        for (var i = 0; i < albums.length; i++) {
            var li = document.createElement("li");
            li.innerText = albums[i];
            (function (album, element) {
                element.addEventListener("click", function () {
                    _this.loadSongs(album);
                });
            })(albums[i], li);
            this.albums_element.appendChild(li);
        }

        // Add header
        var nav = document.createElement("a");
        nav.innerText = artist;
        nav.addEventListener("click", function () {
            _this.showAlbums();
        });
        this.nav_element.appendChild(nav);

        // Hide artists
        this.artists_element.classList.add("hidden");

        // Show albums
        this.albums_element.classList.remove("hidden");
    };

    LibraryPane.prototype.loadSongs = function (album) {
        var songs = this.getTunr().library.songsin(album);
        this.songs_element.innerHTML = "";
        for (var i = 0; i < songs.length; i++) {
            var li = document.createElement("li");
            li.innerHTML = '<span class="track">' + ('00').slice(-2) + '</span>' + htmlEscape(songs[i]); //TODO: we need the track number
            (function (song, element) {
                element.addEventListener("click", function () {
                    console.log("played song" + song);
                });
            })(songs[i], li);
            this.songs_element.appendChild(li);
        }

        // Add header
        var nav = document.createElement("a");
        nav.innerText = album;
        this.nav_element.appendChild(nav);

        // Hide albums
        this.albums_element.classList.add("hidden");

        // Show songs
        this.songs_element.classList.remove("hidden");
    };
    return LibraryPane;
})(Component);
//# sourceMappingURL=LibraryPane.js.map
