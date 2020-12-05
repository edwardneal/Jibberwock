<template>
  <Promised :promise="internalPromise">
    <template v-slot:combined="{ isPending, error }">
      <v-card :loading="isPending">
        <v-alert v-if="error !== null" dense dismissible outlined type="error">
          <v-tooltip bottom>
            <template v-slot:activator="{ on, attrs }">
              <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToGetTenant }}</span>
            </template>
            <span>{{ error.message }}</span>
          </v-tooltip>
        </v-alert>

        <v-card-title v-if="! error && ! isPending" class="pb-0">
          {{ tenant.name }}
        </v-card-title>

        <v-list v-if="! error && ! isPending">
          <v-list-item>
            <v-list-item-icon class="mr-4 align-self-center">
              <v-avatar rounded size="40" color="primary">
                <span class="white--text headline">{{ tenant.billingContact.fullName.substring(0, 1).toLocaleUpperCase() }}</span>
              </v-avatar>
            </v-list-item-icon>

            <v-list-item-content>
              <v-list-item-title>{{ tenant.billingContact.fullName }}</v-list-item-title>
              <v-list-item-subtitle v-if="tenant.billingContact.telephoneNumber">
                <v-icon left>
                  mdi-phone
                </v-icon>
                <a :href="'tel:' + tenant.billingContact.telephoneNumber">{{ tenant.billingContact.telephoneNumber }}</a>
              </v-list-item-subtitle>
              <v-list-item-subtitle v-if="tenant.billingContact.emailAddress">
                <v-icon left>
                  mdi-email
                </v-icon>
                <a :href="'mailto:' + tenant.billingContact.emailAddress">{{ tenant.billingContact.emailAddress }}</a>
              </v-list-item-subtitle>
            </v-list-item-content>
          </v-list-item>

          <slot name="product" :retrieved-tenant="tenant" />

          <v-list-item :to="{ name: 'tenants/id/security', params: { id: tenant.id } }" nuxt>
            <v-list-item-icon class="mr-4 my-0 align-self-center">
              <v-icon size="40">
                mdi-account-multiple
              </v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title>
                {{ languageStrings.tenantList.sections.tenants.buttons.security }}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>

          <v-list-item :to="{ name: 'tenants/id/subscriptions', params: { id: tenant.id } }" nuxt>
            <v-list-item-icon class="mr-4 my-0 align-self-center">
              <v-icon size="40">
                mdi-cash
              </v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title>
                {{ languageStrings.tenantList.sections.tenants.buttons.subscriptions }}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>

          <v-list-item :to="{ name: 'tenants/id/api-keys', params: { id: tenant.id } }" nuxt>
            <v-list-item-icon class="mr-4 my-0 align-self-center">
              <v-icon size="40">
                mdi-api
              </v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title>
                {{ languageStrings.tenantList.sections.tenants.buttons.apiKeys }}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list>

        <v-card-text>
          <slot name="explanation" />
        </v-card-text>
      </v-card>
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
    tenantId: {
      type: [String, Number],
      required: true
    }
  },
  data () {
    return {
      internalPromise: Promise.resolve(),
      tenant: null
    }
  },
  computed: {
  },
  watch: {
    tenantId () {
      this.internalPromise = this.getTenant(this.tenantId)
    }
  },
  mounted () {
    this.internalPromise = this.getTenant(this.tenantId)
  },
  methods: {
    ...mapActions({
      getTenantInternal: 'tenants/getTenant'
    }),
    getTenant (id) {
      return this.getTenantInternal(id)
        .then((resp) => {
          this.tenant = resp.data
          this.$emit('tenant-retrieved', this.tenant)

          return resp
        })
    }
  }
}
</script>
