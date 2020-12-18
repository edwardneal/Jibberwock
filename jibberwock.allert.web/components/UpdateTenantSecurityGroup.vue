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
                CONFIRM VALIDATION. MAKE THE ADD+DELETE BUTTONS WORK. CREATE THE THINGY TO SAVE A GROUP. NO API CALLS NOW - THAT'S FINE FOR NOW.<br />
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
                    <v-chip v-if="item.enabled" color="success" small @click="if (canDisableMember(item)) { item.enabled = false }">
                      <v-icon small>
                        mdi-check
                      </v-icon>
                    </v-chip>
                    <v-chip v-else color="error" small @click="item.enabled = true">
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
                        @input="$v.accessControlEntries.entryToAdd.permission.$touch()"
                        @blur="$v.accessControlEntries.entryToAdd.permission.$touch()"
                      />

                      <v-toolbar-items>
                        <v-btn depressed :disabled="updatedSecurityGroup.wellKnownGroupType !== null || (! (accessControlEntries.entryToAdd.permission !== null && accessControlEntries.entryToAdd.resource !== null))" @click="addPermission">
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
            <v-btn color="primary" small :disabled="$v.$anyError">
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
        pendingMemberRemovals: []
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
      memberToAdd: {
        permission: {
          required
        },
        resource: {
          required
        }
      }
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
    }
  },
  watch: {
    visible () {
      // We might be coming here from a group list, so force the users/accessControlEntries properties to contain values
      this.dataAccessPromise = this.getSingleTenantSecurityGroupInternal({
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
      getSingleTenantSecurityGroupInternal: 'tenants/getSingleTenantSecurityGroup'
    }),
    hideForm () {
      this.$emit('update:visible', false)
    },
    update () {
      this.$emit('update-security-group', this.updatedSecurityGroup)
      this.hideForm()
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
    addMember () {
      alert('add a member to the list!')
    },
    removeMember (_mem) {
      alert('remove a member from the list!')
    },
    addPermission () {
      alert('add a permission to the list!')
    },
    removePermission (_perm) {
      alert('remove a permission from the list!')
    }
  }
}
</script>
