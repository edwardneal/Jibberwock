<template>
  <Promised v-if="visible" :promise="notifyPromise">
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
            <span class="headline">{{ languageStrings.forms.notify.title }}</span>
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col cols="12">
                {{ languageStrings.forms.notify.description }}
                <v-alert v-if="error !== null" dense dismissible outlined type="error">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on, attrs }">
                      <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToNotify }}</span>
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
                  :label="languageStrings.forms.fields.subject"
                  :error-messages="subjectErrors"
                  autofocus
                  @input="$v.notification.subject.$touch()"
                  @blur="$v.notification.subject.$touch()"
                />
                <v-textarea
                  v-model="notification.message"
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
                    <CalendarDropdown :label="languageStrings.forms.fields.startDate" :selected-date.sync="notification.startDate" :max-date="notification.endDate" />
                  </v-col>
                  <v-col sm="6" md="12" cols="6">
                    <CalendarDropdown :label="languageStrings.forms.fields.endDate" :selected-date.sync="notification.endDate" :min-date="notification.startDate" />
                  </v-col>
                  <v-col cols="12">
                    <v-checkbox v-model="resultantSendAsEmail" :label="languageStrings.forms.fields.sendAsEmail" />
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
            <v-btn color="primary" small :disabled="$v.$anyError" @click="notify">
              {{ languageStrings.forms.buttons.notify }}
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

const notificationDefaults = {
  targetUser: null,
  targetTenant: null,
  emailBatch: null,
  status: 1,
  type: 2,
  priority: { id: null, name: 'normal', order: null },
  creationDate: null,
  startDate: null,
  endDate: null,
  subject: null,
  message: null,
  allowDismissal: true
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
    targetUsers: {
      type: Array,
      required: false,
      default: null
    },
    targetTenants: {
      type: Array,
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
      notifyPromise: Promise.resolve(),
      showStartDateMenu: false,
      showEndDateMenu: false,
      notification: { ...notificationDefaults }
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
      if (this.targetUsers !== null) {
        const targetUserName = this.targetUsers.map(u => u.name).join('; ')

        return {
          name: targetUserName,
          icon: 'mdi-account',
          tooltip: this.languageStrings.forms.notify.tooltips.specificUser
        }
      } else if (this.targetTenants !== null) {
        const targetTenantName = this.targetTenants.map(u => u.name).join('; ')

        return {
          name: targetTenantName,
          icon: 'mdi-account-group',
          tooltip: this.languageStrings.forms.notify.tooltips.specificTenant
        }
      } else {
        return {
          name: this.languageStrings.notificationList.groupHeaders.allUsers,
          icon: 'mdi-account-group',
          tooltip: this.languageStrings.forms.notify.tooltips.allUsers
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
      notifyUsersInternal: 'users/notify',
      notifyTenantInternal: 'tenants/notify'
    }),
    hideForm () {
      this.$emit('update:visible', false)
      this.notification = { ...notificationDefaults }
    },
    notify () {
      let notificationPromises = []

      if (this.targetTenants !== null && this.targetTenants.length !== 0) {
        // Send this notification to one or more tenants
        notificationPromises = this.targetTenants.map(t =>
          this.notifyTenantInternal({
            tenantId: t.id,
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
          }))
      } else if (this.targetUsers !== null && this.targetUsers.length !== 0) {
        // Send this notification to one or more users
        notificationPromises = this.targetUsers.map(u =>
          this.notifyUsersInternal({
            userId: u.id,
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
              sendAsEmail: this.resultantSendAsEmail
            }
          }))
      } else {
        // Send a notification to all users in the platform
        notificationPromises.push(this.notifyUsersInternal({
          userId: 'all',
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
            sendAsEmail: this.resultantSendAsEmail
          }
        }))
      }

      this.notifyPromise = Promise.all(notificationPromises)
        .then((values) => {
          return values.flatMap(v => v.data)
        })
        .then((notification) => {
          notification.forEach(n => this.$emit('notified', n))
          this.hideForm()
        })
    }
  }
}
</script>
