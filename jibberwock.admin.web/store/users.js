const baseUrl = process.env.NODE_ENV === 'production' ? 'https://admin.jibberwock.com/api' : 'https://localhost:44349/api'

export const state = () => ({
  urls: {
    findUser: baseUrl + '/user?name={searchString}',
    controlAccess: baseUrl + '/user/{id}',
    notify: baseUrl + '/user/{id}/notifications',
    getNotifications: baseUrl + '/user/{id}/notifications',
    updateNotification: baseUrl + '/user/{id}/notifications/{notificationId}',
    getTenants: baseUrl + '/user/{id}/tenants'
  }
})

export const actions = {
  findUsers ({ state }, searchString) {
    const findUrl = state.urls.findUser.replace('{searchString}', encodeURIComponent(searchString))

    return this.$axios.get(findUrl)
  },
  notify ({ state }, { userId, notification }) {
    if (typeof userId === 'undefined' || userId === null || userId === '') {
      userId = 'all'
    }

    const notificationUrl = state.urls.notify.replace('{id}', encodeURIComponent(userId))

    return this.$axios.post(notificationUrl, notification)
  },
  getNotifications ({ state }, userId) {
    const notificationsUrl = state.urls.getNotifications.replace('{id}', encodeURIComponent(userId))

    return this.$axios.get(notificationsUrl)
  },
  updateNotification ({ state }, { userId, notificationId, notification }) {
    if (typeof userId === 'undefined' || userId === null || userId === '') {
      userId = 'all'
    }

    const notificationUrl = state.urls.updateNotification.replace('{id}', encodeURIComponent(userId)).replace('{notificationId}', encodeURIComponent(notificationId))

    return this.$axios.put(notificationUrl, notification)
  },
  controlUserAccess ({ state }, { userId, enabled }) {
    const accessUrl = state.urls.controlAccess.replace('{id}', encodeURIComponent(userId))

    return this.$axios.put(accessUrl, { enabled })
  },
  getTenants ({ state }, userId) {
    const tenantsUrl = state.urls.getTenants.replace('{id}', encodeURIComponent(userId))

    return this.$axios.get(tenantsUrl)
  }
}
