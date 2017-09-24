const path = require("path");
const appConfig = require("config-tsx");
const CheckerPlugin = require("awesome-typescript-loader").CheckerPlugin;
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const merge = require("webpack-merge");
const webpack = require("webpack");

const options = {
    "client-root": "ClientApp",
    "input-dir": path.normalize(`${__dirname}/ClientApp/`),
    "entry-file": path.normalize(`${__dirname}/ClientApp/boot-server.tsx`),
    "output-dir": path.normalize(`${__dirname}/ClientApp/dist`)
};

const appSettings = appConfig.createPaths(__dirname, options);

appConfig.createTsConfig(__dirname, appSettings["out-dir"]);

module.exports = env => {
    const isDevBuild = !(env && env.prod);
    console.log("webpack: ", isDevBuild);

    const sharedConfig = () => ({
        stats: {
            modules: false
        },
        resolve: {
            extensions: [".js", ".jsx", ".ts", ".tsx"]
        },
        output: {
            filename: "[name].bundle.js",
            publicPath: "dist/"
        },
        module: {
            rules: [{
                    test: /\.tsx?$/,
                    include: [
                        appSettings["input-dir"]
                    ],
                    loader: "awesome-typescript-loader",
                    options: {
                        useBabel: true,
                        babelOptions: {
                            presets: [
                                "es2015",
                                "react"
                            ]
                        },
                        useCache: true
                    }
                },
            ]
        },
        plugins: [new CheckerPlugin()]
    });

    const serverConfig = merge(sharedConfig(), {
        resolve: {
            mainFields: ["main"]
        },
        entry: {
            "server": path.normalize(`${appSettings["input-dir"]}/boot-server.tsx`)
        },
        output: {
            libraryTarget: "commonjs",
            path: appSettings["output-dir"]
        },
        target: "node",
        devtool: "inline-source-map",
        plugins: [
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require(`./ClientApp/dist/vendor-manifest.json`),
                name: "./vendor",
                sourceType: "commonjs2"
            })
        ]
    });

    const clientBundleOutputDir = path.normalize(`${__dirname}/wwwroot/dist`);
    const clientConfig = merge(sharedConfig(), {
        entry: {
            "client": path.normalize(`${appSettings["input-dir"]}/boot-client.tsx`)
        },
        module: {
            rules: [
                {
                    test: /\.css$/,
                    use: ExtractTextPlugin.extract({ use: isDevBuild ? "css-loader" : "css-loader?minimize"})
                }
            ]
        },
        output: {
            path: clientBundleOutputDir
        },
        plugins: [
            isDevBuild ? 
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map', // Remove this line if you prefer inline source maps
                moduleFilenameTemplate: path.relative(clientBundleOutputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
            }) : 
            new webpack.optimize.UglifyJsPlugin(),
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require(`${clientBundleOutputDir}/vendor-manifest.json`)
            }),
            new ExtractTextPlugin("style.css")
        ],
    });

    return [clientConfig, serverConfig];
}