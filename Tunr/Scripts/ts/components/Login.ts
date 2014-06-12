class Login extends Component {
	constructor(tunr: Tunr) {
		super(tunr, "Login");
		TiltEffect.addTilt((<HTMLElement>this.getElement().getElementsByClassName("submission")[0])); // No can do for now...
		this.getElement().getElementsByTagName("form")[0].addEventListener("submit", (e) => {
			e.preventDefault();
			this.login_submit();
		});
	}

	public login_submit(): void {
		// Get input values ...
		var email = (<HTMLInputElement>this.getElement().getElementsByTagName("input")[0]).value;
		var password = (<HTMLInputElement>this.getElement().getElementsByTagName("input")[1]).value;

		// Lock input elements
		(<HTMLFormElement>this.getElement().getElementsByTagName("form")[0]).disabled = true;
		(<HTMLInputElement>this.getElement().getElementsByTagName("input")[0]).disabled = true;
		(<HTMLInputElement>this.getElement().getElementsByTagName("input")[1]).disabled = true;

		// Shift button to progress meter
		(<HTMLElement>this.getElement().getElementsByClassName("submit")[0]).classList.add("pressed");
		(<HTMLElement>this.getElement().getElementsByClassName("progress")[0]).classList.add("showing");

		this.getTunr().api.getAuthentication().auth_password(email, password).then(() => {
			// Success!
			this.getTunr().initialize();
			this.hide();
		}, () => {
				//Failure D:
				// Enable inputs
				(<HTMLFormElement>this.getElement().getElementsByTagName("form")[0]).disabled = false;
				(<HTMLInputElement>this.getElement().getElementsByTagName("input")[0]).disabled = false;
				(<HTMLInputElement>this.getElement().getElementsByTagName("input")[1]).disabled = false;
				// Show an error message on the submit button ...
				(<HTMLInputElement>this.getElement().getElementsByClassName("submit")[0]).value = "Error";
				// Revert buttons
				(<HTMLElement>this.getElement().getElementsByClassName("submit")[0]).classList.remove("pressed");
				(<HTMLElement>this.getElement().getElementsByClassName("progress")[0]).classList.remove("showing");
			});
	}
}