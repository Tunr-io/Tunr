class TiltEffect {
	private static ANGLE: number = 15;
	private static tilter: TiltEffect = new TiltEffect();

	private tilt_elements: Array<HTMLElement> = new Array<HTMLElement>();
	private tilting: Array<HTMLElement> = new Array<HTMLElement>();

	constructor() {
		document.body.addEventListener("mousemove", (e) => {
			this.updateTilts(e.pageX,e.pageY);
		});
		document.body.addEventListener("mouseup", (e) => {
			this.clearTilts();
		});
		//document.body.addEventListener("mouseout", (e) => {
		//	var event:HTMLElement = <HTMLElement>(e.toElement || e.relatedTarget);
		//	if (event.parentNode == document.body || event == this) {
		//		return;
		//	}
		//	console.log("MOUSEOUT");
		//	this.clearTilts();
		//});
	}

	public static addTilt(element: HTMLElement) {
		TiltEffect.tilter.addTilt(element);
	}

	private addTilt(element: HTMLElement) {
		element.addEventListener("mousedown", (e) => {
			this.startTilt(element, e.pageX, e.pageY);
		});
		//element.addEventListener("mouseout", (e) => {
		//	this.endTilt(element);
		//});
		//element.addEventListener("mouseup", (e) => {
		//	this.endTilt(element);
		//});
	}

	private startTilt(element: HTMLElement, mouseX: number, mouseY: number) {
		this.tilting.push(element);
		this.updateTilts(mouseX, mouseY);
	}

	private clearTilts(): void {
		for (var i = 0; i < this.tilting.length; i++) {
			this.tilting[i].style["transform"] = "";
		}
		this.tilting.splice(0);
	}

	private endTilt(element: HTMLElement) {
		var i = this.tilting.indexOf(element);
		if (i >= 0) {
			this.tilting.splice(i, 1);
		}
		element.style["transform"] = "";
	}

	private updateTilts(mouseX: number, mouseY: number): void {
		for (var i = 0; i < this.tilting.length; i++) {
			this.tilt(this.tilting[i], mouseX, mouseY);
		}
	}

	private tilt(element: HTMLElement, mouseX: number, mouseY: number): void {
		// Get the x and y offset of the mouse from the element's origin
		var elementX: number = element.offsetLeft;
		var elementY: number = element.offsetTop;

		for (var parentElement: HTMLElement = <HTMLElement>element.offsetParent; parentElement != null; parentElement = parentElement.parentElement) {
			elementX += parentElement.offsetLeft;
			elementY += parentElement.offsetTop;
			elementY -= parentElement.scrollTop;
		}

		// Center the origin in the element
		elementX += (element.clientWidth / 2);
		elementY += (element.clientHeight / 2);

		// Get the offset of the mouse
		var offsetX: number = mouseX - elementX;
		var offsetY: number = elementY - mouseY;

		// Get the offset percentage
		var percentageX: number = offsetX / (element.clientWidth / 2);
		var percentageY: number = offsetY / (element.clientHeight / 2);

		element.style["transform"] = "rotate3d(" + percentageY + "," + percentageX + ",0," + TiltEffect.ANGLE + "deg)";
	}
}