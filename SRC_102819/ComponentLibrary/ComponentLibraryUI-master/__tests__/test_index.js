// test/test_index.js

// require all modules ending in "_test" from the
// current directory and all subdirectories

window.SUMOLOGIC_CONFIG = {
  endpoint: 'sumologic'
};

import 'babel-polyfill';
var testsContext = require.context(".", true, /_tests$/);
testsContext.keys().forEach(testsContext);

