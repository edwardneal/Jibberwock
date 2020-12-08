<template>
  <v-container fill-height fluid class="pa-0 blue darken-3">
    <v-container fill-height fluid class="blue darken-4 rounded-br-pill">
      <v-row>
        <v-spacer />
        <v-col cols="12" md="3">
          <ClientTenantDetails :language-strings="languageStrings" :tenant-id="$route.params.id">
            <template v-slot:product="{ retrievedTenant }">
              <v-list-item :to="{ name: 'tenants/id/allert', params: { id: retrievedTenant.id.toString() } }" nuxt>
                <v-list-item-icon class="mr-4 my-0 align-self-center">
                  <v-icon size="40">
                    mdi-cloud-alert
                  </v-icon>
                </v-list-item-icon>
                <v-list-item-content>
                  <v-list-item-title>
                    {{ languageStrings.tenantList.sections.tenants.buttons.allert }}
                  </v-list-item-title>
                </v-list-item-content>
              </v-list-item>
            </template>

            <template v-slot:explanation>
              <p>Welcome to your tenant's <strong>Security</strong> page. You can use this page for three purposes:</p>
              <ul>
                <li>Invite users to this tenant (or revoke their invitations.)</li>
                <li>Create custom groups and associate them with sets of permissions.</li>
                <li>Change users' group memberships</li>
              </ul>
              <p>
                To do these things, go to the <strong>{{ languageStrings.pages.tenant_security.tabs.invitations }}</strong>,
                <strong>{{ languageStrings.pages.tenant_security.tabs.users }}</strong> and
                <strong>{{ languageStrings.pages.tenant_security.tabs.roles }}</strong> tabs {{ $vuetify.breakpoint.smAndDown ? 'below' : 'on the right' }}.
                You'll find more guidance on the next steps on those tabs.
              </p>
            </template>
          </ClientTenantDetails>
        </v-col>
        <v-col cols="12" md="7">
          <v-card>
            <v-tabs>
              <v-tab>{{ languageStrings.pages.tenant_security.tabs.invitations }}</v-tab>
              <v-tab>{{ languageStrings.pages.tenant_security.tabs.users }}</v-tab>
              <v-tab>{{ languageStrings.pages.tenant_security.tabs.roles }}</v-tab>

              <v-tab-item>
                <v-card>
                  <v-card-text>
                    {{ languageStrings.pages.tenant_security.tabs.invitations }}
                  </v-card-text>
                  <v-card-text>
                    INVITE BUTTON. CREATE INVITATION API CALL.
                    LIST OF INVITATIONS. ABILITY TO REVOKE AN INVITATION.
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item>
                <v-card>
                  <v-card-text>
                    {{ languageStrings.pages.tenant_security.tabs.users }}
                  </v-card-text>
                  <v-card-text>
                    LIST OF USERS. CLICK ON ONE OF THESE, ADD/REMOVE FROM A GROUP OR DISABLE THEIR MEMBERSHIP.
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item>
                <TenantSecurityGroupList :language-strings="languageStrings" :tenant-id="tenant.id" />
              </v-tab-item>
            </v-tabs>
          </v-card>
        </v-col>
        <v-spacer />
      </v-row>
    </v-container>
  </v-container>
</template>

<style>
  .v-card > .v-tabs > .v-tabs-bar > div > .v-tabs-bar__content > .v-tab:nth-child(2)::before {
    border-top-left-radius: 4px;
  }

  .v-card > .v-tabs > .v-tabs-bar > div > .v-tabs-bar__content > .v-tab:nth-child(2) > .v-ripple__container {
    border-top-left-radius: 4px;
  }
</style>

<script>
import { mapActions } from 'vuex'
import TenantSecurityGroupList from '~/components/TenantSecurityGroupList.vue'

export default {
  components: {
    TenantSecurityGroupList
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      tenant: {
        id: this.$route.params.id
      }
    }
  },
  computed: {
  },
  methods: {
    ...mapActions({
      getTenantSecurityGroupsInternal: 'tenants/getTenantSecurityGroups'
    }),
    updateSelectedSecurityGroup (value) {
      this.securityGroups.selectedGroups = value
    }
  },
  meta: {
    auth: { required: true },
    mandatoryParams: ['id']
  },
  head () {
    return {
      title: this.languageStrings.pages.tenant_security.title
    }
  }
}
</script>
