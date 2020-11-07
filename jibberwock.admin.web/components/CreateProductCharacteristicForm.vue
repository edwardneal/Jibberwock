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
            <span class="headline">{{ languageStrings.forms.createProductCharacteristic.title }}</span>
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col cols="12">
                {{ languageStrings.forms.createProductCharacteristic.description }}
                <v-alert v-if="error !== null" dense dismissible outlined type="error">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on, attrs }">
                      <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToCreateProductCharacteristic }}</span>
                    </template>
                    <span>{{ error.message }}</span>
                  </v-tooltip>
                </v-alert>
              </v-col>
            </v-row>
            <v-row dense>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="characteristic.name"
                  :label="languageStrings.forms.fields.name"
                  :error-messages="nameErrors"
                  @input="$v.characteristic.name.$touch()"
                  @blur="$v.characteristic.name.$touch()"
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-select
                  v-model="characteristic.valueType"
                  :items="languageStrings.pages.characteristics.characteristicValueTypes"
                  :label="languageStrings.forms.fields.valueType"
                  item-text="label"
                  item-value="id"
                  hide-details
                />
              </v-col>
            </v-row>
            <v-row no-gutters>
              <v-col cols="12">
                <v-text-field
                  v-model="characteristic.description"
                  :label="languageStrings.forms.fields.description"
                  :error-messages="descriptionErrors"
                  @input="$v.characteristic.description.$touch()"
                  @blur="$v.characteristic.description.$touch()"
                />
              </v-col>
              <v-col cols="6">
                <v-checkbox v-model="characteristic.enabled" hide-details :label="languageStrings.forms.fields.enabled" />
              </v-col>
              <v-col cols="6">
                <v-checkbox v-model="characteristic.visible" hide-details :label="languageStrings.forms.fields.visible" />
              </v-col>
            </v-row>

            <v-overlay :value="isPending" absolute>
              <v-progress-circular indeterminate size="64" />
            </v-overlay>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn color="primary" small :disabled="$v.$anyError" @click="createCharacteristic">
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

const characteristicDefaults = {
  name: null,
  description: null,
  visible: true,
  enabled: true,
  valueType: 1
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
      characteristic: { ...characteristicDefaults }
    }
  },
  validations: {
    characteristic: {
      name: {
        required,
        maxLength: maxLength(128)
      },
      description: {
        maxLength: maxLength(256)
      },
      valueType: {
        required
      }
    }
  },
  computed: {
    nameErrors () {
      const errors = []

      if (!this.$v.characteristic.name.required) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.characteristic.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    descriptionErrors () {
      const errors = []

      if (!this.$v.characteristic.description.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.descriptionTooLong)
      }
      return errors
    }
  },
  methods: {
    ...mapActions({
      createCharacteristicInternal: 'product-characteristic/create'
    }),
    hideForm () {
      this.$emit('update:visible', false)
      this.characteristic = { ...characteristicDefaults }
    },
    createCharacteristic () {
      this.creationPromise = this.createCharacteristicInternal(this.characteristic)
        .then((ch) => {
          this.$emit('created', ch)
          this.hideForm()
        })
    }
  }
}
</script>
