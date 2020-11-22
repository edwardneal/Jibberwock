<template>
  <v-container fill-height fluid class="pa-0 blue darken-3">
    <v-container fill-height fluid class="blue darken-4 rounded-br-pill">
      <div class="mx-auto">
        <CreateTenantForm :language-strings="languageStrings" />
      </div>
    </v-container>
  </v-container>
</template>

<script>
import CreateTenantForm from '~/components/CreateTenantForm.vue'

export default {
  components: {
    CreateTenantForm
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
    }
  },
  computed: {
  },
  head () {
    return {
      title: this.languageStrings.pages.signUp.title
    }
  }
}</script>
