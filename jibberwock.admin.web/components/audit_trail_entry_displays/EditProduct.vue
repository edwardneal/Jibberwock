<template>
  <v-simple-table>
    <tbody>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.id }}</td>
        <td>{{ entry.type === 2 ? entryMetadata.Product.Id : entryMetadata.Id }}</td>
      </tr>
      <tr v-if="entry.type === 2">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.identifier }}</td>
        <td>{{ entryMetadata.Product.ResourceIdentifier }}</td>
      </tr>
      <tr v-if="entry.type === 2">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.creatingProduct }}</td>
        <td>
          <v-chip v-if="entryMetadata.NewProduct" color="success" small>
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
      <tr v-if="entry.type === 2">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.name }}</td>
        <td>{{ entryMetadata.Product.Name }}</td>
      </tr>
      <tr v-if="entry.type === 2">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.description }}</td>
        <td>{{ entryMetadata.Product.Description }}</td>
      </tr>
      <tr v-if="entry.type === 2">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.moreInformationUrl }}</td>
        <td v-if="entryMetadata.Product.MoreInformationUrl === null || entryMetadata.Product.MoreInformationUrl === ''">
          (not set)
        </td>
        <td v-else>
          <a :href="entryMetadata.Product.MoreInformationUrl" rel="noreferrer" target="_blank">{{ entryMetadata.Product.MoreInformationUrl }}</a>
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
