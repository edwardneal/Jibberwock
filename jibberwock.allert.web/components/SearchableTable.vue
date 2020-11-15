<template>
  <Promised :promise="internalPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-data-table
        v-model="selectedRecords"
        show-select
        single-select
        :headers="headers"
        :loading="started && isPending"
        :items="started && !isPending && error === null ? data.data : undefined"
      >
        <template v-slot:top>
          <v-text-field
            v-model="searchString"
            :label="languageStrings.actions.search"
            clearable
            autofocus
            :error-messages="searchStringErrors"
            @input="$v.searchString.$touch()"
            @blur="$v.searchString.$touch()"
            @keyup.enter="if (! $v.$anyError) { findRecords() }"
          >
            <template v-slot:append-outer>
              <v-btn color="primary" small :disabled="(started && isPending) || ($v.$anyError)" @click="findRecords">
                {{ languageStrings.pages.users.searchButton }}
              </v-btn>
            </template>
          </v-text-field>
          <v-alert v-if="error !== null" dense dismissible outlined type="error">
            <v-tooltip bottom>
              <template v-slot:activator="{ on, attrs }">
                <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToSearch }}</span>
              </template>
              <span>{{ error.message }}</span>
            </v-tooltip>
          </v-alert>
          <v-toolbar dense>
            <slot name="toolbar-actions" v-bind="{ shouldDisable: isPending || error !== null }" />
          </v-toolbar>
        </template>
        <template v-slot:item.enabled="{ item }">
          <v-chip v-if="item.enabled" color="success" small>
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
      </v-data-table>
    </template>
  </Promised>
</template>

<style>
  .v-toolbar--dense .v-toolbar__content {
    padding-left: 0px;
    padding-right: 0px;
  }
</style>

<script>
import { Promised } from 'vue-promised'
import { required, maxLength } from 'vuelidate/lib/validators'

export default {
  components: {
    Promised
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    headers: {
      type: Array,
      required: true
    },
    searchFunction: {
      type: Function,
      required: true
    },
    maxSearchLength: {
      type: Number,
      required: true,
      default: 128
    }
  },
  data () {
    return {
      started: false,
      searchString: '',
      internalPromise: undefined,
      internalRecords: []
    }
  },
  validations () {
    return {
      searchString: {
        required,
        maxLength: maxLength(this.maxSearchLength)
      }
    }
  },
  computed: {
    searchStringErrors () {
      const errors = []

      if (!this.$v.searchString.required) {
        errors.push(this.languageStrings.validationErrorMessages.noSearchString)
      }
      if (!this.$v.searchString.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.searchStringTooLong)
      }
      return errors
    },
    selectedRecords: {
      get () { return this.internalRecords },
      set (value) {
        this.internalRecords = value
        this.$emit('selection-changed', value)
      }
    }
  },
  methods: {
    findRecords () {
      this.started = true
      this.selectedRecords = []
      this.internalPromise = this.searchFunction(this.searchString ?? '')
    }
  }
}
</script>
