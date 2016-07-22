var request = require('request');
var express = require('express');
var constants = require('../constants');
var config = require('../config');
var database = require('../utils/database');
var Cab = require('../../models/Cab');
var Fare = require('../../models/Fare');
const url = require('url');
const querystring = require('querystring');
var response = constants.response;
var router = express.Router();
router.post('/', function (req, res, next) {
	var email = req.decoded.email;
	var lat = req.body.lat;
	var lng = req.body.lng;
	var name = req.body.name;
	var store = true;
	if (lat == "" || lat == null || lng == "" || lng == null) {
		database.executeQuery({
			sql: 'SELECT lat,lng FROM user_locations WHERE `email`= ? AND `name` = ?',
			values : [email, name]
		}, function (qe, qr, qf) {
			if (!qe) {
				
				var success = JSON.parse(JSON.stringify(response.success));
				if (qr.length == 0 || qr == null) {
					res.end(JSON.stringify(response.mysql.noSuchValue));
				} else {
					success.data = qr[0];
					res.end(JSON.stringify(success));
				}
			} else {
				res.end(JSON.stringify(response.dbError));
			}
		});
	} else {
		database.executeQuery({
			sql: 'INSERT INTO user_locations VALUES(?, ?, ?, ?)',
			values : [name, lat, lng, email]
		}, function (qe, qr, qf) {
			if (!qe) {
				var success = JSON.parse(JSON.stringify(response.success));
				// success.data = qr[0];
				res.end(JSON.stringify(success));
			} else {
				res.end(JSON.stringify(response.dbError));
			}
		});
	}

});

module.exports = router;