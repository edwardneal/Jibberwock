const baseUrl = process.env.NODE_ENV === 'production' ? 'https://www.jibberwock.com/api' : 'https://localhost:44399/api'

export const state = () => ({
  urls: {
    listTenants: baseUrl + '/tenant',
    createTenant: baseUrl + '/tenant',
    getTenant: baseUrl + '/tenant/{id}',
    getTenantSecurityGroups: baseUrl + '/tenant/{id}/groups',
    getSingleTenantSecurityGroup: baseUrl + '/tenant/{id}/groups/{groupId}',
    getTenantSecurableResources: baseUrl + '/tenant/{id}/securableresources?filter={filter}',
    getTenantMembers: baseUrl + '/tenant/{id}/members'
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
  },
  getSingleTenantSecurityGroup ({ state }, { tenantId, groupId }) {
    const getSingleTenantSecurityGroupUrl = state.urls.getSingleTenantSecurityGroup.replace('{id}', encodeURIComponent(tenantId)).replace('{groupId}', encodeURIComponent(groupId))

    return this.$axios.get(getSingleTenantSecurityGroupUrl)
  },
  getTenantSecurableResources ({ state }, { tenantId, filter }) {
    const getTenantSecurableResourcesUrl = state.urls.getTenantSecurableResources.replace('{id}', encodeURIComponent(tenantId)).replace('{filter}', encodeURIComponent(filter))

    return this.$axios.get(getTenantSecurableResourcesUrl)
  },
  getTenantMembers ({ state }, tenantId) {
    const getTenantMembersUrl = state.urls.getTenantMembers.replace('{id}', encodeURIComponent(tenantId))

    return this.$axios.get(getTenantMembersUrl)
  }
}
