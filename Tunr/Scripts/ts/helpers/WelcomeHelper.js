var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WelcomeHelper = (function (_super) {
    __extends(WelcomeHelper, _super);
    function WelcomeHelper() {
        _super.apply(this, arguments);
    }
    WelcomeHelper.prototype.init = function () {
    };

    WelcomeHelper.prototype.hide = function () {
        this.element.classList.add("hidden");
    };
    return WelcomeHelper;
})(Helper);
//# sourceMappingURL=WelcomeHelper.js.map
