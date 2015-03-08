class Player extends Component {
	constructor(tunr: Tunr) {
		super(tunr, "Player");
	}

	public windowResize(): void {
		console.log("Resize player!");
		this.getHelper("PlayingHelper").resize();
	}
} 