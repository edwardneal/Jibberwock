export function groupBy (array, key) {
  return array.reduce((storage, currVal) => {
    const val = key instanceof Function ? key(currVal) : currVal[key]
    const ele = storage.find(r => r && r.key === val)

    if (ele) {
      ele.values.push(currVal)
    } else {
      storage.push({ key: val, values: [currVal] })
    }
    return storage
  }, [])
}
