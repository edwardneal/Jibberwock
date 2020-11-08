const baseUrl = process.env.NODE_ENV === 'production' ? 'https://admin.jibberwock.com/api' : 'https://localhost:44349/api'

export const state = () => ({
  urls: {
    list: baseUrl + '/product?includeHiddenProducts=true',
    listTiers: baseUrl + '/product/{id}/tiers?includeHiddenTiers=true',
    getTier: baseUrl + '/product/{id}/tiers/{tierId}',
    create: baseUrl + '/product',
    update: baseUrl + '/product/{id}',
    createTier: baseUrl + '/product/{id}/tiers',
    updateTier: baseUrl + '/product/{id}/tiers/{tierId}'
  }
})

export const actions = {
  list ({ state }) {
    const listUrl = state.urls.list

    return this.$axios.get(listUrl)
  },
  listTiers ({ state }, productId) {
    const listUrl = state.urls.listTiers.replace('{id}', encodeURIComponent(productId))

    return this.$axios.get(listUrl)
  },
  getTier ({ state }, { productId, tierId }) {
    const listUrl = state.urls.getTier.replace('{id}', encodeURIComponent(productId)).replace('{tierId}', encodeURIComponent(tierId))

    return this.$axios.get(listUrl)
  },
  create ({ state }, product) {
    const createUrl = state.urls.create

    return this.$axios.post(createUrl, product)
  },
  update ({ state }, { productId, product }) {
    const updateUrl = state.urls.update.replace('{id}', encodeURIComponent(productId))

    return this.$axios.put(updateUrl, product)
  },
  createTier ({ state }, { productId, tier }) {
    const createUrl = state.urls.createTier.replace('{id}', encodeURIComponent(productId))

    return this.$axios.post(createUrl, tier)
  },
  updateTier ({ state }, { productId, tierId, tier }) {
    const updateUrl = state.urls.updateTier.replace('{id}', encodeURIComponent(productId)).replace('{tierId}', encodeURIComponent(tierId))

    return this.$axios.put(updateUrl, tier)
  }
}
