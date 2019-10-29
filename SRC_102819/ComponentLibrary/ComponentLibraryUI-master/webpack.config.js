var webpack = require('webpack');
const { resolve } = require('path');

module.exports = {
  entry: [
    'react-hot-loader/patch',
    // activate HMR for React

    'webpack-dev-server/client?http://localhost:8080',
    // bundle the client for webpack-dev-server
    // and connect to the provided endpoint

    'webpack/hot/only-dev-server',
    // bundle the client for hot reloading
    // only- means to only hot reload for successful updates

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
  devtool: 'eval',

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
        test: /\.(woff|ttf|eot)$/,
        loader: 'url-loader'
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
  devServer: {
    hot: true,
    publicPath: '/',
    contentBase: __dirname,
    historyApiFallback: {
      index: 'index.html',
      disableDotRule: true
    },
    proxy: {
      "/static": {
        target: "http://localhost:8080",
        pathRewrite: { "^/static": "" }
      }
    }
  },
  plugins: [
    new webpack.HotModuleReplacementPlugin(),
    // enable HMR globally

    new webpack.NamedModulesPlugin(),
    // prints more readable module names in the browser console on HMR updates,
  ],
};
