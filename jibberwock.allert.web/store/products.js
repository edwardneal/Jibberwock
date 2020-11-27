const baseUrl = process.env.NODE_ENV === 'production' ? 'https://www.jibberwock.com/api' : 'https://localhost:44399/api'

export const state = () => ({
  urls: {
    listProducts: baseUrl + '/product'
  }
})

export const actions = {
  listProducts ({ state }) {
    return this.$axios.get(state.urls.listProducts)
  }
}
