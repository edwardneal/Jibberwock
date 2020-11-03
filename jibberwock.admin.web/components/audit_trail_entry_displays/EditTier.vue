<template>
  <div>
    <v-simple-table>
      <tbody>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.id }}</td>
          <td>{{ entryMetadata.Tier.Id }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.externalId }}</td>
          <td v-if="entryMetadata.Tier.ExternalId === null || entryMetadata.Tier.ExternalId === ''">
            {{ languageStrings.noValue.externalTierId }}
          </td>
          <td v-else>
            {{ entryMetadata.Tier.ExternalId }}
          </td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.creatingTier }}</td>
          <td>
            <v-chip v-if="entryMetadata.NewTier" color="success" small>
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
          <td>{{ entryMetadata.Tier.Name }}</td>
        </tr>
        <tr v-if="typeof entryMetadata.TierProduct !== 'undefined' && entryMetadata.Tier.Product !== null">
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.productId }}</td>
          <td>{{ entryMetadata.Tier.Product.Id }}</td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.visible }}</td>
          <td>
            <v-chip v-if="entryMetadata.Tier.Visible" color="success" small>
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
          <td v-if="entryMetadata.Tier.StartDate === null || entryMetadata.Tier.StartDate === ''">
            {{ languageStrings.noValue.tierStartDate }}
          </td>
          <td v-else>
            {{ new Date(entryMetadata.Tier.StartDate).toLocaleDateString() }}
          </td>
        </tr>
        <tr>
          <td>{{ languageStrings.auditTrailEntries.editTier.fields.endDate }}</td>
          <td v-if="entryMetadata.Tier.EndDate === null || entryMetadata.Tier.EndDate === ''">
            {{ languageStrings.noValue.tierEndDate }}
          </td>
          <td v-else>
            {{ new Date(entryMetadata.Tier.EndDate).toLocaleDateString() }}
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
        <tr v-for="(ch, chIdx) in entryMetadata.Tier.Characteristics" :key="chIdx">
          <td>
            {{ ch.ProductCharacteristic.Name }}
          </td>
          <td v-if="ch.ProductCharacteristic.ValueType === 2">
            <v-chip v-if="ch.CharacteristicValue" color="success" small>
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
            {{ ch.CharacteristicValue }}
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
