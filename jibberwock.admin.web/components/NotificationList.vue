<template>
  <Promised :promise="masterNotificationPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-alert v-if="error !== null" dense dismissible outlined type="error">
        <v-tooltip bottom>
          <template v-slot:activator="{ on, attrs }">
            <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToListNotifications }}</span>
          </template>
          <span>{{ error.message }}</span>
        </v-tooltip>
      </v-alert>
      <v-data-table
        :headers="masterHeaders"
        :loading="isPending"
        :items="!isPending && error === null ? data : undefined"
        :custom-group="groupItems"
        :custom-sort="sortItems"
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
          {{ languageStrings.notificationList.notificationTypes.find(tp => tp.id === item.type).label }}
        </template>
        <template v-slot:item.startDate="{ item }">
          {{ typeof item.startDate !== 'undefined' && item.startDate !== null ? new Date(item.startDate).toLocaleDateString() : '-' }}
        </template>
        <template v-slot:item.endDate="{ item }">
          {{ typeof item.endDate !== 'undefined' && item.endDate !== null ? new Date(item.endDate).toLocaleDateString() : '-' }}
        </template>
        <template v-slot:item._actions="{ item }">
          <v-btn icon color="primary" @click.stop="$emit('notification-selected', item)">
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
      getUserNotificationsInternal: 'users/getNotifications',
      getTenantNotificationsInternal: 'tenants/getNotifications'
    }),
    updateMasterNotificationPromise () {
      const lookupUsers = {}
      const lookupTenants = {}
      let currUsr = null
      let currTen = null
      let currNot = null

      let allPromiseList = []

      if (typeof this.tenants !== 'undefined' && this.tenants !== null) {
        // Turn the list of selected tenants into a dictionary, keyed by ID
        for (let i = 0; i < this.tenants.length; i++) {
          currTen = this.tenants[i]

          if (typeof lookupTenants[currTen.id] === 'undefined') {
            lookupTenants[currTen.id] = currTen
          }
        }

        allPromiseList = allPromiseList.concat(
          this.tenants.map((item) => { return this.getTenantNotificationsInternal(item.id) })
        )
      } else {
        allPromiseList.push(this.getUserNotificationsInternal('all'))

        if (typeof this.users !== 'undefined' && this.users !== null) {
          // Turn the list of selected users into a dictionary, keyed by ID
          for (let i = 0; i < this.users.length; i++) {
            currUsr = this.users[i]

            if (typeof lookupUsers[currUsr.id] === 'undefined') {
              lookupUsers[currUsr.id] = currUsr
            }
          }

          allPromiseList = allPromiseList.concat(
            this.users.map((item) => { return this.getUserNotificationsInternal(item.id) })
          )
        }
      }
      this.masterNotificationPromise = Promise.all(allPromiseList).then((values) => {
        return values.flatMap((v) => { return v.data })
      }).then((notifications) => {
        // Start rattling through the targetTenant and targetUsers, setting their names
        for (let i = 0; i < notifications.length; i++) {
          currNot = notifications[i]

          if (currNot.targetTenant !== null && typeof lookupTenants[currNot.targetTenant.id] !== 'undefined') {
            currNot.targetTenant.name = lookupTenants[currNot.targetTenant.id].name
          }
          if (currNot.targetUser !== null && typeof lookupUsers[currNot.targetUser.id] !== 'undefined') {
            currNot.targetUser.name = lookupUsers[currNot.targetUser.id].name
          }
        }

        return notifications
      })
    },
    groupItems (items) {
      let currItem = null
      let currItemKey = ''
      let currGroupRecord = {}
      const returnedGroups = []

      for (let i = 0; i < items.length; i++) {
        currItem = items[i]

        // Build a friendly group text, ready for display. Simultaneously, try to find a matching group record
        currGroupRecord = null
        if (currItem.targetTenant === null && currItem.targetUser === null) {
          currItemKey = this.languageStrings.notificationList.groupHeaders.allUsers
          currGroupRecord = returnedGroups.find(g => g.groupedTenant === null && g.groupedUser === null)
        } else if (currItem.targetTenant === null && currItem.targetUser !== null) {
          currItemKey = this.languageStrings.notificationList.groupHeaders.specificUser + currItem.targetUser.name
          currGroupRecord = returnedGroups.find(g => g.groupedTenant === null && g.groupedUser !== null && g.groupedUser.id === currItem.targetUser.id)
        } else if (currItem.targetTenant !== null) {
          currItemKey = this.languageStrings.notificationList.groupHeaders.specificTenant + currItem.targetTenant.name
          currGroupRecord = returnedGroups.find(g => g.groupedTenant !== null && g.groupedTenant.id === currItem.targetTenant.id)
        }

        if (typeof currGroupRecord === 'undefined' || currGroupRecord === null) {
          currGroupRecord = { name: currItemKey, items: [], groupedUser: currItem.targetUser, groupedTenant: currItem.targetTenant }

          returnedGroups.push(currGroupRecord)
        }

        currGroupRecord.items.push(currItem)
      }

      return returnedGroups
    },
    sortItems (items, indexes, isDescendingValues) {
      if (indexes.length === 1) {
        return items
      }

      items.sort((a, b) => {
        const index = indexes[1]
        const isDescending = isDescendingValues[1]
        let firstItem = a[index]
        let secondItem = b[index]

        if (index === 'startDate' || index === 'endDate') {
          if (typeof firstItem !== 'undefined' && firstItem !== null) {
            firstItem = new Date(firstItem)
          }
          if (typeof secondItem !== 'undefined' && secondItem !== null) {
            secondItem = new Date(secondItem)
          }
        }

        if (isDescending) {
          return secondItem < firstItem ? -1 : 1
        } else {
          return firstItem < secondItem ? -1 : 1
        }
      })

      return items
    }
  }
}
</script>
