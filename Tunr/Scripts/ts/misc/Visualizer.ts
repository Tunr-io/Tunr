class Visualizer {
	private playingpane: PlayingHelper;
	private canvas: HTMLCanvasElement;
	private stage: createjs.Stage;
	private song: Song;
	private currentVisual: Visual; // The visual that is currently being played.
	private currentBG: VisualBG; // The visual background that is currently being played.

	constructor(playingpane: PlayingHelper, canvas: HTMLCanvasElement) {
		this.playingpane = playingpane;
		this.canvas = canvas;
	}

	public init(): void {
		var parent: HTMLElement = this.canvas.parentElement;
		this.canvas.width = parent.clientWidth;
		this.canvas.height = parent.clientHeight;
		this.stage = new createjs.Stage(this.canvas);
		this.stage.autoClear = true;
		createjs.Ticker.setFPS(60);
		var tick_bind = this.tick.bind(this);
		createjs.Ticker.addEventListener("tick", tick_bind);
	}

	/**
	 * Starts the visualizer for the given song.
	 */
	public start(song: Song): void {
		// Fetch images from API and show them.
		// (don't re-fetch if it's the same artist.
		if (this.song == null || this.song.tagPerformers[0].toUpperCase() != song.tagPerformers[0].toUpperCase()) {
			if (this.currentBG != null) {
				((bg: VisualBG) => {
					bg.stop();
					createjs.Tween.get(bg).to({ alpha: 0 }, 2000, createjs.Ease.quartIn).call(() => {
						this.stage.removeChild(bg);
					});
				})(this.currentBG);
			}
			this.playingpane.parent.getTunr().api.get("Library/" + song.songId + "/ArtistInfo").then((info: ArtistInfo) => {
				if (info.imageUrls.length > 0) {
					this.showBG(info.imageUrls);
				}
			});
		}

		this.song = song;
		this.showVisual();
	}

	/**
	 * Stops visualizing.
	 */
	public stop(): void {
		this.song = null;
	}

	private showBG(images: string[]) {
		this.currentBG = new VisualBG(this.song, images);
		this.stage.addChild(this.currentBG);
		this.currentBG.transition();
	}

	private showVisual() {
		if (this.currentVisual != null) return;
		if (this.playingpane.getSong() == null) return;
		this.currentVisual = new LineScrollVisual(this.playingpane.getSong());
		this.stage.addChild(this.currentVisual);
		this.currentVisual.show(() => {
			this.stage.removeChild(this.currentVisual);
			this.currentVisual = null;
			this.showVisual();
		});
	}

	public tick(): void {
		this.stage.update();
	}
} 

/**
 * VisualBG is the image that's displayed behind the visuals.
 */
class VisualBG extends createjs.Container {
	public static MAX_ZOOM_PERCENTAGE: number = 30;
	public static IMAGE_ALPHA: number = 0.15;
	public static PAN_MIN_TIME: number = 20000;
	public static PAN_MAX_TIME: number = 35000;

	public song: Song;
	public imageURLs: string[];
	public currentIndex: number;
	public currentBitmap: createjs.Bitmap;

	constructor(song: Song, images: string[]) {
		this.song = song;
		this.imageURLs = images;
		this.currentIndex = 0;
		super();
	}

	public fadeOutBitmap(bitmap: createjs.Bitmap) {
		createjs.Tween.get(bitmap).to({ alpha: 0 }, 1500, createjs.Ease.quartIn).call(() => {
			this.removeChild(bitmap);
		});
	}

	public transition(): void {
		if (this.imageURLs == null || this.imageURLs.length <= 0) return;
		if (this.currentBitmap != null) {
			((bitmap) => {
				this.fadeOutBitmap(bitmap);
			})(this.currentBitmap);
						
			this.currentIndex++;
			if (this.currentIndex >= this.imageURLs.length) {
				this.currentIndex = 0;
			}
		}
		var cwidth: number = this.getStage().canvas.width;
		var cheight: number = this.getStage().canvas.height;
		var caspect: number = cwidth / cheight;

		this.x = cwidth / 2;
		this.y = cheight / 2;
		this.regX = cwidth / 2;
		this.regY = cheight / 2;

		var image = new Image();
		image.src = this.imageURLs[this.currentIndex];
		image.onload = () => {
			console.log("BITMAP LOADED.");
			this.currentBitmap = new createjs.Bitmap(image);
			var bitmapAspect = image.width / image.height;
			if (bitmapAspect > caspect) {
				// Match height
				var scale = cheight / image.height;
			} else {
				// Match width
				var scale = cwidth / image.width;
			}

			// Center it for now ....
			this.currentBitmap.scaleX = scale;
			this.currentBitmap.scaleY = scale;
			this.currentBitmap.regX = (image.width) / 2;
			this.currentBitmap.regY = (image.height) / 2;
			this.currentBitmap.x = cwidth / 2;
			this.currentBitmap.y = cheight / 2;
			this.currentBitmap.alpha = 0;

			// TODO: pan AND zoom.
			var zoomPercentage: number = (Math.floor(Math.random() * VisualBG.MAX_ZOOM_PERCENTAGE) / 100) + 1;

			// We need to zoom out to give ourselves room to pan.
			this.currentBitmap.scaleX *= zoomPercentage;
			this.currentBitmap.scaleY *= zoomPercentage;

			// Now we have to figure out our 'leeway' for panning
			var panLeewayX = (image.width * this.currentBitmap.scaleX) - (image.width * scale);
			var panLeewayY = (image.height * this.currentBitmap.scaleY) - (image.height * scale);

			// Select a semi-random relative X and Y start position based on leeway
			var panDirX = (Math.round(Math.random()) * 2 - 1);
			var panStartX = panDirX * Math.floor(((Math.random() * 0.5) + 0.5) * ((panLeewayX/2) + 1));
			var panEndX = -1 * panDirX * Math.floor(((Math.random() * 0.5) + 0.5) * ((panLeewayX/2) + 1));
			var panDirY = (Math.round(Math.random()) * 2 - 1);
			var panStartY = panDirY * Math.floor(((Math.random() * 0.5) + 0.5) * ((panLeewayY/2) + 1));
			var panEndY = -1 * panDirY * Math.floor(((Math.random() * 0.5) + 0.5) * ((panLeewayY/2) + 1));

			// Set start position
			this.currentBitmap.x += panStartX;
			this.currentBitmap.y += panStartY;

			// Add to stage
			this.addChild(this.currentBitmap);

			// Begin tweens
			var panTime = Math.floor(Math.random() * (VisualBG.PAN_MAX_TIME - VisualBG.PAN_MIN_TIME)) + VisualBG.PAN_MIN_TIME;
			createjs.Tween.get(this.currentBitmap).to({ alpha: VisualBG.IMAGE_ALPHA }, 1500, createjs.Ease.quartOut);
			createjs.Tween.get(this.currentBitmap).to({ x: (cwidth / 2) + panEndX, y: (cheight / 2) + panEndY }, panTime, createjs.Ease.quadInOut).call(() => {
				this.transition();
			});
		};
	}

	public stop(): void {
		this.imageURLs = null; // Null out our images - will stop the transition cycle.
	}
}

/**
 * Superclass for all visuals
 */
class Visual extends createjs.Container {
	public song: Song;
	constructor(song: Song) {
		this.song = song;
		super();
	}

	public show(callback: () => void): void {

	}

	public hide(): void {

	}
}

class LineScrollVisual extends Visual {
	// Fields of the Song object that we can use in our visualization.
	public static fields: string[] = ["tagAlbum", "tagPerformers", "tagTitle", "tagYear"];
	public static MIN_FONT_SIZE: number = 80;
	public static MAX_FONT_SIZE: number = 250;
	public static VISUAL_MIN_TIME: number = 12000;
	public static VISUAL_MAX_TIME: number = 25000;
	public static TWEEN_MIN_POW: number = 0.4;
	public static TWEEN_MAX_POW: number = 0.5;
	public static ROTATION_MIN: number = -90;
	public static ROTATION_MAX: number = 90;
	public static ROTATION_INTERVAL: number = 45;

	constructor(song: Song) {
		super(song);
	}

	/**
	 * The visualization of two or more lines of text sliding past each other
	 * TODO: More than two lines!
	 * TODO: Randomize text
	 */
	public show(callback: () => void): void {

		// Pick a number of fields to show... has to be at least two.
		var numLines = 2 + Math.floor(Math.random() * (LineScrollVisual.fields.length - 1));

		// Pick an initial side (for the first line to fly in from)
		var firstSide = Math.floor(Math.random() * 2); // 0 = left, 1 = right

		// Pick font size (and weight) for each line.
		// We need this to calculate the total vertical height of the visual.
		var fieldSelection = LineScrollVisual.fields.slice(); // Get a copy of the fields
		var textLines: VisualizerText[] = new Array<VisualizerText>();
		var totalHeight: number = 0;
		for (var i = 0; i < numLines; i++) {
			var size: number = Math.floor(Math.random() * (LineScrollVisual.MAX_FONT_SIZE - LineScrollVisual.MIN_FONT_SIZE + 1)) + LineScrollVisual.MIN_FONT_SIZE;
			var bold: boolean = (Math.floor(Math.random() * 2) == 0);
			var field: string = fieldSelection.splice(Math.floor(Math.random() * fieldSelection.length), 1)[0];
			var str: string = "";
			if (Array.isArray(this.song[field])) {
				str = this.song[field][0];
			} else {
				str = this.song[field];
			}
			var text = new VisualizerText(str + "", size, bold);
			textLines.push(text);
			totalHeight += text.getBounds().height;
		}

		// Figure out positioning
		var cwidth = this.getStage().canvas.width;
		var cheight = this.getStage().canvas.height;
		var minY: number = -1 * Math.floor(totalHeight * (1 / 3));
		var maxY: number = cheight - Math.floor(totalHeight * (2 / 3));
		var baseY: number = Math.floor(Math.random() * (maxY - minY + 1)) + minY;

		this.x = cwidth / 2;
		this.y = cheight / 2;
		this.regX = cwidth / 2;
		this.regY = cheight / 2;
		var rotationIntervals = (LineScrollVisual.ROTATION_MAX - LineScrollVisual.ROTATION_MIN) / LineScrollVisual.ROTATION_INTERVAL;
		this.rotation = LineScrollVisual.ROTATION_MIN + (LineScrollVisual.ROTATION_INTERVAL * Math.floor(Math.random() * (rotationIntervals + 1)));
		
		// Figure out timing
		var visualTime = Math.floor(Math.random() * (LineScrollVisual.VISUAL_MAX_TIME - LineScrollVisual.VISUAL_MIN_TIME)) + LineScrollVisual.VISUAL_MIN_TIME;

		// Position the text elements and let 'em roll
		var yOffset: number = 0;
		for (var i = 0; i < textLines.length; i++) {
			var targetX: number;
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
			((textLine, tarX, i) => {
				var tweenPow = (Math.random() * (LineScrollVisual.TWEEN_MAX_POW - LineScrollVisual.TWEEN_MIN_POW)) + LineScrollVisual.TWEEN_MIN_POW;
				var tween = createjs.Tween.get(textLine).to({ x: tarX }, visualTime, createjs.Ease.getPowInOut(tweenPow));
				if (i == 0) {
					tween.call(callback);
				}
			})(textLines[i], targetX, i);
		}
	}
}

class VisualizerText extends createjs.Text {
	private size: number;
	private topMultiplier: number;
	private heightMultiplier: number;

	/**
	 * Constructor for visualizer text
	 * @param {string} text The text that should be displayed
	 * @param {size} the font size
	 * @param {heavy} align Whether the text should be heavy (true) or light (false).
	 */
	constructor(text: string, size: number, heavy: boolean) {
		var font: string;
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

		super(text.toUpperCase(), font, "#fff");
		//size * 0.34
		this.setBounds(0, Math.round(size * this.topMultiplier), this.getBounds().width, Math.round(size * this.heightMultiplier));
		this.alpha = 0.2;
		//this.lineHeight = 1.5;
	}
}

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

class VisualizerFont {
	public weight: number;
	public size: number;
	public topBounds: number;
	public height: number;
}