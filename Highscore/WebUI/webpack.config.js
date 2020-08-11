// import modules
var webpack = require('webpack');
var path = require('path');

// define consts
const SOURCE_DIR = path.resolve(__dirname, './src/');
const BUILD_DIR = path.resolve(__dirname, 'build/public');
const PORT_DEV_SERVER = 3434;

// config and build process
var config = {
    mode: 'production',
    entry: ['webpack-dev-server/client?http://localhost:' + PORT_DEV_SERVER, SOURCE_DIR + '/index.js'],
    output: {
        path: BUILD_DIR,
        publicPath: 'public/',
        filename: "bundle.js"
    },
    // set dev-server configuration
    devServer: {
        inline: true,
        progress: true,
        contentBase: path.resolve('build'),
        port: PORT_DEV_SERVER
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                loader: 'babel-loader'
            },
            {
                test: /\.(sass|css)$/,
                use: ['style-loader', 'css-loader', 'sass-loader']
            },
            {
                test: /\.(png|woff|woff2|eot|ttf|svg|mp3|wav)$/i,
                loader: 'url-loader?limit=100000'
              }
        ]
    }
}

module.exports = config;
