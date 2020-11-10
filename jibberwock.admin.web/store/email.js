const baseUrl = process.env.NODE_ENV === 'production' ? 'https://admin.jibberwock.com/api' : 'https://localhost:44349/api'

export const state = () => ({
  urls: {
    getBatches: baseUrl + '/email/batches',
    getEmailRecords: baseUrl + '/email'
  }
})

export const actions = {
  getBatches ({ state }) {
    const getBatchesUrl = state.urls.getBatches

    return this.$axios.get(getBatchesUrl)
  },
  getEmailRecords ({ state }, criteria) {
    const queryStrings = []

    if (typeof criteria.startTime !== 'undefined' && criteria.startTime !== null && criteria.startTime !== '') {
      queryStrings.push('start=' + encodeURIComponent(new Date(criteria.startTime).toISOString()))
    }
    if (typeof criteria.endTime !== 'undefined' && criteria.endTime !== null && criteria.endTime !== '') {
      queryStrings.push('end=' + encodeURIComponent(new Date(criteria.endTime).toISOString()))
    }
    if (typeof criteria.batch !== 'undefined' && criteria.batch !== null && criteria.batch !== '') {
      queryStrings.push('batchId=' + encodeURIComponent(criteria.batch.id))
    }
    if (typeof criteria.batchType !== 'undefined' && criteria.batchType !== null && criteria.batchType !== '') {
      queryStrings.push('batchTypeId=' + encodeURIComponent(criteria.batchType))
    }
    if (typeof criteria.serviceBusMessageId !== 'undefined' && criteria.serviceBusMessageId !== null && criteria.serviceBusMessageId !== '') {
      queryStrings.push('serviceBusMessageId=' + encodeURIComponent(criteria.serviceBusMessageId))
    }
    if (typeof criteria.emailAddress !== 'undefined' && criteria.emailAddress !== null && criteria.emailAddress !== '') {
      queryStrings.push('emailAddress=' + encodeURIComponent(criteria.emailAddress))
    }

    const emailRecordUrl = state.urls.getEmailRecords +
      (queryStrings.length === 0 ? '' : '?') +
      queryStrings.join('&')

    return this.$axios.get(emailRecordUrl)
  }
}
