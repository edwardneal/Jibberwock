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
          <template v-slot:toolbar-actions="{ isPending, error }">
            <v-toolbar-items>
              <v-btn depressed :disabled="isPending || error !== null || !userDetails.selection.some((s) => !s.enabled)" class="pl-2">
                <v-icon>mdi-lightning-bolt</v-icon>
                {{ languageStrings.pages.users.actions.enable }}
              </v-btn>
              <v-btn depressed :disabled="isPending || error !== null || !userDetails.selection.some((s) => s.enabled)" class="pl-2">
                <v-icon>mdi-lightning-bolt-outline</v-icon>
                {{ languageStrings.pages.users.actions.disable }}
              </v-btn>
              <v-btn depressed :disabled="isPending || error !== null || userDetails.selection.length === 0" class="pl-3">
                <v-icon>mdi-message-draw</v-icon>
                {{ languageStrings.pages.users.actions.notify }}
              </v-btn>
              <v-btn depressed class="pl-3">
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
      :post-update-callback="updateNotification.postUpdateCallback"
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

export default {
  components: {
    SearchableTable,
    NotificationList,
    UpdateNotificationForm
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
      }
    }
  },
  computed: {
  },
  methods: {
    ...mapActions({
      findUsersInternal: 'users/findUsers',
      getUserNotificationsInternal: 'users/getNotifications'
    }),
    updateSelection (value) {
      this.userDetails.selection = value
    },
    showUpdateForm (notification, updateCallback) {
      this.updateNotification.postUpdateCallback = () => {
        this.updateNotification.showDialog = false
        updateCallback()
      }
      this.updateNotification.recordToUpdate = { ...notification }
      this.updateNotification.showDialog = true
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
