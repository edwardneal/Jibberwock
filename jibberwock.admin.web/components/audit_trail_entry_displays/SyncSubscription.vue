<template>
  <v-simple-table>
    <tbody>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.syncSubscriptions.fields.status }}</td>
        <td>{{ languageStrings.auditTrailEntries.syncSubscriptions.subscriptionStatuses.find(st => st.id === entryMetadata.newStatus).label }}</td>
      </tr>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.syncSubscriptions.fields.latestInvoiceIdentifier }}</td>
        <td>{{ entryMetadata.latestInvoiceExternalIdentifier }}</td>
      </tr>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.syncSubscriptions.fields.externalSubscriptionIdentifier }}</td>
        <td>{{ entryMetadata.externalSubscriptionIdentifier }}</td>
      </tr>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.syncSubscriptions.fields.subscriptionIds }}</td>
        <td>
          <v-chip v-for="sId in entryMetadata.subscriptionIds" :key="sId" small>
            {{ sId }}
          </v-chip>
        </td>
      </tr>
    </tbody>
  </v-simple-table>
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
    }
  }
}
</script>
