class Visualizer {
	private playingpane: PlayingPane;
	private canvas: HTMLCanvasElement;
	private stage: createjs.Stage;

	constructor(playingpane: PlayingPane, canvas: HTMLCanvasElement) {
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
		this.showVisual();
	}

	private showVisual() {
		setTimeout(() => {
			this.showVisual();
		}, (15000 + Math.random() * 12000));
		if (this.playingpane.getSong() == null) return;
		var visual = new Visual(this.playingpane.getSong());
		this.stage.addChild(visual);
		visual.slide(() => {
			this.stage.removeChild(visual);
		});
	}

	public tick(): void {
		this.stage.update();
	}
} 

class Visual extends createjs.Container {
	private song: Song;
	constructor(song: Song) {
		this.song = song;
		super();
	}

	/**
	 * The visualization of two or more lines of text sliding past each other
	 * TODO: More than two lines!
	 * TODO: Randomize text
	 */
	public slide(callback: () => void): void {
		var cwidth = this.getStage().canvas.width;
		var cheight = this.getStage().canvas.height;
		var bold:boolean = Math.round(Math.random()) == 0;
		var textOne = new VisualizerText(this.song.artist,bold);
		var textTwo = new VisualizerText(this.song.title,!bold);
		textOne.x = cwidth;
		textOne.y = (Math.random() * cheight) - textOne.getBounds().y;
		textTwo.x = - textTwo.getBounds().width;
		textTwo.y = textOne.y + textOne.getBounds().height - textTwo.getBounds().y;

		this.addChild(textOne);
		this.addChild(textTwo);

		createjs.Tween.get(textOne).to({ x: -1 * textOne.getBounds().width }, 12000, createjs.Ease.getPowInOut(0.45)).call(callback);
		createjs.Tween.get(textTwo).to({ x: cwidth }, 12000, createjs.Ease.getPowInOut(0.45));
	}
}

class VisualizerText extends createjs.Text {
	/**
	 * Constructor for visualizer text
	 * @param {string} text The text that should be displayed
	 * @param {heavy} align Whether the text should be heavy (true) or light (false).
	 */
	constructor(text: string, heavy: boolean) {
		var font: string;
		var size: number;
		size = 150 + (Math.random()*100);
		var topMultiplier: number;
		var heightMultiplier: number;

		if (heavy) {
			font = '900 ' + size + 'px "Open Sans"';
			topMultiplier = 0.34;
			heightMultiplier = 1.075;
		} else {
			font = '100 ' + size + 'px "Open Sans"';
			topMultiplier = 0.312;
			heightMultiplier = 1.055;
		}
		super(text.toUpperCase(), font, "#fff");
		//size * 0.34
		this.setBounds(0, Math.round(size * topMultiplier), this.getBounds().width, Math.round(size * heightMultiplier));
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