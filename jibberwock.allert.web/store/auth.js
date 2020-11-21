const baseRedirectUrl = process.env.NODE_ENV === 'production' ? 'https://allert.jibberwock.com' : 'http://localhost:3000'
const claimTypes = {
  idp: 'http://schemas.microsoft.com/identity/claims/identityprovider',
  name: 'name',
  emailAddress: 'emails'
}

export const state = () => ({
  urls: {
    logIn: 'https://www.jibberwock.com/.auth/login/aad?post_login_redirect_url={redirectTo}',
    me: 'https://www.jibberwock.com/.auth/me',
    logOut: 'https://www.jibberwock.com/.auth/logout?post_logout_redirect_url={redirectTo}',
    editProfile: 'https://jibberwockb2c.b2clogin.com/idp.jibberwock.com/oauth2/v2.0/authorize?p=B2C_1_EditProfile&client_id=98010cfe-0e61-4daf-a403-94464e8899cb&nonce=defaultNonce&redirect_uri={redirectTo}&scope=openid&response_type=id_token&prompt=login',
    resetPassword: 'https://jibberwockb2c.b2clogin.com/idp.jibberwock.com/oauth2/v2.0/authorize?p=B2C_1_ResetPassword&client_id=98010cfe-0e61-4daf-a403-94464e8899cb&nonce=defaultNonce&redirect_uri={redirectTo}&scope=openid&response_type=id_token&prompt=login'
  },
  loggedIn: false,
  rawUserInfo: null,
  userInfo: null
})

export const mutations = {
  logIn (state, userInfo) {
    state.loggedIn = true

    if (userInfo.length === 1) {
      state.rawUserInfo = userInfo[0]
    } else {
      state.rawUserInfo = userInfo
    }

    state.userInfo = {
      identityProvider: state.rawUserInfo.user_claims.find(c => c.typ === claimTypes.idp).val,
      name: state.rawUserInfo.user_claims.find(c => c.typ === claimTypes.name).val,
      emailAddresses: state.rawUserInfo.user_claims.filter(c => c.typ === claimTypes.emailAddress)
        .map(c => c.val)
    }
  },
  logOut (state) {
    state.loggedIn = false
    state.rawUserInfo = null
  }
}

export const getters = {
  getLogInUrl: state => (url) => {
    return state.urls.logIn.replace('{redirectTo}', encodeURIComponent(baseRedirectUrl + url))
  },
  getLogOutUrl: state => (url) => {
    return state.urls.logOut.replace('{redirectTo}', encodeURIComponent(baseRedirectUrl + url))
  },
  getEditProfileUrl: state => (url) => {
    return state.urls.editProfile.replace('{redirectTo}', encodeURIComponent(baseRedirectUrl + url))
  },
  getResetPasswordUrl: state => (url) => {
    return state.urls.resetPassword.replace('{redirectTo}', encodeURIComponent(baseRedirectUrl + url))
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
