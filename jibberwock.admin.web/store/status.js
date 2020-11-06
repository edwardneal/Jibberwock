const baseUrl = process.env.NODE_ENV === 'production' ? 'https://admin.jibberwock.com/api' : 'https://localhost:44349/api'

export const state = () => ({
  urls: {
    getExternalComponents: baseUrl + '/status/externalcomponents',
    getExceptions: baseUrl + '/status/exceptions',
    getFailedRequests: baseUrl + '/status/requests'
  }
})

export const actions = {
  getExternalComponents ({ state }) {
    return this.$axios.get(state.urls.getExternalComponents)
  },
  getExceptions ({ state }) {
    return this.$axios.get(state.urls.getExceptions)
  },
  getFailedRequests ({ state }) {
    return this.$axios.get(state.urls.getFailedRequests)
  }
}
