﻿var TiltEffect = (function () {
    function TiltEffect() {
        var _this = this;
        this.tilt_elements = new Array();
        this.tilting = new Array();
        document.body.addEventListener("mousemove", function (e) {
            _this.updateTilts(e.pageX, e.pageY);
        });
        document.body.addEventListener("mouseup", function (e) {
            _this.clearTilts();
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
    TiltEffect.addTilt = function (element) {
        TiltEffect.tilter.addTilt(element);
    };

    TiltEffect.prototype.addTilt = function (element) {
        var _this = this;
        element.addEventListener("mousedown", function (e) {
            _this.startTilt(element, e.pageX, e.pageY);
        });
        //element.addEventListener("mouseout", (e) => {
        //	this.endTilt(element);
        //});
        //element.addEventListener("mouseup", (e) => {
        //	this.endTilt(element);
        //});
    };

    TiltEffect.prototype.startTilt = function (element, mouseX, mouseY) {
        this.tilting.push(element);
        this.updateTilts(mouseX, mouseY);
    };

    TiltEffect.prototype.clearTilts = function () {
        for (var i = 0; i < this.tilting.length; i++) {
            this.tilting[i].style["transform"] = "";
        }
        this.tilting.splice(0);
    };

    TiltEffect.prototype.endTilt = function (element) {
        var i = this.tilting.indexOf(element);
        if (i >= 0) {
            this.tilting.splice(i, 1);
        }
        element.style["transform"] = "";
    };

    TiltEffect.prototype.updateTilts = function (mouseX, mouseY) {
        for (var i = 0; i < this.tilting.length; i++) {
            this.tilt(this.tilting[i], mouseX, mouseY);
        }
    };

    TiltEffect.prototype.tilt = function (element, mouseX, mouseY) {
        // Get the x and y offset of the mouse from the element's origin
        var elementX = element.offsetLeft;
        var elementY = element.offsetTop;

        for (var parentElement = element.offsetParent; parentElement != null; parentElement = parentElement.parentElement) {
            elementX += parentElement.offsetLeft;
            elementY += parentElement.offsetTop;
            elementY -= parentElement.scrollTop;
        }

        // Center the origin in the element
        elementX += (element.clientWidth / 2);
        elementY += (element.clientHeight / 2);

        // Get the offset of the mouse
        var offsetX = mouseX - elementX;
        var offsetY = elementY - mouseY;

        // Get the offset percentage
        var percentageX = offsetX / (element.clientWidth / 2);
        var percentageY = offsetY / (element.clientHeight / 2);

        element.style["transform"] = "rotate3d(" + percentageY + "," + percentageX + ",0," + TiltEffect.ANGLE + "deg)";
    };
    TiltEffect.ANGLE = 15;
    TiltEffect.tilter = new TiltEffect();
    return TiltEffect;
})();
//# sourceMappingURL=TiltEffect.js.map