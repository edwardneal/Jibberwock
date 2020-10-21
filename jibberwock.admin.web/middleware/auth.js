export default async function ({ store }) {
  var populateLoggedInState = store._actions["auth/populateLoggedInState"];

  // On every page load, repopulate the logged-in status.
  // This isn't perfect yet, but will help to some degree.
  // Further work will be needed to make sure that this is present on every API call.
  if (typeof populateLoggedInState !== 'undefined') {
    return populateLoggedInState[0]();
  }
}
