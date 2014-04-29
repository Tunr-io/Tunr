var API = (function () {
    function API() {
        this.authentication = new Authentication();
    }
    API.prototype.getAuthentication = function () {
        return this.authentication;
    };
    return API;
})();

var Authentication = (function () {
    function Authentication() {
    }
    Authentication.prototype.is_authenticated = function () {
        return (typeof this.access_token !== 'undefined' && this.expire_time.getTime() - (new Date()).getTime() > 0);
    };

    Authentication.prototype.auth_password = function (email, password) {
        var _this = this;
        var retval = $.Deferred();
        $.Deferred;
        $.ajax("/Token", {
            type: "POST",
            dataType: "JSON",
            data: { grant_type: "password", username: email, password: password },
            success: function (data) {
                _this.access_token = data.access_token;

                //this.refresh_token = data.refresh_token;
                _this.expire_time = new Date(data[".expires"]);
                retval.resolve();
            },
            error: function () {
                retval.reject("Failure.");
            }
        });
        return retval.promise();
    };
    return Authentication;
})();
//# sourceMappingURL=API.js.map
