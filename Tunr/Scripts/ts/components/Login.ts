class Login extends Component {
	constructor() {
		super("Login");
		this.getElement().getElementsByTagName("form")[0].addEventListener("submit", (e) => {
			e.preventDefault();
			(<HTMLElement>this.getElement().getElementsByClassName("submit")[0]).classList.add("pressed");
			(<HTMLElement>this.getElement().getElementsByClassName("progress")[0]).classList.add("showing");
			setTimeout(() => {
				this.hide();
			}, 1000);
		});
	}
}