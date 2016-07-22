var express = require('express');
var config = require('../config');
var constants = require('../constants');
var smsapi = require('../utils/smsapi');
var bcrypt = require('bcrypt-nodejs');
var database = require('../utils/database');
var router = express.Router();
var response = constants.response;
var max = 999999;
var min = 100000;

router.post('/', function (req, res, next) {
    var mobile = req.body.mobile;
    var newPassword = req.body.newPassword;
    if (mobile == null || mobile == "" || mobile.length != 10) {
        res.end(JSON.stringify(response.mysql.nullValues));
    } else if (newPassword == null || newPassword == "") {
        var otp = Math.floor(Math.random() * (max - min)) + min;
        smsapi.sendMessage(mobile, "Your OTP for Contoso Cabs is " + otp, function (err, resp, html) {
            if (!err) {
                var op = JSON.parse(JSON.stringify(response.success));
                op.otp = otp;
                res.end(JSON.stringify(op));
            } else {
                res.end(JSON.stringify(response.dbError));
            }
        });

    } else {
        var password = bcrypt.hashSync(newPassword, bcrypt.genSaltSync(10));
		database.executeQuery({ sql : 'UPDATE `users` SET `password` = ? WHERE `mobile` = ?', values : [password, mobile] }, callback);
        function callback(err, results, fields) {
            if (!err) {
                res.end(JSON.stringify(response.success));
            } else {
                console.log('critical error with reset password');
                console.log(err);
                res.end(JSON.stringify(response.dbError));
            }
        }
    }
});
module.exports = router;