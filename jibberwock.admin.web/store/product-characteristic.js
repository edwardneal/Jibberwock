const baseUrl = process.env.NODE_ENV === 'production' ? 'https://admin.jibberwock.com/api' : 'https://localhost:44349/api'

export const state = () => ({
  urls: {
    list: baseUrl + '/productcharacteristic',
    create: baseUrl + '/productcharacteristic',
    update: baseUrl + '/productcharacteristic/{id}',
    delete: baseUrl + '/productcharacteristic/{id}'
  }
})

export const actions = {
  list ({ state }) {
    const listUrl = state.urls.list

    return this.$axios.get(listUrl)
  },
  create ({ state }, characteristic) {
    const createUrl = state.urls.create

    return this.$axios.post(createUrl, characteristic)
  },
  update ({ state }, { characteristicId, characteristic }) {
    const updateUrl = state.urls.update.replace('{id}', encodeURIComponent(characteristicId))

    return this.$axios.put(updateUrl, characteristic)
  },
  delete ({ state }, characteristicId) {
    const deleteUrl = state.urls.delete.replace('{id}', encodeURIComponent(characteristicId))

    return this.$axios.delete(deleteUrl)
  }
}
