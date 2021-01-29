<template>
  <v-dialog
    v-if="visible"
    v-model="visible"
    :width="$vuetify.breakpoint.mdAndDown ? '100%' : '60%'"
    no-click-animation
    persistent
    scrollable
  >
    <Promised :promise="dataAccessPromise">
      <template v-slot:combined="{ isPending }">
        <v-card :loading="isPending">
          <v-card-title>
            <h2 class="headline">
              {{ languageStrings.dialogs.createTenantSecurityGroup.title }}
            </h2>
          </v-card-title>

          <v-card-text>
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="securityGroup.name"
                  :label="languageStrings.dialogs.createTenantSecurityGroup.fields.group.name"
                  :error-messages="securityGroupNameMessages"
                  autofocus
                  @input="$v.securityGroup.name.$touch()"
                  @blur="$v.securityGroup.name.$touch()"
                />
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" md="6">
                <h3 class="text--h4">
                  {{ languageStrings.dialogs.createTenantSecurityGroup.fields.group.members }}
                </h3>
                <v-data-table :items="securityGroup.users" :headers="members.headers">
                  <template v-slot:top>
                    <v-toolbar class="control-toolbar mb-2 flex flex-fill flex-row" flat>
                      <TenantMemberDropdown
                        v-model="members.memberToAdd.user"
                        :language-strings="languageStrings"
                        :tenant-id="tenantId"
                        :title="languageStrings.dialogs.createTenantSecurityGroup.fields.member.name"
                        :excluded-members="securityGroup.users.map(mem => mem.user)"
                        class="pr-2 grow"
                      />

                      <v-toolbar-items>
                        <v-btn depressed :disabled="!members.memberToAdd || !members.memberToAdd.user" @click="addMember">
                          {{ languageStrings.forms.buttons.add }}
                        </v-btn>
                      </v-toolbar-items>
                    </v-toolbar>
                  </template>

                  <template v-slot:item.enabled="{ item }">
                    <v-chip v-if="item.enabled" color="success" small @click="disableMember(item)">
                      <v-icon small>
                        mdi-check
                      </v-icon>
                    </v-chip>
                    <v-chip v-else color="error" small @click="enableMember(item)">
                      <v-icon small>
                        mdi-block-helper
                      </v-icon>
                    </v-chip>
                  </template>
                  <template v-slot:item._actions="{ item }">
                    <v-btn icon :disabled="! canRemoveMember(item)" @click="removeMember(item)">
                      <v-icon>mdi-delete-forever</v-icon>
                    </v-btn>
                  </template>
                </v-data-table>
              </v-col>
              <v-col cols="12" md="6">
                <h3 class="text--h4">
                  {{ languageStrings.dialogs.createTenantSecurityGroup.fields.group.accessControlEntries }}
                </h3>
                <v-data-table :items="securityGroup.accessControlEntries" :headers="accessControlEntries.headers">
                  <template v-slot:top>
                    <v-toolbar class="control-toolbar mb-2" flat>
                      <TenantSecurableResourceDropdown
                        v-model="accessControlEntries.entryToAdd.resource"
                        :language-strings="languageStrings"
                        :tenant-id="tenantId"
                        :title="languageStrings.dialogs.createTenantSecurityGroup.fields.accessControlEntry.resourceName"
                        class="pr-2"
                      />

                      <v-select
                        v-model="accessControlEntries.entryToAdd.permission"
                        :items="languageStrings.dropdownValues.permission.filter(p => p.allowAdd)"
                        :label="languageStrings.dialogs.createTenantSecurityGroup.fields.accessControlEntry.permission"
                        item-text="label"
                        item-value="id"
                        hide-details
                        single-line
                        class="pr-2"
                      />

                      <v-toolbar-items>
                        <v-btn depressed :disabled="! canAddPermission" @click="addPermission">
                          {{ languageStrings.forms.buttons.add }}
                        </v-btn>
                      </v-toolbar-items>
                    </v-toolbar>
                  </template>

                  <template v-slot:item.permission="{ item }">
                    {{ languageStrings.dropdownValues.permission.find(v => v.id === item.permission).label }}
                  </template>

                  <template v-slot:item.resource.name="{ item }">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on, attrs }">
                        <v-chip small label v-bind="attrs" v-on="on">
                          <v-icon small left>
                            {{ languageStrings.dropdownValues.securableResourceType.find(t => t.id === item.resource.resourceType).icon }}
                          </v-icon>
                          {{ item.resource.name }}
                        </v-chip>
                      </template>
                      {{ languageStrings.dropdownValues.securableResourceType.find(t => t.id === item.resource.resourceType).label }}
                    </v-tooltip>
                  </template>

                  <template v-slot:item._actions="{ item }">
                    <v-btn icon @click="removePermission(item)">
                      <v-icon>mdi-delete-forever</v-icon>
                    </v-btn>
                  </template>
                </v-data-table>
              </v-col>
            </v-row>
          </v-card-text>

          <v-card-actions>
            <v-spacer />
            <v-btn color="primary" small :disabled="$v.$anyError" @click="update">
              {{ languageStrings.actions.update }}
            </v-btn>
            <v-btn color="error" small @click="hideForm">
              {{ languageStrings.actions.cancel }}
            </v-btn>
          </v-card-actions>
        </v-card>
      </template>
    </Promised>
  </v-dialog>
</template>

<style>
  .control-toolbar.v-toolbar > .v-toolbar__content { padding-right: 0px; }
</style>

<script>
import { required, maxLength } from 'vuelidate/lib/validators'
import { mapActions } from 'vuex'
import { Promised } from 'vue-promised'
import TenantSecurableResourceDropdown from '~/components/TenantSecurableResourceDropdown'
import TenantMemberDropdown from '~/components/TenantMemberDropdown'

export default {
  components: {
    Promised,
    TenantSecurableResourceDropdown,
    TenantMemberDropdown
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    tenantId: {
      type: String,
      required: false,
      default: null
    },
    visible: {
      type: Boolean,
      required: true
    }
  },
  data () {
    return {
      dataAccessPromise: Promise.resolve(),
      securityGroup: { name: null, users: [], accessControlEntries: [] },
      members: {
        headers: [
          { text: this.languageStrings.dialogs.createTenantSecurityGroup.fields.member.name, value: 'user.name', sortable: true, filterable: false },
          { text: this.languageStrings.dialogs.createTenantSecurityGroup.fields.member.enabled, value: 'enabled', sortable: true, filterable: false },
          { text: '', value: '_actions', sortable: false, filterable: false, align: 'right' }
        ],
        memberToAdd: {
          enabled: true,
          user: null
        }
      },
      accessControlEntries: {
        headers: [
          { text: this.languageStrings.dialogs.createTenantSecurityGroup.fields.accessControlEntry.resourceName, value: 'resource.name', sortable: true, filterable: false },
          { text: this.languageStrings.dialogs.createTenantSecurityGroup.fields.accessControlEntry.permission, value: 'permission', sortable: true, filterable: false },
          { text: '', value: '_actions', sortable: false, filterable: false, align: 'right' }
        ],
        entryToAdd: {
          permission: null,
          resource: null
        }
      }
    }
  },
  validations: {
    securityGroup: {
      name: {
        required,
        maxLength: maxLength(256)
      }
    },
    members: {
      memberToAdd: {
        user: {
          required
        }
      }
    },
    accessControlEntries: {
    }
  },
  computed: {
    securityGroupNameMessages () {
      const errors = []

      if (!this.$v.securityGroup.name.required) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.securityGroup.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    canAddPermission () {
      return this.accessControlEntries.entryToAdd.permission !== null && this.accessControlEntries.entryToAdd.resource !== null
    }
  },
  watch: {
  },
  methods: {
    ...mapActions({
      createSecurityGroup: 'groups/addGroup'
    }),
    hideForm () {
      this.$emit('update:visible', false)
    },
    update () {
      const scopedThis = this

      scopedThis.dataAccessPromise = scopedThis.createSecurityGroup({
        ...scopedThis.securityGroup,
        tenant: {
          id: Number.parseInt(scopedThis.tenantId)
        }
      }).then((resp) => {
        if (resp.status === 200) {
          scopedThis.members.memberToAdd = { enabled: true, user: null }

          scopedThis.accessControlEntries.entryToAdd = { permission: null, resource: null }

          scopedThis.$emit('created-security-group', scopedThis.securityGroup)

          scopedThis.securityGroup = { name: null, users: [], accessControlEntries: [] }
          scopedThis.hideForm()
        }
      })
    },
    canDisableMember (mem) {
      return (mem.enabled)
    },
    canRemoveMember (_mem) {
      return true
    },
    enableMember (mem) {
      mem.enabled = true
    },
    disableMember (mem) {
      if (this.canDisableMember(mem)) {
        mem.enabled = false
      }
    },
    addMember () {
      this.securityGroup.users.push(this.members.memberToAdd)

      this.members.memberToAdd = { enabled: true, user: null }
    },
    removeMember (mem) {
      const securityGroupIndex = this.securityGroup.users.indexOf(mem)

      if (securityGroupIndex !== -1) {
        this.securityGroup.users.splice(securityGroupIndex, 1)
      }
    },
    addPermission () {
      this.securityGroup.accessControlEntries.push(this.accessControlEntries.entryToAdd)

      this.accessControlEntries.entryToAdd = { permission: null, resource: null }
    },
    removePermission (perm) {
      // This has the same logic as removeMember, but with ACEs
      const securityGroupIndex = this.securityGroup.accessControlEntries.indexOf(perm)

      if (securityGroupIndex !== -1) {
        this.securityGroup.accessControlEntries.splice(securityGroupIndex, 1)
      }
    }
  }
}
</script>
