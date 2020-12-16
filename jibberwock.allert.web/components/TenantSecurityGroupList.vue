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
                  <v-btn icon v-bind="attrs" v-on="on" @click="showUpdateForm(item)">
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
          <Promised v-if="securityGroups.selectedGroups && securityGroups.selectedGroups.length > 0" :promise="securityGroups.detailsPromise">
            <template v-slot:combined="{ isPending, error }">
              <v-card :loading="isPending" flat>
                <v-toolbar dense>
                  <v-toolbar-items>
                    <v-btn class="pl-3" :disabled="isPending && ! error" @click="showUpdateForm(securityGroups.selectedGroups[0])">
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

                <v-alert v-if="error !== null" dense dismissible outlined type="error">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on, attrs }">
                      <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToGetSecurityGroup }}</span>
                    </template>
                    <span>{{ error.message }}</span>
                  </v-tooltip>
                </v-alert>

                <v-card-text v-if="securityGroups.selectedGroups[0].users !== null">
                  <h4 class="text--h3">
                    {{ languageStrings.pages.tenant_security.sections.roles.headings.members }}
                  </h4>
                  <template v-for="(mem, memIdx) in securityGroups.selectedGroups[0].users">
                    <v-chip
                      v-if="memIdx < 2"
                      :key="memIdx"
                      :color="! mem.enabled ? 'error' : undefined"
                      class="my-1"
                      small
                      label
                    >
                      {{ mem.user.name }}
                    </v-chip>
                  </template>

                  <v-chip v-if="securityGroups.selectedGroups[0].users.length > 2" class="my-1" small label>
                    ...
                  </v-chip>
                </v-card-text>

                <template v-if="securityGroups.selectedGroups[0].accessControlEntries !== null">
                  <v-card-text
                    v-for="(permGroup, permGroupIdx) in getPermissionGroups(securityGroups.selectedGroups[0].accessControlEntries)"
                    :key="permGroupIdx"
                  >
                    <h4 class="text--h3">
                      {{ languageStrings.pages.tenant_security.sections.roles.headings.permissions }}: {{ permGroup.label }}
                    </h4>
                    <v-tooltip v-for="res in permGroup.resources" :key="permGroupIdx + '.tooltip.' + res.name" bottom>
                      <template v-slot:activator="{ on, attrs }">
                        <v-chip class="ma-1" small label v-bind="attrs" v-on="on">
                          <v-icon small left>
                            {{ res.type.icon }}
                          </v-icon>
                          {{ res.name }}
                        </v-chip>
                      </template>
                      {{ res.type.label }}
                    </v-tooltip>
                  </v-card-text>
                </template>
              </v-card>
            </template>
          </Promised>

          <UpdateTenantSecurityGroup :language-strings="languageStrings" :tenant-id="tenantId" :security-group="securityGroups.updateForm.groupToEdit" :visible.sync="securityGroups.updateForm.visible" />
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
import { mapActions } from 'vuex'
import { Promised } from 'vue-promised'
import PromisedTable from '~/components/PromisedTable.vue'
import UpdateTenantSecurityGroup from '~/components/UpdateTenantSecurityGroup.vue'
import { groupBy } from '~/utility/collections'

export default {
  components: {
    PromisedTable,
    Promised,
    UpdateTenantSecurityGroup
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
        selectedGroups: [],
        detailsPromise: Promise.resolve(),
        updateForm: {
          visible: false,
          groupToEdit: null
        }
      }
    }
  },
  computed: {
  },
  methods: {
    ...mapActions({
      getTenantSecurityGroupsInternal: 'tenants/getTenantSecurityGroups',
      getSingleTenantSecurityGroupInternal: 'tenants/getSingleTenantSecurityGroup'
    }),
    updateSelectedSecurityGroup (value) {
      this.securityGroups.detailsPromise = Promise.resolve(value)
        .then((newSelection) => {
          this.securityGroups.selectedGroups = newSelection

          // When we're selecting a group, we want to get its details (members and permissions.)
          // To keep our references straight, we'll just upgrade the users and accessControlEntries properties.
          return newSelection.length === 0
            ? Promise.resolve()
            : this.getSingleTenantSecurityGroupInternal({
              tenantId: this.tenantId,
              groupId: newSelection[0].id
            })
              .then((data) => {
                this.securityGroups.selectedGroups[0].users = data.data.users
                this.securityGroups.selectedGroups[0].accessControlEntries = data.data.accessControlEntries
              })
        })
    },
    // Converts the full list of access control entries to a grouped list by permission
    getPermissionGroups (accessControlEntries) {
      return groupBy(accessControlEntries, 'permission')
        .map((perm) => {
          const permDetails = this.languageStrings.dropdownValues.permission.find(p => p.id === perm.key)

          return {
            label: permDetails.label,
            resources: perm.values.map(v => ({
              name: v.resource.name,
              type: this.languageStrings.dropdownValues.securableResourceType.find(rt => rt.id === v.resource.resourceType)
            }))
          }
        })
    },
    showUpdateForm (group) {
      this.securityGroups.updateForm.groupToEdit = group
      this.securityGroups.updateForm.visible = true
    }
  }
}
</script>
