/**
* Tunr
* This is the class that initializes the web-app and starts everything rolling.
*/
var Tunr = (function () {
    function Tunr() {
        Tunr.instance = this; // Set the current running instance...
        var l = new Login();
        l.show();
    }
    return Tunr;
})();

// Start 'er up.
new Tunr();
//# sourceMappingURL=Tunr.js.map
