var request = require('request');
var express = require('express');
var constants = require('../constants');
var config = require('../config');
const url = require('url');
const querystring = require('querystring');
var router = express.Router();
var response = constants.response;
router.post('/', function (req, res, next) {
    var latlng = req.body.latlng;
    if (latlng == null || latlng == "") {
        res.end(JSON.stringify(response.mysql.nullValues));
    }
    else {
        var regeoMapsParams = JSON.parse(JSON.stringify(config.maps.reverseGeoRequiredParametersLatlng));
        regeoMapsParams.latlng = latlng;
        var regeoMapsUrl = url.parse(config.maps.geoEndPoint);
        regeoMapsUrl.query = regeoMapsParams;
        console.log(regeoMapsUrl.format());
        request.get(regeoMapsUrl.format(), function (e, r, b) {
			if (!e) {
                var results = JSON.parse(b).results[0].formatted_address;
                var success = JSON.parse(JSON.stringify(response.success));
                success.location = results;
                res.end(JSON.stringify(success));
            } else {
                res.end(JSON.stringify(response.gMapsError));
            }
        });
    }
});
module.exports = router;