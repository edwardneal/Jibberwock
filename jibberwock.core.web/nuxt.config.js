module.exports = {
    /*
    ** Headers of the page
    */
    head: {
        htmlAttrs: {
            lang: 'en',
            style: 'overflow-y: inherit;'
        },
        title: 'Homepage - Jibberwock',
        meta: [
            { charset: 'utf-8' },
            { 'http-equiv': 'X-UA-Compatible', content: 'IE=edge' },
            { name: 'viewport', content: 'width=device-width, initial-scale=1' },
            { name: 'rating', content: 'General' },

            { name: 'twitter:card', content: 'summary' },
            { name: 'twitter:dnt', content: 'on' },
            { name: 'twitter:image', content: 'https://www.jibberwock.com/images/favicons/android-chrome-512x512.png' },
            { name: 'twitter:image:alt', content: 'Jibberwock logo' },

            { hid: 'description', name: 'description', content: 'Jibberwock is a site to display a number of open-source projects.' },
            { name: 'subject', content: 'Jibberwock' },
            { name: 'application-name', content: 'Jibberwock' },

            { hid: 'og-title', property: 'og:title', content: 'Homepage - Jibberwock' },
            { property: 'og:site_name', content: 'Jibberwock' },
            { property: 'og:description', content: 'Jibberwock is a site to display a number of open-source projects.' },
            { hid: 'og-url', property: 'og:url', content: 'https://www.jibberwock.com' },
            { name: 'og:image', content: 'https://www.jibberwock.com/images/favicons/android-chrome-512x512.png' },
            { property: 'og:type', content: 'website' },

            { name: 'apple-mobile-web-app-capable', content: 'yes' },
            { name: 'msapplication-config', content: '/manifests/browserconfig.xml' },
        ],
        link: [
            { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
            { rel: 'shortcut icon', type: 'image/x-icon', href: '/favicon.ico' },
            { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css?family=Roboto:100,300,400,500,700,900&display=swap' },
            {
                rel: 'stylesheet', crossorigin: 'anonymous', href: 'https://cdn.jsdelivr.net/npm/@mdi/font@latest/css/materialdesignicons.min.css'
            },

            { rel: 'apple-touch-icon', sizes: '180x180', href: '/images/favicons/apple-touch-icon.png' },

            { rel: 'icon', type: 'image/png', sizes: '32x32', href: '/images/favicons/favicon-32x32.png' },
            { rel: 'icon', type: 'image/png', sizes: '192x192', href: '/images/favicons/android-chrome-192x192.png' },
            { rel: 'icon', type: 'image/png', sizes: '16x16', href: '/images/favicons/favicon-16x16.png' },
            { rel: 'mask-icon', color: '#5bbad5', href: '/images/favicons/safari-pinned-tab.svg' },

            { rel: 'manifest', href: '/manifests/site.webmanifest' },
        ],
        metaInfo: {
            noscript: [
                { innerHTML: 'Sorry, but this site doesn\'t work properly without JavaScript enabled. Please enable it to continue.' }
            ]
        }
    },
    router: {
        routeNameSplitter: '/'
    },
    srcDir: 'src/',
    /*
    ** Customize the progress bar color
    */
    loading: { color: '#3B8070' },
    /*
    ** Build configuration
    */
    build: {
        /*
        ** Run ESLint on save
        */
        extend(config, { isDev, isClient }) {
            if (isDev && isClient) {
                config.module.rules.push({
                    enforce: 'pre',
                    test: /\.(js|vue)$/,
                    loader: 'eslint-loader',
                    exclude: /(node_modules)/
                })
            }
        },

        extractCSS: true,

        output: {
            filename: 'js/[name]-[hash].js',
            chunkFilename: 'js/[name]-[hash].js'
        }
    },
    buildModules: [
        '@nuxtjs/vuetify',
        ['@nuxtjs/google-analytics', { id: 'UA-177636773-1', disabled: true, debug: { trace: true, sendHitTask: true }, fields: { cookieFlags: 'SameSite=None; Secure' } }]
    ],
    modules: [
        '@nuxtjs/applicationinsights',
        ['cookie-universal-nuxt', { alias: 'cookies', parseJSON: false } ],
    ],
    plugins: [
    ],
    appInsights: {
        instrumentationKey: '021a29af-b319-41dd-bafd-375dc3520595',
        disableServerSide: true,
        clientConfig: {
            disableTelemetry: true,
            isCookieUseDisabled: true,
            samplingPercentage: 20,
            correlationHeaderDomains: ['api.jibberwock.com'],
            enableCorsCorrelation: true
        }
    },

    target: 'static'
}

