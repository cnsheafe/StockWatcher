const path = require("path");
const appConfig = require("config-tsx");


const options = {
    "client-root": "Client",
    "input-dir": path.posix.normalize(`${__dirname}/Client/src`),
    "entry-file": path.posix.normalize(`${__dirname}/Client/src/index.tsx`),
    "output-dir": path.posix.normalize(`${__dirname}/Client/dist`)
};

const appSettings = appConfig.createPaths(__dirname, options);

appConfig.createTsConfig(__dirname, appSettings["out-dir"]);

module.exports = {
    entry: appSettings["entry-file"],
    output: {
        path: appSettings["output-dir"],
        filename: "bundle.js"
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
    resolve: {
        modules: [
            "node_modules",
            appSettings["input-dir"]
        ],
        extensions: [".js", ".jsx", ".json", ".ts", ".tsx"]
    },
    devtool: "source-map",
    devServer: {
        contentBase: appSettings["output-dir"],
        compress: true,
        port: 3000,
        historyApiFallback: true
    },
    context: __dirname,
    externals: {
        react: "React",
        "react-dom": "ReactDOM"
    }
}