var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Visualizer = (function () {
    function Visualizer(playingpane, canvas) {
        this.playingpane = playingpane;
        this.canvas = canvas;
    }
    Visualizer.prototype.init = function () {
        var parent = this.canvas.parentElement;
        this.canvas.width = parent.clientWidth;
        this.canvas.height = parent.clientHeight;
        this.stage = new createjs.Stage(this.canvas);
        this.stage.autoClear = true;
        createjs.Ticker.setFPS(60);
        var tick_bind = this.tick.bind(this);
        createjs.Ticker.addEventListener("tick", tick_bind);
    };

    /**
    * Starts the visualizer for the given song.
    */
    Visualizer.prototype.start = function (song) {
        var _this = this;
        this.song = song;
        this.showVisual();

        // Fetch images from API and show them.
        this.playingpane.getTunr().api.get("Library/" + song.songId + "/images").then(function (images) {
            if (images.length > 0) {
                _this.showBG(images);
            }
        });
    };

    /**
    * Stops visualizing.
    */
    Visualizer.prototype.stop = function () {
        this.song = null;
    };

    Visualizer.prototype.showBG = function (images) {
        if (this.currentBG != null) {
            this.stage.removeChild(this.currentBG);
        }
        this.currentBG = new VisualBG(this.song, images);
        this.stage.addChild(this.currentBG);
        this.currentBG.transition();
    };

    Visualizer.prototype.showVisual = function () {
        var _this = this;
        if (this.currentVisual != null)
            return;
        if (this.playingpane.getSong() == null)
            return;
        this.currentVisual = new LineScrollVisual(this.playingpane.getSong());
        this.stage.addChild(this.currentVisual);
        this.currentVisual.show(function () {
            _this.stage.removeChild(_this.currentVisual);
            _this.currentVisual = null;
            _this.showVisual();
        });
    };

    Visualizer.prototype.tick = function () {
        this.stage.update();
    };
    return Visualizer;
})();

/**
* VisualBG is the image that's displayed behind the visuals.
*/
var VisualBG = (function (_super) {
    __extends(VisualBG, _super);
    function VisualBG(song, images) {
        this.song = song;
        this.images = images;
        this.currentIndex = 0;
        _super.call(this);
    }
    VisualBG.prototype.transition = function () {
        var _this = this;
        if (this.currentBitmap != null) {
            this.removeChild(this.currentBitmap);
            this.currentIndex++;
            if (this.currentIndex >= this.images.length) {
                this.currentIndex = 0;
            }
        }
        var cwidth = this.getStage().canvas.width;
        var cheight = this.getStage().canvas.height;
        var caspect = cwidth / cheight;

        this.x = cwidth / 2;
        this.y = cheight / 2;
        this.regX = cwidth / 2;
        this.regY = cheight / 2;

        var image = new Image();
        image.src = this.images[this.currentIndex];
        image.onload = function () {
            console.log("BITMAP LOADED.");
            _this.currentBitmap = new createjs.Bitmap(image);
            var bitmapAspect = image.width / image.height;
            if (bitmapAspect > caspect) {
                // Match height
                var scale = cheight / image.height;
            } else {
                // Match width
                var scale = cwidth / image.width;
            }

            // Center it for now ....
            _this.currentBitmap.scaleX = scale;
            _this.currentBitmap.scaleY = scale;
            _this.currentBitmap.regX = (image.width) / 2;
            _this.currentBitmap.regY = (image.height) / 2;
            _this.currentBitmap.x = cwidth / 2;
            _this.currentBitmap.y = cheight / 2;
            _this.currentBitmap.alpha = 0;
            _this.addChild(_this.currentBitmap);
            createjs.Tween.get(_this.currentBitmap).to({ alpha: 0.15 }, 1500, createjs.Ease.quartOut);
        };
    };
    return VisualBG;
})(createjs.Container);

/**
* Superclass for all visuals
*/
var Visual = (function (_super) {
    __extends(Visual, _super);
    function Visual(song) {
        this.song = song;
        _super.call(this);
    }
    Visual.prototype.show = function (callback) {
    };

    Visual.prototype.hide = function () {
    };
    return Visual;
})(createjs.Container);

var LineScrollVisual = (function (_super) {
    __extends(LineScrollVisual, _super);
    function LineScrollVisual(song) {
        _super.call(this, song);
    }
    /**
    * The visualization of two or more lines of text sliding past each other
    * TODO: More than two lines!
    * TODO: Randomize text
    */
    LineScrollVisual.prototype.show = function (callback) {
        // Pick a number of fields to show... has to be at least two.
        var numLines = 2 + Math.floor(Math.random() * (LineScrollVisual.fields.length - 1));

        // Pick an initial side (for the first line to fly in from)
        var firstSide = Math.floor(Math.random() * 2);

        // Pick font size (and weight) for each line.
        // We need this to calculate the total vertical height of the visual.
        var fieldSelection = LineScrollVisual.fields.slice();
        var textLines = new Array();
        var totalHeight = 0;
        for (var i = 0; i < numLines; i++) {
            var size = Math.floor(Math.random() * (LineScrollVisual.MAX_FONT_SIZE - LineScrollVisual.MIN_FONT_SIZE + 1)) + LineScrollVisual.MIN_FONT_SIZE;
            var bold = (Math.floor(Math.random() * 2) == 0);
            var field = fieldSelection.splice(Math.floor(Math.random() * fieldSelection.length), 1)[0];
            var text = new VisualizerText(this.song[field] + "", size, bold);
            textLines.push(text);
            totalHeight += text.getBounds().height;
        }

        // Figure out positioning
        var cwidth = this.getStage().canvas.width;
        var cheight = this.getStage().canvas.height;
        var minY = -1 * Math.floor(totalHeight * (1 / 3));
        var maxY = cheight - Math.floor(totalHeight * (2 / 3));
        var baseY = Math.floor(Math.random() * (maxY - minY + 1)) + minY;

        this.x = cwidth / 2;
        this.y = cheight / 2;
        this.regX = cwidth / 2;
        this.regY = cheight / 2;
        var rotationIntervals = (LineScrollVisual.ROTATION_MAX - LineScrollVisual.ROTATION_MIN) / LineScrollVisual.ROTATION_INTERVAL;
        this.rotation = LineScrollVisual.ROTATION_MIN + (LineScrollVisual.ROTATION_INTERVAL * Math.floor(Math.random() * (rotationIntervals + 1)));

        // Figure out timing
        var visualTime = Math.floor(Math.random() * (LineScrollVisual.VISUAL_MAX_TIME - LineScrollVisual.VISUAL_MIN_TIME)) + LineScrollVisual.VISUAL_MIN_TIME;

        // Position the text elements and let 'em roll
        var yOffset = 0;
        for (var i = 0; i < textLines.length; i++) {
            var targetX;
            textLines[i].y = baseY + yOffset - textLines[i].getBounds().y;
            yOffset += (textLines[i].getBounds().height - textLines[i].getBounds().y);
            if (i % 2 == firstSide) {
                textLines[i].x = (-1 * textLines[i].getBounds().width);
                targetX = cwidth;
            } else {
                textLines[i].x = cwidth;
                targetX = (-1 * textLines[i].getBounds().width);
            }
            this.addChild(textLines[i]);
            (function (textLine, tarX, i) {
                var tweenPow = (Math.random() * (LineScrollVisual.TWEEN_MAX_POW - LineScrollVisual.TWEEN_MIN_POW)) + LineScrollVisual.TWEEN_MIN_POW;
                var tween = createjs.Tween.get(textLine).to({ x: tarX }, visualTime, createjs.Ease.getPowInOut(tweenPow));
                if (i == 0) {
                    tween.call(callback);
                }
            })(textLines[i], targetX, i);
        }
    };
    LineScrollVisual.fields = ["album", "artist", "title", "year"];
    LineScrollVisual.MIN_FONT_SIZE = 80;
    LineScrollVisual.MAX_FONT_SIZE = 250;
    LineScrollVisual.VISUAL_MIN_TIME = 12000;
    LineScrollVisual.VISUAL_MAX_TIME = 25000;
    LineScrollVisual.TWEEN_MIN_POW = 0.4;
    LineScrollVisual.TWEEN_MAX_POW = 0.5;
    LineScrollVisual.ROTATION_MIN = -90;
    LineScrollVisual.ROTATION_MAX = 90;
    LineScrollVisual.ROTATION_INTERVAL = 45;
    return LineScrollVisual;
})(Visual);

var VisualizerText = (function (_super) {
    __extends(VisualizerText, _super);
    /**
    * Constructor for visualizer text
    * @param {string} text The text that should be displayed
    * @param {size} the font size
    * @param {heavy} align Whether the text should be heavy (true) or light (false).
    */
    function VisualizerText(text, size, heavy) {
        var font;
        this.size = size;

        if (heavy) {
            font = '900 ' + size + 'px "Open Sans"';
            this.topMultiplier = 0.34;
            this.heightMultiplier = 1.075;
        } else {
            font = '100 ' + size + 'px "Open Sans"';
            this.topMultiplier = 0.312;
            this.heightMultiplier = 1.055;
        }

        _super.call(this, text.toUpperCase(), font, "#fff");

        //size * 0.34
        this.setBounds(0, Math.round(size * this.topMultiplier), this.getBounds().width, Math.round(size * this.heightMultiplier));
        this.alpha = 0.2;
        //this.lineHeight = 1.5;
    }
    return VisualizerText;
})(createjs.Text);

/**
* LIGHT:
* 250px, top: 78 (0.312), height: 264 (1.056)
* 200px, top: 62 (0.31), height: 210 (1.05)
* 175px, top: 55 (0.314), height: 185 (1.057)
*
* HEAVY:
* 250px, top: 85 (0.34), height: 268 (1.072)
* 200px, top: 68 (0.34), height: 215 (1.075)
* 175px, top: 58 (0.331), height: 186 (1.062)
* 150px, top: 51 (0.34), height: 162 (1.08)
*/
var VisualizerFont = (function () {
    function VisualizerFont() {
    }
    return VisualizerFont;
})();
//# sourceMappingURL=Visualizer.js.map
