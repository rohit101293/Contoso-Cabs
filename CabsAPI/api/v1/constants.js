module.exports = {
	response : {
		success: {
			code: 0,
			message: "Successful Excecution"
		},
		partialSuccess: {
			code: 1,
			message: "Partially Successful Excecution"
		},
		olaError: {
			code: 9,
			message: "Error with ola cabs"
		},
		gMapsError: {
			code: 5,
			message: "Error with google maps api"
		},
		uberError: {
			code: 8,
			message: "Error with uber cabs"
		},
		nullValues : {
			code : 7,
			message : "Null values in query string"
		},
		noCabsAvailable : {
			code : 3,
			message : "No cabs available now"
		},
		sessionExpired : {
			code : 2,
			message : "User session expired"
		},
		permissionsRequired : {
			code : 4,
			message : "Authentication is required"
		},
		securityError : {
			code : 777,
			message : "Internal security error"
		},
		dbError: {
			code : 888,
			message : "Internal database error"
		},
		inServerError : {
			code : 500,
			message : "Internal server error"
		},
		mysql : {
			duplicates: {
				code : 100,
				message : "Duplicate parameters in query"
			},
			nullValues: {
				code : 101,
				message : "Null values in query"
			},
			invalidQueryParameters : {
				code : 104,
				message : "Invalid query parameters"
			},
			noSuchValue: {
				code : 102,
				message : "No such value exists"
			},
			fieldsMismatch : {
				code : 103,
				message : "One or more fields don't match"
			}
		}
	}
};