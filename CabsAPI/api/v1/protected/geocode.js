var request = require('request');
var express = require('express');
var constants = require('../constants');
var config = require('../config');
const url = require('url');
const querystring = require('querystring');
var router = express.Router();
var response = constants.response;
router.post('/', function (req, res, next) {
    var address = req.body.address;
    if (address == null || address == "") {
        res.end(JSON.stringify(response.mysql.nullValues));
    }
    else {
        var geoMapsParams = JSON.parse(JSON.stringify(config.maps.geoRequiredParameters));
        geoMapsParams.address = address;
        var geoMapsUrl = url.parse(config.maps.geoEndPoint);
        geoMapsUrl.query = geoMapsParams;
        request.get(geoMapsUrl.format(), function (e, r, b) {
			if (!e) {
				try {
					
					var lat = JSON.parse(b).results[0].geometry.location.lat;
					var lng = JSON.parse(b).results[0].geometry.location.lng;
					var success = JSON.parse(JSON.stringify(response.success));
					success.position = {
						lat: lat,
						lng: lng,
					};
					res.end(JSON.stringify(success));
				} catch (ex) {
					res.end(JSON.stringify(response.gMapsError));
				}

            } else {
                res.end(JSON.stringify(response.gMapsError));
            }
        });
    }
});
module.exports = router;