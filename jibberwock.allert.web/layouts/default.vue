<template>
  <v-app>
    <v-app-bar fixed app clipped-right>
      <v-spacer />
      <v-breadcrumbs :items="breadcrumbs" large divider=">" />
      <v-spacer />
      <div v-if="$store.state.auth.loggedIn" id="messageButtonContainer">
        <v-menu v-model="showProfileDropdown" offset-y auto>
          <template v-slot:activator="{ on }">
            <v-btn text v-on="on" class="text-capitalize">
              <v-icon left>
                {{ languageStrings.dropdownValues.identityProvider.find(idp => idp.id === $store.state.auth.userInfo.identityProvider).icon }}
              </v-icon>
              {{ effectiveUsername }}
              <v-icon v-if="! showProfileDropdown" right>
                mdi-menu-down
              </v-icon>
              <v-icon v-if="showProfileDropdown" right>
                mdi-menu-up
              </v-icon>
            </v-btn>
          </template>
          <v-list>
            <v-list-item :href="getEditProfileUrl($route.fullPath)" link class="text-justify">
              <v-list-item-icon>
                <v-icon>mdi-account-edit</v-icon>
              </v-list-item-icon>
              <v-list-item-title>{{ languageStrings.auth.editProfile }}</v-list-item-title>
            </v-list-item>
            <v-list-item :href="getResetPasswordUrl($route.fullPath)" link class="text-justify">
              <v-list-item-icon>
                <v-icon>mdi-account-key</v-icon>
              </v-list-item-icon>
              <v-list-item-title>{{ languageStrings.auth.resetPassword }}</v-list-item-title>
            </v-list-item>
            <v-divider />
            <v-list-item :href="getLogOutUrl($route.fullPath)" link class="text-justify">
              <v-list-item-icon>
                <v-icon>mdi-account-arrow-left</v-icon>
              </v-list-item-icon>
              <v-list-item-title>{{ languageStrings.auth.logOut }}</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
        <v-divider inset vertical />
        <Promised :promise="notificationsPromise">
          <template v-slot:combined="{ isPending, error }">
            <v-btn v-if="! error" icon @click="showNotificationsBar = !showNotificationsBar">
              <v-progress-circular v-if="isPending" indeterminate />
              <v-badge v-else :value="$store.state.users.notifications.length > 0" :content="$store.state.users.notifications.length" color="green" overlap>
                <v-icon large>
                  mdi-email-outline
                </v-icon>
              </v-badge>
            </v-btn>
            <v-tooltip v-else left>
              <template v-slot:activator="{ on, attrs }">
                <v-icon large v-bind="attrs" v-on="on">
                  mdi-alert-circle-outline
                </v-icon>
              </template>
              {{ error }}
            </v-tooltip>
          </template>
        </Promised>
      </div>
    </v-app-bar>
    <v-main>
      <Promised :promise="notificationsPromise">
        <template v-slot:combined="{ isPending, error }">
          <v-layout id="app-container" column fill-height>
            <div v-if="! isPending && ! error" id="high-priority-notifications">
              <v-window v-model="visibleNotification">
                <v-window-item
                  v-for="(notif, notifIdx) in highPriorityNotifications"
                  :key="notifIdx"
                >
                  <v-alert
                    :type="languageStrings.notificationList.notificationTypes.find(t => t.id === notif.type).alertType"
                    :dismissible="notif.allowDismissal"
                    border="left"
                    class="mb-0"
                    tile
                  >
                    <template v-slot:prepend>
                      <v-btn v-if="notifIdx > 0" icon @click="visibleNotification--">
                        <v-icon>
                          {{ $vuetify.icons.values['prev'] }}
                        </v-icon>
                      </v-btn>
                      <v-icon class="mr-3">
                        {{ $vuetify.icons.values[languageStrings.notificationList.notificationTypes.find(t => t.id === notif.type).alertType] }}
                      </v-icon>
                    </template>
                    <template v-slot:append>
                      <Promised :promise="notif.dismissalPromise">
                        <template v-slot:combined="dpSlot">
                          <v-tooltip left>
                            <template v-slot:activator="{ on, attrs }">
                              <v-btn
                                :loading="notif.dismissalPromise && dpSlot.isPending"
                                :disabled="! notif.allowDismissal"
                                v-bind="attrs"
                                icon
                                v-on="error ? on : null"
                                @click.stop="dismissNotification(notif)"
                              >
                                <v-icon>mdi-close-circle</v-icon>
                              </v-btn>
                            </template>
                            {{ dpSlot.error }}
                          </v-tooltip>
                        </template>
                      </Promised>
                      <v-btn v-if="notifIdx < highPriorityNotifications.length - 1" icon @click="visibleNotification++">
                        <v-icon>
                          {{ $vuetify.icons.values['next'] }}
                        </v-icon>
                      </v-btn>
                    </template>
                    <template v-slot:close />
                    <h3 class="text-h5">
                      {{ notif.subject }}
                    </h3>
                    <div>{{ notif.message }}</div>
                  </v-alert>
                </v-window-item>
              </v-window>
            </div>
            <router-view :language-strings="languageStrings" />
          </v-layout>
          <ClientNotificationList v-if="! isPending && ! error && $store.state.auth.loggedIn" :language-strings="languageStrings" :notifications="$store.state.users.notificationsByPriority" :visible.sync="showNotificationsBar" />
        </template>
      </Promised>
    </v-main>
    <v-footer app>
      <span>&copy; {{ new Date().getFullYear() }} <a href="https://www.jibberwock.com">Jibberwock</a></span>
    </v-footer>
  </v-app>
</template>

<style>
  #app-container > .v-sheet:first-child, #app-container > .v-sheet:nth-child(2) { padding:16px; height: 100% }
</style>

<script>
import { Promised } from 'vue-promised'
import { mapActions, mapGetters } from 'vuex'
import ClientNotificationList from '~/components/ClientNotificationList.vue'
import lang from '~/static-data/lang/en'

export default {
  components: {
    Promised,
    ClientNotificationList
  },
  data () {
    return {
      languageStrings: lang,
      title: lang.productName,
      notificationsPromise: Promise.resolve(),
      showNotificationsBar: false,
      visibleNotification: 0,
      showProfileDropdown: false
    }
  },
  computed: {
    ...mapGetters({
      getLogOutUrl: 'auth/getLogOutUrl',
      getEditProfileUrl: 'auth/getEditProfileUrl',
      getResetPasswordUrl: 'auth/getResetPasswordUrl'
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
    },
    highPriorityNotifications () {
      return this.$store.state.users.notifications.filter(n => n.priority.id === 3)
    },
    effectiveUsername () {
      const maxLength = 18
      // If the username is more than 18 characters long, it'll overflow on small screens.
      // To bypass that, if it's too long then only take the first 15 characters and some ellipses
      const userName = this.$store.state.auth.userInfo.name

      if (userName.length > maxLength && this.$vuetify.breakpoint.xs) {
        return userName.substring(0, maxLength - '...'.length) + '...'
      }

      return userName
    }
  },
  mounted () {
    if (this.$store.state.auth.loggedIn) {
      this.notificationsPromise = this.$store.dispatch('users/populateNotifications')
    }
  },
  methods: {
    ...mapActions({
      dismissNotificationInternal: 'users/dismissNotification'
    }),
    dismissNotification (notification) {
      const nestedThis = this

      notification.dismissalPromise = this.dismissNotificationInternal(notification.id)
        .then(() => {
          nestedThis.$store.commit('users/removeNotification', notification)
        })
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
