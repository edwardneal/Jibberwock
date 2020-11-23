<template>
  <span>{{ this.$route.params.id }}</span>
</template>

<script>
export default {
  components: {
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
    auth: { required: true },
    mandatoryParams: ['id']
  },
  head () {
    return {
      title: this.languageStrings.pages.tenant.title
    }
  }
}
</script>
