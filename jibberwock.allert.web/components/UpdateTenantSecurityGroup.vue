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
      <template v-slot:combined="{ isPending, error }">
        <v-card :loading="isPending">
          <v-card-title>
            <h2 class="headline">
              {{ languageStrings.dialogs.updateTenantSecurityGroup.title }}
            </h2>
          </v-card-title>

          <v-card-text v-if="error === null && updatedSecurityGroup">
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="updatedSecurityGroup.name"
                  :label="languageStrings.dialogs.updateTenantSecurityGroup.fields.group.name"
                  :error-messages="securityGroupNameMessages"
                  autofocus
                  @input="$v.updatedSecurityGroup.name.$touch()"
                  @blur="$v.updatedSecurityGroup.name.$touch()"
                />
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12">
                CHANGE THE GROUP MEMBERSHIP API AND DATA MODEL, INDICATING WHETHER IT'S A PROPER USER OR AN INVITATION
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" md="6">
                <h3 class="text--h4">
                  {{ languageStrings.dialogs.updateTenantSecurityGroup.fields.group.members }}
                </h3>
                MEMBERS - WELL-KNOWN GROUPS MUST ALWAYS HAVE AT LEAST ONE ENABLED MEMBERSHIP (NOT AN INVITATION!)
                <v-data-table :items="updatedSecurityGroup.users" :headers="members.headers">
                  <template v-slot:top>
                    <v-toolbar class="control-toolbar mb-2 flex flex-fill flex-row" flat>
                      <TenantMemberDropdown
                        v-if="updatedSecurityGroup.wellKnownGroupType !== 6"
                        v-model="members.memberToAdd.user"
                        :language-strings="languageStrings"
                        :tenant-id="tenantId"
                        :title="languageStrings.dialogs.updateTenantSecurityGroup.fields.member.name"
                        :excluded-members="updatedSecurityGroup.users.map(mem => mem.user)"
                        class="pr-2 grow"
                      />

                      <v-toolbar-items>
                        <v-btn v-if="updatedSecurityGroup.wellKnownGroupType !== 6" depressed :disabled="!members.memberToAdd || !members.memberToAdd.user" @click="addMember">
                          {{ languageStrings.forms.buttons.add }}
                        </v-btn>
                        <v-btn v-else depressed @click="inviteUser">
                          {{ languageStrings.actions.invite }}
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
                  {{ languageStrings.dialogs.updateTenantSecurityGroup.fields.group.accessControlEntries }}
                </h3>
                <v-data-table :items="updatedSecurityGroup.accessControlEntries" :headers="accessControlEntries.headers">
                  <template v-slot:top>
                    <v-toolbar class="control-toolbar mb-2" flat>
                      <TenantSecurableResourceDropdown
                        v-model="accessControlEntries.entryToAdd.resource"
                        :language-strings="languageStrings"
                        :tenant-id="tenantId"
                        :title="languageStrings.dialogs.updateTenantSecurityGroup.fields.accessControlEntry.resourceName"
                        class="pr-2"
                      />

                      <v-select
                        v-model="accessControlEntries.entryToAdd.permission"
                        :items="languageStrings.dropdownValues.permission.filter(p => p.allowAdd)"
                        :label="languageStrings.dialogs.updateTenantSecurityGroup.fields.accessControlEntry.permission"
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
                    <v-btn icon :disabled="updatedSecurityGroup.wellKnownGroupType !== null" @click="removePermission(item)">
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
    securityGroup: {
      type: Object,
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
      updatedSecurityGroup: null,
      members: {
        headers: [
          { text: this.languageStrings.dialogs.updateTenantSecurityGroup.fields.member.name, value: 'user.name', sortable: true, filterable: false },
          { text: this.languageStrings.dialogs.updateTenantSecurityGroup.fields.member.enabled, value: 'enabled', sortable: true, filterable: false },
          { text: '', value: '_actions', sortable: false, filterable: false, align: 'right' }
        ],
        memberToAdd: {
          enabled: true,
          user: null
        },
        pendingMemberAdditions: [],
        pendingMemberRemovals: [],
        pendingMemberUpdates: []
      },
      accessControlEntries: {
        headers: [
          { text: this.languageStrings.dialogs.updateTenantSecurityGroup.fields.accessControlEntry.resourceName, value: 'resource.name', sortable: true, filterable: false },
          { text: this.languageStrings.dialogs.updateTenantSecurityGroup.fields.accessControlEntry.permission, value: 'permission', sortable: true, filterable: false },
          { text: '', value: '_actions', sortable: false, filterable: false, align: 'right' }
        ],
        entryToAdd: {
          permission: null,
          resource: null
        },
        pendingEntryAdditions: [],
        pendingEntryRemovals: []
      }
    }
  },
  validations: {
    updatedSecurityGroup: {
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

      if (!this.$v.updatedSecurityGroup.name.required) {
        errors.push(this.languageStrings.validationErrorMessages.noName)
      }
      if (!this.$v.updatedSecurityGroup.name.maxLength) {
        errors.push(this.languageStrings.validationErrorMessages.nameTooLong)
      }
      return errors
    },
    canAddPermission () {
      return this.updatedSecurityGroup.wellKnownGroupType === null && this.accessControlEntries.entryToAdd.permission !== null && this.accessControlEntries.entryToAdd.resource !== null
    }
  },
  watch: {
    visible () {
      // We might be coming here from a group list, so force the users/accessControlEntries properties to contain values
      this.dataAccessPromise = this.getSingleSecurityGroup({
        tenantId: this.tenantId,
        groupId: this.securityGroup.id
      })
        .then((data) => {
          this.updatedSecurityGroup = { ...this.securityGroup }

          this.updatedSecurityGroup.users = data.data.users
          this.updatedSecurityGroup.accessControlEntries = data.data.accessControlEntries
        })
    }
  },
  methods: {
    ...mapActions({
      getSingleSecurityGroup: 'groups/getTenantGroup',
      updateSecurityGroup: 'groups/updateGroup',
      addSecurityGroupMember: 'groups/addMember',
      removeSecurityGroupMember: 'groups/removeMember',
      updateSecurityGroupMember: 'groups/updateMember',
      addSecurityGroupPermission: 'groups/addPermission',
      removeSecurityGroupPermission: 'groups/removePermission'
    }),
    hideForm () {
      this.$emit('update:visible', false)
    },
    update () {
      const scopedThis = this

      scopedThis.dataAccessPromise = scopedThis.updateSecurityGroup({
        ...scopedThis.updatedSecurityGroup,
        tenant: {
          id: scopedThis.tenantId
        }
      }).then((resp) => {
        // Handle the removal of various members and access control entries
        return Promise.all(
          scopedThis.accessControlEntries.pendingEntryRemovals.map(e => scopedThis.removeSecurityGroupPermission(e.id))
            .concat(
              scopedThis.members.pendingMemberRemovals.map(gm => scopedThis.removeSecurityGroupMember({
                tenantId: Number.parseInt(scopedThis.tenantId),
                groupId: scopedThis.updatedSecurityGroup.id,
                groupMembershipId: gm.id
              }))
            )
            .concat(
              Promise.resolve(resp)
            )
        )
      }).then((resps) => {
        // Now perform the additions and updates
        return Promise.all(
          scopedThis.accessControlEntries.pendingEntryAdditions.map(e => scopedThis.addSecurityGroupPermission({
            group: {
              id: scopedThis.updatedSecurityGroup.id,
              tenant: {
                id: Number.parseInt(scopedThis.tenantId)
              }
            },
            permission: e.permission,
            resource: e.resource
          }))
            .concat(
              scopedThis.members.pendingMemberAdditions.map(gm => scopedThis.addSecurityGroupMember({
                enabled: gm.enabled,
                group: {
                  id: scopedThis.updatedSecurityGroup.id,
                  tenant: {
                    id: Number.parseInt(scopedThis.tenantId)
                  }
                },
                user: { id: gm.user.id }
              }))
            )
            .concat(
              scopedThis.members.pendingMemberUpdates.map(gm => scopedThis.updateSecurityGroupMember({
                id: gm.id,
                enabled: gm.enabled,
                group: {
                  id: scopedThis.updatedSecurityGroup.id,
                  tenant: {
                    id: Number.parseInt(scopedThis.tenantId)
                  }
                },
                user: { id: gm.user.id }
              }))
            )
            .concat(
              resps.map(r => Promise.resolve(r))
            )
        )
      }).then((resps) => {
        if (!resps.some(resp => resp.status !== 200)) {
          scopedThis.members.pendingMemberAdditions = []
          scopedThis.members.pendingMemberUpdates = []
          scopedThis.members.pendingMemberRemovals = []
          scopedThis.members.memberToAdd = { enabled: true, user: null }

          scopedThis.accessControlEntries.pendingEntryAdditions = []
          scopedThis.accessControlEntries.pendingEntryRemovals = []
          scopedThis.accessControlEntries.entryToAdd = { permission: null, resource: null }

          scopedThis.$emit('update-security-group', resps[0].data)
          scopedThis.hideForm()
        }
      })
    },
    inviteUser () {
      this.$emit('invitation')
      this.hideForm()
    },
    canDisableMember (mem) {
      // For a well-known group, a person's group membership can be disabled only if there are other enabled members
      //  (i.e. there must always be at least one enabled member of each group)
      if (this.updatedSecurityGroup.wellKnownGroupType !== null) {
        // todo: this needs to make sure the users aren't shadow (i.e. invitation) users
        return (mem.enabled && (this.updatedSecurityGroup.users.some(u => u.user.id !== mem.user.id && u.enabled)))
      } else {
        return (mem.enabled)
      }
    },
    canRemoveMember (_mem) {
      // A well-known group must always contain at least one member
      // todo: this needs to make sure the users aren't shadow (i.e. invitation) users
      return (this.updatedSecurityGroup.wellKnownGroupType === null || this.updatedSecurityGroup.users.length > 1)
    },
    enableMember (mem) {
      // No need to push this onto the list of updates if it's already on the list of additions - we'll just create it in the right state to begin with
      if (!this.members.pendingMemberUpdates.includes(mem) && !this.members.pendingMemberAdditions.includes(mem)) {
        this.members.pendingMemberUpdates.push(mem)
      }

      mem.enabled = true
    },
    disableMember (mem) {
      if (this.canDisableMember(mem)) {
        if (!this.members.pendingMemberUpdates.includes(mem) && !this.members.pendingMemberAdditions.includes(mem)) {
          this.members.pendingMemberUpdates.push(mem)
        }

        mem.enabled = false
      }
    },
    addMember () {
      this.updatedSecurityGroup.users.push(this.members.memberToAdd)

      this.members.pendingMemberAdditions.push(this.members.memberToAdd)
      this.members.memberToAdd = { enabled: true, user: null }
    },
    removeMember (mem) {
      // Look for a member in the "pending additions" list. If it's present, we just remove it from that list.
      // Otherwise, create a "pending removal" record
      const memberAdditionIndex = this.members.pendingMemberAdditions.indexOf(mem)
      const memberUpdateIndex = this.members.pendingMemberUpdates.indexOf(mem)
      const securityGroupIndex = this.updatedSecurityGroup.users.indexOf(mem)

      if (memberAdditionIndex === -1) {
        this.members.pendingMemberRemovals.push(mem)
      } else {
        this.members.pendingMemberAdditions.splice(memberAdditionIndex, 1)
      }

      // Removing a member also removes it from the "pending updates" list.
      if (memberUpdateIndex !== -1) {
        this.members.pendingMemberUpdates.splice(memberUpdateIndex, 1)
      }

      if (securityGroupIndex !== -1) {
        this.updatedSecurityGroup.users.splice(securityGroupIndex, 1)
      }
    },
    addPermission () {
      this.updatedSecurityGroup.accessControlEntries.push(this.accessControlEntries.entryToAdd)

      this.accessControlEntries.pendingEntryAdditions.push(this.accessControlEntries.entryToAdd)
      this.accessControlEntries.entryToAdd = { permission: null, resource: null }
    },
    removePermission (perm) {
      // This has the same logic as removeMember, but with ACEs
      const entryAdditionIndex = this.accessControlEntries.pendingEntryAdditions.indexOf(perm)
      const securityGroupIndex = this.updatedSecurityGroup.accessControlEntries.indexOf(perm)

      if (entryAdditionIndex === -1) {
        this.accessControlEntries.pendingEntryRemovals.push(perm)
      } else {
        this.accessControlEntries.pendingEntryAdditions.splice(entryAdditionIndex, 1)
      }

      if (securityGroupIndex !== -1) {
        this.updatedSecurityGroup.accessControlEntries.splice(securityGroupIndex, 1)
      }
    }
  }
}
</script>
