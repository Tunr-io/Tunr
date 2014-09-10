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
        this.showVisual();
    };

    Visualizer.prototype.showVisual = function () {
        var _this = this;
        setTimeout(function () {
            _this.showVisual();
        }, (15000 + Math.random() * 12000));
        if (this.playingpane.getSong() == null)
            return;
        var visual = new Visual(this.playingpane.getSong());
        this.stage.addChild(visual);
        visual.slide(function () {
            _this.stage.removeChild(visual);
        });
    };

    Visualizer.prototype.tick = function () {
        this.stage.update();
    };
    return Visualizer;
})();

var Visual = (function (_super) {
    __extends(Visual, _super);
    function Visual(song) {
        this.song = song;
        _super.call(this);
    }
    /**
    * The visualization of two or more lines of text sliding past each other
    * TODO: More than two lines!
    * TODO: Randomize text
    */
    Visual.prototype.slide = function (callback) {
        var cwidth = this.getStage().canvas.width;
        var cheight = this.getStage().canvas.height;
        var bold = Math.round(Math.random()) == 0;
        var textOne = new VisualizerText(this.song.artist, bold);
        var textTwo = new VisualizerText(this.song.title, !bold);
        textOne.x = cwidth;
        textOne.y = (Math.random() * cheight) - textOne.getBounds().y;
        textTwo.x = -textTwo.getBounds().width;
        textTwo.y = textOne.y + textOne.getBounds().height - textTwo.getBounds().y;

        this.addChild(textOne);
        this.addChild(textTwo);

        createjs.Tween.get(textOne).to({ x: -1 * textOne.getBounds().width }, 12000, createjs.Ease.getPowInOut(0.45)).call(callback);
        createjs.Tween.get(textTwo).to({ x: cwidth }, 12000, createjs.Ease.getPowInOut(0.45));
    };
    return Visual;
})(createjs.Container);

var VisualizerText = (function (_super) {
    __extends(VisualizerText, _super);
    /**
    * Constructor for visualizer text
    * @param {string} text The text that should be displayed
    * @param {heavy} align Whether the text should be heavy (true) or light (false).
    */
    function VisualizerText(text, heavy) {
        var font;
        var size;
        size = 150 + (Math.random() * 100);
        var topMultiplier;
        var heightMultiplier;

        if (heavy) {
            font = '900 ' + size + 'px "Open Sans"';
            topMultiplier = 0.34;
            heightMultiplier = 1.075;
        } else {
            font = '100 ' + size + 'px "Open Sans"';
            topMultiplier = 0.312;
            heightMultiplier = 1.055;
        }
        _super.call(this, text.toUpperCase(), font, "#fff");

        //size * 0.34
        this.setBounds(0, Math.round(size * topMultiplier), this.getBounds().width, Math.round(size * heightMultiplier));
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
