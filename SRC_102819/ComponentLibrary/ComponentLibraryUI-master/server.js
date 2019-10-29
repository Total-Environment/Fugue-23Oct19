var express = require('express');

var server = express();
server.use('/static', express.static(__dirname + '/static'));
server.get('/*', function(req, res){
    res.sendFile(__dirname + '/index.html');
});

var port = process.env.PORT || 8080;
server.listen(port, function() {
    console.log('server listening on port ' + port);
});
