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
        this.list_helper_classes = { "artist": ArtistListHelper, "album": AlbumListHelper, "title": SongListHelper };
        this.uploading_count = 0;
    }
    LibraryHelper.prototype.init = function () {
        var _this = this;
        this.nav_element = this.element.getElementsByTagName("nav")[0];
        this.tree_structure = ["artist", "album", "title"]; // hard-set for now. user-configurable later.
        this.list_helpers = new Array();
        this.root_name = "Music";
        this.list_filter_state = new Song();
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

    /**
    * Loads user's library from the server, then triggers root list render.
    */
    LibraryHelper.prototype.load = function () {
        var _this = this;
        this.parent.getTunr().library.load().then(function () {
            _this.loadLevel(0);
        }, function () {
            console.error("failed to load library.");
        });
    };

    LibraryHelper.prototype.getFilterState = function () {
        return this.list_filter_state;
    };

    LibraryHelper.prototype.loadChild = function (value) {
        // Get current property name
        var prop = this.tree_structure[this.list_helpers.length - 1];

        // Set filter state
        this.list_filter_state[prop] = value;

        // Load the next list
        this.loadLevel(this.list_helpers.length);
    };

    LibraryHelper.prototype.loadLevel = function (levelIndex) {
        var _this = this;
        var value = this.root_name;
        if (levelIndex > 0) {
            // Get current property name
            var prop = this.tree_structure[this.list_helpers.length - 1];

            // Get new value
            value = this.list_filter_state[prop];
        }

        // Add nav header
        var header = document.createElement("a");
        header.innerHTML = value;
        header.addEventListener("click", function () {
            _this.backLevel(levelIndex);
        });
        this.nav_element.appendChild(header);

        // Prepare list helper
        var listElement = document.createElement("ul");
        listElement.classList.add(this.tree_structure[levelIndex]);
        listElement.style.top = this.nav_element.clientHeight + "px";
        listElement = this.element.appendChild(listElement);
        var listHelper = new this.list_helper_classes[this.tree_structure[levelIndex]](this, listElement);
        this.list_helpers.push(listHelper);
        listHelper.init();

        for (var i = 0; i < this.list_helpers.length - 1; i++) {
            this.list_helpers[i].hide();
        }
    };

    LibraryHelper.prototype.backLevel = function (levelIndex) {
        if (levelIndex == this.list_helpers.length - 1) {
            return;
        }
        var headers = this.nav_element.getElementsByTagName("a");

        // Show the specified list level
        this.list_helpers[levelIndex].show();

        for (var i = this.list_helpers.length - 1; i > levelIndex; i--) {
            // Remove property of this from the filter state
            var prop = this.tree_structure[i];
            this.list_filter_state[prop] = "";

            // Remove the navigation header
            this.nav_element.removeChild(headers[i]);

            // Destroy the list
            var helper = this.list_helpers.pop();
            helper.destroy();
        }
    };

    LibraryHelper.prototype.selectSong = function (song) {
        var playlistHelper = this.parent.getHelper("PlaylistHelper");
        playlistHelper.addSong(song);
    };
    return LibraryHelper;
})(Helper);

var LibraryListHelper = (function (_super) {
    __extends(LibraryListHelper, _super);
    function LibraryListHelper(parent, element) {
        this.library_helper = parent;
        _super.call(this, parent.parent, element);
    }
    LibraryListHelper.prototype.hide = function () {
        this.element.classList.add("hidden");
    };

    LibraryListHelper.prototype.show = function () {
        this.element.classList.remove("hidden");
    };

    LibraryListHelper.prototype.destroy = function () {
        this.element.parentNode.removeChild(this.element);
    };
    return LibraryListHelper;
})(Helper);

var ArtistListHelper = (function (_super) {
    __extends(ArtistListHelper, _super);
    function ArtistListHelper() {
        _super.apply(this, arguments);
    }
    ArtistListHelper.prototype.init = function () {
        var _this = this;
        var artists = this.parent.getTunr().library.filterUniqueProperty(this.library_helper.getFilterState(), "artist");
        this.element.innerHTML = ''; // clear existing entries
        for (var i = 0; i < artists.length; i++) {
            var li = document.createElement("li");
            li.innerHTML = htmlEscape(artists[i].artist);
            (function (artist, element) {
                TiltEffect.addTilt(element);
                element.addEventListener("click", function (e) {
                    _this.library_helper.loadChild(artist);
                });
            })(artists[i].artist, li);
            this.element.appendChild(li);
        }
    };
    return ArtistListHelper;
})(LibraryListHelper);

var AlbumListHelper = (function (_super) {
    __extends(AlbumListHelper, _super);
    function AlbumListHelper() {
        _super.apply(this, arguments);
    }
    AlbumListHelper.prototype.init = function () {
        var _this = this;
        var albums = this.parent.getTunr().library.filterUniqueProperty(this.library_helper.getFilterState(), "album");
        this.element.innerHTML = "";
        for (var i = 0; i < albums.length; i++) {
            var img = document.createElement("img");
            img.src = '/api/LibraryData/' + urlEscape(albums[i].artist) + '/' + urlEscape(albums[i].album) + '/art';
            img.alt = albums[i].album;
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
                    _this.library_helper.loadChild(album);
                });
            })(albums[i].album, li);
            this.element.appendChild(li);
        }
    };
    return AlbumListHelper;
})(LibraryListHelper);

var SongListHelper = (function (_super) {
    __extends(SongListHelper, _super);
    function SongListHelper() {
        _super.apply(this, arguments);
    }
    SongListHelper.prototype.init = function () {
        var _this = this;
        var songs = this.parent.getTunr().library.filter(this.library_helper.getFilterState());
        this.element.innerHTML = "";
        for (var i = 0; i < songs.length; i++) {
            var li = document.createElement("li");
            li.innerHTML = '<span class="track">' + ('0' + songs[i].trackNumber).slice(-2) + '</span>' + htmlEscape(songs[i].title);
            (function (song, element) {
                TiltEffect.addTilt(element);
                element.addEventListener("click", function () {
                    _this.library_helper.selectSong(song);
                });
            })(songs[i], li);
            this.element.appendChild(li);
        }
    };
    return SongListHelper;
})(LibraryListHelper);
//# sourceMappingURL=LibraryHelper.js.map
