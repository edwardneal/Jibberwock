<template>
  <Promised :promise="masterNotificationPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-data-table
        :headers="masterHeaders"
        :loading="isPending"
        :items="!isPending && error === null ? data : undefined"
        :custom-group="groupItems"
        group-by="id"
      >
        <template v-slot:group.header="{ group, items, isOpen, toggle }">
          <td colspan="5" class="text-start" role="button" @click="toggle">
            <v-icon v-if="isOpen">
              mdi-minus
            </v-icon>
            <v-icon v-else>
              mdi-plus
            </v-icon>
            {{ languageStrings.notificationList.groupHeaders.masterTemplate.replace('{group}', group).replace('{count}', items.length).replace('{plural}', items.length === 1 ? '' : 's') }}
          </td>
        </template>
        <template v-slot:item.type="{ item }">
          {{ languageStrings.notificationList.notificationTypes[item.type] }}
        </template>
        <template v-slot:item.startDate="{ item }">
          {{ typeof item.startDate !== 'undefined' && item.startDate !== null ? new Date(item.startDate).toLocaleString() : '-' }}
        </template>
        <template v-slot:item.endDate="{ item }">
          {{ typeof item.endDate !== 'undefined' && item.endDate !== null ? new Date(item.endDate).toLocaleString() : '-' }}
        </template>
        <template v-slot:item._actions="{ item }">
          <v-btn icon color="primary" @click="$emit('notification-selected', item, updateMasterNotificationPromise)">
            <v-icon>
              mdi-information
            </v-icon>
          </v-btn>
        </template>
      </v-data-table>
    </template>
  </Promised>
</template>

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
    },
    users: {
      type: Array,
      required: false,
      default: null
    },
    tenants: {
      type: Array,
      required: false,
      default: null
    }
  },
  data () {
    return {
      userNotificationHeaders: [
        {
          text: this.languageStrings.notificationList.headers.subject,
          value: 'subject',
          sortable: true,
          filterable: true
        },
        {
          text: 'ID',
          value: 'id',
          sortable: true,
          filterable: true
        },
        {
          text: this.languageStrings.notificationList.headers.startDate,
          value: 'startDate',
          sortable: true,
          filterable: true
        },
        {
          text: this.languageStrings.notificationList.headers.endDate,
          value: 'endDate',
          sortable: true,
          filterable: true
        },
        {
          text: this.languageStrings.notificationList.headers.type,
          value: 'type',
          sortable: true,
          filterable: true
        },
        {
          text: '',
          value: '_actions'
        }
      ],
      masterNotificationPromise: null
    }
  },
  computed: {
    masterHeaders () {
      return this.userNotificationHeaders
    }
  },
  watch: {
    users () { this.updateMasterNotificationPromise() },
    tenants () { this.updateMasterNotificationPromise() }
  },
  mounted () {
    this.$nextTick(this.updateMasterNotificationPromise)
  },
  methods: {
    ...mapActions({
      getUserNotificationsInternal: 'users/getNotifications'
    }),
    updateMasterNotificationPromise () {
      let allPromiseList = []

      if (typeof this.tenants !== 'undefined' && this.tenants !== null) {
        //
      } else {
        allPromiseList.push(this.getUserNotificationsInternal('all'))

        if (typeof this.users !== 'undefined' && this.users !== null) {
          allPromiseList = allPromiseList.concat(
            this.users.map((item) => { return this.getUserNotificationsInternal(item.id) })
          )
        }
      }
      this.masterNotificationPromise = Promise.all(allPromiseList).then((values) => {
        return values.flatMap((v) => { return v.data })
      })
    },
    groupItems (items) {
      const lookupUsers = {}
      const lookupTenants = {}
      let currUsr = null
      let currTen = null
      let currItem = null
      let currItemKey = ''
      let currGroupRecord = {}
      const returnedGroups = []

      // First, build a workable lookup table for users and tenants
      if (typeof this.users !== 'undefined' && this.users !== null) {
        for (let i = 0; i < this.users.length; i++) {
          currUsr = this.users[i]

          if (typeof lookupUsers[currUsr.id] === 'undefined') {
            lookupUsers[currUsr.id] = currUsr
          }
        }
      }
      if (typeof this.tenants !== 'undefined' && this.tenants !== null) {
        for (let i = 0; i < this.tenants.length; i++) {
          currTen = this.tenants[i]

          if (typeof lookupTenants[currTen.id] === 'undefined') {
            lookupTenants[currTen.id] = currTen
          }
        }
      }

      for (let i = 0; i < items.length; i++) {
        currItem = items[i]

        // Build a friendly group text, ready for display. Simultaneously, try to find a matching group record
        currGroupRecord = null
        if (currItem.targetTenant === null && currItem.targetUser === null) {
          currItemKey = this.languageStrings.notificationList.groupHeaders.allUsers
          currGroupRecord = returnedGroups.find(g => g.groupedTenant === null && g.groupedUser === null)
        } else if (currItem.targetTenant === null && currItem.targetUser !== null) {
          currItemKey = this.languageStrings.notificationList.groupHeaders.specificUser + lookupUsers[currItem.targetUser.id].name
          currGroupRecord = returnedGroups.find(g => g.groupedTenant === null && g.groupedUser !== null && g.groupedUser.id === currItem.targetUser.id)
        } else if (currItem.targetTenant !== null) {
          currItemKey = this.languageStrings.notificationList.groupHeaders.specificTenant + lookupTenants[currItem.targetTenant.id].name
          currGroupRecord = returnedGroups.find(g => g.groupedTenant !== null && g.groupedTenant.id === currItem.targetTenant.id)
        }

        if (typeof currGroupRecord === 'undefined' || currGroupRecord === null) {
          currGroupRecord = { name: currItemKey, items: [], groupedUser: currItem.targetUser, groupedTenant: currItem.targetTenant }

          returnedGroups.push(currGroupRecord)
        }

        currGroupRecord.items.push(currItem)
      }

      return returnedGroups
    }
  }
}
</script>
