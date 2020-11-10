<template>
  <Promised :promise="emailRecordSearchPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-sheet>
        <v-row justify="center" align="center">
          <v-col cols="12">
            {{ languageStrings.pages.emails.instructions }}
          </v-col>
        </v-row>
        <v-row justify="center" align="center">
          <v-col cols="6" md="3">
            <CalendarDropdown :label="languageStrings.pages.emails.fields.startDate" :selected-date.sync="searchCriteria.startTime" :max-date="searchCriteria.endTime" />
            <CalendarDropdown :label="languageStrings.pages.emails.fields.endDate" :selected-date.sync="searchCriteria.endTime" :min-date="searchCriteria.startTime" />
          </v-col>
          <v-col cols="6" md="3">
            <Promised :promise="emailBatchListPromise">
              <template v-slot:combined="emailBatches">
                <v-autocomplete
                  v-model="searchCriteria.batch"
                  :items="typeof emailBatches.data !== 'undefined' && emailBatches.data !== null ? emailBatches.data.data : []"
                  :loading="emailBatches.isPending"
                  :label="languageStrings.pages.emails.fields.batch"
                  hide-no-data
                  item-text="serviceBusMessageId"
                  hide-details
                  cache-items
                  clearable
                  return-object
                  flat
                />
              </template>
            </Promised>
            <v-select
              v-model="searchCriteria.batchType"
              :items="languageStrings.pages.emails.batchTypes"
              :label="languageStrings.pages.emails.fields.batchType"
              :disabled="typeof searchCriteria.batch !== 'undefined' && searchCriteria.batch !== null"
              item-text="label"
              item-value="id"
              hide-details
              clearable
            />
          </v-col>
          <v-col cols="6" md="3">
            <v-text-field
              v-model="searchCriteria.serviceBusMessageId"
              :label="languageStrings.pages.emails.fields.serviceBusMessageId"
              :disabled="typeof searchCriteria.batch !== 'undefined' && searchCriteria.batch !== null"
              hide-details
              clearable
            />
            <v-text-field
              v-model="searchCriteria.emailAddress"
              :label="languageStrings.pages.emails.fields.emailAddress"
              :disabled="!(typeof searchCriteria.startTime !== 'undefined' && searchCriteria.startTime !== '' && searchCriteria.startTime !== null)"
              hide-details
              clearable
            />
          </v-col>
          <v-col cols="6" md="3">
            <v-btn :loading="isPending" small color="primary" @click="performSearch">
              {{ languageStrings.actions.search }}
            </v-btn>
            <v-menu offset-y :close-on-content-click="false">
              <template v-slot:activator="{ on, attrs }">
                <v-btn small color="primary" v-bind="attrs" v-on="on">
                  {{ languageStrings.pages.emails.actions.showColumns }}
                </v-btn>
              </template>
              <v-list>
                <v-list-item v-for="(col, colIdx) in masterHeaders" :key="colIdx" dense>
                  <v-list-item-content class="pt-0 pb-0">
                    <v-checkbox v-model="col.visible" :disabled="col.mustAppear" :label="col.text" hide-details class="mt-0 pt-0" />
                  </v-list-item-content>
                </v-list-item>
              </v-list>
            </v-menu>
          </v-col>
        </v-row>
        <v-row justify="center" align="center">
          <v-col cols="12">
            <v-alert v-if="error !== null" dense dismissible outlined type="error">
              <v-tooltip bottom>
                <template v-slot:activator="{ on, attrs }">
                  <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToSearchEmails }}</span>
                </template>
                <span>{{ error.message }}</span>
              </v-tooltip>
            </v-alert>
            When an email is selected, show a list of events.
            These events come from App Insights, and we want to see the sendgrid_event_type, smtp_message_id and timestamp all the time - then if possible, smtp_bounce_reason/type, smtp_dropped_reason, smtp_deferred_response
            <v-data-table
              v-model="selectedEntries"
              :headers="resultantHeaders"
              :loading="isPending"
              :items="typeof data !== 'undefined' && data !== null ? data : []"
              fixed-header
              class="sticky-data-table"
              @click:row="selectRow"
            >
              <template v-slot:item.sourceBatch.type.id="{ item }">
                {{ languageStrings.pages.emails.batchTypes.filter(bt => bt.id === item.sourceBatch.type.id)[0].label }}
              </template>
              <template v-slot:item.sourceBatch.dateFirstProcessed="{ item }">
                {{ item.sourceBatch.dateFirstProcessed ? new Date(item.sourceBatch.dateFirstProcessed).toLocaleString() : '-' }}
              </template>
              <template v-slot:item.sourceBatch.dateLastProcessed="{ item }">
                {{ item.sourceBatch.dateLastProcessed ? new Date(item.sourceBatch.dateLastProcessed).toLocaleString() : '-' }}
              </template>
              <template v-slot:item.sourceBatch.processedSuccessfully="{ item }">
                <v-chip v-if="item.sourceBatch.processedSuccessfully" color="success" small>
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
              <template v-slot:item.sendDate="{ item }">
                {{ item.sendDate ? new Date(item.sendDate).toLocaleString() : '-' }}
              </template>
            </v-data-table>
          </v-col>
        </v-row>
      </v-sheet>
    </template>
  </Promised>
</template>

<style>
  #app-container div[role=document] { left: initial; position: initial; }
  #app-container div[role=document] .v-bottom-sheet { box-shadow: none; }

  .sticky-data-table .v-data-table__wrapper { overflow: unset; }
  .sticky-element { position: sticky !important; top: 64px !important; }
</style>

<script>
import { Promised } from 'vue-promised'
import { mapActions } from 'vuex'
import CalendarDropdown from '@/components/CalendarDropdown.vue'

export default {
  components: {
    Promised,
    CalendarDropdown
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      searchCriteria: {
        startTime: null,
        endTime: null,
        batch: null,
        batchType: null,
        serviceBusMessageId: null,
        emailAddress: null
      },
      masterHeaders: [
        { text: this.languageStrings.pages.emails.headers.batchType, value: 'sourceBatch.type.id', sortable: true, visible: true, mustAppear: true, class: 'sticky-element' },
        { text: this.languageStrings.pages.emails.headers.batchServiceBusMessageId, value: 'sourceBatch.serviceBusMessageId', sortable: true, visible: true, mustAppear: false, class: 'sticky-element' },
        { text: this.languageStrings.pages.emails.headers.batchFirstProcessed, value: 'sourceBatch.dateFirstProcessed', sortable: true, visible: false, mustAppear: false, class: 'sticky-element' },
        { text: this.languageStrings.pages.emails.headers.batchLastProcessed, value: 'sourceBatch.dateLastProcessed', sortable: true, visible: false, mustAppear: false, class: 'sticky-element' },
        { text: this.languageStrings.pages.emails.headers.batchProcessedSuccessfully, value: 'sourceBatch.processedSuccessfully', sortable: true, visible: true, mustAppear: false, class: 'sticky-element' },
        { text: this.languageStrings.pages.emails.headers.sendDate, value: 'sendDate', sortable: true, visible: true, mustAppear: true, class: 'sticky-element' },
        { text: this.languageStrings.pages.emails.headers.externalId, value: 'externalEmailId', sortable: true, visible: false, mustAppear: false, class: 'sticky-element' }
      ],
      emailBatchListPromise: this.listEmailBatches(),
      emailRecordSearchPromise: Promise.resolve(),
      selectedEntries: []
    }
  },
  computed: {
    resultantHeaders () {
      return this.masterHeaders.filter(h => h.visible)
    }
  },
  methods: {
    ...mapActions({
      listEmailBatches: 'email/getBatches',
      searchEmailsInternal: 'email/getEmailRecords'
    }),
    performSearch () {
      // We need to flatten everything down a bit: away from the batch-first structure, into an email-first one
      this.emailRecordSearchPromise = this.searchEmailsInternal(this.searchCriteria)
        .then((results) => {
          return results.data.flatMap((batch) => {
            for (let i = 0; i < batch.emails.length; i++) {
              batch.emails[i].sourceBatch = batch
            }
            return batch.emails
          })
        })
    },
    selectRow (clickedItem) {
      alert(clickedItem.externalEmailId)
    }
  },
  meta: {
    auth: { required: true }
  },
  head () {
    return {
      title: this.languageStrings.pages.emails.title,
      meta: [
        { hid: 'og:title', content: this.languageStrings.pages.emails.title + ' - Jibberwock Admin' },
        { hid: 'apple-mobile-web-app-title', content: this.languageStrings.pages.emails.title + ' - Jibberwock Admin' }
      ]
    }
  }
}
</script>
