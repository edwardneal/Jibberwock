<template>
  <Promised v-if="visible" :promise="updateNotificationPromise">
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
            <span class="headline">{{ languageStrings.forms.updateNotification.title }}</span>
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col cols="12">
                {{ languageStrings.forms.updateNotification.description }}
                <v-alert v-if="error !== null" dense dismissible outlined type="error">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on, attrs }">
                      <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToUpdateNotification }}</span>
                    </template>
                    <span>{{ error.message }}</span>
                  </v-tooltip>
                </v-alert>
              </v-col>
            </v-row>
            <v-row>
              <v-col sm="12" md="4" cols="12">
                <v-text-field v-model="resultantTo.name" :label="languageStrings.forms.fields.addressedTo" readonly hide-details>
                  <template v-slot:prepend>
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon v-on="on">
                          {{ resultantTo.icon }}
                        </v-icon>
                      </template>
                      {{ resultantTo.tooltip }}
                    </v-tooltip>
                  </template>
                </v-text-field>
              </v-col>
              <v-col sm="6" md="4" cols="6">
                <v-select
                  v-model="notification.priority.name"
                  :items="languageStrings.notificationList.notificationPriorities"
                  :label="languageStrings.forms.fields.priority"
                  item-text="label"
                  item-value="name"
                  hide-details
                />
              </v-col>
              <v-col sm="6" md="4" cols="6">
                <v-select
                  v-model="notification.type"
                  :items="languageStrings.notificationList.notificationTypes"
                  :label="languageStrings.forms.fields.notificationType"
                  item-text="label"
                  item-value="id"
                  hide-details
                />
              </v-col>
            </v-row>
            <v-row :no-gutters="$vuetify.breakpoint.smAndDown">
              <v-col sm="12" md="8" cols="12">
                <v-text-field
                  v-model="notification.subject"
                  :readonly="! modifiableFields.subject"
                  :label="languageStrings.forms.fields.subject"
                  :error-messages="subjectErrors"
                  @input="$v.notification.subject.$touch()"
                  @blur="$v.notification.subject.$touch()"
                />
                <v-textarea
                  v-model="notification.message"
                  :readonly="! modifiableFields.message"
                  rows="8"
                  :label="languageStrings.forms.fields.message"
                  :error-messages="messageErrors"
                  @input="$v.notification.message.$touch()"
                  @blur="$v.notification.message.$touch()"
                />
              </v-col>
              <v-col sm="12" md="4" cols="12">
                <v-row no-gutters>
                  <v-col sm="6" md="12" cols="6">
                    <v-checkbox v-model="resultantActive" hide-details :label="languageStrings.forms.fields.active" />
                  </v-col>
                  <v-col sm="6" md="12" cols="6">
                    <v-checkbox v-model="notification.allowDismissal" :label="languageStrings.forms.fields.allowDismissal" />
                  </v-col>
                  <v-col sm="6" md="12" cols="6">
                    <v-menu ref="startDateMenu" v-model="showStartDateMenu" offset-y>
                      <template v-slot:activator="{ on, attrs }">
                        <v-text-field
                          v-model="resultantStartDate"
                          v-bind="attrs"
                          :clearable="modifiableFields.startDate"
                          :label="languageStrings.forms.fields.startDate"
                          prepend-icon="mdi-calendar"
                          readonly
                          hide-details
                          v-on="modifiableFields.startDate ? on : null"
                        />
                      </template>
                      <v-date-picker
                        v-model="notification.startDate"
                        :max="notification.endDate"
                        no-title
                        :readonly="!modifiableFields.startDate"
                        scrollable
                        :width="typeof $refs.startDateMenu !== 'undefined' ? $refs.startDateMenu.dimensions.activator.width : null"
                      />
                    </v-menu>
                  </v-col>
                  <v-col sm="6" md="12" cols="6">
                    <v-menu ref="endDateMenu" v-model="showEndDateMenu" offset-y>
                      <template v-slot:activator="{ on, attrs }">
                        <v-text-field
                          v-model="resultantEndDate"
                          :label="languageStrings.forms.fields.endDate"
                          prepend-icon="mdi-calendar"
                          readonly
                          clearable
                          v-bind="attrs"
                          v-on="on"
                        />
                      </template>
                      <v-date-picker v-model="notification.endDate" :min="notification.startDate" :width="typeof $refs.endDateMenu !== 'undefined' ? $refs.endDateMenu.dimensions.activator.width : null" no-title scrollable />
                    </v-menu>
                  </v-col>
                  <v-col cols="12">
                    <v-checkbox v-model="resultantSendAsEmail" :readonly="! modifiableFields.sendAsEmail" :label="languageStrings.forms.fields.sendAsEmail" />
                  </v-col>
                </v-row>
              </v-col>
            </v-row>
            <v-overlay :value="isPending" absolute :opacity="0.8">
              <v-progress-circular indeterminate size="64" />
            </v-overlay>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn color="primary" small :disabled="$v.$anyError" @click="updateNotification">
              {{ languageStrings.forms.buttons.update }}
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
      required: true,
      default: true
    },
    notification: {
      type: Object,
      default: null
    },
    postUpdateCallback: {
      type: Function,
      default: null
    }
  },
  data () {
    return {
      showStartDateMenu: false,
      showEndDateMenu: false,
      updateNotificationPromise: Promise.resolve()
    }
  },
  validations: {
    notification: {
      subject: {
        required,
        maxLength: maxLength(128)
      },
      message: {
        required
      }
    }
  },
  computed: {
    resultantTo () {
      if (typeof this.notification === 'undefined' || this.notification === null) {
        return {}
      }
      if (this.notification.targetUser !== null) {
        return {
          name: this.notification.targetUser.name,
          icon: 'mdi-account',
          tooltip: this.languageStrings.forms.updateNotification.tooltips.specificUser
        }
      } else if (this.notification.targetTenant !== null) {
        return {
          name: this.notification.targetTenant.name,
          icon: 'mdi-account-group',
          tooltip: this.languageStrings.forms.updateNotification.tooltips.specificTenant
        }
      } else {
        return {
          name: this.languageStrings.notificationList.groupHeaders.allUsers,
          icon: 'mdi-account-group',
          tooltip: this.languageStrings.forms.updateNotification.tooltips.allUsers
        }
      }
    },
    resultantSendAsEmail: {
      get () {
        return typeof this.notification !== 'undefined' && this.notification !== null && this.notification.emailBatch !== null
      },
      set (value) {
        this.notification.emailBatch = value ? {} : null
      }
    },
    resultantActive: {
      get () {
        return typeof this.notification !== 'undefined' && this.notification !== null && this.notification.status === 1
      },
      set (value) {
        this.notification.status = value ? 1 : 2
      }
    },
    resultantStartDate () {
      return typeof this.notification !== 'undefined' && this.notification !== null && this.notification.startDate !== null
        ? new Date(this.notification.startDate).toLocaleDateString()
        : null
    },
    resultantEndDate () {
      return typeof this.notification !== 'undefined' && this.notification !== null && this.notification.endDate !== null
        ? new Date(this.notification.endDate).toLocaleDateString()
        : null
    },
    modifiableFields () {
      const sendAsEmailEnabled = this.resultantSendAsEmail
      const parsedStartDate = new Date(this.notification.startDate)
      const currentDate = new Date()
      const retVal = {
        startDate: true,
        subject: true,
        message: true,
        sendAsEmail: true
      }

      // See core.usp_UpdateNotification for these rules
      // NB: this means that someone could uncheck Send As Email, change all of these options,
      // then re-check it. This'd look "weird", but it's not the end of the world. The API protects against
      // this.
      if (sendAsEmailEnabled) {
        retVal.startDate = false

        if (parsedStartDate < currentDate) {
          retVal.subject = false
          retVal.message = false
          retVal.sendAsEmail = false
        }
      }

      return retVal
    },
    subjectErrors () {
      const errors = []

      if (!this.$v.notification.subject.required) {
        errors.push(this.languageStrings.validationErrorMessages.noSubject)
      }
      if (!this.$v.notification.subject.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.subjectTooLong)
      }
      return errors
    },
    messageErrors () {
      const errors = []

      if (!this.$v.notification.message.required) {
        errors.push(this.languageStrings.validationErrorMessages.noMessage)
      }
      return errors
    }
  },
  methods: {
    ...mapActions({
      updateUserNotificationInternal: 'users/updateNotification'
    }),
    hideForm () {
      this.$emit('update:visible', false)
    },
    updateNotification () {
      if (this.notification.targetTenant !== null) {
        // todo: update a tenant notification
      } else {
        // If it's not targeting a tenant, it's targeting at least one user
        const userId = this.notification.targetUser === null ? 'all' : this.notification.targetUser.id

        this.updateNotificationPromise = this.updateUserNotificationInternal({
          userId,
          notificationId: this.notification.id,
          notification: {
            status: this.notification.status,
            type: this.notification.type,
            priority: {
              name: this.notification.priority.name
            },
            startDate: this.notification.startDate,
            endDate: this.notification.endDate,
            subject: this.notification.subject,
            message: this.notification.message,
            allowDismissal: this.notification.allowDismissal,
            sendAsEmail: this.notification.resultantSendAsEmail
          }
        }).then(() => {
          // With this updated, just call the post-update callback
          if (this.postUpdateCallback !== null) {
            this.postUpdateCallback()
          }
        })
      }
    }
  }
}
</script>
