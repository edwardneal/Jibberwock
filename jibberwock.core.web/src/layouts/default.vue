<template>
    <div id="app">
        <v-app class="overflow-hidden" style="max-height: 64px">
            <v-card flat>
                <v-app-bar color="white" elevate-on-scroll scroll-target="#scroll-content">
                    <v-toolbar-title class="text-h5 text--secondary">Jibberwock</v-toolbar-title>
                    <v-spacer></v-spacer>
                    <v-toolbar-items class="text-h5 text--secondary">
                        <nuxt-link to="/" v-slot="{ href, route, navigate }">
                            <v-btn text @click="navigate" :href="href" class="text-capitalize">
                                <span>Home</span>
                            </v-btn>
                        </nuxt-link>
                        <v-divider class="mx-4" inset vertical></v-divider>
                        <v-menu v-model="productsDropdownVisible" offset-y auto>
                            <template v-slot:activator="{ on }">
                                <v-btn text v-on="on" class="text-capitalize">
                                    <span>Projects <v-icon v-if="! productsDropdownVisible">mdi-menu-down</v-icon><v-icon v-if="productsDropdownVisible">mdi-menu-up</v-icon></span>
                                </v-btn>
                            </template>
                            <v-list>
                                <v-list-item v-for="product in products" :key="product.name" link class="text-justify">
                                    <nuxt-link :to="product.url" v-slot="{ href, route, navigate }">
                                        <v-list-item-title @click="navigate" class="text-h6 text--secondary">{{ product.name }}</v-list-item-title>
                                    </nuxt-link>
                                </v-list-item>
                            </v-list>
                        </v-menu>
                        <v-divider class="mx-4" inset vertical></v-divider>
                        <nuxt-link to="/about" v-slot="{ href, route, navigate }">
                            <v-btn text @click="navigate" :href="href" class="text-capitalize">
                                <span class="mr-2">Privacy &amp; About</span>
                            </v-btn>
                        </nuxt-link>
                    </v-toolbar-items>
                    <v-spacer></v-spacer>
                    <v-toolbar-items>
                        <v-btn text href="https://www.github.com/edwardneal" target="_blank" class="text-capitalize" rel="noopener">
                            <span class="pr-2">GitHub</span><v-icon>mdi-export</v-icon>
                        </v-btn>
                    </v-toolbar-items>
                </v-app-bar>
            </v-card>
            <v-container flat id="scroll-content" class="overflow-y-auto" fill-height fluid>
                <nuxt />
            </v-container>
            <transition duration="0">
                <v-banner icon="mdi-lock" color="info lighten-2" min-height="64" style="bottom: 0px; left: 0px; position: fixed; width: 100%; z-index: 9999" v-model="showGdprBanner">
                    We use cookies to improve user experience, and analyze website traffic. For these reasons, we may share your site usage data with our analytics partners. By clicking "Accept Cookies," you consent to store on your device all the technologies described in the Privacy page.
                    <template v-slot:actions>
                        <v-btn @click="acceptGdpr();" style="margin-bottom: 8px; margin-right: 14px;">Accept Cookies <v-icon class="pl-1">mdi-check</v-icon></v-btn>
                    </template>
                </v-banner>
            </transition>
        </v-app>
    </div>
</template>

<style lang="scss">
#app {
  font-family: Roboto, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
}

#nav {
  padding: 30px;

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}
</style>

<script>
    const consentCookieName = 'GDPR-ACCEPTED';

    import products from '@/static-data/products.js';

    function evaluateGdprTelemetry(vueInstance) {
        vueInstance.showGdprBanner = ! vueInstance.$cookies.get(consentCookieName);

        if (!vueInstance.showGdprBanner) {
            vueInstance.$appInsights.config.disableTelemetry = false;
            setTimeout(() => {
                vueInstance.$ga.enable();
                vueInstance.$ga.page(vueInstance.$route.fullPath);
            }, 200);
        }
    }

    export default {
        name: 'App',

        data: () => ({
            productsDropdownVisible: false,
            products: products,
            showGdprBanner: true
        }),

        mounted() {
            evaluateGdprTelemetry(this);
        },

        methods: {
            acceptGdpr() {
                this.$cookies.set(consentCookieName, true, {
                    sameSite: true,
                    secure: false,
                    path: '/',
                    maxAge: 60 * 60 * 24 * 30 * 12
                });
                evaluateGdprTelemetry(this);
            }
        }
    };
</script>
