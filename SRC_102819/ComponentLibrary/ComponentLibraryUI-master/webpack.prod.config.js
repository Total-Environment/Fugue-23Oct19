var webpack = require('webpack');
const { resolve } = require('path');

module.exports = {
  entry: [
    'babel-polyfill',
    'whatwg-fetch',
    './src/index.js'
    // the entry point of our app
  ],
  output: {
    filename: 'bundle.js',
    path: resolve(__dirname, 'static'),
    publicPath: '/static/'
  },

  module: {
  	rules: [
  		{
  			test: /\.js$/,
        exclude: /node_modules/,
  			loader: "babel-loader"
  		},
  		{
  			test: /\.css$/,
  			use: [
          'style-loader',
          { loader: 'css-loader', options: {
            modules: true,
            localIdentName: '[path][name]__[local]--[hash:base64:5]'
          } },
          'postcss-loader'
        ]
  		},
  		{
  			test: /\.png$/,
  			loader: "file-loader"
  		},
      {
        test: /\.svg$/,
        loader: "babel-loader"
      },
      {
        test: /\.ico$/,
        loader: 'file-loader?name=[name].[ext]'
      },
  	],
  },

  externals: {
	  'cheerio': 'window',
	  'react/addons': true,
	  'react/lib/ExecutionEnvironment': true,
	  'react/lib/ReactContext': true
	},
};
