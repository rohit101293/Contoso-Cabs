var request = require('request');
var express = require('express');
var constants = require('../constants');
var config = require('../config');
var Cab = require('../../models/Cab');
var Fare = require('../../models/Fare');
const url = require('url');
const querystring = require('querystring');
var router = express.Router();
router.post('/', function (req, res, next) {
	
	var pickUpLat = req.body.pickUpLat;
	var pickUpLong = req.body.pickUpLong;
	var dropLat = req.body.dropLat;
	var dropLong = req.body.dropLong;
	var carType = req.body.carType;
	var params = [pickUpLat, pickUpLong];
	for (var i in params) {
		if (params[i] == null || params[i] == "") {
			res.end(JSON.stringify(constants.response.nullValues));
		}
	}
	var olaParams = JSON.parse(JSON.stringify(config.ola.productParameters));
	var uberParams = JSON.parse(JSON.stringify(config.uber.productParameters));
	var uberTimeParms = JSON.parse(JSON.stringify(config.uber.timeParameters));
	var uberPriceParams = JSON.parse(JSON.stringify(config.uber.priceParameters));
	uberPriceParams.start_latitude = pickUpLat;
	uberPriceParams.start_longitude = pickUpLong;
	uberPriceParams.end_latitude = dropLat;
	uberPriceParams.end_longitude = dropLong;
	uberTimeParms.start_latitude = pickUpLat;
	uberTimeParms.start_longitude = pickUpLong;
	uberParams.latitude = pickUpLat;
	uberParams.longitude = pickUpLong;
	olaParams.pickup_lat = pickUpLat;
	olaParams.pickup_lng = pickUpLong;
	olaParams.drop_lat = dropLat;
	olaParams.drop_lng = dropLong;
	delete olaParams.category;
	var olaUrl = url.parse(config.ola.url);
	var uberUrl = url.parse(config.uber.url);
	var uberTimeUrl = url.parse(config.uber.url);
	var uberPriceUrl = url.parse(config.uber.url);
	uberUrl.query = uberParams;
	uberTimeUrl.query = uberTimeParms;
	uberPriceUrl.query = uberPriceParams;
	olaUrl.query = olaParams;
	uberPriceUrl.pathname = config.uber.endPoints.price;
	uberTimeUrl.pathname = config.uber.endPoints.time;
	uberUrl.pathname = config.uber.endPoints.products;
	olaUrl.pathname = config.ola.endPoints.products;
	var olaOptions = {
		url : olaUrl.format(),
		headers : config.ola.defaultHeaders
	};
	var olaResponses = [];
	
	function callback(error, response, body) {
		if (!error) {
			olaResponses = parseOlaCabs(JSON.parse(body));
		}
		request(uberUrl.format(), function (e, r, b) {
			if (!e) {
				//console.log(JSON.stringify(JSON.parse(b)));
				var uberResponses = parseUberCabs(JSON.parse(b));
				//res.end(JSON.stringify(uberResponses));
				//console.log(JSON.stringify(uberResponses));
				request(uberTimeUrl.format(), function (ee, rr, bb) {
					// res.end(JSON.stringify(bb));
					modifyUberResponses(uberResponses, bb);
					request(uberPriceUrl.format(), function (eee, rrr, bbb) {
						updateUberSurge(uberResponses, bbb);
						var fResponses = [];
						if (olaResponses == null && uberResponses == null) {
							res.end(JSON.stringify(constants.response.noCabsAvailable));
						} else {
							if (uberResponses == null) {
								fResponses = fResponses.concat(olaResponses);
							} else if (olaResponses == null) {
								fResponses = fResponses.concat(uberResponses);
							} 
							else {
								fResponses = olaResponses.concat(uberResponses);
							}
							var success = JSON.parse(JSON.stringify(constants.response.success));
							success.data = fResponses;
							res.end(JSON.stringify(success));
						}
					});
				});
			}
			else if (olaResponses.length == 0) {
				res.end(JSON.stringify(constants.response.noCabsAvailable));
			} else {
				var success = JSON.parse(JSON.stringify(constants.response.success));
				success.data = olaResponses;
				res.end(JSON.stringify(success));
			}
		})
	}
	request(olaOptions, callback);
	function updateUberSurge(uberResponses, bbb) {
		// console.log(JSON.stringify(bbb));
		var priceData = JSON.parse(bbb).prices;
		for (var i in uberResponses) {
			for (var j in priceData) {
				if (uberResponses[i].type == priceData[j].display_name) {
					if (priceData[j].surge_multiplier == null || priceData[j].surge_multiplier == "") {
						uberResponses[i].fare.surcharge = "No Surge!";
					} else {
						uberResponses[i].fare.surcharge = priceData[j].surge_multiplier || " x";
					}
				}
			}
		}
	}
	function modifyUberResponses(uberResponses, bb) {
		var timeData = JSON.parse(bb).times;
		for (var i in uberResponses) {
			for (var j in timeData) {
				if (uberResponses[i].type == timeData[j].display_name) {
					uberResponses[i].eta = Math.ceil(timeData[j].estimate / 60) + " min";
					break;
				}
			}
		}
	}
	function parseUberCabs(data) {
		//console.log(data);
		var cabData = [];
		if (! data.hasOwnProperty('products')) {
			return null;
		} else {
			var cabs = data.products;
			for (var i in cabs) {
				if (cabs[i].display_name != "POOL") {
					if (cabs[i].price_details == null) {
						continue;
					}
					var cab = new Cab("UBER", cabs[i].display_name, null, null, cabs[i].capacity, cabs[i].image, null);
					var unit = cabs[i].price_details.distance_unit;
					var costPerMinute = "₹" + Math.abs(cabs[i].price_details.cost_per_minute) + "/min";
					var costPerDistance = "₹" + Math.abs(cabs[i].price_details.cost_per_distance) + "/km";
					var basePrice = "₹" + Math.abs(cabs[i].price_details.base);
					cab.fare = new Fare(basePrice, costPerDistance, costPerMinute, "No Surge!");
					cabData.push(cab);
				}
			}
			return cabData;
		}
	}
	function parseOlaCabs(data) {
		console.log(JSON.stringify(data));
		var cabData = [];
		if (! data.hasOwnProperty('categories')) {
			console.log('baava')
			return null;
		} else {
			var cabs = data.categories;
			for (var i in cabs) {
				// for azure changes
				var cab = new Cab("OLA", cabs[i].display_name, Math.abs(cabs[i].eta) + " " + cabs[i].time_unit.substring(0, 3),
                    Math.abs(cabs[i].distance) + " " + cabs[i].distance_unit, 4, cabs[i].image, null);
				var fares = cabs[i].fare_breakup;
				for (var j in fares) {
					if (fares[j].type == "flat_rate") {
						var fare = new Fare("₹" + fares[j].base_fare, "₹" + fares[j].cost_per_distance + "/km"
                            , "₹" + fares[j].ride_cost_per_minute + "/min"
                            , null);
						if (fares[j].surcharge.length == 0) {
							fare.surcharge = "No Surge!";
						} else {
							fare.surcharge = fares[j].surcharge[0].value + "x";
						}
						cab.fare = fare;
					}
				}
				cabData.push(cab);
			}
			return cabData;
		}
	}
});

module.exports = router;