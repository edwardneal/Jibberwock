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
              <p>HI THERE - HOMEPAGE TEXT HERE</p>
            </template>
          </ClientTenantDetails>
        </v-col>
        <v-col cols="12" md="7">
          <v-card>
            <v-tabs>
              <v-tab>{{ languageStrings.pages.tenant.tabs.alertDefinitionGroups }}</v-tab>
              <v-tab>{{ languageStrings.pages.tenant.tabs.alertDefinitions }}</v-tab>
              <v-tab>{{ languageStrings.pages.tenant.tabs.activeAlerts }}</v-tab>

              <v-tab-item>
                <v-card>
                  <v-card-text>
                    {{ languageStrings.pages.tenant.tabs.alertDefinitionGroups }}
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item>
                <v-card>
                  <v-card-text>
                    {{ languageStrings.pages.tenant.tabs.alertDefinitions }}
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item>
                <v-card>
                  <v-card-text>
                    {{ languageStrings.pages.tenant.tabs.activeAlerts }}
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
  .v-card > .v-tabs > .v-tabs-bar > div > .v-tabs-bar__content > .v-tab:nth-child(2)::before{
    border-top-left-radius: 4px;
  }
  .v-card > .v-tabs > .v-tabs-bar > div > .v-tabs-bar__content > .v-tab:nth-child(2) > .v-ripple__container {
    border-top-left-radius: 4px;
  }
</style>

<script>
import ClientTenantDetails from '~/components/ClientTenantDetails.vue'

export default {
  components: {
    ClientTenantDetails
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
        id: this.$route.params.id,
        name: 'TENANT A',
        contact: {
          name: 'my name',
          phoneNumber: '0800 001066',
          emailAddress: 'test@test.com'
        }
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
      title: this.languageStrings.pages.tenant.title
    }
  }
}
</script>
