const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const ExtractTextPlugin = require("extract-text-webpack-plugin");

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);

    console.log(isDevBuild);

    const sharedConfig = {
        stats: { modules: false },
        resolve: { extensions: [ '.js' ] },
        entry: {
            vendor: [
                'react',
                'react-dom',
                'react-router-dom',
                'react-redux',
                'redux',
                'redux-thunk',
                'rxjs',
                'chart.js'
            ],
        },
        output: {
            publicPath: 'dist/',
            filename: '[name].js',
            library: '[name]_[hash]',
        },
        plugins: [
            // new webpack.ProvidePlugin({ $: 'jquery', jQuery: 'jquery' }), // Maps these identifiers to the jQuery package (because Bootstrap expects it to be a global variable)
            new webpack.NormalModuleReplacementPlugin(/\/iconv-loader$/, require.resolve('node-noop')), // Workaround for https://github.com/andris9/encoding/issues/16
            new webpack.DefinePlugin({
                'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
            })
        ].concat(isDevBuild ? [] : [new webpack.optimize.UglifyJsPlugin()])
    };

    const clientBundleConfig = merge(sharedConfig, {
        output: { path: path.join(__dirname, 'wwwroot', 'dist') },
        plugins: [
            new webpack.DllPlugin({
                path: path.join(__dirname, 'wwwroot', 'dist', '[name]-manifest.json'),
                name: '[name]_[hash]'
            })
        ].concat(isDevBuild ? [] : [
            new webpack.optimize.UglifyJsPlugin({
                comments: false,
                compress: {
                    warnings: false
                },
                drop_console: true
            })
        ])
    });

    const serverBundleConfig = merge(sharedConfig, {
        target: 'node',
        resolve: { mainFields: ['main'] },
        output: {
            path: path.join(__dirname, 'ClientApp', 'dist'),
            libraryTarget: 'commonjs2',
        },
        entry: { vendor: ['aspnet-prerendering', 'react-dom/server'] },
        plugins: [
            new webpack.DllPlugin({
                path: path.join(__dirname, 'ClientApp', 'dist', '[name]-manifest.json'),
                name: '[name]_[hash]'
            })
        ]
    });

    return [clientBundleConfig, serverBundleConfig];
};
