var config = require('../config');
var response = require('../constants').response;
module.exports = {
    executeQuery : function (query, callback) {
        var sqlConn = require('mysql').createConnection(config.db);
        sqlConn.connect(function (conError) {
            if (!conError) {
                sqlConn.query({ sql : query.sql, values : query.values }, function (e, r, f) {
                    callback(e, r, f);
                });
            } else {
                console.log(JSON.stringify(conError));
                callback(response.dbError, null, null);
            }
            sqlConn.end(function (err) {

            });
        });

    }
};