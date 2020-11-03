const baseUrl = process.env.NODE_ENV === 'production' ? 'https://admin.jibberwock.com/api' : 'https://localhost:44349/api'

export const state = () => ({
  urls: {
    getExternalComponents: baseUrl + '/status/externalcomponents'
  }
})

export const actions = {
  getExternalComponents ({ state }) {
    return this.$axios.get(state.urls.getExternalComponents)
  }
}
