/**
* The Component class is a super-class of all components.
* It provides default implementations for things like showing, hiding, and animating.
* More specific behaviors are to be implemented by each individual component.
*/
var Component = (function () {
    function Component(tunr, component_id) {
        // Make sure we're associated with an application instance...
        if (tunr != null) {
            this.tunr = tunr;
        } else {
            this.tunr = Tunr.instance; // Default instance if undefined
        }

        // Get the component ID, grab the HTML
        this.component_id = component_id;
        this.element = document.getElementById("component_" + this.component_id).cloneNode(true);
        this.element.classList.remove("COMPONENT");
        this.element.id = '';
        this.animator = new Animator(this);

        // Initialize any helpers...
        this.helpers = new Array();
        var helperElements = this.element.getElementsByClassName("HELPER");
        for (var i = 0; i < helperElements.length; i++) {
            var helperElement = helperElements[i];
            var helperClass = helperElement.attributes.getNamedItem("data-helper-class").value;
            var helperObject = new window[helperClass](this, helperElement);
            this.helpers[helperClass] = helperObject;
        }
    }
    /**
    * Fetches a reference to the helper class specified.
    * @param helperName Name of the helper class.
    */
    Component.prototype.getHelper = function (helperName) {
        return this.helpers[helperName];
    };

    Component.prototype.getTunr = function () {
        return this.tunr;
    };

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

    Component.prototype.windowResize = function () {
    };
    return Component;
})();
//# sourceMappingURL=Component.js.map
