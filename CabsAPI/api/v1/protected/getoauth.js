var request = require('request');
var express = require('express');
var constants = require('../constants');
var response = constants.response;
var config = require('../config');
var Cab = require('../../models/Cab');
var Fare = require('../../models/Fare');
var database = require('../utils/database');
const url = require('url');
const querystring = require('querystring');
var router = express.Router();
router.post('/', function (req, res, next) {
	var code = req.body.code;
	if (code == null || code == "") {
		res.end(JSON.stringify(response.mysql.nullValues));
	}
	else {
		var uberData = JSON.parse(JSON.stringify(config.uber.oAuthParameters));
		uberData.code = code;
		request.post(config.uber.oauthURL, { form : uberData }, function (e, r, b) {
			if (!e) {
				var data = JSON.parse(b);
				if (data.access_token == null) {
					res.end(JSON.stringify(constants.response.uberError));
				} else {
					// res.end(JSON.stringify(constants.response.success));
					var email = req.decoded.email;
					var token = data.access_token;
					// res.end(token);
					var account = "UBER";
					database.executeQuery({
						sql : 'INSERT INTO user_accounts(email, account, oauth_token) VALUES(?,?,?) ON DUPLICATE KEY UPDATE email = VALUES(email)',
						values : [email, account, token]
					}, callback);
				}
				function callback(qe, qr, qf) {
					if (!qe) {
						res.end(JSON.stringify(response.success));
					} else if (qe.code == 'ER_DUP_ENTRY') {
						res.end(JSON.stringify(response.mysql.duplicates));
					} else {
						res.end(JSON.stringify(qe));
						res.end(JSON.stringify(response.dbError));
					}
				}

			} else {
				res.end(JSON.stringify(e));
			}
		});

	}
});
module.exports = router;