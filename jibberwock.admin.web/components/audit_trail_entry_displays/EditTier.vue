<template>
  <div>
    <v-simple-table>
      <tbody>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.id }}</td>
          <td>{{ entryMetadata.tier.id }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.externalId }}</td>
          <td v-if="entryMetadata.tier.externalId === null || entryMetadata.tier.externalId === ''">
            {{ languageStrings.noValue.externalTierId }}
          </td>
          <td v-else>
            {{ entryMetadata.tier.externalId }}
          </td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.creatingTier }}</td>
          <td>
            <v-chip v-if="entryMetadata.newTier" color="success" small>
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
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.name }}</td>
          <td>{{ entryMetadata.tier.name }}</td>
        </tr>
        <tr v-if="typeof entryMetadata.tier.product !== 'undefined' && entryMetadata.tier.product !== null">
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.productId }}</td>
          <td>{{ entryMetadata.tier.product.id }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.visible }}</td>
          <td>
            <v-chip v-if="entryMetadata.tier.visible" color="success" small>
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
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.startDate }}</td>
          <td v-if="entryMetadata.tier.startDate === null || entryMetadata.tier.startDate === ''">
            {{ languageStrings.noValue.tierStartDate }}
          </td>
          <td v-else>
            {{ new Date(entryMetadata.tier.startDate).toLocaleDateString() }}
          </td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.endDate }}</td>
          <td v-if="entryMetadata.tier.endDate === null || entryMetadata.tier.endDate === ''">
            {{ languageStrings.noValue.tierEndDate }}
          </td>
          <td v-else>
            {{ new Date(entryMetadata.tier.endDate).toLocaleDateString() }}
          </td>
        </tr>
      </tbody>
    </v-simple-table>
    <v-simple-table>
      <thead>
        <tr>
          <th>{{ languageStrings.auditTrailEntries.editTier.tierCharacteristicValues.fields.name }}</th>
          <th>{{ languageStrings.auditTrailEntries.editTier.tierCharacteristicValues.fields.value }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(ch, chIdx) in entryMetadata.tier.characteristics" :key="chIdx">
          <td>
            {{ ch.productCharacteristic.Name }}
          </td>
          <td v-if="ch.productCharacteristic.valueType === 2">
            <v-chip v-if="ch.characteristicValue" color="success" small>
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
          <td v-else>
            {{ ch.characteristicValue }}
          </td>
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
