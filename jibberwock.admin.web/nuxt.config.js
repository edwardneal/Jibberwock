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
    titleTemplate: '%s - Jibberwock Admin',
    title: 'Homepage',
    meta: [
      { charset: 'utf-8' },
      { 'http-equiv': 'X-UA-Compatible', content: 'IE=edge' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      { name: 'rating', content: 'General' },

      { name: 'twitter:card', content: 'summary' },
      { name: 'twitter:dnt', content: 'on' },
      { name: 'twitter:image', content: 'https://admin.jibberwock.com/images/favicons/android-chrome-512x512.png' },
      { name: 'twitter:image:alt', content: 'Jibberwock logo' },

      { hid: 'description', name: 'description', content: 'The Jibberwock admin page enables maintenance tasks for all Jibberwock projects.' },
      { name: 'subject', content: 'Jibberwock Admin' },
      { name: 'application-name', content: 'Jibberwock Admin' },

      { hid: 'og-title', property: 'og:title', content: 'Homepage - Jibberwock Admin' },
      { property: 'og:site_name', content: 'Jibberwock Admin' },
      { property: 'og:description', content: 'The Jibberwock admin page enables maintenance tasks for all Jibberwock projects.' },
      { hid: 'og-url', property: 'og:url', content: 'https://admin.jibberwock.com' },
      { name: 'og:image', content: 'https://admin.jibberwock.com/images/favicons/android-chrome-512x512.png' },
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
    ['@nuxtjs/google-analytics', { id: 'UA-177636773-3', disabled: true, debug: { trace: true, sendHitTask: true }, fields: { cookieFlags: 'SameSite=None; Secure' } }]
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
    customVariables: ['~/assets/variables.scss']
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
      correlationHeaderDomains: ['admin.jibberwock.com'],
      enableCorsCorrelation: true
    }
  },
}
