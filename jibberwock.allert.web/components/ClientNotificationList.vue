<template>
  <v-navigation-drawer
    v-model="visibleInternal"
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

        <v-list-item v-if="! notifications || notifications.length === 0">
          <v-list-item-title>{{ languageStrings.notificationList.sections.notifications.emptyList }}</v-list-item-title>
        </v-list-item>

        <v-list-group
          v-for="(np, npIdx) in notifications.filter(np => ((! selectedNotification) || (np.priorityId === selectedNotification.priority.id)))"
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

          <v-list-item
            v-for="(notif, notifIdx) in np.notifications.filter(np => ((! selectedNotification) || (np.id === selectedNotification.id)))"
            :key="notifIdx"
            class="px-4"
            @click="showNotificationDetails(notif)"
          >
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
              <v-list-item-subtitle v-if="selectedNotification !== null" class="text-wrap">
                {{ notif.message }}
              </v-list-item-subtitle>
            </v-list-item-content>
            <Promised :promise="notif.dismissalPromise">
              <template v-slot:combined="{ isPending, error }">
                <v-list-item-action v-if="selectedNotification === null" class="align-self-start">
                  <v-tooltip left>
                    <template v-slot:activator="{ on, attrs }">
                      <v-btn
                        :loading="notif.dismissalPromise && isPending"
                        :disabled="! notif.allowDismissal"
                        v-bind="attrs"
                        icon
                        v-on="error ? on : null"
                        @click.stop="dismissNotification(notif)"
                      >
                        <v-icon>mdi-close-circle</v-icon>
                      </v-btn>
                    </template>
                    {{ error }}
                  </v-tooltip>
                </v-list-item-action>
                <v-list-item-action v-else style="min-width: 76px;" class="align-self-start">
                  <div>
                    <v-btn icon @click.stop="hideNotificationDetails()">
                      <v-icon>
                        mdi-arrow-left
                      </v-icon>
                    </v-btn>
                    <v-tooltip left>
                      <template v-slot:activator="{ on, attrs }">
                        <v-btn
                          :loading="notif.dismissalPromise && isPending"
                          :disabled="! notif.allowDismissal"
                          v-bind="attrs"
                          icon
                          v-on="error ? on : null"
                          @click.stop="dismissNotification(notif)"
                        >
                          <v-icon>mdi-close-circle</v-icon>
                        </v-btn>
                      </template>
                      {{ error }}
                    </v-tooltip>
                  </div>
                </v-list-item-action>
              </template>
            </Promised>
          </v-list-item>
        </v-list-group>
      </v-list-group>
    </v-list>
  </v-navigation-drawer>
</template>

<script>
import { Promised } from 'vue-promised'
import { mapActions } from 'vuex'

export default {
  components: {
    Promised
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    invitations: {
      type: Object,
      required: false,
      default: null
    },
    notifications: {
      type: Array,
      required: true
    },
    visible: {
      type: Boolean,
      required: true
    }
  },
  data () {
    return {
      showNotificationsList: true,
      showInvitationsList: true,

      selectedNotification: null,
      showSingleNotification: false
    }
  },
  computed: {
    visibleInternal: {
      get () {
        return this.visible
      },
      set (value) {
        this.$emit('visible:change', value)
      }
    }
  },
  methods: {
    ...mapActions({
      dismissNotificationInternal: 'users/dismissNotification'
    }),
    showNotificationDetails (notification) {
      this.selectedNotification = notification
      this.showSingleNotification = true
    },
    hideNotificationDetails () {
      this.selectedNotification = null
    },
    dismissNotification (notification) {
      const nestedThis = this

      notification.dismissalPromise = this.dismissNotificationInternal(notification.id)
        .then(() => {
          nestedThis.$store.commit('users/removeNotification', notification)

          if (nestedThis.selectedNotification === notification) {
            nestedThis.selectedNotification = null
          }
        })
    }
  }
}
</script>
