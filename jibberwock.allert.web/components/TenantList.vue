<template>
  <Promised :promise="masterTenantPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-alert v-if="error !== null" dense dismissible outlined type="error">
        <v-tooltip bottom>
          <template v-slot:activator="{ on, attrs }">
            <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToListTenants }}</span>
          </template>
          <span>{{ error.message }}</span>
        </v-tooltip>
      </v-alert>
      <v-data-table
        :headers="tenantHeaders"
        :loading="isPending"
        :items="!isPending && error === null ? data : undefined"
      >
        <template v-slot:item.enabled="{ item }">
          <v-chip v-if="item.enabled" color="success" small>
            <v-icon small>
              mdi-check
            </v-icon>
          </v-chip>
          <v-chip v-else color="error" small>
            <v-icon small>
              mdi-block-helper
            </v-icon>
          </v-chip>
        </template>
      </v-data-table>
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
    users: {
      type: Array,
      required: false,
      default: null
    }
  },
  data () {
    return {
      tenantHeaders: [
        {
          text: this.languageStrings.tenantList.headers.name,
          value: 'name',
          sortable: true
        },
        {
          text: this.languageStrings.tenantList.headers.enabled,
          value: 'enabled',
          sortable: true
        }
      ],
      masterTenantPromise: null
    }
  },
  watch: {
    users () { this.updateMasterTenantPromise() },
    tenants () { this.updateMasterTenantPromise() }
  },
  mounted () {
    this.$nextTick(this.updateMasterTenantPromise)
  },
  methods: {
    ...mapActions({
      getUserTenantsInternal: 'users/getTenants'
    }),
    updateMasterTenantPromise () {
      const getTenantPromiseList = this.users.map((u) => { return this.getUserTenantsInternal(u.id) })

      this.masterTenantPromise = Promise.all(getTenantPromiseList).then((values) => {
        return values.flatMap((v) => {
          return v.data.flatMap((v2) => {
            return {
              name: v2.name,
              enabled: v2.wellKnownGroups['6'].users[0].enabled
            }
          })
        })
      })
    }
  }
}
</script>
