const baseUrl = process.env.NODE_ENV === 'production' ? 'https://www.jibberwock.com/api' : 'https://localhost:44399/api'

export const state = () => ({
  urls: {
    getTenantGroup: baseUrl + '/tenant/{id}/groups/{groupId}',
    updateGroup: baseUrl + '/tenant/{id}/groups/{groupId}',
    addMember: baseUrl + '/tenant/{id}/groups/{groupId}/members',
    removeMember: baseUrl + '/tenant/{id}/groups/{groupId}/members/{groupMembershipId}',
    updateMember: baseUrl + '/tenant/{id}/groups/{groupId}/members/{groupMembershipId}',
    addPermission: baseUrl + '/tenant/{id}/groups/{groupId}/permissions',
    removePermission: baseUrl + '/tenant/{id}/groups/{groupId}/permissions/{accessControlEntryId}'
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
  },
  addMember ({ state }, membership) {
    const addMemberUrl = state.urls.addMember.replace('{id}', encodeURIComponent(membership.group.tenant.id)).replace('{groupId}', encodeURIComponent(membership.group.id))

    return this.$axios.post(addMemberUrl, membership)
  },
  removeMember ({ state }, { tenantId, groupId, groupMembershipId }) {
    const removeMemberUrl = state.urls.removeMember.replace('{id}', encodeURIComponent(tenantId))
      .replace('{groupId}', encodeURIComponent(groupId))
      .replace('{groupMembershipId}', encodeURIComponent(groupMembershipId))

    return this.$axios.delete(removeMemberUrl)
  },
  updateMember ({ state }, membership) {
    const updateMemberUrl = state.urls.updateMember.replace('{id}', encodeURIComponent(membership.group.tenant.id))
      .replace('{groupId}', encodeURIComponent(membership.group.id))
      .replace('{groupMembershipId}', encodeURIComponent(membership.id))

    return this.$axios.put(updateMemberUrl, membership)
  },
  addPermission ({ state }, permission) {
    const addPermissionUrl = state.urls.addPermission.replace('{id}', encodeURIComponent(permission.group.tenant.id)).replace('{groupId}', encodeURIComponent(permission.group.id))

    return this.$axios.post(addPermissionUrl, permission)
  },
  removePermission ({ state }, { tenantId, groupId, accessControlEntryId }) {
    const removePermissionUrl = state.urls.removePermission.replace('{id}', encodeURIComponent(tenantId))
      .replace('{groupId}', encodeURIComponent(groupId))
      .replace('{accessControlEntryId}', encodeURIComponent(accessControlEntryId))

    return this.$axios.delete(removePermissionUrl)
  }
}
