<template>
  <v-dialog
    v-if="visible"
    v-model="visible"
    :width="$vuetify.breakpoint.mdAndDown ? '100%' : '60%'"
    no-click-animation
    persistent
    scrollable
  >
    <v-card>
      <v-card-title>
        <span class="headline">{{ languageStrings.forms.updateProductTier.title }}</span>
      </v-card-title>
      <v-card-text>
        <v-row>
          <v-col cols="12">
            {{ languageStrings.forms.updateProductTier.description }}
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="updatedTier.name"
              :label="languageStrings.forms.fields.name"
              :error-messages="nameErrors"
              @input="$v.updatedTier.name.$touch()"
              @blur="$v.updatedTier.name.$touch()"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="updatedTier.externalId"
              :label="languageStrings.forms.fields.externalIdentifier"
              :error-messages="externalIdErrors"
              @input="$v.updatedTier.externalId.$touch()"
              @blur="$v.updatedTier.externalId.$touch()"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="8">
            <v-data-table
              :headers="characteristicValueHeaders"
              :items="updatedTier.characteristics"
            >
              <template v-slot:top>
                <v-toolbar dense>
                  <v-autocomplete
                    v-model="characteristicToAdd"
                    :items="listAvailableCharacteristics()"
                    item-text="name"
                    hide-no-data
                    hide-details
                    return-object
                    solo
                    flat
                  />
                  <v-toolbar-items>
                    <v-btn :disabled="characteristicToAdd == null" text @click="addCharacteristic">
                      {{ languageStrings.forms.buttons.add }}
                    </v-btn>
                  </v-toolbar-items>
                </v-toolbar>
              </template>

              <template v-slot:item.characteristicValue="{ item }">
                <v-text-field
                  v-if="item.productCharacteristic.valueType === 1"
                  v-model="item.characteristicValue"
                  dense
                  hide-details
                  class="mb-1"
                />
                <v-chip v-else-if="item.productCharacteristic.valueType === 2 && item.characteristicValue" color="success" small @click="item.characteristicValue = false">
                  <v-icon small>
                    mdi-check
                  </v-icon>
                </v-chip>
                <v-chip v-else-if="item.productCharacteristic.valueType === 2 && !item.characteristicValue" color="error" small @click="item.characteristicValue = true">
                  <v-icon small>
                    mdi-block-helper
                  </v-icon>
                </v-chip>
                <v-text-field
                  v-else-if="item.productCharacteristic.valueType === 3"
                  v-model="item.characteristicValue"
                  type="number"
                  dense
                  hide-details
                  class="mb-1"
                />
              </template>

              <template v-slot:item._actions="{ item }">
                <v-btn icon @click="removeCharacteristicValue(item)">
                  <v-icon>mdi-delete-forever</v-icon>
                </v-btn>
              </template>
            </v-data-table>
          </v-col>
          <v-col cols="12" md="4">
            <v-row no-gutters>
              <v-col cols="4" md="12">
                <v-checkbox v-model="updatedTier.visible" hide-details :label="languageStrings.forms.fields.visible" />
              </v-col>
              <v-col cols="4" md="12">
                <CalendarDropdown :label="languageStrings.forms.fields.startDate" :selected-date.sync="updatedTier.startDate" :max-date="updatedTier.endDate" />
              </v-col>
              <v-col cols="4" md="12">
                <CalendarDropdown :label="languageStrings.forms.fields.endDate" :selected-date.sync="updatedTier.endDate" :min-date="updatedTier.startDate" />
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn color="primary" small :disabled="$v.$anyError" @click="update">
          {{ languageStrings.forms.buttons.update }}
        </v-btn>
        <v-btn color="error" small @click="hideForm">
          {{ languageStrings.forms.buttons.cancel }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { required, maxLength } from 'vuelidate/lib/validators'
import CalendarDropdown from '~/components/CalendarDropdown.vue'

export default {
  components: {
    CalendarDropdown
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    tier: {
      type: Object,
      required: false,
      default: null
    },
    visible: {
      type: Boolean,
      required: true
    },
    availableProductCharacteristics: {
      type: Array,
      required: true
    }
  },
  data () {
    return {
      updatedTier: null,
      characteristicToAdd: null,
      characteristicValueHeaders: [
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.characteristicValueName, value: 'productCharacteristic.name', sortable: true, filterable: true },
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.characteristicValue, value: 'characteristicValue', sortable: true, filterable: true },
        { text: '', value: '_actions', align: 'right', sortable: false, filterable: false }
      ]
    }
  },
  validations: {
    updatedTier: {
      name: {
        required,
        maxLength: maxLength(128)
      },
      externalId: {
        maxLength: maxLength(256)
      }
    }
  },
  computed: {
    nameErrors () {
      const errors = []

      if (!this.$v.updatedTier.name.required) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.updatedTier.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    externalIdErrors () {
      const errors = []

      if (!this.$v.updatedTier.externalId.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.externalIdTooLong)
      }
      return errors
    }
  },
  watch: {
    visible () {
      this.updatedTier = { ...this.tier }
      this.updatedTier.characteristics = this.tier.characteristics.map((cv) => {
        return {
          characteristicValue: cv.characteristicValue,
          id: cv.id,
          tier: null,
          // We perform an id-based lookup here, so that listAvailableCharacteristic can work by reference
          productCharacteristic: this.availableProductCharacteristics.find(apc => apc.id === cv.productCharacteristic.id)
        }
      })
    }
  },
  methods: {
    hideForm () {
      this.$emit('update:visible', false)
    },
    update () {
      this.$emit('update-tier', this.updatedTier)
      this.hideForm()
    },
    listAvailableCharacteristics () {
      return this.availableProductCharacteristics
        .filter(ch =>
          !this.updatedTier.characteristics.map(chV => chV.productCharacteristic).includes(ch)
        )
    },
    addCharacteristic () {
      this.updatedTier.characteristics.push({
        characteristicValue: null,
        productCharacteristic: this.characteristicToAdd
      })
      this.characteristicToAdd = null
    },
    removeCharacteristicValue (cv) {
      const arrIdx = this.updatedTier.characteristics.indexOf(cv)

      if (arrIdx !== -1) {
        this.updatedTier.characteristics.splice(arrIdx, 1)
      }
    }
  }
}
</script>
