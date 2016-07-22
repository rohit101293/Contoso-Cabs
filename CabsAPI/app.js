var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var response = require('./api/v1/constants.js').response;
var routes = require('./routes/index');
var users = require('./routes/users');
var signup = require('./api/v1/auth/signup.js');
var signin = require('./api/v1/auth/signin.js');
var forgot = require('./api/v1/auth/forgot.js');
var middleware = require('./api/v1/protected/middleware.js');
var nearby = require('./api/v1/protected/nearby.js');
var search = require('./api/v1/protected/search.js');
var getoauth = require('./api/v1/protected/getoauth.js');
var geocode = require('./api/v1/protected/geocode.js');
var regeocode = require('./api/v1/protected/reverseGeocode.js');
var regeocodelatlng = require('./api/v1/protected/reverseGeocodeLatlng.js');
var profile = require('./api/v1/protected/profile.js');
var estimate = require('./api/v1/protected/estimate.js');
var tandc = require('./routes/tandc.js');
var authSuccess = require('./routes/authSuccess.js');
var places = require('./api/v1/protected/places.js');
var book = require('./api/v1/protected/book.js');
var uberAuth = require('./routes/uberAuth.js');
var app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

// uncomment after placing your favicon in /public
//app.use(favicon(__dirname + '/public/favicon.ico'));
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(require('stylus').middleware(path.join(__dirname, 'public')));
app.use(express.static(path.join(__dirname, 'public')));

app.use('/', routes);
app.use('/users', users);
app.use('/api/v1/signin', signin);
app.use('/api/v1/signup', signup);
app.use('/api/v1/forgot', forgot);
app.use('/api/v1/private', middleware);
app.use('/api/v1/private/nearby', nearby);
app.use('/api/v1/private/search', search);
app.use('/api/v1/private/getoauth', getoauth);
app.use('/api/v1/private/profile', profile);
app.use('/api/v1/private/geocode', geocode);
app.use('/api/v1/private/reverseGeocode', regeocode);
app.use('/api/v1/private/reverseGeocodeLatlng', regeocodelatlng);
app.use('/api/v1/private/estimate', estimate);
app.use('/api/v1/private/book', book);
app.use('/tc', tandc);
app.use('/oauth/uber', authSuccess);
app.use('/api/v1/private/places' , places);
app.use('/uber/login', uberAuth);
// catch 404 and forward to error handler
app.use(function (req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function (err, req, res, next) {
        res.status(err.status || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use(function (err, req, res, next) {
    res.status(err.status || 500);
    res.render(JSON.stringify(response.inServerError));
});


module.exports = app;
