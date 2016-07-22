var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function (req, res) {
	var code = req.query.code;
	if (code == null || code == "") {
		code = "not found";
	}
	res.render('authSuccess', { title: code });
});

module.exports = router;