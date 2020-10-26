const baseUrl = process.env.NODE_ENV === 'production' ? 'https://admin.jibberwock.com/api' : 'https://localhost:44349/api'

export const state = () => ({
  urls: {
    findUser: baseUrl + '/user?name={searchString}',
    getNotifications: baseUrl + '/user/{id}/notifications'
  }
})

export const actions = {
  findUsers ({ state }, searchString) {
    const findUrl = state.urls.findUser.replace('{searchString}', encodeURIComponent(searchString))

    return this.$axios.get(findUrl)
  },
  getNotifications ({ state }, userId) {
    const notificationsUrl = state.urls.getNotifications.replace('{id}', encodeURIComponent(userId))

    return this.$axios.get(notificationsUrl)
  }
}
