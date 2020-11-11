<template>
  <v-sheet>
    <Promised :promise="getExceptionDetails()">
      <template v-slot:combined="{ isPending, error, data }">
        <v-row v-if="error !== null" justify="center" align="center">
          <v-col cols="12">
            <v-alert dense dismissible outlined type="error">
              <v-tooltip bottom>
                <template v-slot:activator="{ on, attrs }">
                  <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToGetExceptions }}</span>
                </template>
                <span>{{ error.message }}</span>
              </v-tooltip>
            </v-alert>
          </v-col>
        </v-row>
        <v-row v-if="!isPending && error === null">
          <v-col cols="12" md="6">
            <Chart
              :chart-data="{ datasets: data.jsExceptionsByPage }"
              :chart-options="jsExceptionsByPage.chartOptions"
              :loading="isPending"
              :selected-data-point.sync="jsExceptionsByPage.selectedDataPoint"
              chart-type="Line"
              style="height: 25%"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-sheet v-if="jsExceptionsByPage.selectedDataPoint === null">
              {{ languageStrings.pages.exceptions.jsExceptionsByPage.detailsTemplate }}
            </v-sheet>
            <v-sheet v-else>
              <p><strong>{{ languageStrings.pages.exceptions.jsExceptionsByPage.pageTemplate }}</strong> {{ jsExceptionsByPage.selectedDataPoint.dataSet.label }} ({{ jsExceptionsByPage.selectedDataPoint.dataPoint.t.toLocaleDateString() }})</p>
              <v-data-table
                :items="jsExceptionsByPage.selectedDataPoint.dataPoint.details"
                :headers="jsExceptionsByPage.tableHeaders"
              >
                <template v-slot:item.timestamp="{ item }">
                  {{ typeof item.timestamp !== 'undefined' && item.timestamp !== null ? new Date(item.timestamp).toLocaleString() : '-' }}
                </template>
              </v-data-table>
            </v-sheet>
          </v-col>
        </v-row>
        <v-row v-if="!isPending && error === null">
          <v-col cols="12" md="6">
            <Chart
              :chart-data="{ datasets: data.failedRequestsByRoute }"
              :chart-options="failedRequestsByRoute.chartOptions"
              :loading="isPending"
              :selected-data-point.sync="failedRequestsByRoute.selectedDataPoint"
              chart-type="Line"
              style="height: 25%"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-sheet v-if="failedRequestsByRoute.selectedDataPoint === null">
              {{ languageStrings.pages.exceptions.failedRequestsByRoute.detailsTemplate }}
            </v-sheet>
            <v-sheet v-else>
              <p><strong>{{ languageStrings.pages.exceptions.failedRequestsByRoute.pageTemplate }}</strong> {{ failedRequestsByRoute.selectedDataPoint.dataSet.label }} ({{ failedRequestsByRoute.selectedDataPoint.dataPoint.t.toLocaleDateString() }})</p>
              <v-data-table
                :items="failedRequestsByRoute.selectedDataPoint.dataPoint.details"
                :headers="failedRequestsByRoute.tableHeaders"
              >
                <template v-slot:item.timestamp="{ item }">
                  {{ typeof item.timestamp !== 'undefined' && item.timestamp !== null ? new Date(item.timestamp).toLocaleString() : '-' }}
                </template>
              </v-data-table>
            </v-sheet>
          </v-col>
        </v-row>
        <v-row v-if="!isPending && error === null">
          <v-col cols="12" md="6">
            <Chart
              :chart-data="{ datasets: data.serverSideErrorsByResource }"
              :chart-options="serverSideErrorsByResource.chartOptions"
              :loading="isPending"
              :selected-data-point.sync="serverSideErrorsByResource.selectedDataPoint"
              chart-type="Line"
              style="height: 25%"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-sheet v-if="serverSideErrorsByResource.selectedDataPoint === null">
              {{ languageStrings.pages.exceptions.serverSideErrorsByResource.detailsTemplate }}
            </v-sheet>
            <v-sheet v-else>
              <p><strong>{{ languageStrings.pages.exceptions.serverSideErrorsByResource.pageTemplate }}</strong> {{ serverSideErrorsByResource.selectedDataPoint.dataSet.label }} ({{ serverSideErrorsByResource.selectedDataPoint.dataPoint.t.toLocaleDateString() }})</p>
              <Chart
                :chart-data="getErrorsByResource(serverSideErrorsByResource.selectedDataPoint)"
                :chart-options="serverSideErrorsByResource.detailsChart.chartOptions"
                chart-type="HorizontalBar"
                style="height: 100%;"
              />
            </v-sheet>
          </v-col>
        </v-row>
        <v-row v-if="!isPending && error === null">
          <v-col cols="12" md="6">
            <Chart
              :chart-data="{ datasets: data.serverSideErrorsByRoute }"
              :chart-options="serverSideErrorsByRoute.chartOptions"
              :loading="isPending"
              :selected-data-point.sync="serverSideErrorsByRoute.selectedDataPoint"
              chart-type="Line"
              style="height: 25%"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-sheet v-if="serverSideErrorsByRoute.selectedDataPoint === null">
              {{ languageStrings.pages.exceptions.serverSideErrorsByRoute.detailsTemplate }}
            </v-sheet>
            <v-sheet v-else>
              <p><strong>{{ languageStrings.pages.exceptions.serverSideErrorsByRoute.pageTemplate }}</strong> {{ serverSideErrorsByRoute.selectedDataPoint.dataSet.label }} ({{ serverSideErrorsByRoute.selectedDataPoint.dataPoint.t.toLocaleDateString() }})</p>
              <v-data-table
                :items="serverSideErrorsByRoute.selectedDataPoint.dataPoint.details"
                :headers="serverSideErrorsByRoute.tableHeaders"
              >
                <template v-slot:item.timestamp="{ item }">
                  {{ typeof item.timestamp !== 'undefined' && item.timestamp !== null ? new Date(item.timestamp).toLocaleString() : '-' }}
                </template>
                <template v-slot:item.type="{ item }">
                  {{ languageStrings.pages.exceptions.serverSideErrorsByRoute.errorTypeMappings[item.type] ? languageStrings.pages.exceptions.serverSideErrorsByRoute.errorTypeMappings[item.type] : item.type }}
                </template>
              </v-data-table>
            </v-sheet>
          </v-col>
        </v-row>
        <v-overlay :value="isPending" absolute :opacity="0.8">
          <v-progress-circular indeterminate size="64" />
        </v-overlay>
      </template>
    </Promised>
  </v-sheet>
</template>

<script>
import { mapActions } from 'vuex'
import { Promised } from 'vue-promised'
import { getDateBuckets } from '~/utility/date.js'
import { groupBy } from '~/utility/collections.js'
import Chart from '~/components/Chart.vue'
import 'chartjs-plugin-colorschemes'

function forceUtcDate (timestamp) {
  const parsedDate = new Date(timestamp)

  return new Date(Date.UTC(parsedDate.getFullYear(), parsedDate.getMonth(), parsedDate.getDate()))
}

function processExceptionDetails (responses) {
  const exs = responses[0].data
  const reqs = responses[1].data

  const jsExceptions = exs.exceptions.filter(ex => ex.source.toLowerCase() === 'javascript')
  const nonJsExceptions = exs.exceptions.filter(ex => ex.source.toLowerCase() !== 'javascript')

  return {
    // JavaScript exceptions by a page: group by the operation (i.e. page name)
    jsExceptionsByPage: groupBy(jsExceptions, 'operation')
      .map((operationGroup, opGroupIndex) => {
        // Get a new set of date buckets for each group!
        const operationDateBuckets = getDateBuckets(exs.startDate, exs.endDate)

        // Now that we've got a series of date buckets, fill them with the number of records
        operationGroup.values.forEach((op) => {
          const forcedDate = forceUtcDate(op.timestamp)
          const matchingBucket = operationDateBuckets.find(b => b.t.getTime() === forcedDate.getTime())

          matchingBucket.y++
          matchingBucket.details.push(op)
        })

        return {
          label: operationGroup.key,
          data: operationDateBuckets,
          // The first record here should fill down to the origin line.
          // The records thereafter should fill down to the previous series' line
          fill: opGroupIndex === 0 ? 'origin' : '-1',
          borderWidth: 2,
          radius: 1,
          hitRadius: 3
        }
      }),
    failedRequestsByRoute: groupBy(reqs.failedRequests, 'name')
      .map((routeGroup, rGroupIndex) => {
        // Once again, get a new set of date buckets for the group and fill them with the number of records
        const requestDateBuckets = getDateBuckets(reqs.startDate, reqs.endDate)

        // Now that we've got a series of date buckets, fill them with the number of records
        routeGroup.values.forEach((rt) => {
          const forcedDate = forceUtcDate(rt.timestamp)
          const matchingBucket = requestDateBuckets.find(b => b.t.getTime() === forcedDate.getTime())

          matchingBucket.y++
          matchingBucket.details.push(rt)
        })

        return {
          label: routeGroup.key,
          data: requestDateBuckets,
          // The first record here should fill down to the origin line.
          // The records thereafter should fill down to the previous series' line
          fill: rGroupIndex === 0 ? 'origin' : '-1',
          borderWidth: 2,
          radius: 1,
          hitRadius: 3
        }
      }),
    serverSideErrorsByResource: groupBy(nonJsExceptions, 'roleName')
      .map((roleGroup, rIndex) => {
        // Once again, get a new set of date buckets for the group and fill them with the number of records
        const requestDateBuckets = getDateBuckets(exs.startDate, exs.endDate)

        // Now that we've got a series of date buckets, fill them with the number of records
        roleGroup.values.forEach((rl) => {
          const forcedDate = forceUtcDate(rl.timestamp)
          const matchingBucket = requestDateBuckets.find(b => b.t.getTime() === forcedDate.getTime())

          matchingBucket.y++
          matchingBucket.details.push(rl)
        })

        return {
          label: roleGroup.key,
          data: requestDateBuckets,
          // The first record here should fill down to the origin line.
          // The records thereafter should fill down to the previous series' line
          fill: rIndex === 0 ? 'origin' : '-1',
          borderWidth: 2,
          radius: 1,
          hitRadius: 3
        }
      }),
    serverSideErrorsByRoute: groupBy(nonJsExceptions, 'operation')
      .map((operationGroup, opGroupIndex) => {
        // Once again, get a new set of date buckets for the group and fill them with the number of records
        const requestDateBuckets = getDateBuckets(reqs.startDate, reqs.endDate)

        // Now that we've got a series of date buckets, fill them with the number of records
        operationGroup.values.forEach((op) => {
          const forcedDate = forceUtcDate(op.timestamp)
          const matchingBucket = requestDateBuckets.find(b => b.t.getTime() === forcedDate.getTime())

          matchingBucket.y++
          matchingBucket.details.push(op)
        })

        return {
          label: operationGroup.key,
          data: requestDateBuckets,
          // The first record here should fill down to the origin line.
          // The records thereafter should fill down to the previous series' line
          fill: opGroupIndex === 0 ? 'origin' : '-1',
          borderWidth: 2,
          radius: 1,
          hitRadius: 3
        }
      })
  }
}

function getErrorsByResource (selectedDataPoint) {
  const groupedOperations = groupBy(selectedDataPoint.dataPoint.details, 'operation')

  return {
    labels: groupedOperations.map(g => g.key ? g.key : selectedDataPoint.dataSet.label),
    datasets: groupBy(selectedDataPoint.dataPoint.details, 'message')
      .map((messageGroup) => {
        return {
          label: messageGroup.key,
          data: groupedOperations.map((operationGroup) => {
            return messageGroup.values.filter(v => v.operation === operationGroup.key).length
          }),
          borderWidth: 2,
          radius: 1,
          hitRadius: 3
        }
      })
  }
}

export default {
  components: {
    Promised,
    Chart
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      jsExceptionsByPage: {
        chartOptions: {
          title: {
            display: true,
            fontSize: 16,
            fontFamily: '"Roboto", sans-serif',
            lineHeight: 1.5,
            text: this.languageStrings.pages.exceptions.jsExceptionsByPage.chartTitle
          },
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            yAxes: [{ stacked: true }],
            xAxes: [{ type: 'time', gridLines: { display: false } }]
          },
          animation: { duration: 0 },
          tooltips: {
            mode: 'index',
            callbacks: {
              title (tti) {
                return new Date(tti[0].label).toLocaleDateString()
              }
            }
          },
          hover: {
            mode: 'y',
            axis: 'y'
          },
          plugins: {
            colorschemes: {
              scheme: 'tableau.Tableau10',
              override: true
            },
            deferred: {
              yOffset: 50
            }
          }
        },
        tableHeaders: [
          { text: this.languageStrings.pages.exceptions.jsExceptionsByPage.headers.timestamp, value: 'timestamp' },
          { text: this.languageStrings.pages.exceptions.jsExceptionsByPage.headers.sessionId, value: 'sessionId' },
          { text: this.languageStrings.pages.exceptions.jsExceptionsByPage.headers.message, value: 'message' }
        ],
        selectedDataPoint: null
      },
      failedRequestsByRoute: {
        chartOptions: {
          title: {
            display: true,
            fontSize: 16,
            fontFamily: '"Roboto", sans-serif',
            lineHeight: 1.5,
            text: this.languageStrings.pages.exceptions.failedRequestsByRoute.chartTitle
          },
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            yAxes: [{ stacked: true }],
            xAxes: [{ type: 'time', gridLines: { display: false } }]
          },
          animation: { duration: 0 },
          tooltips: {
            mode: 'index',
            callbacks: {
              title (tti) {
                return new Date(tti[0].label).toLocaleDateString()
              }
            }
          },
          hover: {
            mode: 'y',
            axis: 'y'
          },
          plugins: {
            colorschemes: {
              scheme: 'tableau.Tableau10',
              override: true
            },
            deferred: {
              yOffset: 50
            }
          }
        },
        tableHeaders: [
          { text: this.languageStrings.pages.exceptions.failedRequestsByRoute.headers.timestamp, value: 'timestamp' },
          { text: this.languageStrings.pages.exceptions.failedRequestsByRoute.headers.resultCode, value: 'resultCode' },
          { text: this.languageStrings.pages.exceptions.failedRequestsByRoute.headers.roleName, value: 'roleName' }
        ],
        selectedDataPoint: null
      },
      serverSideErrorsByResource: {
        chartOptions: {
          title: {
            display: true,
            fontSize: 16,
            fontFamily: '"Roboto", sans-serif',
            lineHeight: 1.5,
            text: this.languageStrings.pages.exceptions.serverSideErrorsByResource.chartTitle
          },
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            yAxes: [{ stacked: true }],
            xAxes: [{ type: 'time', gridLines: { display: false } }]
          },
          animation: { duration: 0 },
          tooltips: {
            mode: 'index',
            callbacks: {
              title (tti) {
                return new Date(tti[0].label).toLocaleDateString()
              }
            }
          },
          hover: {
            mode: 'y',
            axis: 'y'
          },
          plugins: {
            colorschemes: {
              scheme: 'tableau.Tableau10',
              override: true
            },
            deferred: {
              yOffset: 50
            }
          }
        },
        detailsChart: {
          chartOptions: {
            title: {
              display: true,
              fontSize: 16,
              fontFamily: '"Roboto", sans-serif',
              lineHeight: 1.5,
              text: this.languageStrings.pages.exceptions.serverSideErrorsByResource.detailsChart.chartTitle
            },
            legend: {
              display: false
            },
            responsive: true,
            maintainAspectRatio: false,
            scales: {
              yAxes: [{ stacked: true }],
              xAxes: [{ stacked: true, gridLines: { display: false } }]
            },
            animation: { duration: 0 },
            tooltips: {
              mode: 'point'
            },
            plugins: {
              colorschemes: {
                scheme: 'tableau.Tableau10',
                override: true
              },
              deferred: {
                yOffset: 50
              }
            }
          }
        },
        selectedDataPoint: null
      },
      serverSideErrorsByRoute: {
        chartOptions: {
          title: {
            display: true,
            fontSize: 16,
            fontFamily: '"Roboto", sans-serif',
            lineHeight: 1.5,
            text: this.languageStrings.pages.exceptions.serverSideErrorsByRoute.chartTitle
          },
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            yAxes: [{ stacked: true }],
            xAxes: [{ type: 'time', gridLines: { display: false } }]
          },
          animation: { duration: 0 },
          tooltips: {
            mode: 'index',
            callbacks: {
              title (tti) {
                return new Date(tti[0].label).toLocaleDateString()
              }
            }
          },
          hover: {
            mode: 'y',
            axis: 'y'
          },
          plugins: {
            colorschemes: {
              scheme: 'tableau.Tableau10',
              override: true
            },
            deferred: {
              yOffset: 50
            }
          }
        },
        tableHeaders: [
          { text: this.languageStrings.pages.exceptions.serverSideErrorsByRoute.headers.timestamp, value: 'timestamp' },
          { text: this.languageStrings.pages.exceptions.serverSideErrorsByRoute.headers.type, value: 'type' },
          { text: this.languageStrings.pages.exceptions.serverSideErrorsByRoute.headers.message, value: 'message' }
        ],
        selectedDataPoint: null
      }
    }
  },
  methods: {
    ...mapActions({
      getExceptions: 'status/getExceptions',
      getFailedRequests: 'status/getFailedRequests'
    }),
    getExceptionDetails () {
      return Promise.all([this.getExceptions(), this.getFailedRequests()])
        .then(processExceptionDetails)
    },
    selectJavascriptException (event) {
      this.jsExceptionsByPage.selectedDataPoint = event
    },
    getErrorsByResource
  },
  meta: {
    auth: { required: true }
  },
  head () {
    return {
      title: this.languageStrings.pages.exceptions.title,
      meta: [
        { hid: 'og:title', content: this.languageStrings.pages.exceptions.title + ' - Jibberwock Admin' },
        { hid: 'apple-mobile-web-app-title', content: this.languageStrings.pages.exceptions.title + ' - Jibberwock Admin' }
      ]
    }
  }
}
</script>
