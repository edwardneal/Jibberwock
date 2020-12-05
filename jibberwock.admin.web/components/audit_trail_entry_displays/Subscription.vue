<template>
  <v-simple-table>
    <tbody>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.subscription.fields.tierName }}</td>
        <td>{{ entryMetadata.resultantSubscription.productTier.name }}</td>
      </tr>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.subscription.fields.isPaid }}</td>
        <td>
          <v-chip v-if="entryMetadata.resultantSubscription.productTier.externalId" color="success" small>
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
        <td>{{ languageStrings.auditTrailEntries.subscription.fields.subscriptionId }}</td>
        <td>{{ entryMetadata.resultantSubscription.id }}</td>
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
