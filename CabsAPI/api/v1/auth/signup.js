var express = require('express');
var config = require('../config');
var mysql = require('mysql');
var constants = require('../constants');
var bcrypt = require('bcrypt-nodejs');
var database = require('../utils/database');
var router = express.Router();
var response = constants.response;
var request = require('request');
var jwt = require('jsonwebtoken');
router.post('/', function (req, res, next) {
    var data = {
        password: req.body.password,
        email: req.body.email,
        mobile: req.body.mobile,
        name: req.body.name
    };
    for (var key in data) {
        if (data[key] == "" || data[key] == null) {
            res.end(JSON.stringify(response.mysql.nullValues));
            return;
        }
    }
    var salt = bcrypt.genSaltSync(10);
    data.password = bcrypt.hashSync(data.password, salt);
    database.executeQuery({
        sql : 'INSERT INTO USERS(name, password, email, mobile) VALUES(?, ?, ?, ?)',
        values : [data.name, data.password, data.email, data.mobile]
    }, callback);
    function callback(qError, qResults, qFields) {
        if (!qError) {
            var op = JSON.parse(JSON.stringify(response.success));
            op.token = jwt.sign(data, config.secret, {
                expiresIn : 86400
            });
            res.end(JSON.stringify(op));
        } else if (qError.code == 'ER_DUP_ENTRY') {
            res.end(JSON.stringify(response.mysql.duplicates));
        } else {
            console.log(JSON.stringify(qError));
            res.end(JSON.stringify(qError));
        }
    }
});

module.exports = router;