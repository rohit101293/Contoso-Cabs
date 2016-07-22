var request = require('request');
var express = require('express');
var constants = require('../constants');
var response = constants.response;
var config = require('../config');
var Cab = require('../../models/Cab');
var Fare = require('../../models/Fare');
var Estimate = require('../../models/Estimate');
var CabEstimate = require('../../models/CabEstimate');
const url = require('url');
const querystring = require('querystring');
var Uber = require('node-uber');
var database = require('../utils/database');
var router = express.Router();
router.post('/', function (req, res, next) {
	try {
		
		var product_id = req.body.product_id;
		var token = req.decoded.token;
		var start_latitude = req.body.start_latitude;
		var start_longitude = req.body.start_longitude;
		var end_latitude = req.body.end_latitude;
		var end_longitude = req.body.end_longitude;
		var is_real = (req.body.is_real == "true");
		console.log(is_real);
		var email = req.decoded.email;
		var null_check_params = [start_latitude, start_longitude];
		var null_opt = [product_id, end_latitude, end_longitude];
		for (var i in null_check_params) {
			if (null_check_params[i] == null || null_check_params[i] == "") {
				res.end(JSON.stringify(response.mysql.nullValues));
			}
		}
		//get the uber-server-token
		database.executeQuery({ sql : 'SELECT * FROM user_accounts WHERE `email` = ?', values : [email] }, function (qe, qr, qf) {
			if (qe) {
				console.log('error with db');
				console.log(JSON.stringify(qe));
			} else {
				
				if (qr.length == 0 || qr == null || !qr[0].hasOwnProperty('oauth_token')) {
					res.end(JSON.stringify(response.sessionExpired));
				} else {
					var uberUrl = url.parse(config.uber.sandboxUrl);
					if (is_real) {
						uberUrl = url.parse(config.uber.url);
					}
					var oauth_token = qr[0].oauth_token;
					uberUrl.pathname = config.uber.endPoints.requests;
					var uberParameters = JSON.parse(JSON.stringify(config.uber.requestsParameters));
					uberParameters.start_latitude = start_latitude;
					uberParameters.end_latitude = end_latitude;
					uberParameters.start_longitude = start_longitude;
					uberParameters.end_longitude = end_longitude;
					uberParameters.product_id = product_id;
					for (var i in uberParameters) {
						if (uberParameters[i] == "" || uberParameters[i] == null) {
							delete uberParameters[i];
						}
					}
					var options = {
						uri : uberUrl.format(),
						method : 'POST',
						headers : {
							'Authorization' : 'Bearer ' + oauth_token
						},
						json : uberParameters
					};
					request(options, function (e, r, b) {
						if (!e) {
							console.log('error with post to ride');
							console.log(JSON.stringify(b));
							var request_id = b.request_id;
							console.log('logging request_id');
							console.log(request_id);
							if (request_id == null) {
								res.end(JSON.stringify(response.uberError));
							} else {
								if (is_real) {
									setTimeout(getTripDetails(request_id), 9000);
								} else {
									var detailsUrl = url.parse(config.uber.sandboxUrl);
									detailsUrl.pathname = config.uber.endPoints.sandboxRequest;
									var durlstr = detailsUrl.format() + '/' + request_id
									console.log(durlstr);
									var putAcceptedOptions = {
										uri : durlstr,
										method : 'PUT',
										headers : {
											'Authorization' : 'Bearer ' + oauth_token
										},
										json : {
											status : "accepted"
										}
									};
									request(putAcceptedOptions, function (eee, rrr, bbb) {
										if (eee) {
											res.end(JSON.stringify(response.uberError));
										} else {
											getTripDetails(request_id);
										}
									});
								}
							}
						} else {
							res.end(JSON.stringify(response.uberError));
						}
					});
					var getTripDetails = function (request_id) {
						var finalUrl = url.parse(config.uber.sandboxUrl);
						if (is_real) {
							finalUrl = url.parse(config.uber.url);
						}
						finalUrl.pathname = config.uber.endPoints.requests;
						var furlstr = finalUrl.format() + '/' + request_id;
						console.log(furlstr);
						var detailsOptions = {
							uri : furlstr,
							method : 'GET',
							headers : {
								'Authorization' : 'Bearer ' + oauth_token
							}
						}
						request(detailsOptions, function (ee, rr, bb) {
							if (!ee) {
								var success = JSON.parse(JSON.stringify(response.success));
								success.data = JSON.parse(bb);
								res.end(JSON.stringify(success));
							} else {
								res.end(JSON.stringify(response.uberError));
							}
						});
					}
				}
			}
			
		});
	} catch (ex) {
		console.log('exception raised');
		console.log(ex);
		var uberError = JSON.parse(JSON.stringify(response.uberError));
		uberError.error = ex.toString();
		res.end(JSON.stringify(uberError));
	}
});

module.exports = router;