<template>
  <Promised :promise="updatePromise">
    <template v-slot:combined="{ isPending, error }">
      <v-card :loading="isPending" elevation="0">
        <v-card-title>{{ title }}</v-card-title>
        <v-card-text>
          <v-alert v-if="error !== null" dense dismissible outlined type="error">
            <v-tooltip bottom>
              <template v-slot:activator="{ on, attrs }">
                <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToUpdateProductCharacteristic }}</span>
              </template>
              <span>{{ error.message }}</span>
            </v-tooltip>
          </v-alert>
          <v-row dense>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="updatedCharacteristic.name"
                :label="languageStrings.forms.fields.name"
                :error-messages="nameErrors"
                @input="$v.updatedCharacteristic.name.$touch()"
                @blur="$v.updatedCharacteristic.name.$touch()"
              />
            </v-col>
            <v-col cols="12" md="6">
              <v-select
                v-model="updatedCharacteristic.valueType"
                :items="languageStrings.pages.characteristics.characteristicValueTypes"
                :label="languageStrings.forms.fields.valueType"
                readonly
                item-text="label"
                item-value="id"
                hide-details
              />
            </v-col>
          </v-row>
          <v-row no-gutters>
            <v-col cols="12">
              <v-text-field
                v-model="updatedCharacteristic.description"
                :label="languageStrings.forms.fields.description"
                :error-messages="descriptionErrors"
                @input="$v.updatedCharacteristic.description.$touch()"
                @blur="$v.updatedCharacteristic.description.$touch()"
              />
            </v-col>
            <v-col cols="6">
              <v-checkbox v-model="updatedCharacteristic.enabled" hide-details :label="languageStrings.forms.fields.enabled" />
            </v-col>
            <v-col cols="6">
              <v-checkbox v-model="updatedCharacteristic.visible" hide-details :label="languageStrings.forms.fields.visible" />
            </v-col>
          </v-row>

          <v-overlay :value="isPending" absolute>
            <v-progress-circular indeterminate size="64" />
          </v-overlay>
        </v-card-text>
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

export default {
  components: {
    Promised
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
    characteristic: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      updatePromise: Promise.resolve(),
      updatedCharacteristic: { ...this.characteristic }
    }
  },
  validations: {
    updatedCharacteristic: {
      name: {
        required,
        maxLength: maxLength(128)
      },
      description: {
        maxLength: maxLength(256)
      }
    }
  },
  computed: {
    nameErrors () {
      const errors = []

      if (!this.$v.updatedCharacteristic.name.required) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.updatedCharacteristic.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    descriptionErrors () {
      const errors = []

      if (!this.$v.updatedCharacteristic.description.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.descriptionTooLong)
      }
      return errors
    }
  },
  watch: {
    characteristic (_value) {
      this.cancel()
    }
  },
  methods: {
    ...mapActions({
      updateInternal: 'product-characteristic/update'
    }),
    update () {
      this.updatePromise = this.updateInternal({
        characteristicId: this.updatedCharacteristic.id,
        characteristic: {
          name: this.updatedCharacteristic.name,
          description: this.updatedCharacteristic.description,
          visible: this.updatedCharacteristic.visible,
          enabled: this.updatedCharacteristic.enabled
        }
      }).then((retVal) => {
        this.$emit('update:characteristic', retVal.data)
        this.$emit('characteristic-updated', retVal.data)

        return retVal
      })
    },
    cancel () {
      this.updatedCharacteristic = { ...this.characteristic }
    }
  }
}
</script>
