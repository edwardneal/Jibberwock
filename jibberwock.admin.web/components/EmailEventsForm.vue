<template>
  <Promised v-if="visible" :promise="listEvents(email.externalEmailId)">
    <template v-slot:combined="{ isPending, error, data }">
      <v-dialog
        v-model="visible"
        :width="$vuetify.breakpoint.mdAndDown ? '100%' : '80%'"
        no-click-animation
        persistent
        scrollable
      >
        <v-card>
          <v-card-title>
            <span class="headline">{{ languageStrings.forms.getEmailEvents.title }}</span>
          </v-card-title>
          <v-card-text>
            <v-row v-if="error !== null">
              <v-col cols="12">
                <v-alert v-if="error !== null" dense dismissible outlined type="error">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on, attrs }">
                      <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToGetEmailEvents }}</span>
                    </template>
                    <span>{{ error.message }}</span>
                  </v-tooltip>
                </v-alert>
              </v-col>
            </v-row>
            <v-row>
              <v-col cols="12">
                <v-data-table
                  :headers="headers"
                  :loading="isPending"
                  :items="!isPending && error === null ? data.data : undefined"
                >
                  <template v-slot:item.timestamp="{ item }">
                    {{ new Date(item.timestamp).toLocaleString() }}
                  </template>
                </v-data-table>
              </v-col>
            </v-row>
            <v-overlay :value="isPending" absolute :opacity="0.8">
              <v-progress-circular indeterminate size="64" />
            </v-overlay>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
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
    email: {
      type: Object,
      required: false,
      default: null
    },
    visible: {
      type: Boolean,
      required: true
    }
  },
  data () {
    return {
      headers: [
        { text: this.languageStrings.forms.getEmailEvents.headers.timestamp, value: 'timestamp', sortable: true },
        { text: this.languageStrings.forms.getEmailEvents.headers.type, value: 'type', sortable: true },
        { text: this.languageStrings.forms.getEmailEvents.headers.smtpMessageId, value: 'smtpMessageId', sortable: true },
        { text: this.languageStrings.forms.getEmailEvents.headers.smtpBounceType, value: 'smtpBounceType', sortable: true },
        { text: this.languageStrings.forms.getEmailEvents.headers.smtpBounceReason, value: 'smtpBounceReason', sortable: true },
        { text: this.languageStrings.forms.getEmailEvents.headers.smtpDroppedReason, value: 'smtpDroppedReason', sortable: true },
        { text: this.languageStrings.forms.getEmailEvents.headers.smtpDeferredResponse, value: 'smtpDeferredResponse', sortable: true }
      ]
    }
  },
  methods: {
    ...mapActions({
      listEvents: 'email/getEvents'
    }),
    hideForm () {
      this.$emit('update:visible', false)
    }
  }
}
</script>
