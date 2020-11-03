<template>
  <v-menu ref="containingMenu" v-model="showCalendarDropdown" offset-y>
    <template v-slot:activator="{ on, attrs }">
      <v-text-field
        v-model="formattedDate"
        v-bind="attrs"
        :label="label"
        prepend-icon="mdi-calendar"
        readonly
        clearable
        hide-details
        v-on="on"
      />
    </template>
    <v-date-picker
      :value.sync="rawDate"
      :max="maxDate"
      :min="minDate"
      no-title
      scrollable
      :width="typeof $refs.containingMenu !== 'undefined' ? $refs.containingMenu.dimensions.activator.width : null"
      @input="rawDate = $event"
    />
  </v-menu>
</template>

<script>
export default {
  props: {
    minDate: {
      type: String,
      required: false,
      default: null
    },
    maxDate: {
      type: String,
      required: false,
      default: null
    },
    label: {
      type: String,
      required: true
    },
    selectedDate: {
      type: String,
      required: false,
      default: null
    }
  },
  data () {
    return {
      showCalendarDropdown: false
    }
  },
  computed: {
    formattedDate: {
      get () {
        return this.selectedDate !== null
          ? new Date(this.selectedDate).toLocaleDateString()
          : null
      },
      set (value) {
        this.rawDate = (typeof value === 'undefined' || value === null || value === '')
          ? null
          : new Date(value)
      }
    },
    rawDate: {
      get () {
        return this.selectedDate
      },
      set (value) {
        this.$emit('update:selected-date', value)
      }
    }
  }
}
</script>
