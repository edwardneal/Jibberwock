<template>
  <v-sheet>
    <Promised :promise="getDashboardDetails()">
      <template v-slot:combined="{ isPending, error, data }">
        <v-row v-if="error !== null" justify="center" align="center">
          <v-col cols="12">
            <v-alert dense dismissible outlined type="error">
              <v-tooltip bottom>
                <template v-slot:activator="{ on, attrs }">
                  <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToGetKpis }}</span>
                </template>
                <span>{{ error.message }}</span>
              </v-tooltip>
            </v-alert>
          </v-col>
        </v-row>

        <v-row v-if="data">
          <v-col cols="6" md="3">
            <v-card flat nuxt to="/users">
              <v-card-title class="justify-center headline">
                {{ data.userCount }}
              </v-card-title>
              <v-card-title class="justify-center headline">
                {{ languageStrings.pages.homepage.authenticated.userCount }}
              </v-card-title>
            </v-card>
          </v-col>
          <v-col cols="6" md="3">
            <v-card flat nuxt to="/tenants">
              <v-card-title class="justify-center headline">
                {{ data.tenantCount }}
              </v-card-title>
              <v-card-title class="justify-center headline">
                {{ languageStrings.pages.homepage.authenticated.tenantCount }}
              </v-card-title>
            </v-card>
          </v-col>
          <v-col cols="6" md="3">
            <v-card flat nuxt to="/status">
              <v-card-title class="justify-center headline">
                <v-icon x-large>
                  {{ data.componentsHealthy ? 'mdi-check' : 'mdi-close' }}
                </v-icon>
              </v-card-title>
              <v-card-title class="justify-center headline">
                {{ languageStrings.pages.homepage.authenticated.componentStatus }}
              </v-card-title>
            </v-card>
          </v-col>
          <v-col cols="6" md="3">
            <v-card flat nuxt to="/emails">
              <v-card-title class="justify-center headline">
                {{ data.pendingEmailBatches }}
              </v-card-title>
              <v-card-title class="justify-center headline">
                {{ languageStrings.pages.homepage.authenticated.pendingEmailBatches }}
              </v-card-title>
            </v-card>
          </v-col>
        </v-row>
        <v-row v-if="data">
          <v-col cols="12" md="6">
            <v-card flat nuxt to="/audit-trail">
              <v-card-title class="justify-center headline">
                {{ languageStrings.pages.homepage.authenticated.eventsOverTime }}
              </v-card-title>
              <v-card-text>
                <v-sparkline :value="data.activityByDate" smooth line-width="2" color="success" />
              </v-card-text>
            </v-card>
          </v-col>
          <v-col cols="12" md="6">
            <v-card flat nuxt to="/exceptions">
              <v-card-title class="justify-center headline">
                {{ languageStrings.pages.homepage.authenticated.exceptionsOverTime }}
              </v-card-title>
              <v-card-text>
                <v-sparkline :value="data.exceptionsByDate" smooth line-width="2" color="error" />
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
        <v-overlay :value="isPending" absolute :opacity="0.8">
          <v-progress-circular indeterminate size="64" />
        </v-overlay>
      </template>
    </Promised>
  </v-sheet>
</template>

<style>
  .v-card__title { word-break: unset; }
</style>

<script>
import { Promised } from 'vue-promised'
import { mapActions } from 'vuex'
import { getDateBuckets } from '~/utility/date.js'
import { groupBy } from '~/utility/collections'

function processKpiDetails (responses) {
  const exceptions = responses[0].data
  const kpis = responses[1].data

  const exceptionDateBuckets = getDateBuckets(exceptions.startDate, exceptions.endDate)
  // Break the list of exceptions down by date
  const groupedExceptions = groupBy(exceptions.exceptions, itm => new Date(itm.timestamp).toLocaleDateString())

  const kpiDateBuckets = getDateBuckets(kpis.startDate, kpis.endDate)

  for (let i = 0; i < exceptionDateBuckets.length; i++) {
    const edb = exceptionDateBuckets[i]
    const exCount = groupedExceptions.filter(itm => itm.key === edb.t.toLocaleDateString())

    if (exCount !== null && exCount.length !== 0) {
      edb.y = exCount[0].values.length
    }
  }

  for (let i = 0; i < kpiDateBuckets.length; i++) {
    const kdb = kpiDateBuckets[i]
    // toISOString ends with '.000Z'. We'll lop those final five characters off to get the property name.
    // Also need to wrap it up with quotes because of the way that .NET serialises this
    const propertyName = '"' + kdb.t.toISOString().substring(0, 19) + '"'
    const kpiCount = kpis.kpi.activityByDate[propertyName]

    if (typeof kpiCount !== 'undefined' && kpiCount !== null) {
      kdb.y = kpiCount
    }
  }

  return {
    userCount: kpis.kpi.userCount,
    tenantCount: kpis.kpi.tenantCount,
    componentsHealthy: kpis.kpi.componentsHealthy,
    pendingEmailBatches: kpis.kpi.pendingEmailBatches,
    exceptionsByDate: exceptionDateBuckets.map(b => b.y),
    activityByDate: kpiDateBuckets.map(b => b.y)
  }
}

export default {
  components: {
    Promised
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      dashboardPromise: this.getDashboardDetails()
    }
  },
  methods: {
    ...mapActions({
      getExceptions: 'status/getExceptions',
      getKpis: 'status/getKpis'
    }),
    getDashboardDetails () {
      return Promise.all([this.getExceptions(), this.getKpis()])
        .then(processKpiDetails)
    }
  }
}
</script>
