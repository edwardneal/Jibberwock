<template>
  <Promised :promise="listTiers(productId)">
    <template v-slot:combined="{ isPending, error, data }">
      <v-card :loading="isPending" flat>
        <v-card-text v-if="error !== null">
          <v-alert v-if="error !== null" dense dismissible outlined type="error">
            <v-tooltip bottom>
              <template v-slot:activator="{ on, attrs }">
                <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToGetProductTiers }}</span>
              </template>
              <span>{{ error.message }}</span>
            </v-tooltip>
          </v-alert>
        </v-card-text>
        <v-card-text else>
          <v-data-table
            :headers="headers"
            :items="!isPending ? data.data : undefined"
          >
            <template v-slot:item.visible="{ item }">
              <v-chip v-if="item.visible" color="success" small>
                <v-icon small>
                  mdi-check
                </v-icon>
              </v-chip>
              <v-chip v-else color="error" small>
                <v-icon small>
                  mdi-block-helper
                </v-icon>
              </v-chip>
            </template>
            <template v-slot:item._available="{ item }">
              <v-chip v-if="tierAvailable(item)" color="success" small>
                <v-icon small>
                  mdi-check
                </v-icon>
              </v-chip>
              <v-chip v-else color="error" small>
                <v-icon small>
                  mdi-block-helper
                </v-icon>
              </v-chip>
            </template>
            <template v-slot:item._actions="{ item }">
              <v-btn icon color="primary" @click.stop="$emit('tier-selected', item)">
                <v-icon>mdi-information</v-icon>
              </v-btn>
            </template>
          </v-data-table>
        </v-card-text>
      </v-card>
    </template>
  </Promised>
</template>

<script>
import { Promised } from 'vue-promised'
import { mapActions } from 'vuex'

export default {
  components: {
    Promised
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    productId: {
      type: Number,
      required: true
    }
  },
  data () {
    return {
      headers: [
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.name, value: 'name', sortable: true, filterable: true },
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.visible, value: 'visible', sortable: true, filterable: true },
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.available, value: '_available', sortable: true, filterable: true },
        { text: '', value: '_actions', align: 'right', sortable: false, filterable: false }
      ]
    }
  },
  methods: {
    ...mapActions({
      listTiersInternal: 'products/listTiers'
    }),
    listTiers (productId) {
      return this.listTiersInternal(productId)
        .then((resp) => {
          this.$emit('tiers-retrieved', resp.data)

          return resp
        })
    },
    tierAvailable (tier) {
      const currDate = new Date()

      // A tier is available for selection if the current date is between startDate and endDate.
      // If either of these fields are null, the corresponding part of the check doesn't apply
      return (tier.startDate === null || currDate >= new Date(tier.startDate)) &&
        (tier.endDate === null || currDate <= new Date(tier.endDate))
    }
  }
}
</script>
