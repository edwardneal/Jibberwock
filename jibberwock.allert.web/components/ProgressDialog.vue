<template>
  <Promised v-if="activityPromiseFactory !== null && !promiseCompleted && !cancelled" :promise="internalPromise">
    <template v-slot:combined="{ isPending, error }">
      <v-dialog
        v-model="showDialog"
        :width="$vuetify.breakpoint.mdAndDown ? '80%' : '30%'"
        no-click-animation
        persistent
        scrollable
        overlay-opacity="0.2"
      >
        <v-card>
          <v-card-title>
            <span class="headline">{{ languageStrings.dialogs.confirmationDialogHeader }}</span>
          </v-card-title>
          <v-card-text>
            <v-alert v-if="error !== null" dense dismissible outlined type="error">
              <v-tooltip bottom>
                <template v-slot:activator="{ on, attrs }">
                  <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToCompleteAction }}</span>
                </template>
                <span>{{ error.message }}</span>
              </v-tooltip>
            </v-alert>
            {{ prompt }}
            <v-overlay :value="isPending" absolute>
              <v-progress-circular indeterminate size="64" />
            </v-overlay>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn color="primary" small :disabled="isPending" @click="confirm">
              {{ confirmButtonText }}
            </v-btn>
            <v-btn color="error" small :disabled="isPending" @click="cancel">
              {{ cancelButtonText }}
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </template>
  </Promised>
</template>

<script>
import { Promised } from 'vue-promised'

export default {
  components: {
    Promised
  },
  props: {
    prompt: {
      type: String,
      required: true
    },
    languageStrings: {
      type: Object,
      required: true
    },
    activityPromiseFactory: {
      type: Function,
      required: false,
      default: null
    },
    confirmButtonText: {
      type: String,
      required: true
    },
    cancelButtonText: {
      type: String,
      required: true
    },
    closeOnCompletion: {
      type: Boolean,
      required: false,
      default: true
    },
    completionResults: {
      type: Object,
      required: false,
      default: null
    }
  },
  data () {
    return {
      internalPromise: Promise.resolve(),
      promiseCompleted: false,
      cancelled: false
    }
  },
  computed: {
    showDialog () {
      return typeof this.activityPromiseFactory !== 'undefined' && this.activityPromiseFactory !== null
    }
  },
  watch: {
    activityPromiseFactory () {
      this.promiseCompleted = false
      this.cancelled = false
    }
  },
  methods: {
    confirm () {
      this.promiseCompleted = false
      this.cancelled = false
      this.internalPromise = this.activityPromiseFactory().then((val) => {
        this.$emit('update:completion-results', val)

        if (this.closeOnCompletion) {
          this.promiseCompleted = true
        }

        return val
      })
    },
    cancel () {
      this.cancelled = true
      this.$emit('cancelled')
    }
  }
}
</script>
