var path = require('path');
module.exports = {
    mode: "development",
    entry: './script/home.ts',
    //devtool: 'inline-source-map',
    output: {
        path: path.resolve(__dirname, 'wwwroot/js'),
        filename: 'home.js',
        libraryTarget: 'var',
        library: 'Vue'
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/
            }
        ]
    },
    resolve: {
        alias: {
            'vue$': 'vue/dist/vue.esm.js'
        },
        extensions: ['.tsx', '.ts', '.js']
    },
};
//# sourceMappingURL=webpack.config.js.map