var request = require('request');
var express = require('express');
var constants = require('../constants');
var config = require('../config');
const url = require('url');
const querystring = require('querystring');
var router = express.Router();
var response = constants.response;
router.post('/', function (req, res, next) {
	var input = req.body.input;
	var area = req.body.area || "";
	if (input == null || input == "") {
		res.end(JSON.stringify(response.mysql.nullValues));
	} else {
		var gMapsParams = JSON.parse(JSON.stringify(config.maps.requiredParameters));
		gMapsParams.input = input;
		if (area != "" || area != null) {
			gMapsParams.location = area;
		}
		var gMapsUrl = url.parse(config.maps.endPoint);
		gMapsUrl.query = gMapsParams;
		request.get(gMapsUrl.format(), function (e, r, b) {
			if (!e) {
				var data = JSON.parse(b).predictions;
				var success = JSON.parse(JSON.stringify(response.success));
				success.data = data
				res.end(JSON.stringify(success));
			} else {
				res.end(JSON.stringify(response.gMapsError));
			}
		});
	}
});
module.exports = router;
