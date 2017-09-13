String.prototype.format = function (value) {
    return this.replace(new RegExp('#\\w+', 'gi'), function (match) {
        var name = match.substring(1);
        return value.hasOwnProperty(name) ? value[name] : match;
    });
};
String.prototype.isMobile = function () {
    return /^0?(13[0-9]|15[012356789]|18[0236789]|14[57])[0-9]{8}$/.test(this);
};
Array.prototype.where = function (str) { var rs = []; for (var i in this) { var o = this[i]; if (typeof (this[i]) != 'function') if (eval(str)) rs.push(o); } return rs };

var senparc = {
    get: function (url, callback) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', url, true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var result = JSON.parse(xhr.responseText);
                callback(result);
            }
        };
        xhr.send();
    }
};