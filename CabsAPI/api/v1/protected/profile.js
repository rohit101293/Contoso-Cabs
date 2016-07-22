var request = require('request');
var express = require('express');
var constants = require('../constants');
var config = require('../config');
const url = require('url');
const querystring = require('querystring');
var database = require('../utils/database');
var router = express.Router();
var response = constants.response;
router.post('/', function (req, res, next) {
	var email = req.decoded.email;
	var mobile = req.decoded.mobile;
	var name = req.decoded.name;
	database.executeQuery({ sql : 'SELECT account, oauth_token FROM  user_accounts WHERE `email` = ?', values : [email] }, callback);
	function callback(qe, qr, qf) {
		if (!qe) {
			var success = JSON.parse(JSON.stringify(response.success));
			success.data = {
				email : email,
				mobile : mobile,
				name : name,
				accounts : qr
			};
			res.end(JSON.stringify(success));
		} else {
			res.end(JSON.stringify(response.dbError));
		}
	}
});
module.exports = router;