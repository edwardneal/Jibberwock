<template>
  <v-sheet>
    <v-row justify="center" align="center">
      <v-col cols="12">
        {{ languageStrings.pages.users.instructions }}
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-col sm="12" md="6" cols="12">
        <SearchableTable
          :language-strings="languageStrings"
          :headers="userListHeaders"
          :max-search-length="128"
          :search-function="findUsersInternal"
          @selection-changed="updateSelection"
        >
          <template v-slot:toolbar-actions="{ shouldDisable }">
            <v-toolbar-items>
              <v-btn depressed :disabled="shouldDisable || !userDetails.selection.some((s) => !s.enabled)" class="pl-2" @click="enableUser">
                <v-icon>mdi-lightning-bolt</v-icon>
                {{ languageStrings.pages.users.actions.enable }}
              </v-btn>
              <v-btn depressed :disabled="shouldDisable || !userDetails.selection.some((s) => s.enabled)" class="pl-2" @click="disableUser">
                <v-icon>mdi-lightning-bolt-outline</v-icon>
                {{ languageStrings.pages.users.actions.disable }}
              </v-btn>
              <v-btn depressed :disabled="shouldDisable || userDetails.selection.length === 0" class="pl-3" @click="showNotifyForm(false)">
                <v-icon>mdi-message-draw</v-icon>
                {{ languageStrings.pages.users.actions.notify }}
              </v-btn>
              <v-btn depressed class="pl-3" @click="showNotifyForm(true)">
                <v-icon>mdi-message-draw</v-icon>
                {{ languageStrings.pages.users.actions.notifyAll }}
              </v-btn>
            </v-toolbar-items>
          </template>
        </SearchableTable>
      </v-col>
      <v-col sm="12" md="6" cols="12">
        <v-card v-if="userDetails.selection.length === 0" elevation="0">
          <v-card-title>{{ languageStrings.pages.users.detailsPanel.title }}</v-card-title>
          <v-card-text>
            {{ languageStrings.pages.users.errorMessages.selectUserForDetails }}
          </v-card-text>
        </v-card>
        <v-card v-else>
          <v-card-title>{{ languageStrings.pages.users.detailsPanel.title }}</v-card-title>
          <v-card-subtitle class="text-subtitle-1">
            {{ languageStrings.pages.users.detailsPanel.notifications.title }}
          </v-card-subtitle>
          <v-card-text>
            <NotificationList :language-strings="languageStrings" :users="userDetails.selection" @notification-selected="showUpdateForm" />
          </v-card-text>
          <v-card-subtitle>{{ languageStrings.pages.users.detailsPanel.tenants.title }}</v-card-subtitle>
          <v-card-text>Tenants here</v-card-text>
          <v-card-text>
            New API: GET /api/users/:id/tenants
            (will need this for first non-admin page too)
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
    <UpdateNotificationForm
      :visible.sync="updateNotification.showDialog"
      :language-strings="languageStrings"
      :notification="updateNotification.recordToUpdate"
      @notification-updated="refreshNotificationList"
    />
    <NotifyForm
      :visible.sync="notify.showDialog"
      :language-strings="languageStrings"
      :target-users="notify.notifyAllUsers ? null : userDetails.selection"
      @notified="refreshNotificationList"
    />
    <ProgressDialog
      :language-strings="languageStrings"
      :prompt="formatPlural(languageStrings.dialogs.enableUserConfirmation, userDetails.selection)"
      :activity-promise-factory="userAccess.enableUserPromiseFactory"
      :confirm-button-text="languageStrings.actions.confirm"
      :cancel-button-text="languageStrings.actions.cancel"
    />
    <ProgressDialog
      :language-strings="languageStrings"
      :prompt="formatPlural(languageStrings.dialogs.disableUserConfirmation, userDetails.selection)"
      :activity-promise-factory="userAccess.disableUserPromiseFactory"
      :confirm-button-text="languageStrings.actions.confirm"
      :cancel-button-text="languageStrings.actions.cancel"
    />
  </v-sheet>
</template>

<style>
  .v-toolbar--dense .v-toolbar__content { padding-left: 0px; padding-right: 0px; }
</style>

<script>
import { mapActions } from 'vuex'
import SearchableTable from '~/components/SearchableTable.vue'
import NotificationList from '~/components/NotificationList.vue'
import UpdateNotificationForm from '~/components/UpdateNotificationForm.vue'
import ProgressDialog from '~/components/ProgressDialog.vue'
import NotifyForm from '~/components/NotifyForm.vue'

export default {
  components: {
    SearchableTable,
    NotificationList,
    UpdateNotificationForm,
    ProgressDialog,
    NotifyForm
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      userListHeaders: [
        {
          text: this.languageStrings.pages.users.headers.userName,
          value: 'name',
          sortable: true,
          filterable: true
        },
        {
          text: this.languageStrings.pages.users.headers.enabled,
          value: 'enabled',
          sortable: true,
          filterable: true,
          groupable: true
        }
      ],
      userDetails: {
        selection: []
      },

      updateNotification: {
        showDialog: false,
        recordToUpdate: null,
        postUpdateCallback: null
      },

      userAccess: {
        enableUserPromiseFactory: null,
        disableUserPromiseFactory: null
      },

      notify: {
        showDialog: false,
        notifyAllUsers: false
      }
    }
  },
  computed: {
  },
  methods: {
    ...mapActions({
      findUsersInternal: 'users/findUsers',
      getUserNotificationsInternal: 'users/getNotifications',
      controlUserAccess: 'users/controlUserAccess'
    }),
    updateSelection (value) {
      this.userDetails.selection = value
    },
    showUpdateForm (notification) {
      this.updateNotification.recordToUpdate = { ...notification }
      this.updateNotification.showDialog = true
    },
    showNotifyForm (allUsers) {
      this.notify.notifyAllUsers = allUsers
      this.notify.showDialog = true
    },
    refreshNotificationList () {
      // This isn't obvious: it's just re-triggering the population of the notification list
      this.userDetails.selection = [...this.userDetails.selection]
    },
    formatPlural (languageString, collection) {
      const replacementPlural = collection.length === 1 ? '' : 's'
      const replacementThis = collection.length === 1 ? 'this' : 'these'

      return languageString.replace('{thisPlural}', replacementThis).replace('{plural}', replacementPlural)
    },
    enableUser () {
      const selectedUsers = this.userDetails.selection
      const enableSingleUser = (usr) => {
        return this.controlUserAccess({ userId: usr.id, enabled: true })
          .then((v) => {
            usr.enabled = v.data.enabled

            return v
          })
      }

      this.userAccess.enableUserPromiseFactory = () => {
        return Promise.all(selectedUsers.map(enableSingleUser))
      }
    },
    disableUser () {
      const selectedUsers = this.userDetails.selection
      const disableSingleUser = (usr) => {
        return this.controlUserAccess({ userId: usr.id, enabled: false })
          .then((v) => {
            usr.enabled = v.data.enabled

            return v
          })
      }

      this.userAccess.disableUserPromiseFactory = () => {
        return Promise.all(selectedUsers.map(disableSingleUser))
      }
    }
  },
  meta: {
    auth: { required: true }
  },
  head () {
    return {
      title: this.languageStrings.pages.users.title
    }
  }
}
</script>
