var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Login = (function (_super) {
    __extends(Login, _super);
    function Login(tunr) {
        var _this = this;
        _super.call(this, tunr, "Login");

        //TiltEffect.addTilt((<HTMLElement>this.getElement().getElementsByClassName("submit")[0])); // No can do for now...
        this.getElement().getElementsByTagName("form")[0].addEventListener("submit", function (e) {
            e.preventDefault();
            _this.login_submit();
        });
    }
    Login.prototype.login_submit = function () {
        var _this = this;
        // Get input values ...
        var email = this.getElement().getElementsByTagName("input")[0].value;
        var password = this.getElement().getElementsByTagName("input")[1].value;

        // Lock input elements
        this.getElement().getElementsByTagName("form")[0].disabled = true;
        this.getElement().getElementsByTagName("input")[0].disabled = true;
        this.getElement().getElementsByTagName("input")[1].disabled = true;

        // Shift button to progress meter
        this.getElement().getElementsByClassName("submit")[0].classList.add("pressed");
        this.getElement().getElementsByClassName("progress")[0].classList.add("showing");

        this.getTunr().api.getAuthentication().auth_password(email, password).then(function () {
            // Success!
            _this.getTunr().initialize();
            _this.hide();
        }, function () {
            //Failure D:
            // Enable inputs
            _this.getElement().getElementsByTagName("form")[0].disabled = false;
            _this.getElement().getElementsByTagName("input")[0].disabled = false;
            _this.getElement().getElementsByTagName("input")[1].disabled = false;

            // Show an error message on the submit button ...
            _this.getElement().getElementsByClassName("submit")[0].value = "Error";

            // Revert buttons
            _this.getElement().getElementsByClassName("submit")[0].classList.remove("pressed");
            _this.getElement().getElementsByClassName("progress")[0].classList.remove("showing");
        });
    };
    return Login;
})(Component);
//# sourceMappingURL=Login.js.map
