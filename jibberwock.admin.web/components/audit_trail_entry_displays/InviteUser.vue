<template>
  <v-simple-table>
    <tbody>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.inviteUser.fields.tenantName }}</td>
        <td>{{ entry.relatedTenant.name }}</td>
      </tr>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.inviteUser.fields.serviceBusMessageId }}</td>
        <td>{{ entryMetadata.emailBatch.serviceBusMessageId }}</td>
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
