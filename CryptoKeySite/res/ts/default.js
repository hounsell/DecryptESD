var KeySite;
(function (KeySite) {
    function CopyKey(mev) {
        var key = this;
        var range = document.createRange();
        range.selectNode(key);
        var selection = window.getSelection();
        selection.removeAllRanges();
        selection.addRange(range);
        document.execCommand("copy");
    }
    KeySite.CopyKey = CopyKey;
})(KeySite || (KeySite = {}));
var Keys = document.getElementsByClassName("break");
for (var i = 0; i < Keys.length; i++) {
    Keys[i].addEventListener("click", KeySite.CopyKey);
}
;
//# sourceMappingURL=default.js.map