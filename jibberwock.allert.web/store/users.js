import { groupBy } from '~/utility/collections'

const baseUrl = process.env.NODE_ENV === 'production' ? 'https://www.jibberwock.com/api' : 'https://localhost:44399/api'

export const state = () => ({
  urls: {
    getSelf: baseUrl + '/user/me',
    getInvitations: baseUrl + '/user/me/invitations',
    controlInvitation: baseUrl + '/user/me/invitations/{invitationId}',
    getNotifications: baseUrl + '/user/me/notifications',
    dismissNotification: baseUrl + '/user/me/notifications/{notificationId}'
  },
  notifications: [],
  notificationsByPriority: []
})

export const mutations = {
  processNotifications (state, notificationList) {
    state.notifications = notificationList

    const groupedNotifications = groupBy(state.notifications, i => i.priority.id)
      .map((grp) => {
        grp.values.sort((a, b) => {
          const aDate = new Date(a.startDate ? a.startDate : a.creationDate)
          const bDate = new Date(b.startDate ? b.startDate : b.creationDate)

          return aDate < bDate
            ? -1
            : aDate > bDate
              ? 1
              : 0
        })

        return {
          priorityId: grp.key,
          notifications: grp.values
        }
      })

    groupedNotifications.sort((a, b) => {
      return a.priorityId > b.priorityId
        ? -1
        : a.priorityId < b.priorityId
          ? 1
          : 0
    })

    state.notificationsByPriority = groupedNotifications
  },
  removeNotification (state, notification) {
    const notificationGroup = state.notificationsByPriority.find(np => np.priorityId === notification.priority.id)
    const notificationPositionInGroup = notificationGroup.notifications.indexOf(notification)

    const masterNotificationPosition = state.notifications.indexOf(notification)

    // If this notification is the only thing in the group, we can just remove the entire group
    if (notificationGroup.notifications.length === 1) {
      const notificationGroupPosition = state.notificationsByPriority.indexOf(notificationGroup)

      if (notificationGroupPosition !== -1) {
        state.notificationsByPriority.splice(notificationGroupPosition, 1)
      }
    } else {
      notificationGroup.notifications.splice(notificationPositionInGroup, 1)
    }

    if (masterNotificationPosition !== -1) {
      state.notifications.splice(masterNotificationPosition, 1)
    }
  }
}

export const actions = {
  getSelf ({ state }) {
    return this.$axios.get(state.urls.getSelf)
  },
  getNotifications ({ state }) {
    return this.$axios.get(state.urls.getNotifications)
  },
  async populateNotifications ({ commit, dispatch }) {
    await dispatch('getNotifications')
      .then((res) => {
        commit('processNotifications', res.data)
      })
  },
  dismissNotification ({ state }, notificationId) {
    const deleteNotificationUrl = state.urls.dismissNotification.replace('{notificationId}', encodeURIComponent(notificationId))

    return this.$axios.delete(deleteNotificationUrl)
  }
}
