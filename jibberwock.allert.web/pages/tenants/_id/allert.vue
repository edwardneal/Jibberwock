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
              <p>HI THERE - ALLERT TEXT HERE</p>
            </template>
          </ClientTenantDetails>
        </v-col>
        <v-col cols="12" md="7">
          <v-card>
            <v-tabs>
              <v-tab>{{ languageStrings.pages.tenant_security.tabs.users }}</v-tab>
              <v-tab>{{ languageStrings.pages.tenant_security.tabs.invitations }}</v-tab>
              <v-tab>{{ languageStrings.pages.tenant_security.tabs.roles }}</v-tab>

              <v-tab-item>
                <v-card>
                  <v-card-text>
                    {{ languageStrings.pages.tenant_security.tabs.users }}
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item>
                <v-card>
                  <v-card-text>
                    {{ languageStrings.pages.tenant_security.tabs.invitations }}
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item>
                <v-card>
                  <v-card-text>
                    {{ languageStrings.pages.tenant_security.tabs.roles }}
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

<script>
export default {
  components: {
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
