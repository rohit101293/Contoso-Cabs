var express = require('express');
var config = require('../config');
var constants = require('../constants');
var bcrypt = require('bcrypt-nodejs');
var database = require('../utils/database');
var jwt = require('jsonwebtoken');
var router = express.Router();
var response = constants.response;
router.post('/', function (req, res, next) {
    if (req.body.mobile == null || req.body.password == null 
        || req.body.mobile == "" || req.body.password == "") {
        res.end(JSON.stringify(response.mysql.nullValues));
    }
    else {
        var data = {
            mobile: req.body.mobile,
            password: req.body.password
        };
        database.executeQuery({ sql: 'SELECT * FROM `users` WHERE `mobile` = ? ', values: [data.mobile] }, callback);
        function callback(qerr, results, fields) {
            if (!qerr) {
                if (!results) {
                    res.end(JSON.stringify(response.dbError));
                }
                else if (results.length == 0) {
                    res.end(JSON.stringify(response.mysql.noSuchValue));
                }
                else {
                    var hash = results[0].password;
                    hash = hash.replace(/^\$2y(.+)$/i, '\$2a$1');
                    console.log(hash);
                    bcrypt.compare(data.password, hash, function (herr, isValid) {
                        console.log('is valid is : ' + isValid);
                        if (herr) {
                            res.end(JSON.stringify(response.mysql.nullValues));
                        } else if (isValid) {
                            var jwtData = JSON.parse(JSON.stringify(data));
                            jwtData.email = results[0].email;
                            jwtData.name = results[0].name;
                            delete jwtData.password;
                            var details = JSON.parse(JSON.stringify(response.success));
                            details.token = jwt.sign(jwtData, config.secret, {
                                expiresIn: 86400
                            });
                            details.data = {
                                name: results[0].name,
                                email: results[0].email,
                                mobile: results[0].mobile
                            };
                            res.end(JSON.stringify(details));
                        } else {
                            res.end(JSON.stringify(response.mysql.fieldsMismatch));
                        }
                        
                    });
                }
            } else {
                console.log(JSON.stringify(response.dbError));
            }
        }
    }
});

module.exports = router;