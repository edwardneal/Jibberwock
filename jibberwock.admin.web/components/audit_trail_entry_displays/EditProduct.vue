<template>
  <v-simple-table>
    <tbody>
      <tr>
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.id }}</td>
        <td>{{ entry.type === 2 ? entryMetadata.product.id : entryMetadata.Id }}</td>
      </tr>
      <tr v-if="entry.type === 2">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.identifier }}</td>
        <td>{{ entryMetadata.product.resourceIdentifier }}</td>
      </tr>
      <tr v-if="entry.type === 2">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.creatingProduct }}</td>
        <td>
          <v-chip v-if="entryMetadata.newProduct" color="success" small>
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
        <td>{{ entryMetadata.product.name }}</td>
      </tr>
      <tr v-if="entry.type === 2">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.description }}</td>
        <td>{{ entryMetadata.product.description }}</td>
      </tr>
      <tr v-if="entry.type === 2">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.moreInformationUrl }}</td>
        <td v-if="entryMetadata.product.moreInformationUrl === null || entryMetadata.product.moreInformationUrl === ''">
          (not set)
        </td>
        <td v-else>
          <a :href="entryMetadata.product.moreInformationUrl" rel="noreferrer" target="_blank">{{ entryMetadata.product.moreInformationUrl }}</a>
        </td>
      </tr>
      <tr v-if="entry.type === 2 && entryMetadata.product.configurationControlName">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.configurationControlName }}</td>
        <td>{{ entryMetadata.product.configurationControlName }}</td>
      </tr>
      <tr v-if="entry.type === 2 && entryMetadata.product.defaultProductConfiguration">
        <td>{{ languageStrings.auditTrailEntries.editProduct.fields.configurationControlName }}</td>
        <td>
          <v-tooltip top>
            <template v-slot:activator="{ on, attrs }">
              <v-chip small v-bind="attrs" v-on="on">
                <v-icon small>
                  mdi-dots-horizontal
                </v-icon>
              </v-chip>
            </template>
            {{ entryMetadata.product.defaultProductConfiguration.configurationString }}
          </v-tooltip>
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
