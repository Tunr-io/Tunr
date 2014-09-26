var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var LibraryHelper = (function (_super) {
    __extends(LibraryHelper, _super);
    function LibraryHelper() {
        _super.apply(this, arguments);
        this.uploading_count = 0;
    }
    LibraryHelper.prototype.init = function () {
        var _this = this;
        this.nav_element = this.element.getElementsByTagName("nav")[0];
        this.artists_element = this.element.getElementsByClassName("artists")[0];
        this.albums_element = this.element.getElementsByClassName("albums")[0];
        this.songs_element = this.element.getElementsByClassName("songs")[0];

        this.nav_element.getElementsByTagName("a")[0].addEventListener("click", function () {
            _this.showArtists();
        });
        this.nav_element.getElementsByTagName("a")[1].addEventListener("click", function () {
            _this.showAlbums();
        });
        this.nav_element.getElementsByTagName("a")[2].addEventListener("click", function () {
            _this.showSongs();
        });
        this.load();

        // Prepare drag and drop uploading
        this.element.addEventListener("dragenter", function (e) {
            _this.drag_target = e.target;
            if (!_this.element.classList.contains("dragging")) {
                _this.element.classList.add("dragging");
            }
            e.stopPropagation();
            e.preventDefault();
        }, false);
        this.element.addEventListener("dragover", function (e) {
            e.stopPropagation();
            e.preventDefault();
        });
        this.element.addEventListener("dragleave", function (e) {
            if (e.target == _this.drag_target) {
                if (_this.element.classList.contains("dragging")) {
                    _this.element.classList.remove("dragging");
                }
            }
            e.stopPropagation();
            e.preventDefault();
        }, false);
        this.element.addEventListener("drop", function (e) {
            if (_this.element.classList.contains("dragging")) {
                _this.element.classList.remove("dragging");
            }
            _this.readFiles(e.dataTransfer.files);
            _this.drag_target = null;
            e.stopPropagation();
            e.preventDefault();
        }, false);
    };

    LibraryHelper.prototype.readFiles = function (files) {
        var _this = this;
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }

        // now post a new XHR request
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/api/Library');
        xhr.setRequestHeader("Authorization", "Bearer " + this.parent.getTunr().api.getAuthentication().get_access_token());
        xhr.onload = function () {
            // TODO: Report errors to user.
            _this.uploading_count--;
            if (_this.uploading_count <= 0) {
                if (_this.element.classList.contains("uploading")) {
                    _this.element.classList.remove("uploading");
                }
            }
        };

        xhr.upload.onprogress = function (event) {
            if (event.lengthComputable) {
                var complete = (event.loaded / event.total * 100 | 0);
                console.log("Uploading track: " + complete + "%");
            }
        };
        this.uploading_count++;
        if (!this.element.classList.contains("uploading")) {
            this.element.classList.add("uploading");
        }
        xhr.send(formData);
    };

    LibraryHelper.prototype.load = function () {
        var _this = this;
        this.parent.getTunr().library.load().then(function () {
            _this.loadArtists();
        }, function () {
            console.error("failed to load library.");
        });
    };

    LibraryHelper.prototype.loadArtists = function () {
        var _this = this;
        var artists = this.parent.getTunr().library.artists();
        this.artists_element.innerHTML = ''; // clear existing entries
        for (var i = 0; i < artists.length; i++) {
            var li = document.createElement("li");
            li.innerHTML = htmlEscape(artists[i]);
            (function (artist, element) {
                TiltEffect.addTilt(element);
                element.addEventListener("click", function (e) {
                    _this.loadAlbums(artist);
                });
            })(artists[i], li);
            this.artists_element.appendChild(li);
        }
    };

    LibraryHelper.prototype.showArtists = function () {
        this.element.classList.remove("back");
        this.element.classList.add("forward");
        this.element.classList.add("showArtists");
        this.element.classList.remove("showAlbums");
        this.element.classList.remove("showSongs");
    };

    LibraryHelper.prototype.showAlbums = function () {
        if (this.element.classList.contains("showArtists")) {
            this.element.classList.add("forward");
            this.element.classList.remove("back");
        } else {
            this.element.classList.remove("forward");
            this.element.classList.add("back");
        }
        this.element.classList.remove("showArtists");
        this.element.classList.add("showAlbums");
        this.element.classList.remove("showSongs");
    };

    LibraryHelper.prototype.showSongs = function () {
        this.element.classList.remove("back");
        this.element.classList.add("forward");
        this.element.classList.remove("showArtists");
        this.element.classList.remove("showAlbums");
        this.element.classList.add("showSongs");
    };

    LibraryHelper.prototype.loadAlbums = function (artist) {
        var _this = this;
        var albums = this.parent.getTunr().library.albumsin(artist);
        this.nav_element.getElementsByTagName("a")[1].innerText = artist;
        this.albums_element.innerHTML = "";
        for (var i = 0; i < albums.length; i++) {
            var img = document.createElement("img");
            img.src = '/api/LibraryData/' + urlEscape(artist) + '/' + urlEscape(albums[i]) + '/art';
            img.alt = albums[i];
            img.style.opacity = '0';
            (function (imgel) {
                imgel.addEventListener("load", function (ev) {
                    imgel.style.opacity = '1';
                });
            })(img);
            var li = document.createElement("li");
            li.appendChild(img);
            (function (album, element) {
                TiltEffect.addTilt(element);
                element.addEventListener("click", function () {
                    _this.loadSongs(album, artist);
                });
            })(albums[i], li);
            this.albums_element.appendChild(li);
        }
        this.showAlbums();
    };

    LibraryHelper.prototype.loadSongs = function (album, artist) {
        var _this = this;
        var songs = this.parent.getTunr().library.songsin(album, artist);
        this.nav_element.getElementsByTagName("a")[2].innerText = album;
        this.songs_element.innerHTML = "";
        for (var i = 0; i < songs.length; i++) {
            var li = document.createElement("li");
            li.innerHTML = '<span class="track">' + ('0' + songs[i].trackNumber).slice(-2) + '</span>' + htmlEscape(songs[i].title);
            (function (song, element) {
                TiltEffect.addTilt(element);
                element.addEventListener("click", function () {
                    _this.selectSong(song);
                });
            })(songs[i], li);
            this.songs_element.appendChild(li);
        }
        this.showSongs();
    };

    LibraryHelper.prototype.songActions = function (element, song) {
    };

    LibraryHelper.prototype.selectSong = function (song) {
        var playlistHelper = this.parent.getHelper("PlaylistHelper");
        playlistHelper.addSong(song);
    };
    return LibraryHelper;
})(Helper);
//# sourceMappingURL=LibraryHelper.js.map
