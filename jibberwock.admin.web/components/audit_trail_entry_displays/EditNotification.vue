<template>
  <div>
    <v-simple-table>
      <tbody>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.id }}</td>
          <td>{{ entryMetadata.Notification.Id }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.creatingNotification }}</td>
          <td>
            <v-chip v-if="entryMetadata.NewNotification" color="success" small>
              <v-icon small>
                mdi-check
              </v-icon>
            </v-chip>
            <v-chip v-else color="error" small>
              <v-icon small>
                mdi-block-helper
              </v-icon>
            </v-chip>
          </td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.target }}</td>
          <td>
            <v-tooltip bottom>
              <template v-slot:activator="{ on }">
                <v-chip small v-on="on">
                  <v-icon small class="pr-1">
                    {{ resultantTo.icon }}
                  </v-icon>
                  {{ resultantTo.name }}
                </v-chip>
              </template>
              {{ resultantTo.tooltip }}
            </v-tooltip>
          </td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.active }}</td>
          <td>
            <v-chip v-if="entryMetadata.Notification.AllowDismissal" color="success" small>
              <v-icon small>
                mdi-check
              </v-icon>
            </v-chip>
            <v-chip v-else color="error" small>
              <v-icon small>
                mdi-block-helper
              </v-icon>
            </v-chip>
          </td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.allowDismissal }}</td>
          <td>
            <v-chip v-if="entryMetadata.Notification.Status === 1" color="success" small>
              <v-icon small>
                mdi-check
              </v-icon>
            </v-chip>
            <v-chip v-else color="error" small>
              <v-icon small>
                mdi-block-helper
              </v-icon>
            </v-chip>
          </td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.priority }}</td>
          <td>{{ resultantPriority }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.type }}</td>
          <td>{{ resultantType }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.startDate }}</td>
          <td v-if="entryMetadata.Notification.StartDate === null || entryMetadata.Notification.StartDate === ''">{{ languageStrings.noValue.notificationStartDate }}</td>
          <td v-else>{{ new Date(entryMetadata.Notification.StartDate).toLocaleDateString() }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.endDate }}</td>
          <td v-if="entryMetadata.Notification.EndDate === null || entryMetadata.Notification.EndDate === ''">{{ languageStrings.noValue.notificationEndDate }}</td>
          <td v-else>{{ new Date(entryMetadata.Notification.EndDate).toLocaleDateString() }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.sendAsEmail }}</td>
          <td>
            <v-chip v-if="entryMetadata.SendAsEmail" color="success" small>
              <v-icon small>
                mdi-check
              </v-icon>
            </v-chip>
            <v-chip v-else color="error" small>
              <v-icon small>
                mdi-block-helper
              </v-icon>
            </v-chip>
          </td>
        </tr>
        <tr v-if="typeof entryMetadata.ServiceBusMessageId !== 'undefined' && entryMetadata.ServiceBusMessageId !== null && entryMetadata.ServiceBusMessageId !== ''">
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.emailBatchId }}</td>
          <td>{{ entryMetadata.ServiceBusMessageId }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.subject }}</td>
          <td>{{ entryMetadata.Notification.Subject }}</td>
        </tr>
      </tbody>
    </v-simple-table>
    <v-expansion-panels :value="0" flat accordion>
      <v-expansion-panel>
        <v-expansion-panel-header class="pl-4">{{ languageStrings.auditTrailEntries.editNotification.fields.message }}</v-expansion-panel-header>
        <v-expansion-panel-content>{{ entryMetadata.Notification.Message }}</v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>
  </div>
</template>

<script>
export default {
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    entry: {
      type: Object,
      required: true
    }
  },
  computed: {
    entryMetadata () {
      return typeof this.entry !== 'undefined' && this.entry !== null && this.entry.metadata != null
        ? JSON.parse(this.entry.metadata)
        : null
    },
    resultantTo () {
      if (this.entryMetadata.Notification.TargetUser !== null) {
        return {
          name: this.languageStrings.auditTrailEntries.editNotification.formatStrings.singleUserTarget.replace('{name}', this.entry.relatedUser.name),
          icon: 'mdi-account',
          tooltip: this.languageStrings.forms.updateNotification.tooltips.specificUser
        }
      } else if (this.entryMetadata.Notification.TargetTenant !== null) {
        return {
          name: this.languageStrings.auditTrailEntries.editNotification.formatStrings.singleTenantTarget.replace('{name}', this.entry.relatedTenant.name),
          icon: 'mdi-account-group',
          tooltip: this.languageStrings.forms.updateNotification.tooltips.specificTenant
        }
      } else {
        return {
          name: this.languageStrings.notificationList.groupHeaders.allUsers,
          icon: 'mdi-account-group',
          tooltip: this.languageStrings.forms.updateNotification.tooltips.allUsers
        }
      }
    },
    resultantPriority () {
      const priority = this.languageStrings.notificationList.notificationPriorities.find(np => np.name === this.entryMetadata.Notification.Priority.Name)

      return priority.label
    },
    resultantType () {
      const type = this.languageStrings.notificationList.notificationTypes.find(nt => nt.id === this.entryMetadata.Notification.Type)

      return type.label
    }
  }
}
</script>
