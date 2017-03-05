var KeySite;
(function (KeySite) {
    function copyKey(mev) {
        var key = this;
        var range = document.createRange();
        range.selectNode(key);
        var selection = window.getSelection();
        selection.removeAllRanges();
        selection.addRange(range);
        document.execCommand("copy");
    }
    KeySite.copyKey = copyKey;
})(KeySite || (KeySite = {}));
var keys = document.getElementsByClassName("break");
for (var i = 0; i < keys.length; i++) {
    keys[i].addEventListener("click", KeySite.copyKey);
}
;
//# sourceMappingURL=default.js.map