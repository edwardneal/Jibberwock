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
        <span class="headline">{{ languageStrings.forms.createProductTier.title }}</span>
      </v-card-title>
      <v-card-text>
        <v-row>
          <v-col cols="12">
            {{ languageStrings.forms.createProductTier.description }}
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="tier.name"
              :label="languageStrings.forms.fields.name"
              :error-messages="nameErrors"
              @input="$v.tier.name.$touch()"
              @blur="$v.tier.name.$touch()"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="tier.externalId"
              :label="languageStrings.forms.fields.externalIdentifier"
              :error-messages="externalIdErrors"
              @input="$v.tier.externalId.$touch()"
              @blur="$v.tier.externalId.$touch()"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="8">
            <v-data-table
              :headers="characteristicValueHeaders"
              :items="tier.characteristics"
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
                <v-checkbox v-model="tier.visible" hide-details :label="languageStrings.forms.fields.visible" />
              </v-col>
              <v-col cols="4" md="12">
                <CalendarDropdown :label="languageStrings.forms.fields.startDate" :selected-date.sync="tier.startDate" :max-date="tier.endDate" />
              </v-col>
              <v-col cols="4" md="12">
                <CalendarDropdown :label="languageStrings.forms.fields.endDate" :selected-date.sync="tier.endDate" :min-date="tier.startDate" />
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn color="primary" small :disabled="$v.$anyError" @click="create">
          {{ languageStrings.forms.buttons.create }}
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

const productTierDefaults = {
  name: '',
  externalId: '',
  product: null,
  characteristics: [],
  startDate: null,
  endDate: null,
  visible: true
}

export default {
  components: {
    CalendarDropdown
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
      tier: { ...productTierDefaults },
      characteristicToAdd: null,
      characteristicValueHeaders: [
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.characteristicValueName, value: 'productCharacteristic.name', sortable: true, filterable: true },
        { text: this.languageStrings.pages.products.detailsPanel.tabs.tiers.headers.characteristicValue, value: 'characteristicValue', sortable: true, filterable: true },
        { text: '', value: '_actions', align: 'right', sortable: false, filterable: false }
      ]
    }
  },
  validations: {
    tier: {
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

      if (!this.$v.tier.name.required) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.tier.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    externalIdErrors () {
      const errors = []

      if (!this.$v.tier.externalId.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.externalIdTooLong)
      }
      return errors
    }
  },
  methods: {
    hideForm () {
      this.$emit('update:visible', false)
      this.tier = { ...productTierDefaults }
      this.tier.characteristics = []
    },
    create () {
      this.$emit('create-tier', this.tier)
      this.hideForm()
    },
    listAvailableCharacteristics () {
      return this.availableProductCharacteristics
        .filter(ch =>
          !this.tier.characteristics.map(chV => chV.productCharacteristic).includes(ch)
        )
    },
    addCharacteristic () {
      this.tier.characteristics.push({
        characteristicValue: null,
        productCharacteristic: this.characteristicToAdd
      })
      this.characteristicToAdd = null
    },
    removeCharacteristicValue (cv) {
      const arrIdx = this.tier.characteristics.indexOf(cv)

      if (arrIdx !== -1) {
        this.tier.characteristics.splice(arrIdx, 1)
      }
    }
  }
}
</script>
