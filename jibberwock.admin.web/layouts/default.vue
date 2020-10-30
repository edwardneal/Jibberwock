<template>
  <v-app>
    <v-navigation-drawer :clipped="false" fixed app>
      <v-layout column fill-height>
        <v-list v-if="$store.state.auth.loggedIn">
          <v-list-item to="/" router exact>
            <v-list-item-action>
              <v-icon>mdi-wrench</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>{{ languageStrings.productName }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-list-item :href="getLogOutUrl($route.fullPath)" router exact>
            <v-list-item-action>
              <v-icon>mdi-logout</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>{{ languageStrings.auth.logOut }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <template v-for="(section, s) in sections">
            <v-subheader :key="'title.' + s">
              {{ section.title }}
            </v-subheader>
            <v-divider :key="'divider.' + s" />
            <v-list-item v-for="(item, i) in section.items" :key="'section' + s + '.item.' + i" :to="item.to" router exact>
              <v-list-item-action>
                <v-icon>{{ item.icon }}</v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-title v-text="item.title" />
              </v-list-item-content>
            </v-list-item>
          </template>
        </v-list>
        <v-list v-else>
          <v-list-item to="/" router exact>
            <v-list-item-action>
              <v-icon>mdi-wrench</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>{{ languageStrings.productName }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-list-item :href="getLogInUrl($route.fullPath)" router exact>
            <v-list-item-action>
              <v-icon>mdi-login</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>{{ languageStrings.auth.logIn }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list>
        <v-spacer />
        <v-list>
          <v-list-item>
            <v-list-item-content>
              <v-checkbox v-model="$vuetify.theme.dark" label="Dark Mode" hide-details />
            </v-list-item-content>
          </v-list-item>
        </v-list>
      </v-layout>
    </v-navigation-drawer>
    <v-app-bar fixed app>
      <v-toolbar-title v-text="title" />
    </v-app-bar>
    <v-main>
      <v-layout id="app-container" column fill-height>
        <router-view :language-strings="languageStrings" />
      </v-layout>
    </v-main>
    <v-footer app>
      <span>&copy; {{ new Date().getFullYear() }}</span>
    </v-footer>
  </v-app>
</template>

<style>
  #app-container .v-sheet:first-child { padding:16px; height: 100% }
</style>

<script>
import { mapGetters } from 'vuex'
import lang from '~/static-data/lang/en'

export default {
  data () {
    return {
      languageStrings: lang,
      sections: [
        {
          title: lang.sections.users.title,
          items: [
            { icon: 'mdi-account', title: lang.sections.users.items.users, to: '/users' },
            { icon: 'mdi-account-group', title: lang.sections.users.items.tenants, to: '/tenants' }
          ]
        },
        {
          title: lang.sections.service.title,
          items: [
            { icon: 'mdi-book-open', title: lang.sections.service.items.auditTrail, to: '/audit-trail' },
            { icon: 'mdi-email-search', title: lang.sections.service.items.emails, to: '/emails' },
            { icon: 'mdi-gauge', title: lang.sections.service.items.status, to: '/status' },
            { icon: 'mdi-shield-alert', title: lang.sections.service.items.exceptions, to: '/exceptions' }
          ]
        },
        {
          title: lang.sections.products.title,
          items: [
            { icon: 'mdi-sticker', title: lang.sections.products.items.characteristics, to: '/characteristics' },
            { icon: 'mdi-package-variant-closed', title: lang.sections.products.items.products, to: '/products' },
            { icon: 'mdi-cloud-alert', title: lang.sections.products.items.allert, to: '/allert' }
          ]
        }
      ],
      title: lang.productName
    }
  },
  computed: {
    ...mapGetters({
      getLogInUrl: 'auth/getLogInUrl',
      getLogOutUrl: 'auth/getLogOutUrl'
    })
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
