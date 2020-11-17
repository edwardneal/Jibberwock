const baseRedirectUrl = process.env.NODE_ENV === 'production' ? 'https://allert.jibberwock.com' : 'http://localhost:3000'

export const state = () => ({
  urls: {
    logIn: 'https://www.jibberwock.com/.auth/login/aad?post_login_redirect_url={redirectTo}',
    me: 'https://www.jibberwock.com/.auth/me',
    logOut: 'https://www.jibberwock.com/.auth/logout?post_logout_redirect_url={redirectTo}'
  },
  loggedIn: false,
  userInfo: null
})

export const mutations = {
  logIn (state, userInfo) {
    state.loggedIn = true
    state.userInfo = userInfo
  },
  logOut (state) {
    state.loggedIn = false
    state.userInfo = null
  }
}

export const getters = {
  getLogInUrl: state => (url) => {
    return state.urls.logIn.replace('{redirectTo}', encodeURIComponent(baseRedirectUrl + url))
  },
  getLogOutUrl: state => (url) => {
    return state.urls.logOut.replace('{redirectTo}', encodeURIComponent(baseRedirectUrl + url))
  }
}

export const actions = {
  async populateLoggedInState ({ commit, state }) {
    await fetch(state.urls.me, {
      method: 'GET',
      credentials: 'include',
      cache: 'no-cache'
    })
      .then(async (res) => {
        if (res.status === 200) {
          commit('logIn', await res.json())
        } else {
          commit('logOut')
        }
      })
      .catch((error) => {
        if (typeof error.response === 'undefined' ||
          error.response.status === 401) {
          commit('logOut')
        }
      })
  }
}
