import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/about',
    name: 'Privacy',
    component: () => import(/* webpackChunkName: "privacy" */ '../views/About.vue')
  },
  {
    path: '/allert',
    name: 'Allert',
    component: () => import(/* webpackChunkName: "allert" */ '../views/Allert.vue')
  },
  {
    path: '/proxyvirt',
    name: 'ProxyVirt',
    component: () => import(/* webpackChunkName: "proxyvirt" */ '../views/ProxyVirt.vue')
  },
  {
    path: '/statuspageio-client',
    name: 'StatusPageClient',
    component: () => import(/* webpackChunkName: "statuspageio-client" */ '../views/StatusPageIOClient.vue')
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
