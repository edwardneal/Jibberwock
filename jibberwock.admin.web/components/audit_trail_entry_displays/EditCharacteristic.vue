<template>
  <v-simple-table>
    <tbody>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.editCharacteristic.fields.id }}</td>
        <td>{{ entry.type === 4 ? entryMetadata.productCharacteristic.id : entryMetadata.id }}</td>
      </tr>
      <tr v-if="entry.type === 4">
        <td>{{ languageStrings.auditTrailEntries.editCharacteristic.fields.creatingCharacteristic }}</td>
        <td>
          <v-chip v-if="entryMetadata.newProductCharacteristic" color="success" small>
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
      <tr v-if="entry.type === 4">
        <td>{{ languageStrings.auditTrailEntries.editCharacteristic.fields.name }}</td>
        <td>{{ entryMetadata.productCharacteristic.name }}</td>
      </tr>
      <tr v-if="entry.type === 4">
        <td>{{ languageStrings.auditTrailEntries.editCharacteristic.fields.description }}</td>
        <td>{{ entryMetadata.productCharacteristic.description }}</td>
      </tr>
      <tr v-if="entry.type === 4">
        <td>{{ languageStrings.auditTrailEntries.editCharacteristic.fields.enabled }}</td>
        <td>
          <v-chip v-if="entryMetadata.productCharacteristic.enabled" color="success" small>
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
      <tr v-if="entry.type === 4">
        <td>{{ languageStrings.auditTrailEntries.editCharacteristic.fields.visible }}</td>
        <td>
          <v-chip v-if="entryMetadata.productCharacteristic.visible" color="success" small>
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
