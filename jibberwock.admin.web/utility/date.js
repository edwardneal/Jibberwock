export function getDateBuckets (startDate, endDate) {
  const roundedStartDate = new Date(new Date(startDate).toDateString())
  const roundedEndDate = new Date(new Date(endDate).toDateString())
  const dateBuckets = []

  for (let currDate = roundedStartDate; currDate <= roundedEndDate; currDate = new Date(currDate.setDate(currDate.getDate() + 1))) {
    dateBuckets.push({
      t: new Date(Date.UTC(currDate.getFullYear(), currDate.getMonth(), currDate.getDate())),
      y: 0,
      details: []
    })
  }

  return dateBuckets
}
