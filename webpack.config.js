const path = require("path");
const appConfig = require("config-tsx");
const CheckerPlugin = require("awesome-typescript-loader").CheckerPlugin;
const merge = require("webpack-merge");
const webpack = require("webpack");

const options = {
  "client-root": "ClientApp",
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
      filename: "[name].bundle.js",
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
      "server": appSettings["entry-file"]
    },
    output: {
      libraryTarget: "commonjs",
      path: appSettings["output-dir"]
    },
    target: "node",
    devtool: "inline-source-map"
  });

  const clientBundleOutputDir = path.posix.resolve(__dirname, "./wwwroot/dist");
  const clientConfig = merge(sharedConfig(), {
    entry: {
      "client": path.posix.join(appSettings["input-dir"], "boot-client.tsx")
    },
    output: {
      path: clientBundleOutputDir
    },
    plugins: [
      new webpack.SourceMapDevToolPlugin({
        filename: '[file].map', // Remove this line if you prefer inline source maps
        moduleFilenameTemplate: path.relative(clientBundleOutputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
      })
    ],
  });

  return [clientConfig, serverConfig];
}