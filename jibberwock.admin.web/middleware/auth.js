export default async function ({ route, store, redirect }) {
  const populateLoggedInState = store._actions['auth/populateLoggedInState']
  const meta = route.meta

  // On every page load, repopulate the logged-in status.
  // This isn't perfect yet, but will help to some degree.
  // Further work will be needed to make sure that this is present on every API call.
  if (typeof populateLoggedInState !== 'undefined' && populateLoggedInState.length > 0 && typeof populateLoggedInState[0] === 'function') {
    await populateLoggedInState[0]()
  }

  // Now, perform quick authentication checks
  if (typeof meta !== 'undefined' && meta !== null && meta.length > 0) {
    const possibleAuthMetas = meta.filter(m => typeof m.auth !== 'undefined' && m.auth !== null)

    if (possibleAuthMetas.length > 0) {
      const authMeta = possibleAuthMetas[0].auth
      const userLoggedIn = store.state.auth.loggedIn

      // If we're not logged in and authentication is needed, perform a redirect back to the homepage
      // Include the current page in the ReturnURL query string
      if (authMeta.required && !userLoggedIn) {
        redirect('/', { ReturnURL: route.fullPath })
      }
    }
  }
}
