var webpackConfig = require('./webpack.config');
// Karma configuration
module.exports = function(config) {
  config.set({
    browsers: ["PhantomJS"],
    frameworks: ["mocha"],
    // ... normal karma configuration
    files: [
      '__tests__/setup.js',
      // only specify one entry point
      // and require all tests in there
      {pattern: '__tests__/test_index.js', watched: false}
    ],

    webpack: webpackConfig,
    reporters: ['mocha'],

    preprocessors: {
      // add webpack as preprocessor
      '__tests__/test_index.js': ['webpack', 'sourcemap'],
    },

    webpackMiddleware: {
      // webpack-dev-middleware configuration
      // i. e.
      stats: 'errors-only'
    },
    browserNoActivityTimeout: 100000,
    mochaReporter: {
      showDiff: true
    }
  });
};
