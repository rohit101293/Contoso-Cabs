var config = require('../config');
var request = require('request');
module.exports = {
    sendMessage : function (mobile, message, callback) {
        var smsdata = JSON.parse(JSON.stringify(config.otp.data));
        smsdata.mobiles = mobile;
        smsdata.message = message;
        var str = Object.keys(smsdata).map(function (key) {
            return encodeURIComponent(key) + '=' + encodeURIComponent(smsdata[key]);
        }).join('&');
        var url = config.otp.url + '?' + str;
        request.get({ url : url }, function (err, resp, html) {
            console.log(JSON.stringify(resp));
            callback(err, resp, html);
        });
    }
}