export default function ({ route, redirect }) {
  const meta = route.meta

  if (typeof meta !== 'undefined' && meta !== null && meta.length > 0) {
    const possibleMandatoryParamsMetas = meta.filter(m => typeof m.mandatoryParams !== 'undefined' && m.mandatoryParams !== null)

    if (possibleMandatoryParamsMetas.length > 0) {
      const mandatoryParamsMeta = possibleMandatoryParamsMetas[0].mandatoryParams
      const routeParams = route.params

      // If we have mandatory parameters and those parameters aren't provided by the route, redirect to the homepage.
      for (let i = 0; i < mandatoryParamsMeta.length; i++) {
        if (!routeParams[mandatoryParamsMeta[i]]) {
          redirect('/', { reason: 'missing_parameter' })
          return
        }
      }
    }
  }
}
