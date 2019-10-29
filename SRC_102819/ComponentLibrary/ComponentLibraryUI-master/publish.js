var octo = require('@octopusdeploy/octopackjs');
var fs = require('fs');
var ncp = require('ncp').ncp;
var rimraf = require('rimraf');
var dir = require('node-dir');

var push = function (name) {
    octo.push('./bin/' + name, {
        host: 'http://' + process.argv[3],
        apikey: process.argv[2],
        replace: true
    }, function (err, body) {
        if (err) {
            console.log(err);
            process.exit(1);
        }
        console.log("Package Pushed:" + body.Title);
    });
};
var buildNumber = process.argv[4];

if(fs.existsSync('build')) {
    rimraf.sync('build');
}
fs.mkdirSync('build');

var content = fs.readFileSync('package-server.json', {encoding: 'utf8'});
content = content.replace('[buildNumber]', buildNumber);
fs.writeFileSync('build/package.json', content);
fs.writeFileSync('build/server.js', fs.readFileSync('server.js'));
fs.writeFileSync('build/index.html', fs.readFileSync('index-server.html'));
ncp('static', 'build/static', function(err) {
    if(err) {
        console.log(err);
        return;
    }
    ncp('fonts', 'build/static/fonts', function(err) {
      if(err) {
          console.log(err);
          return;
      }
      var pack = octo.pack();

      dir.files('./build', function(err, files){
        files.forEach(function(file) { pack.append(file.replace('build', ''), fs.createReadStream(file)); });
        pack
          .toFile('./bin', function (err, data) {
            if (err) {
              console.log(err);
              process.exit(1);
            }
            console.log("Package Saved: " + data.name);
            push(data.name);
          });
      });
    });
});
