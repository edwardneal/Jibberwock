import colors from 'vuetify/es5/util/colors'

export default {
  // Disable server-side rendering (https://go.nuxtjs.dev/ssr-mode)
  ssr: false,

  // Target (https://go.nuxtjs.dev/config-target)
  target: 'static',

  // Global page headers (https://go.nuxtjs.dev/config-head)
  head: {
    htmlAttrs: {
      lang: 'en',
      style: 'overflow-y: inherit;'
    },
    titleTemplate: '%s - Jibberwock Allert',
    title: 'Homepage',
    meta: [
      { hid: 'charset', charset: 'utf-8' },
      { 'http-equiv': 'X-UA-Compatible', content: 'IE=edge' },
      { name: 'rating', content: 'General' },

      { name: 'twitter:dnt', content: 'on' },
      { name: 'twitter:image', content: 'https://allert.jibberwock.com/images/favicons/android-chrome-512x512.png' },
      { name: 'twitter:image:alt', content: 'Jibberwock logo' },

      { hid: 'description', name: 'description', content: 'Jibberwock Allert is a cloud alerting engine which runs user scripts against input to determine whether or not an alert has occurred. A list of active alerts is stored in Azure Table Storage, and can be retrieved at a later date.' },
      { name: 'subject', content: 'Jibberwock Allert' },
      { name: 'application-name', content: 'Jibberwock Allert' },

      { hid: 'og:description', property: 'og:description', content: 'Jibberwock Allert is a cloud alerting engine which runs user scripts against input to determine whether or not an alert has occurred. A list of active alerts is stored in Azure Table Storage, and can be retrieved at a later date.' },
      { name: 'og:image', content: 'https://allert.jibberwock.com/images/favicons/android-chrome-512x512.png' },

      { name: 'msapplication-config', content: '/manifests/browserconfig.xml' },
    ],
    link: [
      { rel: 'icon', type: 'image/x-icon', href: '/images/favicons/favicon.ico' },
      { rel: 'shortcut icon', type: 'image/x-icon', href: '/images/favicons/favicon.ico' },
      { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css?family=Roboto:100,300,400,500,700,900&display=swap' },
      {
        rel: 'stylesheet', crossorigin: 'anonymous', integrity: 'sha512-x96qcyADhiw/CZY7QLOo7dB8i/REOEHZDhNfoDuJlyQ+yZzhdy91eAa4EkO7g3egt8obvLeJPoUKEKu5C5JYjA==',
        href: 'https://cdnjs.cloudflare.com/ajax/libs/MaterialDesign-Webfont/5.8.55/css/materialdesignicons.min.css'
      },

      { rel: 'apple-touch-icon', sizes: '180x180', href: '/images/favicons/apple-touch-icon.png' },

      { rel: 'icon', type: 'image/png', sizes: '32x32', href: '/images/favicons/favicon-32x32.png' },
      { rel: 'icon', type: 'image/png', sizes: '192x192', href: '/images/favicons/android-chrome-192x192.png' },
      { rel: 'icon', type: 'image/png', sizes: '16x16', href: '/images/favicons/favicon-16x16.png' },
      { rel: 'mask-icon', color: '#5bbad5', href: '/images/favicons/safari-pinned-tab.svg' },
    ],
    metaInfo: {
      noscript: [
        { innerHTML: 'Sorry, but this site doesn\'t work properly without JavaScript enabled. Please enable it to continue.' }
      ]
    }
  },

  loading: false,

  // Global CSS (https://go.nuxtjs.dev/config-css)
  css: [
  ],

  // Plugins to run before rendering page (https://go.nuxtjs.dev/config-plugins)
  plugins: [
    '~/plugins/vuelidate'
  ],

  // Auto import components (https://go.nuxtjs.dev/config-components)
  components: true,

  // Modules for dev and build (recommended) (https://go.nuxtjs.dev/config-modules)
  buildModules: [
    // https://go.nuxtjs.dev/typescript
    '@nuxt/typescript-build',
    // https://go.nuxtjs.dev/vuetify
    '@nuxtjs/vuetify',
    ['@nuxtjs/google-analytics', { id: 'UA-177636773-2', disabled: true, debug: { trace: true, sendHitTask: true }, fields: { cookieFlags: 'SameSite=None; Secure' } }]
  ],

  // Modules (https://go.nuxtjs.dev/config-modules)
  modules: [
    // https://go.nuxtjs.dev/axios
    '@nuxtjs/axios',
    // https://go.nuxtjs.dev/pwa
    '@nuxtjs/pwa',
    '@nuxtjs/applicationinsights',
    ['cookie-universal-nuxt', { alias: 'cookies', parseJSON: false }],
  ],

  // Middleware
  router: {
    middleware: 'auth',
    routeNameSplitter: '/'
  },

  // Axios module configuration (https://go.nuxtjs.dev/config-axios)
  axios: {},

  // Vuetify module configuration (https://go.nuxtjs.dev/config-vuetify)
  vuetify: {
    customVariables: ['~/assets/variables.scss'],
    theme: {
      options: { cspNonce: 'nrKlSSBeSxaJ5FGaqwl9' },
      dark: true
    },
    defaultAssets: false
  },

  // Build Configuration (https://go.nuxtjs.dev/config-build)
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

  appInsights: {
    instrumentationKey: '021a29af-b319-41dd-bafd-375dc3520595',
    disableServerSide: true,
    clientConfig: {
      disableTelemetry: true,
      isCookieUseDisabled: true,
      samplingPercentage: 20,
      correlationHeaderDomains: ['allert.jibberwock.com'],
      enableCorsCorrelation: true
    }
  },

  pwa: {
    meta: {
      name: 'Jibberwock Allert',
      mobileApp: true,
      mobileAppIOS: true,
      theme_color: '#ffffff',
      ogHost: 'https://allert.jibberwock.com',
      twitterCard: 'summary',
      favicon: false
    },
    manifest: {
      useWebmanifestExtension: true,
      name: 'Jibberwock Allert',
      short_name: 'Jibberwock Allert',
      icons: [
        {
          src: '/images/favicons/android-chrome-192x192.png',
          sizes: '192x192',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/images/favicons/android-chrome-512x512.png',
          sizes: '512x512',
          type: 'image/png',
          purpose: 'any maskable'
        }
      ],
      start_url: ''
    },
    // Deliberately not using this part of nuxt-pwa.
    // I've already generated the necessary icons, which are a much smaller size.
    icon: false,
    workbox: {
      workboxURL: 'https://cdnjs.cloudflare.com/ajax/libs/workbox-sw/5.1.4/workbox-sw.min.js',
      offlineAnalytics: true,
      runtimeCaching: [
        'https://fonts.googleapis.com/.*',
        'https://fonts.gstatic.com/.*',
        'https://cdnjs.cloudflare.com/.*'
      ].map((u) => {
        return {
          urlPattern: u, handler: 'cacheFirst', method: 'GET', strategyOptions: {
            cacheableResponse: { statuses: [0, 200] }
          }
        }
      }).concat([
        { urlPattern: '/.auth/.*', handler: 'networkFirst', method: 'GET' }
      ])
    }
  }
}
