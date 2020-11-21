<template>
  <div>
    <v-simple-table>
      <tbody>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editNotification.fields.id }}</td>
          <td>{{ entryMetadata.notification.id }}</td>
        </tr>
      </tbody>
    </v-simple-table>
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
    }
  }
}
</script>
