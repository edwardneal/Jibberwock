<script>
import Themeable from 'vuetify/lib/mixins/themeable'
import { Line, mixins } from 'vue-chartjs'
const { reactiveProp } = mixins

export default {
  extends: Line,
  mixins: [Themeable, reactiveProp],
  props: {
    chartData: {
      type: Object,
      required: false,
      default: null
    },
    options: {
      type: Object,
      required: true
    },
    selectedDataPoint: {
      type: Object,
      required: false,
      default: null
    }
  },
  watch: {
    chartData () {
      this.syncMaterialTheme()
      this.setupEventHandler()

      this.$data._chart.update()
    },
    options () {
      this.syncMaterialTheme()
      this.setupEventHandler()

      this.$nextTick(() => this.renderChart(this.chartData, this.options))
    },
    isDark () {
      this.syncMaterialTheme()
      this.setupEventHandler()

      this.$nextTick(() => this.renderChart(this.chartData, this.options))
    }
  },
  mounted () {
    this.syncMaterialTheme()
    this.setupEventHandler()
    this.renderChart(this.chartData, this.options)
  },
  methods: {
    syncMaterialTheme () {
      if (this.options) {
        this.options.defaultFontColor = this.isDark ? '#fff' : '#000'

        if (this.options.title) {
          this.options.title.fontColor = this.options.defaultFontColor
        }
        if (this.options.scales) {
          if (this.options.scales.xAxes) {
            this.options.scales.xAxes.forEach((xA) => {
              if (!xA.gridLines) {
                xA.gridLines = {}
              }
              xA.gridLines.color = this.isDark ? 'rgba(255,255,255,0.1)' : 'rgba(0,0,0,0.1)'
            })
          }

          if (this.options.scales.yAxes) {
            this.options.scales.yAxes.forEach((yA) => {
              if (!yA.gridLines) {
                yA.gridLines = {}
              }
              yA.gridLines.color = this.isDark ? 'rgba(255,255,255,0.1)' : 'rgba(0,0,0,0.1)'
            })
          }
        }
      }
    },
    setupEventHandler () {
      if (this.options) {
        this.options.onClick = (_evt, clickedOn) => {
          if (clickedOn) {
            for (let i = 0; i < clickedOn.length; i++) {
              const dataPoint = clickedOn[i]

              if (dataPoint && dataPoint._chart && dataPoint._chart.config && dataPoint._chart.config.data && dataPoint._chart.config.data.datasets) {
                const allDataSets = dataPoint._chart.config.data.datasets

                if (dataPoint._datasetIndex < allDataSets.length && dataPoint._datasetIndex >= 0) {
                  const dataSet = allDataSets[dataPoint._datasetIndex]

                  if (dataSet && dataSet.data && dataPoint._index < dataSet.data.length && dataPoint._index >= 0) {
                    this.$emit('data-point-click', {
                      dataSet,
                      dataPoint: dataSet.data[dataPoint._index]
                    })
                    this.$emit('update:selected-data-point', {
                      dataSet,
                      dataPoint: dataSet.data[dataPoint._index]
                    })
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}
</script>
