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
                <strong>{{ languageStrings.pages.tenant_security.tabs.roles }}</strong> tabs {{ this.$vuetify.breakpoint.smAndDown ? 'below' : 'on the right' }}.
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
                <v-card>
                  <v-card-text>
                    <v-row>
                      <v-col cols="12">
                        <p>
                          You can use this page to edit a tenant's groups. You can create new groups and add users to them, and control what kind of permissions these groups have.
                          Some groups are known as &quot;well-known groups&quot;. Although you can rename these groups and control which users are members of them, you can't delete
                          them or change their permissions, and they must always have at least one enabled member.
                        </p>
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col cols="12" md="6">
                        <PromisedTable :language-strings="languageStrings" :headers="securityGroups.headers" :populate-function="securityGroups.populate" @selection-changed="updateSelectedSecurityGroup">
                          <template v-slot:toolbar-actions="{ shouldDisable }">
                            <v-toolbar-items>
                              <v-btn :disabled="shouldDisable" class="pl-3" color="success">
                                <v-icon left>
                                  mdi-plus
                                </v-icon>
                                {{ languageStrings.actions.create }}
                              </v-btn>
                            </v-toolbar-items>
                          </template>
                          <template v-slot:item-actions="{ item }">
                            <v-tooltip bottom>
                              <template v-slot:activator="{ on, attrs }">
                                <v-btn icon v-bind="attrs" v-on="on">
                                  <v-icon>
                                    mdi-pencil
                                  </v-icon>
                                </v-btn>
                              </template>
                              {{ languageStrings.actions.edit }}
                            </v-tooltip>
                            <v-tooltip bottom>
                              <template v-slot:activator="{ on, attrs }">
                                <v-btn :disabled="item.wellKnownGroupType !== null" v-bind="attrs" icon color="error" v-on="on">
                                  <v-icon>
                                    mdi-delete-forever
                                  </v-icon>
                                </v-btn>
                              </template>
                              {{ languageStrings.actions.delete }}
                            </v-tooltip>
                          </template>
                        </PromisedTable>
                      </v-col>

                      <v-col cols="12" md="6">
                        <v-card v-if="securityGroups.selectedGroups && securityGroups.selectedGroups.length > 0" flat>
                          <v-toolbar dense>
                            <v-toolbar-items>
                              <v-btn class="pl-3">
                                <v-icon left>
                                  mdi-pencil
                                </v-icon>
                                {{ languageStrings.actions.edit }}
                              </v-btn>
                              <v-btn :disabled="securityGroups.selectedGroups[0].wellKnownGroupType !== null" class="pl-3" color="error">
                                <v-icon left>
                                  mdi-delete-forever
                                </v-icon>
                                {{ languageStrings.actions.delete }}
                              </v-btn>
                            </v-toolbar-items>
                          </v-toolbar>

                          <v-card-title>{{ securityGroups.selectedGroups[0].name }}</v-card-title>
                          <v-card-text v-if="securityGroups.selectedGroups[0].wellKnownGroupType !== null">
                            {{ languageStrings.validationErrorMessages.cannotDeleteWellKnownGroup }}
                          </v-card-text>

                          <v-card-text>
                            <h4 class="text--h3">Members</h4>
                            <v-chip class="my-1" small label>Member 1</v-chip>
                            <v-chip class="my-1" small label color="error">Member 2</v-chip>
                            <v-chip class="my-1" small label>Member 3</v-chip>
                            <v-chip class="my-1" small label>...</v-chip>
                          </v-card-text>

                          <v-card-text>
                            <h4 class="text--h3">Permissions: Read</h4>
                            <v-tooltip bottom>
                              <template v-slot:activator="{ on, attrs }">
                                <v-chip class="my-1" small label v-bind="attrs" v-on="on">
                                  <v-icon small left>
                                    mdi-account-group
                                  </v-icon>
                                  Paid Allert!
                                </v-chip>
                              </template>
                              THIS IS A TENANT
                            </v-tooltip>
                          </v-card-text>
                        </v-card>
                      </v-col>
                    </v-row>
                  </v-card-text>
                </v-card>
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
import PromisedTable from '~/components/PromisedTable.vue'

export default {
  components: {
    PromisedTable
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
      },
      securityGroups: {
        populate: () => this.getTenantSecurityGroupsInternal(this.tenant.id),
        headers: [
          { text: this.languageStrings.pages.tenant_security.sections.roles.headings.roleName, value: 'name', sortable: true, filterable: false },
          { text: '', value: '_actions', sortable: false, filterable: false, align: 'right' }
        ],
        selectedGroups: []
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
