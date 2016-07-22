var express = require('express');
var jwt = require('jsonwebtoken');
var constants = require('../constants');
var config = require('../config');
var response = constants.response;
var router = express.Router();

router.use('/', function (req, res, next) {
    var token = req.body.token || null;
    if (token) {
        jwt.verify(token, config.secret, { ignoreExpiration: true }, function (err, decoded) {
            if (err) {
                console.log(err);
                res.send(response.sessionExpired);
            } else {
                req.decoded = decoded;
                next();
            }
        });
    }
    else {
        res.status(403).send(response.permissionsRequired);
    }
});

module.exports = router;