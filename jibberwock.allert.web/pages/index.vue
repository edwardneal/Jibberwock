<template>
  <v-sheet v-if="$store.state.auth.loggedIn">
    YOU ARE LOGGED IN.

    <p>to do:</p>
    <ol>
      <li>test dismissal api logic</li>
      <li>add a username banner at the top-right</li>
      <li>figure out the user story. they need to log in, then create a tenant - can i simplify this?</li>
    </ol>
  </v-sheet>
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
          <v-btn nuxt to="/sign-up">
            {{ languageStrings.auth.signUp }}
          </v-btn>
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

export default {
  components: {
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  computed: {
    ...mapGetters({
      getLogInUrl: 'auth/getLogInUrl' //,
      // getLogOutUrl: 'auth/getLogOutUrl'
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
