<template>
  <Promised :promise="auditTrailSearchPromise">
    <template v-slot:combined="{ isPending, error, data }">
      <v-sheet>
        <v-row justify="center" align="center">
          <v-col cols="12">
            {{ languageStrings.pages.audit_trail.instructions }}
          </v-col>
        </v-row>
        <v-row justify="center" align="center">
          <v-col sm="6" md="3" cols="6">
            <CalendarDropdown :label="languageStrings.pages.audit_trail.fields.startDate" :selected-date.sync="searchCriteria.startTime" :max-date="searchCriteria.endTime" />
            <CalendarDropdown :label="languageStrings.pages.audit_trail.fields.endDate" :selected-date.sync="searchCriteria.endTime" :min-date="searchCriteria.startTime" />
          </v-col>
          <v-col sm="6" md="3" cols="6">
            <UserDropdown :language-strings="languageStrings" :label="languageStrings.pages.audit_trail.fields.performedBy" :selected-user-id.sync="searchCriteria.performedBy" />
            <v-select
              v-model="searchCriteria.eventType"
              :items="languageStrings.pages.audit_trail.eventTypes"
              :label="languageStrings.pages.audit_trail.fields.eventType"
              item-text="label"
              item-value="id"
              hide-details
              clearable
            />
          </v-col>
          <v-col sm="6" md="3" cols="6">
            <UserDropdown :language-strings="languageStrings" :label="languageStrings.pages.audit_trail.fields.relatedUser" :selected-user-id.sync="searchCriteria.relatedUserId" />
            <TenantDropdown :language-strings="languageStrings" :label="languageStrings.pages.audit_trail.fields.relatedTenant" :selected-user-id.sync="searchCriteria.relatedTenantId" />
          </v-col>
          <v-col sm="6" md="3" cols="6">
            <v-btn :loading="isPending" small color="primary" @click="performSearch">
              {{ languageStrings.actions.search }}
            </v-btn>
            <v-menu offset-y :close-on-content-click="false">
              <template v-slot:activator="{ on, attrs }">
                <v-btn small color="primary" v-bind="attrs" v-on="on">
                  {{ languageStrings.pages.audit_trail.actions.showColumns }}
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
                  <span v-bind="attrs" v-on="on">{{ languageStrings.validationErrorMessages.unableToGetAuditTrail }}</span>
                </template>
                <span>{{ error.message }}</span>
              </v-tooltip>
            </v-alert>
            <v-row>
              <v-col sm="12" lg="8" cols="12">
                <v-data-table
                  v-model="selectedEntries"
                  :headers="resultantHeaders"
                  :loading="isPending"
                  :items="typeof data !== 'undefined' && data !== null ? data.data : []"
                  single-select
                  fixed-header
                  class="sticky-data-table"
                  @click:row="selectRow"
                >
                  <template v-slot:item.occurrence="{ item }">
                    {{ new Date(item.occurrence).toLocaleString() }}
                  </template>
                  <template v-slot:item.type="{ item }">
                    {{ getEventTypeName(item.type) }}
                  </template>
                </v-data-table>
              </v-col>
              <v-col sm="12" lg="4" cols="12">
                <div v-if="selectedEntries.length > 0" class="sticky-element">
                  <v-expansion-panels :value="[0, 1]" flat accordion multiple>
                    <v-expansion-panel>
                      <v-expansion-panel-header>{{ languageStrings.pages.audit_trail.accordionHeaders.generalDetails }}</v-expansion-panel-header>
                      <v-expansion-panel-content>
                        <v-simple-table>
                          <tbody>
                            <tr>
                              <td>{{ languageStrings.pages.audit_trail.headers.type }}</td>
                              <td>{{ getEventTypeName(selectedEntries[0].type) }}</td>
                            </tr>
                            <tr>
                              <td>{{ languageStrings.pages.audit_trail.headers.occurrence }}</td>
                              <td>{{ new Date(selectedEntries[0].occurrence).toLocaleString() }}</td>
                            </tr>
                            <tr>
                              <td>{{ languageStrings.pages.audit_trail.headers.performedBy }}</td>
                              <td>{{ selectedEntries[0].performedBy.name }}</td>
                            </tr>
                            <tr>
                              <td>{{ languageStrings.pages.audit_trail.headers.originatingService }}</td>
                              <td>{{ selectedEntries[0].originatingService.name }}</td>
                            </tr>
                            <tr>
                              <td>{{ languageStrings.pages.audit_trail.headers.originatingConnectionId }}</td>
                              <td>{{ selectedEntries[0].originatingConnectionId }}</td>
                            </tr>
                            <tr>
                              <td>{{ languageStrings.pages.audit_trail.headers.comment }}</td>
                              <td>{{ selectedEntries[0].comment }}</td>
                            </tr>
                          </tbody>
                        </v-simple-table>
                      </v-expansion-panel-content>
                    </v-expansion-panel>
                    <v-expansion-panel>
                      <v-expansion-panel-header>{{ languageStrings.pages.audit_trail.accordionHeaders.specificDetails.replace('{type}', getEventTypeName(selectedEntries[0].type)) }}</v-expansion-panel-header>
                      <v-expansion-panel-content>
                        <component :is="getEntryDetailsComponent(selectedEntries[0].type)" :language-strings="languageStrings" :entry="selectedEntries[0]" />
                      </v-expansion-panel-content>
                    </v-expansion-panel>
                  </v-expansion-panels>
                </div>
              </v-col>
            </v-row>
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
import UserDropdown from '@/components/UserDropdown.vue'
import TenantDropdown from '@/components/TenantDropdown.vue'
import EditUser from '@/components/audit_trail_entry_displays/EditUser.vue'
import EditProduct from '@/components/audit_trail_entry_displays/EditProduct.vue'
import EditCharacteristic from '@/components/audit_trail_entry_displays/EditCharacteristic.vue'
import EditTier from '@/components/audit_trail_entry_displays/EditTier.vue'
import EditNotification from '@/components/audit_trail_entry_displays/EditNotification.vue'
import DismissNotification from '@/components/audit_trail_entry_displays/DismissNotification.vue'

export default {
  components: {
    Promised,
    CalendarDropdown,
    UserDropdown,
    TenantDropdown,
    EditUser,
    EditProduct,
    EditCharacteristic,
    EditTier,
    EditNotification,
    DismissNotification
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
        performedBy: null,
        eventType: null,
        relatedUserId: null,
        relatedTenantId: null
      },
      masterHeaders: [
        { text: this.languageStrings.pages.audit_trail.headers.occurrence, value: 'occurrence', sortable: true, visible: true, mustAppear: true, class: 'sticky-element' },
        { text: this.languageStrings.pages.audit_trail.headers.relatedUser, value: 'relatedUser.name', sortable: true, visible: true, mustAppear: false, class: 'sticky-element' },
        { text: this.languageStrings.pages.audit_trail.headers.relatedTenant, value: 'relatedTenant.name', sortable: true, visible: true, mustAppear: false, class: 'sticky-element' },
        { text: this.languageStrings.pages.audit_trail.headers.performedBy, value: 'performedBy.name', sortable: true, visible: true, mustAppear: false, class: 'sticky-element' },
        { text: this.languageStrings.pages.audit_trail.headers.originatingConnectionId, value: 'originatingConnectionId', sortable: true, visible: false, mustAppear: false, class: 'sticky-element' },
        { text: this.languageStrings.pages.audit_trail.headers.originatingService, value: 'originatingService.name', sortable: true, visible: false, mustAppear: false, class: 'sticky-element' },
        { text: this.languageStrings.pages.audit_trail.headers.type, value: 'type', sortable: true, visible: true, mustAppear: true, class: 'sticky-element' },
        { text: this.languageStrings.pages.audit_trail.headers.comment, value: 'comment', sortable: true, visible: true, mustAppear: false, class: 'sticky-element' }
      ],
      auditTrailSearchPromise: Promise.resolve(),
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
      getAuditTrailInternal: 'audit-trail/getAuditTrail'
    }),
    getEventTypeName (id) {
      const matchingType = this.languageStrings.pages.audit_trail.eventTypes.find(t => t.id === id)

      return matchingType.label
    },
    performSearch () {
      this.auditTrailSearchPromise = this.getAuditTrailInternal(this.searchCriteria)
    },
    selectRow (_clickedItem, itemSlotData) {
      itemSlotData.select(true)
    },
    getEntryDetailsComponent (id) {
      const matchingType = this.languageStrings.pages.audit_trail.eventTypes.find(t => t.id === id)

      return matchingType.component
    }
  },
  meta: {
    auth: { required: true }
  },
  head () {
    return {
      title: this.languageStrings.pages.audit_trail.title,
      meta: [
        { hid: 'og:title', content: this.languageStrings.pages.audit_trail.title + ' - Jibberwock Admin' },
        { hid: 'apple-mobile-web-app-title', content: this.languageStrings.pages.audit_trail.title + ' - Jibberwock Admin' }
      ]
    }
  }
}
</script>
