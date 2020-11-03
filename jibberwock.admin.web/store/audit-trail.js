const baseUrl = process.env.NODE_ENV === 'production' ? 'https://admin.jibberwock.com/api' : 'https://localhost:44349/api'

export const state = () => ({
  urls: {
    getAuditTrail: baseUrl + '/audittrail'
  }
})

export const actions = {
  getAuditTrail ({ state }, criteria) {
    const queryStrings = []

    if (typeof criteria.startTime !== 'undefined' && criteria.startTime !== null && criteria.startTime !== '') {
      queryStrings.push('start=' + encodeURIComponent(new Date(criteria.startTime).toISOString()))
    }
    if (typeof criteria.endTime !== 'undefined' && criteria.endTime !== null && criteria.endTime !== '') {
      queryStrings.push('end=' + encodeURIComponent(new Date(criteria.endTime).toISOString()))
    }
    if (typeof criteria.performedBy !== 'undefined' && criteria.performedBy !== null && criteria.performedBy !== '') {
      queryStrings.push('performedBy=' + encodeURIComponent(criteria.performedBy))
    }
    if (typeof criteria.eventType !== 'undefined' && criteria.eventType !== null && criteria.eventType !== '') {
      queryStrings.push('type=' + encodeURIComponent(criteria.eventType))
    }
    if (typeof criteria.relatedUserId !== 'undefined' && criteria.relatedUserId !== null && criteria.relatedUserId !== '') {
      queryStrings.push('userId=' + encodeURIComponent(criteria.relatedUserId))
    }
    if (typeof criteria.relatedTenantId !== 'undefined' && criteria.relatedTenantId !== null && criteria.relatedTenantId !== '') {
      queryStrings.push('tenantId=' + encodeURIComponent(criteria.relatedTenantId))
    }

    const auditTrailUrl = state.urls.getAuditTrail +
      (queryStrings.length === 0 ? '' : '?') +
      queryStrings.join('&')

    return this.$axios.get(auditTrailUrl)
  }
}
