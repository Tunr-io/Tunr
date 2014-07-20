﻿var __extends = this.__extends || function (d, b) {
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
        this.uploading_count = 0;
        this.nav_element = this.getElement().getElementsByTagName("nav")[0];
        this.artists_element = this.getElement().getElementsByClassName("artists")[0];
        this.albums_element = this.getElement().getElementsByClassName("albums")[0];
        this.songs_element = this.getElement().getElementsByClassName("songs")[0];

        this.nav_element.getElementsByTagName("a")[0].addEventListener("click", function () {
            _this.showArtists();
        });
        this.load();

        // Prepare drag and drop uploading
        this.getElement().addEventListener("dragenter", function (e) {
            _this.drag_target = e.target;
            if (!_this.getElement().classList.contains("dragging")) {
                _this.getElement().classList.add("dragging");
            }
            e.stopPropagation();
            e.preventDefault();
        }, false);
        this.getElement().addEventListener("dragover", function (e) {
            e.stopPropagation();
            e.preventDefault();
        });
        this.getElement().addEventListener("dragleave", function (e) {
            if (e.target == _this.drag_target) {
                if (_this.getElement().classList.contains("dragging")) {
                    _this.getElement().classList.remove("dragging");
                }
            }
            e.stopPropagation();
            e.preventDefault();
        }, false);
        this.getElement().addEventListener("drop", function (e) {
            if (_this.getElement().classList.contains("dragging")) {
                _this.getElement().classList.remove("dragging");
            }
            _this.readFiles(e.dataTransfer.files);
            _this.drag_target = null;
            e.stopPropagation();
            e.preventDefault();
        }, false);
    }
    LibraryPane.prototype.readFiles = function (files) {
        var _this = this;
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }

        // now post a new XHR request
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/api/Library');
        xhr.setRequestHeader("Authorization", "Bearer " + this.getTunr().api.getAuthentication().get_access_token());
        xhr.onload = function () {
            _this.uploading_count--;
            if (_this.uploading_count <= 0) {
                if (_this.getElement().classList.contains("uploading")) {
                    _this.getElement().classList.remove("uploading");
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
        if (!this.getElement().classList.contains("uploading")) {
            this.getElement().classList.add("uploading");
        }
        xhr.send(formData);
    };

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

        // Add header
        var nav = document.createElement("a");
        nav.innerHTML = htmlEscape(artist);
        nav.addEventListener("click", function () {
            _this.showAlbums();
        });
        this.nav_element.appendChild(nav);

        // Hide artists
        this.artists_element.classList.add("hidden");

        // Show albums
        this.albums_element.classList.remove("hidden");
    };

    LibraryPane.prototype.loadSongs = function (album, artist) {
        var _this = this;
        var songs = this.getTunr().library.songsin(album, artist);
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

        // Add header
        var nav = document.createElement("a");
        nav.innerHTML = htmlEscape(album);
        this.nav_element.appendChild(nav);

        // Hide albums
        this.albums_element.classList.add("hidden");

        // Show songs
        this.songs_element.classList.remove("hidden");
    };

    LibraryPane.prototype.songActions = function (element, song) {
    };

    LibraryPane.prototype.selectSong = function (song) {
        this.getTunr().playlistpane.addSong(song);
    };
    return LibraryPane;
})(Component);
//# sourceMappingURL=LibraryPane.js.map
