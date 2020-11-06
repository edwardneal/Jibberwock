<template>
  <v-card :loading="loading" elevation="0">
    <component :is="chartFile" v-if="! loading" :chart-data="chartData" :options="chartOptions" @data-point-click="$emit('update:selected-data-point', $event)" />
  </v-card>
</template>

<script>
export default {
  props: {
    chartData: {
      type: Object,
      required: false,
      default: null
    },
    chartOptions: {
      type: Object,
      required: true
    },
    chartType: {
      type: String,
      required: true
    },
    loading: {
      type: Boolean,
      required: false,
      default: false
    },
    selectedDataPoint: {
      type: Object,
      required: false,
      default: null
    }
  },
  computed: {
    chartFile () {
      return () => import('~/components/charts/' + this.chartType + 'Chart.vue')
    }
  }
}
</script>
