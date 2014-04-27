﻿/**
 * The Component class is a super-class of all components.
 * It provides default implementations for things like showing, hiding, and animating.
 * More specific behaviors are to be implemented by each individual component.
 */
class Component {
	private component_id: string;
	private element: HTMLElement;
	private animator: Animator;

	constructor(component_id: string) {
		// Get the component ID, grab the 
		this.component_id = component_id;
		this.element = <HTMLDivElement> document.getElementById("component_" + this.component_id).cloneNode(true);
		this.element.classList.remove("COMPONENT");
		this.element.id = '';
		this.animator = new Animator(this);
	}

	public getElement(): HTMLElement {
		return this.element;
	}

	public show(): void {
		this.element = <HTMLElement>document.body.appendChild(this.element);
		this.animator.animate("in") // IN is the default event for showing.
	}

	public hide(): void {
		this.animator.animate("out") // OUT is the default event for showing.
		// We have 300ms to animate out before we're taken off the DOM Tree.
		setTimeout(() => {
			this.element = <HTMLElement>document.body.removeChild(this.element);
		}, 300); 
	}
} 