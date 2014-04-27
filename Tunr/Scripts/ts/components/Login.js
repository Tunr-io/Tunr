var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Login = (function (_super) {
    __extends(Login, _super);
    function Login() {
        var _this = this;
        _super.call(this, "Login");
        this.getElement().getElementsByTagName("form")[0].addEventListener("submit", function (e) {
            e.preventDefault();
            _this.getElement().getElementsByClassName("submit")[0].classList.add("pressed");
            _this.getElement().getElementsByClassName("progress")[0].classList.add("showing");
            setTimeout(function () {
                Tunr.instance.initialize();
                _this.hide();
            }, 1000);
        });
    }
    return Login;
})(Component);
//# sourceMappingURL=Login.js.map
