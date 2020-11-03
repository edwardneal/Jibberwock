<template>
  <v-sheet>
    <v-row justify="center" align="center">
      <v-col cols="12">
        {{ languageStrings.pages.tenants.instructions }}
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-col sm="12" md="6" cols="12">
        <SearchableTable
          :language-strings="languageStrings"
          :headers="tenantListHeaders"
          :max-search-length="128"
          :search-function="findTenantsInternal"
          @selection-changed="updateSelection"
        >
          <template v-slot:toolbar-actions="{ shouldDisable }">
            <v-toolbar-items>
              <v-btn depressed :disabled="shouldDisable || tenantDetails.selection.length === 0" class="pl-3" @click="showNotifyForm">
                <v-icon>mdi-message-draw</v-icon>
                {{ languageStrings.pages.users.actions.notify }}
              </v-btn>
            </v-toolbar-items>
          </template>
        </SearchableTable>
      </v-col>
      <v-col sm="12" md="6" cols="12">
        <v-card v-if="tenantDetails.selection.length === 0" elevation="0">
          <v-card-title>{{ languageStrings.pages.tenants.detailsPanel.title }}</v-card-title>
          <v-card-text>
            {{ languageStrings.pages.tenants.errorMessages.selectTenantForDetails }}
          </v-card-text>
        </v-card>
        <v-card v-else>
          <v-card-title>{{ languageStrings.pages.tenants.detailsPanel.title }}</v-card-title>
          <v-card-subtitle class="text-subtitle-1">
            {{ languageStrings.pages.tenants.detailsPanel.notifications.title }}
          </v-card-subtitle>
          <v-card-text>
            <NotificationList :language-strings="languageStrings" :tenants="tenantDetails.selection" @notification-selected="showUpdateForm" />
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
      :target-tenants="tenantDetails.selection"
      @notified="refreshNotificationList"
    />
  </v-sheet>
</template>

<script>
import { mapActions } from 'vuex'
import SearchableTable from '~/components/SearchableTable.vue'

export default {
  components: {
    SearchableTable
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      tenantListHeaders: [
        {
          text: this.languageStrings.pages.tenants.headers.tenantName,
          value: 'name',
          sortable: true,
          filterable: true
        }
      ],
      tenantDetails: {
        selection: []
      },

      updateNotification: {
        showDialog: false,
        recordToUpdate: null,
        postUpdateCallback: null
      },

      notify: {
        showDialog: false
      }
    }
  },
  methods: {
    ...mapActions({
      findTenantsInternal: 'tenants/findTenants'
    }),
    updateSelection (value) {
      this.tenantDetails.selection = value
    },
    showUpdateForm (notification) {
      this.updateNotification.recordToUpdate = { ...notification }
      this.updateNotification.showDialog = true
    },
    showNotifyForm () {
      this.notify.showDialog = true
    },
    refreshNotificationList () {
      // This isn't obvious: it's just re-triggering the population of the notification list
      this.tenantDetails.selection = [...this.tenantDetails.selection]
    }
  },
  meta: {
    auth: { required: true }
  },
  head () {
    return {
      title: this.languageStrings.pages.tenants.title,
      meta: [
        { hid: 'og:title', content: this.languageStrings.pages.tenants.title + ' - Jibberwock Admin' },
        { hid: 'apple-mobile-web-app-title', content: this.languageStrings.pages.tenants.title + ' - Jibberwock Admin' }
      ]
    }
  }
}
</script>
