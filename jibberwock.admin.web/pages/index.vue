<template>
  <v-row justify="center" align="center">
    <v-col cols="12">
      <AdminDashboard v-if="$store.state.auth.loggedIn" :language-strings="languageStrings" />
      <v-card v-else>
        <v-card-title class="headline">
          {{ languageStrings.pages.homepage.unauthenticated.header }}
        </v-card-title>
        <v-card-text>
          <p>{{ languageStrings.pages.homepage.unauthenticated.comment }}</p>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
import AdminDashboard from '~/components/AdminDashboard.vue'

export default {
  components: {
    AdminDashboard
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  computed: {
  },
  meta: {
    auth: { required: false }
  },
  head () {
    return {
      title: this.languageStrings.pages.homepage.title
    }
  }
}
</script>
