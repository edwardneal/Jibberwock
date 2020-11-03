<template>
  <Promised :promise="loadingPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-autocomplete
        v-model="selectedValue"
        :items="typeof data != 'undefined' && data !== null && data.data !== null ? data.data : []"
        :loading="isPending"
        :label="label"
        :search-input.sync="searchString"
        :filter="passAllFilter"
        hide-no-data
        item-text="name"
        item-value="id"
        hide-details
        clearable
        return-objecte
      />
    </template>
  </Promised>
</template>

<script>
import { Promised } from 'vue-promised'
import { mapActions } from 'vuex'

export default {
  components: {
    Promised
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    label: {
      type: String,
      required: true
    },
    selectedUserId: {
      type: Number,
      required: false,
      default: null
    }
  },
  data () {
    return {
      loadingPromise: Promise.resolve(),
      searchString: ''
    }
  },
  computed: {
    selectedValue: {
      get () {
        return this.selectedUserId
      },
      set (value) {
        this.$emit('update:selected-user-id', value)
      }
    }
  },
  watch: {
    searchString (value) {
      if (typeof value !== 'undefined' && value !== null && value !== '') {
        this.loadingPromise = this.findUsersInternal('*' + value + '*')
      }
    }
  },
  methods: {
    ...mapActions({
      findUsersInternal: 'users/findUsers'
    }),
    passAllFilter (_item, _queryText, _itemText) {
      return true
    }
  }
}
</script>
