const path = require("path");
const appConfig = require("config-tsx");
const CheckerPlugin = require("awesome-typescript-loader").CheckerPlugin;
const merge = require("webpack-merge");

const options = {
    "client-root": "Client",
    "input-dir": path.posix.normalize(`${__dirname}/ClientApp/`),
    "entry-file": path.posix.normalize(`${__dirname}/ClientApp/boot-server.tsx`),
    "output-dir": path.posix.normalize(`${__dirname}/ClientApp/dist`)
};

const appSettings = appConfig.createPaths(__dirname, options);

appConfig.createTsConfig(__dirname, appSettings["out-dir"]);

module.exports = env => {
    const sharedConfig = () => ({
        stats: { 
            modules: false 
        },
        resolve: {
            extensions: [".js", ".jsx", ".ts", ".tsx"]
        },
        output: {
            filename: "[name].js",
            publicPath: "dist/"
        },
        module: {
            rules: [
                {
                    test: /\.jsx?$/,
                    include: [
                        appSettings["input-dir"]
                    ],
                    loader: "babel-loader",
                    options: {
                        presets: [
                            "es2015",
                            "react"
                        ]
                    }
                },
                {
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
                }
            ]
        },
        plugins: [new CheckerPlugin()]
    });

    const serverConfig = merge(sharedConfig(), {
        resolve: {
            mainFields: ["main"]
        },
        entry: {
            "main-server": appSettings["entry-file"]
        },
        output: {
            libraryTarget: "commonjs",
            path: appSettings["output-dir"]
        },
        target: "node",
        devtool: "inline-source-map"
    });

    return serverConfig;
    // entry: appSettings["entry-file"],
    // output: {
    //     path: appSettings["output-dir"],
    //     publicPath: "/dist/",
    //     filename: "main-server.js"
    // },
    // ,
    // plugins: [new CheckerPlugin()],
    // resolve: {
    //     modules: [
    //         "node_modules",
    //         appSettings["input-dir"]
    //     ],
    //     extensions: [".js", ".jsx", ".json", ".ts", ".tsx"]
    // },
    // devtool: "source-map",
    // // devServer: {
    // //     contentBase: appSettings["output-dir"],
    // //     compress: true,
    // //     port: 3000,
    // //     historyApiFallback: true
    // // },
    // context: __dirname,
    // externals: {
    //     react: "React",
    //     "react-dom": "ReactDOM"
    // },
    // target: "node"
}