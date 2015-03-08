var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Player = (function (_super) {
    __extends(Player, _super);
    function Player(tunr) {
        _super.call(this, tunr, "Player");
    }
    Player.prototype.windowResize = function () {
        console.log("Resize player!");
        this.getHelper("PlayingHelper").resize();
    };
    return Player;
})(Component);
//# sourceMappingURL=Player.js.map