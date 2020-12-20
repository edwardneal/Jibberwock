<template>
  <Promised :promise="srPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-autocomplete
        v-if="! error"
        v-model="selectedItem"
        :items="typeof data !== 'undefined' && data !== null ? data : []"
        :loading="isPending"
        :label="title"
        hide-no-data
        item-text="user.name"
        hide-details
        return-object
        flat
      >
        <template v-slot:item="{ item, on, attrs }">
          <v-list-item v-bind="attrs" v-on="on">
            <v-list-item-icon>
              <v-icon v-if="item.enabled" color="success">
                mdi-check
              </v-icon>
              <v-icon v-else color="error">
                mdi-block-helper
              </v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title>
                {{ item.user.name }}
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
    },
    excludedMembers: {
      type: Array,
      required: false,
      default: () => []
    }
  },
  data () {
    return {
      srPromise: this.getMembers(this.tenantId),
      selectedItem: null
    }
  },
  watch: {
    selectedItem (val) {
      this.$emit('input', val?.user)
    },
    excludedMembers (_val) {
      // todo: this is triggered whenever any records are added. This needs to be fixed.
      this.srPromise = this.getMembers(this.tenantId)
    }
  },
  methods: {
    ...mapActions({
      getMembersInternal: 'tenants/getTenantMembers'
    }),
    getMembers (tenantId) {
      return this.getMembersInternal(tenantId)
        .then((resp) => {
          return resp.data.filter((mem) => {
            return !this.excludedMembers.map(u => u.id).includes(mem.user.id)
          })
        })
    }
  }
}
</script>
