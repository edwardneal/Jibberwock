<template>
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
              <h4 class="text--h3">
                {{ languageStrings.pages.tenant_security.sections.roles.headings.members }}
              </h4>
              <v-chip class="my-1" small label>Member 1</v-chip>
              <v-chip class="my-1" small label color="error">Member 2</v-chip>
              <v-chip class="my-1" small label>Member 3</v-chip>
              <v-chip class="my-1" small label>...</v-chip>
            </v-card-text>

            <v-card-text>
              <h4 class="text--h3">
                {{ languageStrings.pages.tenant_security.sections.roles.headings.permissions }}: Read
              </h4>
              <v-tooltip bottom v-for="srType in languageStrings.dropdownValues.securableResourceType" :key="srType.id">
                <template v-slot:activator="{ on, attrs }">
                  <v-chip class="ma-1" small label v-bind="attrs" v-on="on">
                    <v-icon small left>
                      {{ srType.icon }}
                    </v-icon>
                    {{ srType.label }}
                  </v-chip>
                </template>
                {{ srType.label }}
              </v-tooltip>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

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
      securityGroups: {
        populate: () => this.getTenantSecurityGroupsInternal(this.tenantId),
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
  }
}
</script>
