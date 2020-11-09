<template>
  <Promised :promise="listTiersPromise">
    <template v-slot:combined="{ isPending, error }">
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
            :items="!isPending ? masterList : undefined"
          >
            <template v-slot:top>
              <v-toolbar dense>
                <v-toolbar-items>
                  <v-btn text @click="showCreateDialog">
                    <v-icon left>
                      mdi-pencil
                    </v-icon>
                    {{ languageStrings.pages.products.detailsPanel.tabs.tiers.actions.create }}
                  </v-btn>
                </v-toolbar-items>
              </v-toolbar>
            </template>
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
              <v-tooltip left>
                <template v-slot:activator="{ on, attrs }">
                  <v-btn v-if="typeof item.id === 'undefined'" icon v-bind="attrs" @click.stop="removePendingTier(item)" v-on="on">
                    <v-icon>mdi-delete-forever</v-icon>
                  </v-btn>
                </template>
                <span>{{ languageStrings.pages.products.detailsPanel.tabs.tiers.tooltips.canDeletePendingTier }}</span>
              </v-tooltip>
              <v-btn icon color="primary" @click.stop="showUpdateDialog(item)">
                <v-icon>mdi-information</v-icon>
              </v-btn>
            </template>
          </v-data-table>
        </v-card-text>

        <CreateProductTierForm
          :language-strings="languageStrings"
          :product="product"
          :available-product-characteristics="productCharacteristics"
          :visible.sync="createDialogVisible"
          @create-tier="queueTierCreation"
        />
        <UpdateProductTierForm
          :language-strings="languageStrings"
          :tier="updateDialog.tier"
          :available-product-characteristics="productCharacteristics"
          :visible.sync="updateDialog.visible"
          @update-tier="queueTierUpdate"
        />
      </v-card>
    </template>
  </Promised>
</template>

<script>
import { Promised } from 'vue-promised'
import { mapActions } from 'vuex'
import CreateProductTierForm from '~/components/CreateProductTierForm.vue'
import UpdateProductTierForm from '~/components/UpdateProductTierForm.vue'

export default {
  components: {
    Promised,
    CreateProductTierForm,
    UpdateProductTierForm
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    product: {
      type: Object,
      required: true
    },
    productCharacteristics: {
      type: Array,
      required: true
    }
  },
  data () {
    return {
      listTiersPromise: this.listTiers(this.product.id),
      listTiersResult: [],
      headers: [
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.name, value: 'name', sortable: true, filterable: true },
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.visible, value: 'visible', sortable: true, filterable: true },
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.available, value: '_available', sortable: true, filterable: true },
        { text: '', value: '_actions', align: 'right', sortable: false, filterable: false }
      ],
      createDialogVisible: false,
      updateDialog: {
        visible: false,
        tier: null
      },
      masterList: [],
      tiersToCreate: [],
      tiersToUpdate: []
    }
  },
  methods: {
    ...mapActions({
      listTiersInternal: 'products/listTiers',
      createTierInternal: 'products/createTier',
      updateTierInternal: 'products/updateTier'
    }),
    listTiers (productId) {
      return this.listTiersInternal(productId)
        .then((resp) => {
          // Perform a deep clone, so we've got a static snapshot to work from
          this.listTiersResult = resp.data
          this.masterList = JSON.parse(JSON.stringify(this.listTiersResult))
          this.$emit('tiers-retrieved', this.masterList)

          return resp
        })
    },
    tierAvailable (tier) {
      const currDate = new Date()

      // A tier is available for selection if the current date is between startDate and endDate.
      // If either of these fields are null, the corresponding part of the check doesn't apply
      return (tier.startDate === null || currDate >= new Date(tier.startDate)) &&
        (tier.endDate === null || currDate <= new Date(tier.endDate))
    },
    queueTierCreation (tier) {
      this.masterList.push(tier)
      this.tiersToCreate.push(tier)
    },
    showCreateDialog () {
      this.createDialogVisible = true
    },
    removePendingTier (tier) {
      const pendingTierIndex = this.tiersToCreate.indexOf(tier)
      const masterTierIndex = this.masterList.indexOf(tier)

      if (pendingTierIndex !== -1 && masterTierIndex !== -1) {
        this.tiersToCreate.splice(pendingTierIndex, 1)
        this.masterList.splice(masterTierIndex, 1)
      }
    },
    showUpdateDialog (tier) {
      this.updateDialog.tier = tier
      this.updateDialog.visible = true
    },
    queueTierUpdate (tier) {
      // Always update the properties on this.updateDialog.tier
      // This will also update the record in masterList and tiersToCreate or tiersToUpdate, if needed
      Object.getOwnPropertyNames(tier)
        .forEach((prop) => {
          this.updateDialog.tier[prop] = tier[prop]
        })

      // If the tier has an ID, and that ID doesn't exist in tiersToUpdate,
      // add a reference to it
      if (typeof tier.id !== 'undefined' && tier.id !== null && tier.id > 0 &&
        !this.tiersToUpdate.some(tu => tu.id === tier.id)) {
        this.tiersToUpdate.push(this.updateDialog.tier)
      }
    },
    syncTiers () {
      // perform the inserts, then the updates
      // then, call listTiers followed by resetTierList
      const creationPromises = this.tiersToCreate.map((t) => {
        return this.createTierInternal({
          productId: this.product.id,
          tier: {
            externalId: t.externalId,
            name: t.name,
            visible: t.visible,
            startDate: t.startDate,
            endDate: t.endDate,
            characteristicValues: t.characteristics.map((tc) => {
              return {
                characteristicId: tc.productCharacteristic.id,
                value: tc.characteristicValue
              }
            })
          }
        })
      })
      const updatePromises = this.tiersToUpdate.map((t) => {
        return this.updateTierInternal({
          productId: this.product.id,
          tierId: t.id,
          tier: {
            externalId: t.externalId,
            name: t.name,
            visible: t.visible,
            startDate: t.startDate,
            endDate: t.endDate,
            characteristicValues: t.characteristics.map((tc) => {
              return {
                characteristicId: tc.productCharacteristic.id,
                value: tc.characteristicValue
              }
            })
          }
        })
      })

      return Promise.all(creationPromises.concat(updatePromises))
        .then(() => {
          return this.listTiers(this.product.id)
        })
        .then(() => {
          this.resetTierList()
        })
    },
    resetTierList () {
      this.masterList = JSON.parse(JSON.stringify(this.listTiersResult))
      this.tiersToCreate = []
      this.tiersToUpdate = []
    }
  }
}
</script>
