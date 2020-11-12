<template>
  <Promised v-if="visible" :promise="creationPromise">
    <template v-slot:combined="{ isPending, error }">
      <v-dialog
        v-model="visible"
        :width="$vuetify.breakpoint.mdAndDown ? '100%' : '60%'"
        no-click-animation
        persistent
        scrollable
      >
        <v-card>
          <v-card-title>
            <span class="headline">{{ languageStrings.forms.createProduct.title }}</span>
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col cols="12">
                {{ languageStrings.forms.createProduct.description }}
                <v-alert v-if="error !== null" dense dismissible outlined type="error">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on, attrs }">
                      <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToCreateProduct }}</span>
                    </template>
                    <span>{{ error.message }}</span>
                  </v-tooltip>
                </v-alert>
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" md="8">
                <v-text-field
                  v-model="product.name"
                  :label="languageStrings.forms.fields.name"
                  :error-messages="nameErrors"
                  @input="$v.product.name.$touch()"
                  @blur="$v.product.name.$touch()"
                />
              </v-col>
              <v-col cols="12" md="4">
                <v-checkbox v-model="product.visible" hide-details :label="languageStrings.forms.fields.visible" />
              </v-col>
            </v-row>
            <v-row>
              <v-col cols="12" md="4">
                <v-text-field
                  v-model="product.description"
                  :label="languageStrings.forms.fields.description"
                  :error-messages="descriptionErrors"
                  @input="$v.product.description.$touch()"
                  @blur="$v.product.description.$touch()"
                />
                <v-text-field
                  v-model="product.moreInformationUrl"
                  :label="languageStrings.forms.fields.moreInformationUrl"
                  :error-messages="moreInformationUrlErrors"
                  @input="$v.product.moreInformationUrl.$touch()"
                  @blur="$v.product.moreInformationUrl.$touch()"
                />
              </v-col>
              <v-col cols="12" md="8">
                <v-data-table
                  :headers="characteristicTableHeaders"
                  :items="product.applicableCharacteristics"
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
              </v-col>
            </v-row>

            <v-overlay :value="isPending" absolute>
              <v-progress-circular indeterminate size="64" />
            </v-overlay>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn color="primary" small :disabled="$v.$anyError" @click="createProduct">
              {{ languageStrings.forms.buttons.create }}
            </v-btn>
            <v-btn color="error" small @click="hideForm">
              {{ languageStrings.forms.buttons.cancel }}
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </template>
  </Promised>
</template>

<script>
import { Promised } from 'vue-promised'
import { required, maxLength } from 'vuelidate/lib/validators'
import { mapActions } from 'vuex'

const productDefaults = {
  name: null,
  description: null,
  moreInformationUrl: null,
  visible: true,
  applicableCharacteristics: []
}

export default {
  components: {
    Promised
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    visible: {
      type: Boolean,
      required: true
    }
  },
  data () {
    return {
      creationPromise: Promise.resolve(),
      listCharacteristicsPromise: this.listProductCharacteristicsInternal(),
      product: { ...productDefaults },
      characteristicToAdd: null,
      characteristicTableHeaders: [
        { text: this.languageStrings.pages.characteristics.headers.name, value: 'name', sortable: true, filterable: true },
        { text: '', value: '_actions', align: 'right', sortable: false, filterable: false }
      ]
    }
  },
  validations: {
    product: {
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

      if (!this.$v.product.name.required) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.product.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    descriptionErrors () {
      const errors = []

      if (!this.$v.product.description.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.descriptionTooLong)
      }
      return errors
    },
    moreInformationUrlErrors () {
      const errors = []

      if (!this.$v.product.moreInformationUrl.required) {
        errors.push(this.languageStrings.validationErrorMessages.noMoreInformationUrl)
      }
      if (!this.$v.product.moreInformationUrl.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.moreInformationUrlTooLong)
      }
      return errors
    }
  },
  methods: {
    ...mapActions({
      listProductCharacteristicsInternal: 'product-characteristic/list',
      createProductInternal: 'products/create'
    }),
    listProductCharacteristics () {
      const existingCharacteristicIds = this.product.applicableCharacteristics.map(ch => ch.id)

      return this.listCharacteristicsPromise
        .then((resp) => {
          return resp.data.filter(ch => !existingCharacteristicIds.includes(ch.id))
        })
    },
    addCharacteristic () {
      this.product.applicableCharacteristics.push(this.characteristicToAdd)
      this.characteristicToAdd = null
    },
    removeCharacteristic (characteristic) {
      const arrIdx = this.product.applicableCharacteristics.indexOf(characteristic)

      if (arrIdx !== -1) {
        this.product.applicableCharacteristics.splice(arrIdx, 1)
      }
    },
    hideForm () {
      this.product = { ...productDefaults, applicableCharacteristics: [] }
      this.$emit('update:visible', false)
    },
    createProduct () {
      this.creationPromise = this.createProductInternal({
        name: this.product.name,
        description: this.product.description,
        moreInformationUrl: this.product.moreInformationUrl,
        visible: this.product.visible,
        applicableCharacteristicIds: this.product.applicableCharacteristics.map(ch => ch.id)
      })
        .then((prod) => {
          this.$emit('created', prod)
          this.hideForm()
        })
    }
  }
}
</script>
