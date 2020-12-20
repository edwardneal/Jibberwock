<template>
  <Promised :promise="srPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-autocomplete
        v-if="! error"
        v-model="selectedItem"
        :items="typeof data !== 'undefined' && data !== null ? data.data : []"
        :loading="isPending"
        :label="title"
        :search-input.sync="searchInput"
        hide-no-data
        item-text="name"
        hide-details
        return-object
        flat
      >
        <template v-slot:item="{ item, on, attrs }">
          <v-list-item v-bind="attrs" v-on="on">
            <v-list-item-icon>
              <v-tooltip left>
                <template v-slot:activator="tt">
                  <v-icon v-bind="tt.attrs" v-on="tt.on">
                    {{ languageStrings.dropdownValues.securableResourceType.find(t => t.id === item.resourceType).icon }}
                  </v-icon>
                </template>
                {{ languageStrings.dropdownValues.securableResourceType.find(t => t.id === item.resourceType).label }}
              </v-tooltip>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title>
                {{ item.name }}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
      </v-autocomplete>

      <slot v-if="! isPending && error" name="error" v-bind="{ isPending, error, data }" />
    </template>
  </Promised>
</template>

<script>
import { mapActions } from 'vuex'
import { Promised } from 'vue-promised'

export default {
  components: {
    Promised
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    tenantId: {
      type: String,
      required: false,
      default: null
    },
    title: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      srPromise: Promise.resolve(),
      searchInput: null,
      selectedItem: null
    }
  },
  watch: {
    // todo: this doesn't get updated when the value is set to null, which looks misleading. This needs to be fixed - it needs to be cleared
    searchInput (filter) {
      this.srPromise = this.getSecurableResources({ tenantId: this.tenantId, filter: '*' + (filter ?? '*') + '*' })
    },
    selectedItem (val) {
      this.$emit('input', val)
    }
  },
  methods: {
    ...mapActions({
      getSecurableResources: 'tenants/getTenantSecurableResources'
    })
  }
}
</script>
