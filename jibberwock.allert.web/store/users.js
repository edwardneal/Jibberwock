const baseUrl = process.env.NODE_ENV === 'production' ? 'https://www.jibberwock.com/api' : 'https://localhost:44399/api'

export const state = () => ({
  urls: {
    getSelf: baseUrl + '/user/me',
    getInvitations: baseUrl + '/user/me/invitations',
    controlInvitation: baseUrl + '/user/me/invitations/{invitationId}',
    getNotifications: baseUrl + '/user/me/notifications',
    dismissNotification: baseUrl + '/user/me/notifications/{notificationId}'
  }
})

export const actions = {
  getSelf ({ state }) {
    return this.$axios.get(state.urls.getSelf)
  },
  getNotifications ({ state }) {
    return this.$axios.get(state.urls.getNotifications)
  }
}
