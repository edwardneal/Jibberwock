<template>
  <v-container v-if="$store.state.auth.loggedIn" fill-height fluid class="pa-0 blue darken-3">
    <v-container fill-height fluid class="blue darken-4 rounded-br-pill">
      <v-row>
        <v-spacer />
        <v-col cols="12" md="5">
          <v-card>
            <v-list-item>
              <v-list-item-icon class="mr-4">
                <v-avatar rounded size="32" color="primary">
                  <v-icon>
                    {{ languageStrings.dropdownValues.identityProvider.find(idp => idp.id === $store.state.auth.userInfo.identityProvider).icon }}
                  </v-icon>
                </v-avatar>
              </v-list-item-icon>

              <v-list-item-content>
                <v-list-item-title>{{ $store.state.auth.userInfo.name }}</v-list-item-title>
                <v-list-item-subtitle>{{ $store.state.auth.userInfo.emailAddresses[0] }}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>

            <v-card-text>
              <p>
                Welcome to {{ languageStrings.shortProductName }}. You've successfully logged in with the account details above. If you want to change these details later, just click your name in the top-right corner
                of the page.
              </p>
              <p>
                Next to your name in the top-right, you will also see an envelope icon, which might also have a number next to it. Click on this icon to display a list of notifications and invitations.
                While any high-priority notifications will appear at the top of every page, (and some notifications may be sent to your email address) you can see every notification in this list.
                Click on the notification to see its message, and click on the cross next to it in order to dismiss it.
              </p>
              <p>
                Please refer to the <strong>{{ languageStrings.tenantList.sections.tenants.title }}</strong> section {{ this.$vuetify.breakpoint.smAndDown ? 'below' : 'on the right' }}.
                If you're using {{ languageStrings.shortProductName }} for the first time, just click the <strong>{{ languageStrings.actions.create }}</strong> button to create your tenant. Otherwise, click the name of your
                tenant to go to its page. You can also click the ellipses to navigate directly to the tenant's <strong>{{ languageStrings.tenantList.sections.tenants.buttons.allert }}</strong>,
                <strong>{{ languageStrings.tenantList.sections.tenants.buttons.security }}</strong> or <strong>{{ languageStrings.tenantList.sections.tenants.buttons.subscriptions }}</strong> pages.
              </p>
              <p>
                You might also see a <strong>{{ languageStrings.tenantList.sections.invitations.title }}</strong> subheading in the <strong>{{ languageStrings.tenantList.sections.tenants.title }}</strong> section.
                If you see this subheading, you've been invited to another person's tenant. To access the tenant, you need to accept the invitation using the green &quot;tick&quot; icon on the right.
                If you don't recognise the name of the tenant, click the <strong>{{ languageStrings.tenantList.sections.invitations.buttons.reject }}</strong> button.
              </p>
              <p>
                For more help about a specific feature of {{ languageStrings.shortProductName }}, please open a tenant's page; you'll be given more targeted support about the options available to you.
                For more information about the product, click <nuxt-link to="/about">here</nuxt-link> to visit the About page or <nuxt-link to="/roadmap">here</nuxt-link> to see its long-term roadmap.
              </p>
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="12" md="5">
          <v-card>
            <v-card-title class="pb-0">
              {{ languageStrings.tenantList.sections.tenants.title }}
            </v-card-title>
            <v-card-text>
              <ClientTenantList :language-strings="languageStrings" />
            </v-card-text>
          </v-card>
        </v-col>
        <v-spacer />
      </v-row>
    </v-container>
  </v-container>
  <v-container v-else fill-height fluid class="pa-0 blue darken-3">
    <v-container fill-height fluid class="blue darken-4 rounded-br-pill">
      <div class="mx-auto">
        <h1 class="text-h2">
          {{ languageStrings.shortProductName }}
        </h1>
        <h2 class="text-h3">
          {{ languageStrings.pages.homepage.unauthenticated.description }}
        </h2>
        <div class="pt-2 text-right link-container">
          <v-btn text nuxt to="/about">
            {{ languageStrings.pages.about.title }}
            <v-icon class="pl-1">
              mdi-arrow-right-bold
            </v-icon>
          </v-btn>
          <v-btn text nuxt to="/roadmap">
            {{ languageStrings.pages.roadmap.title }}
            <v-icon class="pl-1">
              mdi-arrow-right-bold
            </v-icon>
          </v-btn>
        </div>
        <div class="pt-2">
          <v-btn :href="$route.query.ReturnURL && $route.query.ReturnURL.startsWith('/') ? getLogInUrl($route.query.ReturnURL) : getLogInUrl($route.fullPath)">
            {{ languageStrings.auth.logIn }}
          </v-btn>
        </div>
      </div>
      <div class="bottom-icon">
        <v-icon>mdi-cloud-alert</v-icon>
      </div>
    </v-container>
  </v-container>
</template>

<style>
  .link-container { margin-right: -16px; }
  .bottom-icon { position: absolute; bottom: 32px; right: 38px; }
  .bottom-icon > .v-icon { font-size: 12rem; }
</style>

<script>
import { mapGetters } from 'vuex'
import ClientTenantList from '~/components/ClientTenantList.vue'

export default {
  components: {
    ClientTenantList
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  computed: {
    ...mapGetters({
      getLogInUrl: 'auth/getLogInUrl'
    })
  },
  meta: {
    auth: { required: false }
  },
  head () {
    return {
      title: this.languageStrings.pages.homepage.title
    }
  }
}
</script>
