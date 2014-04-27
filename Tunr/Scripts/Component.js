/**
* The Component class is a super-class of all components.
* It provides default implementations for things like showing, hiding, and animating.
* More specific behaviors are to be implemented by each individual component.
*/
var Component = (function () {
    function Component(component_id) {
        // Get the component ID, grab the
        this.component_id = component_id;
        this.element = document.getElementById("component_" + this.component_id).cloneNode(true);
        this.element.classList.remove("COMPONENT");
        this.element.id = '';
        this.animator = new Animator(this);
    }
    Component.prototype.getElement = function () {
        return this.element;
    };

    Component.prototype.show = function () {
        this.element = document.body.appendChild(this.element);
        this.animator.animate("in");
    };

    Component.prototype.hide = function () {
        var _this = this;
        this.animator.animate("out");

        // We have 300ms to animate out before we're taken off the DOM Tree.
        setTimeout(function () {
            _this.element = document.body.removeChild(_this.element);
        }, 300);
    };
    return Component;
})();
//# sourceMappingURL=Component.js.map
