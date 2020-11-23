<template>
  <Promised :promise="refreshPromise">
    <template v-slot:combined="{ isPending, error }">
      <v-card :loading="isPending">
        <v-card-title class="pb-0">
          {{ languageStrings.tenantList.sections.tenants.title }}
        </v-card-title>
        <v-card-text>
          <v-alert v-if="error !== null" dense dismissible outlined type="error">
            <v-tooltip bottom>
              <template v-slot:activator="{ on, attrs }">
                <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToLoadTenantList }}</span>
              </template>
              <span>{{ error.message }}</span>
            </v-tooltip>
          </v-alert>
          <div class="text-right">
            <v-btn color="success" tile class="rounded-t" nuxt to="/create-tenant">
              <v-icon left>
                mdi-plus
              </v-icon>
              {{ languageStrings.actions.create }}
            </v-btn>
          </div>
          <v-list expand outlined subheader class="rounded-b rounded-tl tenant-list">
            <v-list-group v-if="invitations.length > 0" v-model="display.expandInvitations" color="undef-color" prepend-icon="mdi-account-group">
              <template v-slot:activator>
                <v-list-item-title>{{ languageStrings.tenantList.sections.invitations.title }}</v-list-item-title>
              </template>

              <v-list-item v-for="(invite, invIdx) in invitations" :key="invIdx">
                <v-list-item-title>
                  {{ invite.name }}
                </v-list-item-title>
                <v-list-item-action style="min-width: 76px;">
                  <div>
                    <v-tooltip top>
                      <template v-slot:activator="{ on }">
                        <v-btn icon color="success" v-on="on">
                          <v-icon>
                            mdi-check
                          </v-icon>
                        </v-btn>
                      </template>
                      {{ languageStrings.tenantList.sections.invitations.buttons.accept }}
                    </v-tooltip>

                    <v-tooltip top>
                      <template v-slot:activator="{ on }">
                        <v-btn icon color="error" v-on="on">
                          <v-icon small>
                            mdi-block-helper
                          </v-icon>
                        </v-btn>
                      </template>
                      {{ languageStrings.tenantList.sections.invitations.buttons.reject }}
                    </v-tooltip>
                  </div>
                </v-list-item-action>
              </v-list-item>
            </v-list-group>

            <v-list-group v-model="display.expandTenants" color="undef-color" prepend-icon="mdi-account-group">
              <template v-slot:activator>
                <v-list-item-title>{{ languageStrings.tenantList.sections.tenants.title }}</v-list-item-title>
              </template>

              <v-list-item v-if="isPending">
                <v-list-item-icon>
                  <v-progress-circular indeterminate />
                </v-list-item-icon>
                <v-list-item-title>{{ languageStrings.tenantList.sections.tenants.loading }}</v-list-item-title>
              </v-list-item>

              <v-list-item v-if="! isPending && ! error && (! $store.state.tenants.tenants || $store.state.tenants.tenants.length === 0)">
                <v-list-item-title>{{ languageStrings.tenantList.sections.tenants.noTenants }}</v-list-item-title>
              </v-list-item>

              <v-list-item v-if="! isPending && error" color="error">
                <v-list-item-icon>
                  <v-icon large color="error">
                    mdi-alert
                  </v-icon>
                </v-list-item-icon>
                <v-list-item-title class="error--text">
                  {{ languageStrings.validationErrorMessages.unableToLoadTenantList }}
                </v-list-item-title>
              </v-list-item>

              <v-list-item v-for="(ten, tenIdx) in $store.state.tenants.tenants.filter(() => ! isPending)" :key="tenIdx" :to="{ name: 'tenants/id', params: { id: ten.id } }" nuxt>
                <v-list-item-title>
                  {{ ten.name }}
                </v-list-item-title>

                <v-list-item-action>
                  <v-speed-dial
                    v-model="display.tenantOptions[tenIdx].showDetails"
                    direction="left"
                    transition="slide-x-reverse-transition"
                  >
                    <template v-slot:activator>
                      <v-btn icon @click.capture.prevent.stop="display.tenantOptions[tenIdx].showDetails = ! display.tenantOptions[tenIdx].showDetails">
                        <v-icon v-if="display.tenantOptions[tenIdx].showDetails">
                          mdi-close
                        </v-icon>
                        <v-icon v-else>
                          mdi-dots-horizontal
                        </v-icon>
                      </v-btn>
                    </template>

                    <v-tooltip top>
                      <template v-slot:activator="{ on }">
                        <v-btn icon color="success" v-on="on">
                          <v-icon>mdi-cash</v-icon>
                        </v-btn>
                      </template>
                      {{ languageStrings.tenantList.sections.tenants.buttons.subscriptions }}
                    </v-tooltip>

                    <v-tooltip top>
                      <template v-slot:activator="{ on }">
                        <v-btn icon color="orange darken-3" v-on="on">
                          <v-icon>mdi-account-multiple</v-icon>
                        </v-btn>
                      </template>
                      {{ languageStrings.tenantList.sections.tenants.buttons.security }}
                    </v-tooltip>

                    <v-tooltip top>
                      <template v-slot:activator="{ on }">
                        <v-btn icon color="primary" v-on="on">
                          <v-icon>mdi-cloud-alert</v-icon>
                        </v-btn>
                      </template>
                      {{ languageStrings.tenantList.sections.tenants.buttons.allert }}
                    </v-tooltip>
                  </v-speed-dial>
                </v-list-item-action>
              </v-list-item>
            </v-list-group>
          </v-list>
        </v-card-text>
      </v-card>
    </template>
  </Promised>
</template>

<style>
  .tenant-list { max-height: 60vh; overflow-y: auto;}
</style>

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
    }
  },
  data: () => {
    return {
      // todo: When an invitations data model/API is worked out, replace this with a reference to the store
      invitations: [
        { id: 99, name: 'SAMPLE INVITATION 1' },
        { id: 101, name: 'SAMPLE INVITATION 2' }
      ],
      display: {
        expandInvitations: true,
        expandTenants: true,
        tenantOptions: []
      },
      refreshPromise: null
    }
  },
  mounted () {
    this.refreshPromise = Promise.all([this.listTenants()])
      .then(() => {
        this.display.tenantOptions = this.$store.state.tenants.tenants.map(() => ({ showDetails: false }))
      })
  },
  methods: {
    ...mapActions({
      listTenants: 'tenants/listTenants'
    })
  }
}
</script>
