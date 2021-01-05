const baseUrl = process.env.NODE_ENV === 'production' ? 'https://www.jibberwock.com/api' : 'https://localhost:44399/api'

export const state = () => ({
  urls: {
    getTenantGroup: baseUrl + '/tenant/{id}/groups/{groupId}',
    updateGroup: baseUrl + '/tenant/{id}/groups/{groupId}',
    addMember: baseUrl + '/tenant/{id}/groups/{groupId}/members',
    removeMember: baseUrl + '/tenant/{id}/groups/{groupId}/members',
    updateMember: baseUrl + '/tenant/{id}/groups/{groupId}/members',
    addPermission: baseUrl + '/tenant/{id}/groups/{groupId}/permissions',
    removePermission: baseUrl + '/tenant/{id}/groups/{groupId}/permissions'
  }
})

export const mutations = {
}

export const actions = {
  getTenantGroup ({ state }, { tenantId, groupId }) {
    const getTenantGroupUrl = state.urls.getTenantGroup.replace('{id}', encodeURIComponent(tenantId)).replace('{groupId}', encodeURIComponent(groupId))

    return this.$axios.get(getTenantGroupUrl)
  },
  updateGroup ({ state }, { tenant, name, id }) {
    const updateTenantGroupUrl = state.urls.updateGroup.replace('{id}', encodeURIComponent(tenant.id)).replace('{groupId}', encodeURIComponent(id))

    return this.$axios.put(updateTenantGroupUrl, { name })
  }
}
