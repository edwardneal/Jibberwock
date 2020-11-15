<template>
  <v-app>
    <v-app-bar fixed app>
      <v-spacer />
      <v-breadcrumbs :items="breadcrumbs" large divider=">" />
      <v-spacer />
    </v-app-bar>
    <v-main>
      <v-layout id="app-container" column fill-height>
        <router-view :language-strings="languageStrings" />
      </v-layout>
    </v-main>
    <v-footer app>
      <span>&copy; {{ new Date().getFullYear() }} <a href="https://www.jibberwock.com">Jibberwock</a></span>
    </v-footer>
  </v-app>
</template>

<style>
  #app-container > .v-sheet:first-child { padding:16px; height: 100% }
</style>

<script>
import { mapGetters } from 'vuex'
import lang from '~/static-data/lang/en'

export default {
  data () {
    return {
      languageStrings: lang,
      title: lang.productName
    }
  },
  computed: {
    ...mapGetters({
      getLogInUrl: 'auth/getLogInUrl',
      getLogOutUrl: 'auth/getLogOutUrl'
    }),
    breadcrumbs () {
      const crumbs = []

      if (this.$route.path === '/') {
        crumbs.push({
          text: 'Jibberwock',
          disabled: false,
          href: 'https://www.jibberwock.com',
          link: true
        })

        crumbs.push({
          text: this.languageStrings.shortProductName,
          nuxt: true,
          disabled: false,
          to: '/',
          link: true
        })
      } else {
        crumbs.push({
          text: this.languageStrings.shortProductName,
          nuxt: true,
          disabled: false,
          to: '/',
          link: true
        })

        crumbs.push({
          text: this.title,
          nuxt: true,
          disabled: false,
          to: this.$route.fullPath,
          link: true
        })
      }

      return crumbs
    }
  },
  head () {
    return {
      changed: (metaInfo) => {
        this.title = metaInfo.titleChunk
      }
    }
  }
}
</script>
