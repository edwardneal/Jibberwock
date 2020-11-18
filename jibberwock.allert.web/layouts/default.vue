<template>
  <v-app>
    <v-app-bar fixed app clipped-right>
      <v-spacer />
      <v-breadcrumbs :items="breadcrumbs" large divider=">" />
      <v-spacer />
      <div v-if="$store.state.auth.loggedIn" id="messageButtonContainer">
        <Promised :promise="notificationsPromise">
          <template v-slot:combined="{ isPending, error }">
            <v-btn v-if="! error" icon @click="showNotificationsBar = !showNotificationsBar">
              <v-progress-circular v-if="isPending" indeterminate />
              <v-badge v-else :content="$store.state.users.notifications.length" color="green" overlap>
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
      <v-layout id="app-container" column fill-height>
        <router-view :language-strings="languageStrings" />
        <Promised :promise="notificationsPromise">
          <template v-slot:combined="{ isPending, error }">
            <div v-if="! isPending && ! error" id="high-priority-notifications">
              <v-alert
                v-for="(notif, notifIdx) in $store.state.users.notificationsByPriority.find(n => n.priorityId === 3).notifications"
                :key="notifIdx"
                :type="languageStrings.notificationList.notificationTypes.find(t => t.id === notif.type).alertType"
                :dismissible="notif.allowDismissal"
                border="left"
              >
                <h3 class="text-h5">
                  {{ notif.subject }}
                </h3>
                <div>{{ notif.message }}</div>
              </v-alert>
            </div>
            <div v-else id="high-priority-notifications" />
          </template>
        </Promised>
      </v-layout>
      <v-navigation-drawer
        v-if="$store.state.auth.loggedIn"
        v-model="showNotificationsBar"
        app
        clipped
        right
        width="320"
      >
        <v-list expand>
          <v-list-group v-model="showInvitationsList" color="undef-color" prepend-icon="mdi-account-group">
            <template v-slot:activator>
              <v-list-item-title>{{ languageStrings.notificationList.sections.invitations.title }}</v-list-item-title>
            </template>
            <v-list-item>
              <v-list-item-title>
                SAMPLE TENANT 1
              </v-list-item-title>
              <v-list-item-action style="min-width: 76px;">
                <div>
                  <v-btn icon color="success">
                    <v-icon>
                      mdi-check
                    </v-icon>
                  </v-btn>
                  <v-btn icon color="error">
                    <v-icon small>
                      mdi-block-helper
                    </v-icon>
                  </v-btn>
                </div>
              </v-list-item-action>
            </v-list-item>
          </v-list-group>

          <v-list-group v-model="showNotificationsList" color="undef-color" prepend-icon="mdi-email-outline">
            <template v-slot:activator>
              <v-list-item-title>{{ languageStrings.notificationList.sections.notifications.title }}</v-list-item-title>
            </template>

            <v-list-group
              v-for="(np, npIdx) in $store.state.users.notificationsByPriority"
              :key="'np.' + npIdx"
              :value="true"
              color="undef-color"
              no-action
              sub-group
            >
              <template v-slot:activator>
                <v-list-item-title>
                  <v-icon v-if="np.priorityId === 3" color="error" left>
                    mdi-exclamation-thick
                  </v-icon>
                  {{ languageStrings.notificationList.sections.notifications.priorityFormat.replace('{label}', languageStrings.notificationList.notificationPriorities.find(p => p.id === np.priorityId).label) }}
                </v-list-item-title>
              </template>

              <v-list-item v-for="(notif, notifIdx) in np.notifications" :key="notifIdx" link class="px-4">
                <v-list-item-content>
                  <v-list-item-title>
                    {{ notif.subject }}
                  </v-list-item-title>
                  <v-list-item-subtitle>
                    {{ languageStrings.notificationList.notificationTypes.find(t => t.id === notif.type).label }}
                  </v-list-item-subtitle>
                  <v-list-item-subtitle>
                    {{ new Date(notif.creationDate).toLocaleString() }}
                  </v-list-item-subtitle>
                </v-list-item-content>
                <v-list-item-action v-if="notif.allowDismissal">
                  <v-btn icon @click.stop="">
                    <v-icon>mdi-close-circle</v-icon>
                  </v-btn>
                </v-list-item-action>
              </v-list-item>
            </v-list-group>
          </v-list-group>
        </v-list>
      </v-navigation-drawer>
    </v-main>
    <v-footer app>
      <span>&copy; {{ new Date().getFullYear() }} <a href="https://www.jibberwock.com">Jibberwock</a></span>
    </v-footer>
  </v-app>
</template>

<style>
  #app-container > .v-sheet:first-child { padding:16px; height: 100% }
  #high-priority-notifications { padding: 16px; position: fixed; width: 100%; }
</style>

<script>
import { Promised } from 'vue-promised'
import lang from '~/static-data/lang/en'

export default {
  components: {
    Promised
  },
  data () {
    return {
      languageStrings: lang,
      title: lang.productName,
      notificationsPromise: Promise.resolve(),
      showNotificationsBar: false,
      showNotificationsList: true,
      showInvitationsList: true
    }
  },
  computed: {
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
  actions: {
  },
  mounted () {
    if (this.$store.state.auth.loggedIn) {
      this.notificationsPromise = this.$store.dispatch('users/populateNotifications')
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
