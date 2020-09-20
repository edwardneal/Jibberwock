const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = {
    rules: [
        {
            test: /\.s(c|a)ss$/,
            use: [
                'vue-style-loader',
                MiniCssExtractPlugin.loader,
                'css-loader',
                {
                    loader: 'sass-loader',
                    // Requires sass-loader@^7.0.0
                    options: {
                        implementation: require('sass'),
                        fiber: require('fibers'),
                        indentedSyntax: false // optional
                    },
                    // Requires sass-loader@^8.0.0
                    options: {
                        implementation: require('sass'),
                        sassOptions: {
                            fiber: require('fibers'),
                            indentedSyntax: false // optional
                        },
                    },
                },
            ],
        },
    ],
    output: {
        filename: 'js/[name]-[hash].js',
        chunkFilename: 'js/[name]-[hash].js'
    }
}
