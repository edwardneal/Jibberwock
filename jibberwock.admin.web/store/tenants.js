const baseUrl = process.env.NODE_ENV === 'production' ? 'https://admin.jibberwock.com/api' : 'https://localhost:44349/api'

export const state = () => ({
  urls: {
    findTenant: baseUrl + '/tenant?name={searchString}',
    notify: baseUrl + '/tenant/{id}/notifications',
    getNotifications: baseUrl + '/tenant/{id}/notifications',
    updateNotification: baseUrl + '/tenant/{id}/notifications/{notificationId}'
  }
})

export const actions = {
  findTenants ({ state }, searchString) {
    const findUrl = state.urls.findTenant.replace('{searchString}', encodeURIComponent(searchString))

    return this.$axios.get(findUrl)
  },
  notify ({ state }, { tenantId, notification }) {
    const notificationUrl = state.urls.notify.replace('{id}', encodeURIComponent(tenantId))

    return this.$axios.post(notificationUrl, notification)
  },
  getNotifications ({ state }, tenantId) {
    const notificationsUrl = state.urls.getNotifications.replace('{id}', encodeURIComponent(tenantId))

    return this.$axios.get(notificationsUrl)
  },
  updateNotification ({ state }, { tenantId, notificationId, notification }) {
    const notificationUrl = state.urls.updateNotification.replace('{id}', encodeURIComponent(tenantId)).replace('{notificationId}', encodeURIComponent(notificationId))

    return this.$axios.put(notificationUrl, notification)
  }
}
