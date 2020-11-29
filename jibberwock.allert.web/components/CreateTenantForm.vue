<template>
  <Promised :promise="tenant.creationPromise">
    <template v-slot:combined="creationPromiseResults">
      <v-stepper v-model="selectedTab" style="min-width: 420px;">
        <v-stepper-header>
          <v-stepper-step :rules="getTenantDetailsError()" :editable="! creationPromiseResults.isPending" step="1">
            {{ languageStrings.forms.createTenant.steps.tenantDetails }}
          </v-stepper-step>
          <template v-for="(step, stepIdx) in steps">
            <v-stepper-step :key="'h.' + stepIdx" :rules="getProductDetailsError(stepIdx)" :step="stepIdx + 2" :editable="! creationPromiseResults.isPending">
              {{ step.name }}
            </v-stepper-step>
          </template>
          <v-stepper-step :rules="getInvitationDetailsError()" :step="steps.length + 2" :editable="! creationPromiseResults.isPending">
            {{ languageStrings.forms.createTenant.steps.invitations }}
          </v-stepper-step>
          <v-stepper-step :step="steps.length + 3" :editable="!$v.tenant.$invalid && !creationPromiseResults.isPending">
            {{ languageStrings.forms.createTenant.steps.readyToCreate }}
          </v-stepper-step>
        </v-stepper-header>

        <v-stepper-content step="1" class="pa-0">
          <Promised :promise="productPromise">
            <template v-slot:combined="{ isPending, error }">
              <v-card flat class="rounded-t-0">
                <v-card-title class="text-h5">
                  {{ languageStrings.forms.createTenant.steps.tenantDetails }}
                </v-card-title>
                <v-card-text>
                  <v-alert v-if="error !== null" dense dismissible outlined type="error">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on, attrs }">
                        <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToLoadAvailableProductList }}</span>
                      </template>
                      <span>{{ error.message }}</span>
                    </v-tooltip>
                  </v-alert>

                  <v-text-field
                    v-model="tenant.name"
                    :label="languageStrings.forms.fields.tenant.name"
                    :error-messages="tenantNameMessages"
                    autofocus
                    @input="$v.tenant.name.$touch()"
                    @blur="$v.tenant.name.$touch()"
                  />
                  <v-checkbox
                    v-model="tenant.contact.useOwnDetails"
                    :label="languageStrings.forms.fields.tenant.useOwnContactDetails"
                    @input="$v.tenant.contact.name.$touch(); $v.tenant.contact.emailAddress.$touch()"
                    @blur="$v.tenant.contact.name.$touch(); $v.tenant.contact.emailAddress.$touch()"
                  />
                  <v-text-field
                    v-if="!tenant.contact.useOwnDetails"
                    v-model="tenant.contact.name"
                    :label="languageStrings.forms.fields.tenant.contactName"
                    :error-messages="contactNameMessages"
                    @input="$v.tenant.contact.name.$touch()"
                    @blur="$v.tenant.contact.name.$touch()"
                  />
                  <v-text-field
                    v-if="!tenant.contact.useOwnDetails"
                    v-model="tenant.contact.emailAddress"
                    :label="languageStrings.forms.fields.tenant.contactEmailAddress"
                    :error-messages="contactEmailAddressMessages"
                    @input="$v.tenant.contact.emailAddress.$touch()"
                    @blur="$v.tenant.contact.emailAddress.$touch()"
                  />
                  <v-select
                    v-model="tenant.selectedProducts"
                    :label="languageStrings.forms.fields.tenant.selectedProducts"
                    :loading="isPending"
                    :items="availableProducts"
                    item-text="name"
                    return-object
                    multiple
                    chips
                  />
                </v-card-text>
                <v-card-actions>
                  <v-spacer />
                  <v-btn text :disabled="isPending" @click="moveNext">
                    {{ languageStrings.forms.buttons.next }}
                  </v-btn>
                </v-card-actions>
              </v-card>
            </template>
          </Promised>
        </v-stepper-content>
        <template v-for="(step, stepIdx) in steps">
          <v-stepper-content :key="'c.' + stepIdx" :step="stepIdx + 2" class="pa-0">
            <v-card flat class="rounded-t-0">
              <v-card-title class="text-h5">
                {{ step.name }}
              </v-card-title>
              <v-card-text>
                <v-select
                  v-model="step.selectedTier"
                  :items="step.tiers"
                  :label="languageStrings.forms.fields.tenant.selectedProductTier"
                  :error-messages="getProductPlanMessages(stepIdx)"
                  item-text="name"
                  return-object
                  @input="$v.tenant.selectedProducts.$each[stepIdx].$touch()"
                  @blur="$v.tenant.selectedProducts.$each[stepIdx].$touch()"
                >
                  <template v-slot:item="{ item, on, attrs }">
                    <v-list-item v-bind="attrs" v-on="on">
                      <v-list-item-icon>
                        <v-icon v-if="item.externalId">
                          mdi-credit-card
                        </v-icon>
                        <v-icon v-else>
                          mdi-credit-card-off-outline
                        </v-icon>
                      </v-list-item-icon>
                      <v-list-item-content>
                        <v-list-item-title>
                          {{ item.name }}
                        </v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </template>
                  <template v-slot:prepend>
                    <v-tooltip v-if="step.selectedTier && step.selectedTier.externalId" top>
                      <template v-slot:activator="{ on, attrs }">
                        <v-icon left v-bind="attrs" v-on="on">
                          mdi-credit-card
                        </v-icon>
                      </template>
                      {{ languageStrings.forms.createTenant.tooltips.paidTier }}
                    </v-tooltip>
                    <v-tooltip v-if="step.selectedTier && ! step.selectedTier.externalId" top>
                      <template v-slot:activator="{ on, attrs }">
                        <v-icon left v-bind="attrs" v-on="on">
                          mdi-credit-card-off-outline
                        </v-icon>
                      </template>
                      {{ languageStrings.forms.createTenant.tooltips.freeTier }}
                    </v-tooltip>
                    <v-icon v-if="! step.selectedTier" left color="error">
                      mdi-alert
                    </v-icon>
                  </template>
                </v-select>
                <component
                  :is="getProductComponent(step)"
                  v-if="step.configurationControlName"
                  :language-strings="languageStrings"
                  :product-configuration.sync="step"
                />
              </v-card-text>
              <v-card-actions>
                <v-btn text @click="movePrevious">
                  {{ languageStrings.forms.buttons.previous }}
                </v-btn>
                <v-spacer />
                <v-btn text @click="moveNext">
                  {{ languageStrings.forms.buttons.next }}
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-stepper-content>
        </template>
        <v-stepper-content :step="steps.length + 2" class="pa-0">
          <v-card flat class="rounded-t-0">
            <v-card-title class="text-h5">
              {{ languageStrings.forms.createTenant.steps.invitations }}
            </v-card-title>
            <v-card-text>
              <v-data-table :items="tenant.invitations" :headers="invitationHeaders">
                <template v-slot:top>
                  <v-toolbar class="control-toolbar mb-2" flat>
                    <v-text-field
                      v-model="invitationToAdd.emailAddress"
                      :label="languageStrings.forms.fields.invitation.emailAddress"
                      :error-messages="invitationEmailAddressMessages"
                      hide-details
                      class="pr-1 invitation-email-address"
                      type="email"
                      @input="$v.invitationToAdd.emailAddress.$touch()"
                      @blur="$v.invitationToAdd.emailAddress.$touch()"
                    />
                    <v-select
                      v-model="invitationToAdd.identityProvider"
                      :items="languageStrings.dropdownValues.identityProvider"
                      :error-messages="invitationIdentityProviderMessages"
                      :label="languageStrings.forms.fields.invitation.identityProvider"
                      item-text="label"
                      item-value="id"
                      hide-details
                      single-line
                      class="pr-1"
                      @input="$v.invitationToAdd.identityProvider.$touch()"
                      @blur="$v.invitationToAdd.identityProvider.$touch()"
                    />

                    <v-toolbar-items>
                      <v-btn depressed :disabled="!invitationToAdd.emailAddress || invitationEmailAddressMessages.length > 0" @click="addInvitation">
                        {{ languageStrings.forms.buttons.add }}
                      </v-btn>
                    </v-toolbar-items>
                  </v-toolbar>
                </template>
                <template v-slot:item.identityProvider="{ item }">
                  <v-icon>
                    {{ languageStrings.dropdownValues.identityProvider.find(idp => idp.id === item.identityProvider).icon }}
                  </v-icon>
                  {{ languageStrings.dropdownValues.identityProvider.find(idp => idp.id === item.identityProvider).label }}
                </template>
                <template v-slot:item._actions="{ item }">
                  <v-btn icon @click="removeInvitation(item)">
                    <v-icon>mdi-delete-forever</v-icon>
                  </v-btn>
                </template>
              </v-data-table>
            </v-card-text>
            <v-card-actions>
              <v-btn text @click="movePrevious">
                {{ languageStrings.forms.buttons.previous }}
              </v-btn>
              <v-spacer />
              <v-btn :disabled="$v.tenant.$invalid" text @click="moveNext">
                {{ languageStrings.forms.buttons.next }}
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-stepper-content>
        <v-stepper-content :step="steps.length + 3" class="pa-0">
          <v-card :loading="creationPromiseResults.isPending" flat class="rounded-t-0">
            <v-card-title class="text-h5">
              {{ languageStrings.forms.createTenant.steps.readyToCreate }}
            </v-card-title>
            <v-card-text>
              <p>
                {{ languageStrings.forms.createTenant.creationMessages.summary.replace('{tenant}', tenant.name) }}
                {{ (tenant.selectedProducts.some(p => p.selectedTier && p.selectedTier.externalId)) ? languageStrings.forms.createTenant.creationMessages.somePaidPlans : languageStrings.forms.createTenant.creationMessages.noPaidPlans }}
              </p>
            </v-card-text>
            <v-card-actions>
              <v-btn text @click="movePrevious">
                {{ languageStrings.forms.buttons.previous }}
              </v-btn>
              <v-spacer />
              <v-btn :disabled="$v.tenant.$invalid" :loading="creationPromiseResults.isPending" color="primary" @click="createTenant">
                {{ languageStrings.auth.signUp }}
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-stepper-content>
      </v-stepper>
    </template>
  </Promised>
</template>

<style>
  .control-toolbar.v-toolbar > .v-toolbar__content { padding-right: 0px; }
  .invitation-email-address.v-input { min-width: 48%; }

  .v-stepper__step--active.v-stepper__step--error.error--text > .v-stepper__label {
      text-shadow: none !important;
  }
  .v-stepper__step.v-stepper__step--error.error--text:hover > .v-stepper__label {
    text-shadow: none !important;
  }
</style>

<script>
import { required, requiredUnless, maxLength, email } from 'vuelidate/lib/validators'
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
    }
  },
  data () {
    return {
      productPromise: this.listProducts(),
      availableProducts: null,
      tenant: {
        name: '',
        contact: {
          name: '',
          emailAddress: '',
          useOwnDetails: true
        },
        selectedProducts: [],
        invitations: [],
        creationPromise: Promise.resolve()
      },
      selectedTab: 1,
      invitationHeaders: [
        { text: this.languageStrings.forms.fields.invitation.emailAddress, value: 'emailAddress', sortable: false },
        { text: this.languageStrings.forms.fields.invitation.identityProvider, value: 'identityProvider', sortable: false },
        { text: '', value: '_actions', sortable: false }
      ],
      invitationToAdd: {
        emailAddress: '',
        identityProvider: 'google.com',
        sendEmail: true,
        loginRedirectUrl: window.location.origin
      }
    }
  },
  validations: {
    tenant: {
      name: {
        required,
        maxLength: maxLength(256)
      },
      contact: {
        name: {
          requiredUnless: requiredUnless(function () { return this.tenant.contact.useOwnDetails }),
          maxLength: maxLength(256)
        },
        emailAddress: {
          requiredUnless: requiredUnless(function () { return this.tenant.contact.useOwnDetails }),
          maxLength: maxLength(256),
          email
        }
      },
      selectedProducts: {
        $each: {
          selectedTier: {
            required
          }
        }
      }
    },
    invitationToAdd: {
      emailAddress: {
        maxLength: maxLength(256),
        email
      },
      identityProvider: {
        required,
        maxLength: maxLength(32)
      }
    }
  },
  computed: {
    steps () {
      return this.tenant.selectedProducts
    },
    tenantNameMessages () {
      const errors = []

      if (!this.$v.tenant.name.required) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.tenant.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    contactNameMessages () {
      const errors = []

      if (!this.$v.tenant.contact.name.requiredUnless) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.tenant.contact.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    contactEmailAddressMessages () {
      const errors = []

      if (!this.$v.tenant.contact.emailAddress.requiredUnless) {
        errors.push(this.languageStrings.validationErrorMessages.noEmailAddress)
      }
      if (!this.$v.tenant.contact.emailAddress.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.emailAddressTooLong)
      }
      if (!this.$v.tenant.contact.emailAddress.email) {
        errors.push(this.languageStrings.validationErrorMessages.invalidEmailAddress)
      }
      return errors
    },
    invitationEmailAddressMessages () {
      const errors = []

      if (!this.$v.invitationToAdd.emailAddress.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.emailAddressTooLong)
      }
      if (!this.$v.invitationToAdd.emailAddress.email) {
        errors.push(this.languageStrings.validationErrorMessages.invalidEmailAddress)
      }
      return errors
    },
    invitationIdentityProviderMessages () {
      const errors = []

      if (!this.$v.invitationToAdd.identityProvider.required) {
        errors.push(this.languageStrings.validationErrorMessages.noIdentityProvider)
      }
      if (!this.$v.invitationToAdd.identityProvider.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.identityProviderTooLong)
      }
      return errors
    }
  },
  methods: {
    ...mapActions({
      listProductsInternal: 'products/listProducts',
      createTenantInternal: 'tenants/createTenant'
    }),
    listProducts () {
      return this.listProductsInternal()
        .then((products) => {
          this.availableProducts = products.data
          this.availableProducts.forEach((p) => {
            if (p.defaultProductConfiguration && p.defaultProductConfiguration.configurationString) {
              p.defaultProductConfiguration.configuration = JSON.parse(p.defaultProductConfiguration.configurationString)
            }
          })

          this.tenant.selectedProducts = this.availableProducts.filter(p => p.name && p.name.toLowerCase() === this.languageStrings.shortProductName.toLowerCase())
          this.tenant.selectedProducts.forEach((p) => {
            const freeTiers = p.tiers.filter(t => !t.externalId)

            if (freeTiers.length > 0) {
              p.selectedTier = freeTiers[0]
            }
          })
        })
    },
    createTenant () {
      this.tenant.creationPromise = this.createTenantInternal({
        name: this.tenant.name,
        contact: this.tenant.contact,
        invitations: this.tenant.invitations,
        subscriptions: this.tenant.selectedProducts.map((sp) => {
          return {
            productTierId: sp.selectedTier.id,
            productConfiguration: JSON.stringify(sp.defaultProductConfiguration.configuration)
          }
        })
      })
    },
    getProductComponent (prod) {
      return require('~/components/product-configuration/' + prod.configurationControlName + '.vue').default
    },
    moveNext () {
      this.selectedTab++
    },
    movePrevious () {
      if (this.selectedTab > 1) {
        this.selectedTab--
      }
    },
    addInvitation () {
      this.tenant.invitations.push({ ...this.invitationToAdd })
      this.invitationToAdd.emailAddress = ''
      this.invitationToAdd.identityProvider = 'google.com'
    },
    removeInvitation (invitation) {
      const pendingInvitationIndex = this.tenant.invitations.indexOf(invitation)

      if (pendingInvitationIndex !== -1) {
        this.tenant.invitations.splice(pendingInvitationIndex, 1)
      }
    },
    getTenantDetailsError () {
      return [
        () => this.tenantNameMessages.length === 0,
        () => this.contactNameMessages.length === 0,
        () => this.contactEmailAddressMessages.length === 0
      ]
    },
    getProductPlanMessages (idx) {
      const errors = []

      if (!this.$v.tenant.selectedProducts.$each[idx].selectedTier.required) {
        errors.push(this.languageStrings.validationErrorMessages.noTier)
      }
      return errors
    },
    getProductDetailsError (idx) {
      return [
        () => this.getProductPlanMessages(idx).length === 0
      ]
    },
    getInvitationDetailsError () {
      return [
        () => this.invitationEmailAddressMessages.length === 0,
        () => this.invitationIdentityProviderMessages.length === 0
      ]
    }
  }
}
</script>
