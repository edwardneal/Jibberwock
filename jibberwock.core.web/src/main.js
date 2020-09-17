import Vue from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify';
import router from './router'
import VueGtag from 'vue-gtag';
import VueAppInsights from 'vue-application-insights';
import VueCookies from 'vue-cookies'

import titleMixin from './mixins/title';

Vue.config.productionTip = false

Vue.use(VueCookies);

Vue.use(VueGtag, {
    config: {
        id: 'UA-177636773-1'
    },
    enabled: false
}, router);

Vue.use(VueAppInsights, {
    id: '021a29af-b319-41dd-bafd-375dc3520595',
    router,
    baseName: 'Jibberwock',
    appInsightsConfig: {
        disableTelemetry: true,
        isCookieUseDisabled: true,
        samplingPercentage: 20,
        correlationHeaderDomains: ['api.jibberwock.com'],
        enableCorsCorrelation: true
    }
})

Vue.mixin(titleMixin);

Window.Vue = new Vue({
    vuetify,
    router,
    VueGtag,
    render: h => h(App)
});
Window.Vue.$cookies.config('1d', null, null, true, 'Strict');
Window.Vue.$mount('#app')
