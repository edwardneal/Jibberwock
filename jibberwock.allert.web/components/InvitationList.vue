<template>
  <v-card>
    <v-card-text>
      <v-row>
        <v-col cols="12">
          <p>
            You can use this page to invite a new user to your tenant, or to revoke an invitation you've already made.
            To invite somebody, you need to provide an email address and to describe how they'll sign in (via Google, Microsoft or GitHub accounts, or using a "local" Allert username.)
            They will then receive an email address inviting them to log in, and when they log in they will be able to accept or reject their invitation.
          </p>
          <p>
            APIS TO INVITE, REVOKE INVITATION
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12">
          <PromisedTable ref="invitationList" :language-strings="languageStrings" :headers="invitations.headers" :populate-function="invitations.populate" :selectable="false">
            <template v-slot:toolbar-actions="{}">
              <v-text-field
                v-model="invitations.invitationToIssue.emailAddress"
                :label="languageStrings.forms.fields.invitation.emailAddress"
                :error-messages="invitationEmailAddressMessages"
                hide-details
                class="pr-2 pl-4"
                type="email"
                @input="$v.invitations.invitationToIssue.emailAddress.$touch()"
                @blur="$v.invitations.invitationToIssue.emailAddress.$touch()"
              />
              <v-select
                v-model="invitations.invitationToIssue.identityProvider"
                :items="languageStrings.dropdownValues.identityProvider"
                :error-messages="invitationIdentityProviderMessages"
                :label="languageStrings.forms.fields.invitation.identityProvider"
                item-text="label"
                item-value="id"
                hide-details
                single-line
                class="pl-2 pr-4 invitation-identity-provider"
                @input="$v.invitations.invitationToIssue.identityProvider.$touch()"
                @blur="$v.invitations.invitationToIssue.identityProvider.$touch()"
              />

              <v-toolbar-items>
                <v-btn :disabled="$v.invitations.invitationToIssue.$invalid" class="pl-3" color="success" @click="invite">
                  <v-icon left>
                    mdi-plus
                  </v-icon>
                  {{ languageStrings.actions.invite }}
                </v-btn>
              </v-toolbar-items>
            </template>
            <template v-slot:item-actions="{ item }">
              <v-tooltip bottom>
                <template v-slot:activator="{ on, attrs }">
                  <v-btn icon v-bind="attrs" v-on="on" @click="showRevokeConfirmation(item)">
                    <v-icon color="error">
                      mdi-delete-forever
                    </v-icon>
                  </v-btn>
                </template>
                {{ languageStrings.actions.revoke }}
              </v-tooltip>
            </template>

            <template v-slot:item.emailAddress="{ item }">
              <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon left v-on="on">
                    {{ languageStrings.dropdownValues.identityProvider.find(idp => idp.id === item.externalIdentityProvider).icon }}
                  </v-icon>
                </template>
                {{ languageStrings.dropdownValues.identityProvider.find(idp => idp.id === item.externalIdentityProvider).label }}
              </v-tooltip>
              {{ item.emailAddress }}
            </template>
            <template v-slot:item.invitationDate="{ item }">
              {{ new Date(item.invitationDate).toLocaleString() }}
            </template>
            <template v-slot:item.expirationDate="{ item }">
              {{ item.expirationDate ? new Date(item.expirationDate).toLocaleString() : '-' }}
            </template>
          </PromisedTable>

          <ProgressDialog
            :language-strings="languageStrings"
            :prompt="languageStrings.pages.tenant_security.sections.invitations.confirmations.revoke"
            :activity-promise-factory="invitations.revocationProgressFactory"
            :confirm-button-text="languageStrings.actions.revoke"
            :cancel-button-text="languageStrings.actions.cancel"
          />
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<style>
  .invitation-identity-provider.v-input { max-width: fit-content; }
</style>

<script>
import { required, maxLength, email } from 'vuelidate/lib/validators'
import { mapActions } from 'vuex'
import PromisedTable from '~/components/PromisedTable.vue'
import ProgressDialog from '~/components/ProgressDialog.vue'

export default {
  components: {
    PromisedTable,
    ProgressDialog
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    tenantId: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      tenant: {
        id: this.$route.params.id
      },
      invitations: {
        populate: () => this.getTenantInvitations(this.tenantId),
        headers: [
          { text: this.languageStrings.pages.tenant_security.sections.invitations.headings.emailAddress, value: 'emailAddress', sortable: true, filterable: false },
          { text: this.languageStrings.pages.tenant_security.sections.invitations.headings.invitationDate, value: 'invitationDate', sortable: true, filterable: false },
          { text: this.languageStrings.pages.tenant_security.sections.invitations.headings.expirationDate, value: 'expirationDate', sortable: true, filterable: false },
          { text: '', value: '_actions', sortable: false, filterable: false, align: 'right' }
        ],
        invitationToIssue: {
          identityProvider: null,
          emailAddress: null
        },
        revocationProgressFactory: null
      }
    }
  },
  validations: {
    invitations: {
      invitationToIssue: {
        emailAddress: {
          required,
          maxLength: maxLength(256),
          email
        },
        identityProvider: {
          required,
          maxLength: maxLength(32)
        }
      }
    }
  },
  computed: {
    invitationEmailAddressMessages () {
      const errors = []

      if (!this.$v.invitations.invitationToIssue.emailAddress.required) {
        errors.push(this.languageStrings.validationErrorMessages.noEmailAddress)
      }
      if (!this.$v.invitations.invitationToIssue.emailAddress.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.emailAddressTooLong)
      }
      if (!this.$v.invitations.invitationToIssue.emailAddress.email) {
        errors.push(this.languageStrings.validationErrorMessages.invalidEmailAddress)
      }
      return errors
    },
    invitationIdentityProviderMessages () {
      const errors = []

      if (!this.$v.invitations.invitationToIssue.identityProvider.required) {
        errors.push(this.languageStrings.validationErrorMessages.noIdentityProvider)
      }
      if (!this.$v.invitations.invitationToIssue.identityProvider.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.identityProviderTooLong)
      }
      return errors
    }
  },
  methods: {
    ...mapActions({
      getTenantInvitations: 'tenants/getTenantInvitations',
      inviteInternal: 'tenants/invite',
      revokeInvitation: 'tenants/revokeTenantInvitation'
    }),
    invite () {
      this.inviteInternal({
        tenantId: this.tenant.id,
        invitation: this.invitations.invitationToIssue
      }).then((resp) => {
        if (resp.status === 200) {
          this.invitations.invitationToIssue.identityProvider = null
          this.invitations.invitationToIssue.emailAddress = null

          this.$refs.invitationList.refresh()

          this.$emit('invited', resp.data)
        }
      })
    },
    showRevokeConfirmation (invitation) {
      this.invitations.revocationProgressFactory = () => this.revokeInvitation({ tenantId: this.tenant.id, invitationId: invitation.id })
    }
  }
}
</script>
