module.exports = {
  "transpileDependencies": [
    "vuetify"
    ],
    "productionSourceMap": false,
    "configureWebpack": {
        "output": {
            "filename": 'js/[name]-[hash].js',
            "chunkFilename": 'js/[name]-[hash].js'
        }
    },
    "integrity": true,
    "pages": {
        "index": {
            entry: 'src/main.js',
            title: 'Homepage - Jibberwock',
            filename: 'index.html',
            pageDescription: 'Jibberwock is a site to display a number of open-source projects.',
            childPath: ''
        },
        "proxyvirt/index": {
            entry: 'src/main.js',
            title: 'ProxyVirt - Jibberwock',
            filename: 'proxyvirt/index.html',
            pageDescription: 'ProxyVirt virtualises a standard web proxy, allowing client traffic to be dynamically routed between networks.',
            childPath: '/proxyvirt'
        },
        "statuspageio-client/index": {
            entry: 'src/main.js',
            title: 'statuspage.io Client - Jibberwock',
            filename: 'statuspageio-client/index.html',
            pageDescription: 'The statuspage.io client allows .NET and PowerShell clients to access data from a public statuspage.io site.',
            childPath: '/statuspageio-client'
        },
        "allert/index": {
            entry: 'src/main.js',
            title: 'Allert - Jibberwock',
            filename: 'allert/index.html',
            pageDescription: 'Jibberwock Allert is a cloud alerting engine which evaluates data from any source and runs user-specified rules to generate alerts.',
            childPath: '/allert'
        },
        "privacy/index": {
            entry: 'src/main.js',
            title: 'Privacy - Jibberwock',
            filename: 'privacy/index.html',
            pageDescription: 'Jibberwock is a site to display a number of open-source projects.',
            childPath: '/privacy'
        }
    }
}