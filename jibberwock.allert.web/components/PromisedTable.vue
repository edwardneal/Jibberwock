<template>
  <Promised :promise="internalPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-data-table
        v-model="selectedRecords"
        :show-select="selectable"
        :single-select="selectable"
        :headers="headers"
        :loading="started && isPending"
        :items="started && !isPending && error === null ? data.data : undefined"
      >
        <template v-slot:top>
          <v-alert v-if="error !== null" dense dismissible outlined type="error">
            <v-tooltip bottom>
              <template v-slot:activator="{ on, attrs }">
                <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToLoadRecords }}</span>
              </template>
              <span>{{ error.message }}</span>
            </v-tooltip>
          </v-alert>
          <v-toolbar dense>
            <slot name="toolbar-actions" v-bind="{ shouldDisable: isPending || error !== null }" />
          </v-toolbar>
        </template>
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
        <template v-slot:item.visible="{ item }">
          <v-chip v-if="item.visible" color="success" small>
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
        <template v-slot:item._actions="{ item }">
          <slot name="item-actions" v-bind="{ item }" />
        </template>
        <template v-for="slotName in Object.getOwnPropertyNames($scopedSlots).filter(pN => pN.toLowerCase().startsWith('item.'))" v-slot:[slotName]="{ item }">
          <slot :name="slotName" v-bind="{ item }" />
        </template>
      </v-data-table>
    </template>
  </Promised>
</template>

<style>
  .v-toolbar--dense .v-toolbar__content {
    padding-left: 0px;
    padding-right: 0px;
  }
</style>

<script>
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
    headers: {
      type: Array,
      required: true
    },
    populateFunction: {
      type: Function,
      required: true
    },
    selectable: {
      type: Boolean,
      required: false,
      default: true
    }
  },
  data () {
    return {
      started: false,
      internalPromise: undefined,
      internalRecords: []
    }
  },
  computed: {
    selectedRecords: {
      get () { return this.internalRecords },
      set (value) {
        this.internalRecords = value
        this.$emit('selection-changed', value)
      }
    }
  },
  mounted () {
    this.refresh()
  },
  methods: {
    refresh () {
      this.started = true
      this.selectedRecords = []
      this.internalPromise = this.populateFunction()
    }
  }
}
</script>
