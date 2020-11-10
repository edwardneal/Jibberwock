<template>
  <Promised :promise="updatePromise">
    <template v-slot:combined="{ isPending, error }">
      <v-card :loading="isPending" elevation="0">
        <v-card-title>{{ title }}</v-card-title>
        <v-card-text v-if="error !== null">
          <v-alert v-if="error !== null" dense dismissible outlined type="error">
            <v-tooltip bottom>
              <template v-slot:activator="{ on, attrs }">
                <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToUpdateProduct }}</span>
              </template>
              <span>{{ error.message }}</span>
            </v-tooltip>
          </v-alert>
        </v-card-text>

        <v-tabs v-model="tabs.selectedItem" grow>
          <v-tab>
            <v-icon v-if="$v.updatedProduct.$anyError" color="error" left>
              mdi-alert-circle
            </v-icon>
            {{ languageStrings.pages.products.detailsPanel.tabs.general.title }}
          </v-tab>
          <v-tab>{{ languageStrings.pages.products.detailsPanel.tabs.applicableCharacteristics.title }}</v-tab>
          <v-tab>{{ languageStrings.pages.products.detailsPanel.tabs.tiers.title }}</v-tab>
        </v-tabs>
        <v-tabs-items v-model="tabs.selectedItem">
          <v-tab-item>
            <v-card flat>
              <v-card-text>
                <v-row dense>
                  <v-col cols="12" md="8">
                    <v-text-field
                      v-model="updatedProduct.name"
                      :label="languageStrings.forms.fields.name"
                      :error-messages="nameErrors"
                      @input="$v.updatedProduct.name.$touch()"
                      @blur="$v.updatedProduct.name.$touch()"
                    />
                  </v-col>
                  <v-col cols="12" md="4">
                    <v-checkbox v-model="updatedProduct.visible" hide-details :label="languageStrings.forms.fields.visible" />
                  </v-col>
                </v-row>
                <v-row no-gutters>
                  <v-col cols="12">
                    <v-text-field
                      v-model="updatedProduct.description"
                      :label="languageStrings.forms.fields.description"
                      :error-messages="descriptionErrors"
                      @input="$v.updatedProduct.description.$touch()"
                      @blur="$v.updatedProduct.description.$touch()"
                    />
                  </v-col>
                  <v-col cols="12">
                    <v-text-field
                      v-model="updatedProduct.moreInformationUrl"
                      :label="languageStrings.forms.fields.moreInformationUrl"
                      :error-messages="moreInformationUrlErrors"
                      @input="$v.updatedProduct.moreInformationUrl.$touch()"
                      @blur="$v.updatedProduct.moreInformationUrl.$touch()"
                    />
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card flat>
              <v-card-text>
                <v-data-table
                  :headers="characteristicTableHeaders"
                  :items="updatedProductCharacteristics"
                >
                  <template v-slot:top>
                    <Promised :promise="listProductCharacteristics()">
                      <template v-slot:combined="prodChar">
                        <v-toolbar dense>
                          <v-autocomplete
                            v-model="characteristicToAdd"
                            :items="typeof prodChar.data !== 'undefined' && prodChar.data !== null ? prodChar.data : []"
                            :loading="prodChar.isPending"
                            :label="languageStrings.pages.products.detailsPanel.tabs.applicableCharacteristics.associate"
                            hide-no-data
                            item-text="name"
                            hide-details
                            return-object
                            solo
                            flat
                          />
                          <v-toolbar-items>
                            <v-btn :disabled="characteristicToAdd === null" text @click="addCharacteristic">
                              {{ languageStrings.forms.buttons.add }}
                            </v-btn>
                          </v-toolbar-items>
                        </v-toolbar>
                      </template>
                    </Promised>
                  </template>
                  <template v-slot:item._actions="{ item }">
                    <v-btn icon @click="removeCharacteristic(item)">
                      <v-icon>mdi-delete-forever</v-icon>
                    </v-btn>
                  </template>
                </v-data-table>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item eager>
            <ProductTierList
              ref="tiers"
              :language-strings="languageStrings"
              :product="product"
              :product-characteristics="updatedProductCharacteristics"
            />
          </v-tab-item>
        </v-tabs-items>
        <v-overlay :value="isPending" absolute>
          <v-progress-circular indeterminate size="64" />
        </v-overlay>

        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isPending || $v.$anyError" color="primary" small @click="update">
            {{ languageStrings.forms.buttons.update }}
          </v-btn>
          <v-btn :disabled="isPending" color="error" small @click="cancel">
            {{ languageStrings.forms.buttons.cancel }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </template>
  </Promised>
</template>

<script>
import { Promised } from 'vue-promised'
import { required, maxLength } from 'vuelidate/lib/validators'
import { mapActions } from 'vuex'
import ProductTierList from '~/components/ProductTierList.vue'

export default {
  components: {
    Promised,
    ProductTierList
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    title: {
      type: String,
      required: true
    },
    product: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      updatePromise: Promise.resolve(),
      updatedProduct: { ...this.product },
      updatedProductCharacteristics: [...this.product.applicableCharacteristics],
      tabs: {
        selectedItem: 0
      },
      characteristicToAdd: null,
      characteristicTableHeaders: [
        { text: this.languageStrings.pages.characteristics.headers.name, value: 'name', sortable: true, filterable: true },
        { text: '', value: '_actions', align: 'right', sortable: false, filterable: false }
      ]
    }
  },
  validations: {
    updatedProduct: {
      name: {
        required,
        maxLength: maxLength(128)
      },
      description: {
        maxLength: maxLength(256)
      },
      moreInformationUrl: {
        required,
        maxLength: maxLength(256)
      }
    }
  },
  computed: {
    nameErrors () {
      const errors = []

      if (!this.$v.updatedProduct.name.required) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.updatedProduct.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    descriptionErrors () {
      const errors = []

      if (!this.$v.updatedProduct.description.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.descriptionTooLong)
      }
      return errors
    },
    moreInformationUrlErrors () {
      const errors = []

      if (!this.$v.updatedProduct.moreInformationUrl.required) {
        errors.push(this.languageStrings.validationErrorMessages.noMoreInformationUrl)
      }
      if (!this.$v.updatedProduct.moreInformationUrl.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.moreInformationUrlTooLong)
      }
      return errors
    }
  },
  watch: {
    product (_value) {
      this.cancel()
    }
  },
  methods: {
    ...mapActions({
      listProductCharacteristicsInternal: 'product-characteristic/list',
      listTiers: 'products/listTiers',
      updateProduct: 'products/update'
    }),
    listProductCharacteristics () {
      const existingCharacteristicIds = this.updatedProductCharacteristics.map(ch => ch.id)

      return this.listProductCharacteristicsInternal()
        .then((resp) => {
          return resp.data.filter(ch => !existingCharacteristicIds.includes(ch.id))
        })
    },
    addCharacteristic () {
      this.updatedProductCharacteristics.push(this.characteristicToAdd)
      this.characteristicToAdd = null
    },
    removeCharacteristic (characteristic) {
      const arrIdx = this.updatedProductCharacteristics.indexOf(characteristic)

      if (arrIdx !== -1) {
        this.updatedProductCharacteristics.splice(arrIdx, 1)
      }
    },
    update () {
      this.updatePromise = this.updateProduct({
        productId: this.updatedProduct.id,
        product: {
          name: this.updatedProduct.name,
          description: this.updatedProduct.description,
          moreInformationUrl: this.updatedProduct.moreInformationUrl,
          visible: this.updatedProduct.visible,
          applicableCharacteristicIDs: this.updatedProductCharacteristics.map(c => c.id)
        }
      })
        .then((productResponse) => {
          return this.$refs.tiers.syncTiers()
            .then((tierResponses) => {
              this.$emit('update:product', productResponse.data)
              this.$emit('product-updated', productResponse.data)

              return {
                productResponse,
                tierResponses
              }
            })
        })
    },
    cancel () {
      this.updatedProduct = { ...this.product }
      this.updatedProductCharacteristics = [...this.product.applicableCharacteristics]
      this.$refs.tiers.resetTierList()
    }
  }
}
</script>
