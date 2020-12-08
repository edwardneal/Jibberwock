const baseUrl = process.env.NODE_ENV === 'production' ? 'https://www.jibberwock.com/api' : 'https://localhost:44399/api'

export const state = () => ({
  urls: {
    listTenants: baseUrl + '/tenant',
    createTenant: baseUrl + '/tenant',
    getTenant: baseUrl + '/tenant/{id}',
    getTenantSecurityGroups: baseUrl + '/tenant/{id}/groups'
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
  },
  createTenant ({ state }, tenant) {
    return this.$axios.post(state.urls.createTenant, tenant)
  },
  getTenant ({ state }, tenantId) {
    const getTenantUrl = state.urls.getTenant.replace('{id}', encodeURIComponent(tenantId))

    return this.$axios.get(getTenantUrl)
  },
  getTenantSecurityGroups ({ state }, tenantId) {
    const getTenantSecurityGroupsUrl = state.urls.getTenantSecurityGroups.replace('{id}', encodeURIComponent(tenantId))

    return this.$axios.get(getTenantSecurityGroupsUrl)
  }
}
