<template>
  <v-sheet>
    <Promised :promise="getExternalComponentStatuses()">
      <template v-slot:combined="{ isPending, error, data }">
        <v-row v-if="error !== null" justify="center" align="center">
          <v-col cols="12">
            <v-alert dense dismissible outlined type="error">
              <v-tooltip bottom>
                <template v-slot:activator="{ on, attrs }">
                  <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToGetExternalComponentStatuses }}</span>
                </template>
                <span>{{ error.message }}</span>
              </v-tooltip>
            </v-alert>
          </v-col>
        </v-row>
        <v-row v-if="!isPending && error === null">
          <v-col
            v-for="(comp, compIdx) in data.data"
            :key="compIdx"
            cols="12"
            sm="6"
            md="4"
            lg="2"
          >
            <v-tooltip bottom>
              <template v-slot:activator="{ on }">
                <v-card :color="comp.status.available ? 'success' : 'error'" v-on="on">
                  <v-card-title>{{ languageStrings.pages.status.externalComponentPurposes[comp.purpose.name] }}</v-card-title>
                  <v-card-text align="center">
                    <v-icon x-large>
                      {{ comp.status.available ? 'mdi-check' : 'mdi-close' }}
                    </v-icon>
                  </v-card-text>
                </v-card>
              </template>
              {{ languageStrings.pages.status.tooltip.replace('{date}', new Date(comp.status.retrievalDate).toLocaleString()) }}
            </v-tooltip>
          </v-col>
        </v-row>
      </template>
    </Promised>
  </v-sheet>
</template>

<style>
  .row div .v-card { padding: unset !important; }
  .v-card__title { word-break: unset; }
</style>

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
    }
  },
  data () {
    return {
    }
  },
  methods: {
    ...mapActions({
      getExternalComponentStatuses: 'status/getExternalComponents'
    })
  },
  meta: {
    auth: { required: true }
  },
  head () {
    return {
      title: this.languageStrings.pages.status.title,
      meta: [
        { hid: 'og:title', content: this.languageStrings.pages.status.title + ' - Jibberwock Admin' },
        { hid: 'apple-mobile-web-app-title', content: this.languageStrings.pages.status.title + ' - Jibberwock Admin' }
      ]
    }
  }
}
</script>
