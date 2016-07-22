module.exports = {
	ola: {
		appToken: '2c623e867c1a4cf39c43a4757a207f08',
		url: 'http://sandbox-t.olacabs.com/',
		authBearer: 'Bearer 631404119bfc4097b6aac95ce160933c',
		endPoints : {
			products : '/v1/products'
		},
		productParameters : {
			pickup_lat : 12.9491416,
			pickup_lng : 77.64298,
			drop_lat : 12.96,
			drop_lng : 77.678,
			category : 'sedan'
		},
		defaultHeaders : {
			'X-APP-TOKEN' : '2c623e867c1a4cf39c43a4757a207f08',
			'Authorization' : 'Bearer 631404119bfc4097b6aac95ce160933c'
		}
	},
	uber: {
		serverToken : 'xWdF0OJ17kVJmOCGSE4sm4eD_FmxK7QjtWbE310a',
		clientID : 'Em97LD4IkphirJAmHJHMtUNvzmFSS9U3',
		clientSecret : 'jB1PDlt2OXwdQGG8AGqZmlc_n_unVcGrOltjT_SB',
		oauthURL : 'https://login.uber.com/oauth/v2/token',
		redirectURI : 'https://contosocabs.azurewebsites.net/oauth/uber',
		url : 'https://api.uber.com/',
		sandboxUrl : 'https://sandbox-api.uber.com/',
		endPoints : {
			products : '/v1/products',
			time : '/v1/estimates/time',
			price : '/v1/estimates/price',
			requests : '/v1/requests',
			sandboxRequest : '/v1/sandbox/requests'
		},
		oAuthParameters: {
			client_secret : 'jB1PDlt2OXwdQGG8AGqZmlc_n_unVcGrOltjT_SB',
			client_id : 'Em97LD4IkphirJAmHJHMtUNvzmFSS9U3',
			grant_type : 'authorization_code',
			redirect_uri : 'https://contosocabs.azurewebsites.net/oauth/uber',
			code : 'put it here'
		},
		productParameters : {
			latitude : 12.9491416,
			longitude : 77.64298,
			server_token : 'xWdF0OJ17kVJmOCGSE4sm4eD_FmxK7QjtWbE310a'
		},
		timeParameters : {
			start_latitude : 12.9491416,
			start_longitude : 77.64298,
			server_token  : 'xWdF0OJ17kVJmOCGSE4sm4eD_FmxK7QjtWbE310a'
		},
		priceParameters : {
			start_latitude : 12.9491416,
			start_longitude : 77.64298,
			end_latitude : 12.96,
			end_longitude : 77.678,
			server_token  : 'xWdF0OJ17kVJmOCGSE4sm4eD_FmxK7QjtWbE310a'
		},
		requestsParameters : {
			start_latitude : 12.9491416,
			start_longitude : 77.64298,
			end_latitude : 12.96,
			end_longitude : 77.678,
			product_id : 'aaa'
		}
	},
	otp : {
		url : 'https://control.msg91.com/api/sendhttp.php',
		data : {
			authkey : '111347ANDsZtRM1DZu57278d8e',
			sender : 'COCABS',
			route : '4',
			country : '91',
			response : 'json'
		}
	},
	db : {
		host : 'ap-cdbr-azure-southeast-b.cloudapp.net',
		user : 'b3ef0de8c6032b',
		password : 'a0e61f91',
		database : 'cortanacabs',
		port : 3306
	},
	maps : {
		apiKey : 'AIzaSyBhlUdNlPVoQ926MmBsAIIPI0v96ENgW_Q',
		endPoint: 'https://maps.googleapis.com/maps/api/place/autocomplete/json',
		geoEndPoint: 'https://maps.googleapis.com/maps/api/geocode/json',
		reGeoEndPoint: 'https://maps.googleapis.com/maps/api/geocode/json',
		requiredParameters : {
			input : 'manjeera',
			key : 'AIzaSyBhlUdNlPVoQ926MmBsAIIPI0v96ENgW_Q',
			location : '17.4293763,78.3398589',
			radius : 2000
		},
		geoRequiredParameters: {
			address: 'microsoft hyderabad',
			key: 'AIzaSyBhlUdNlPVoQ926MmBsAIIPI0v96ENgW_Q',
		},
		reverseGeoRequiredParametersPlaceid: {
			place_id: 'ChIJL1sy6oKTyzsRKXDK3BQcle4',
			key: 'AIzaSyBhlUdNlPVoQ926MmBsAIIPI0v96ENgW_Q'
		},
		reverseGeoRequiredParametersLatlng: {
			latlng : '',
			key: 'AIzaSyBhlUdNlPVoQ926MmBsAIIPI0v96ENgW_Q'
		}
	},
	secret : 'conisstobcghtsconasnrtasia'
};