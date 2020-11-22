<template>
  <v-container class="pa-0">
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

        <v-list-item v-if="! tenants || tenants.length === 0">
          <v-list-item-title>{{ languageStrings.tenantList.sections.tenants.noTenants }}</v-list-item-title>
        </v-list-item>

        <v-list-item v-for="(ten, tenIdx) in tenants" :key="tenIdx">
          <v-list-item-title>
            {{ ten.name }}
          </v-list-item-title>

          <v-list-item-action>
            <v-speed-dial
              v-model="ten.showDetails"
              direction="left"
              transition="slide-x-reverse-transition"
            >
              <template v-slot:activator>
                <v-btn v-model="ten.showDetails" icon>
                  <v-icon v-if="ten.showDetails">
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
  </v-container>
</template>

<style>
  .tenant-list { max-height: 60vh; overflow-y: auto;}
</style>

<script>
export default {
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data: () => {
    return {
      invitations: [
        { id: 99, name: 'SAMPLE INVITATION 1' },
        { id: 101, name: 'SAMPLE INVITATION 2' }
      ],
      tenants: [
        { id: 1, name: 'TENANT 1' },
        { id: 2, name: 'TENANT 2' },
        { id: 3, name: 'TENANT 3' },
        { id: 4, name: 'TENANT 4' },
        { id: 5, name: 'TENANT 5' },
        { id: 6, name: 'TENANT 6' },
        { id: 7, name: 'TENANT 7' },
        { id: 8, name: 'TENANT 8' }
      ],
      display: {
        expandInvitations: true,
        expandTenants: true
      }
    }
  }
}
</script>
