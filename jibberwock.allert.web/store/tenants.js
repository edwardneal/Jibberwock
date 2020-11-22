const baseUrl = process.env.NODE_ENV === 'production' ? 'https://www.jibberwock.com/api' : 'https://localhost:44399/api'

export const state = () => ({
  urls: {
    listTenants: baseUrl + '/tenant'
  },
  tenants: []
})

export const mutations = {
  processTenants (state, tenantList) {
    state.tenants = tenantList
  }
}

export const actions = {
  listTenants ({ state, commit }) {
    return this.$axios.get(state.urls.listTenants)
      .then((res) => {
        commit('processTenants', res.data)

        return res
      })
  }
}
