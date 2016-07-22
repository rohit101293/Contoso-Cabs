var express = require('express');
var router = express.Router();
var Uber = require('node-uber');
var config = require('../api/v1/config.js');
/* GET home page. */
router.get('/', function (req, res) {
	var uber = new Uber({
		client_id: config.uber.clientID,
		client_secret: config.uber.clientSecret,
		server_token: config.uber.serverToken,
		redirect_uri: config.uber.redirectURI,
		name: 'CONTOSOCABS',
		language: 'en_US', // optional, defaults to en_US
		sandbox: true // optional, defaults to false
	});
	var url = uber.getAuthorizeUrl(['history', 'profile', 'request', 'places']);
	res.redirect(url);
});

module.exports = router;