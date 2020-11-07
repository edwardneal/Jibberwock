<template>
  <v-sheet>
    <v-row justify="center" align="center">
      <v-col cols="12">
        {{ languageStrings.pages.characteristics.instructions }}
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-col sm="12" md="6" cols="12">
        <PromisedTable
          ref="characteristicList"
          :language-strings="languageStrings"
          :headers="characteristicListHeaders"
          :populate-function="listCharacteristics"
          @selection-changed="updateSelection"
        >
          <template v-slot:toolbar-actions="{ shouldDisable }">
            <v-toolbar-items>
              <v-btn depressed :disabled="shouldDisable" class="pl-3" @click="performRefresh">
                <v-icon>mdi-refresh</v-icon>
                {{ languageStrings.pages.characteristics.actions.refresh }}
              </v-btn>
              <v-btn depressed :disabled="shouldDisable" class="pl-3" @click="showCreateForm">
                <v-icon>mdi-pencil</v-icon>
                {{ languageStrings.pages.characteristics.actions.create }}
              </v-btn>
              <v-btn depressed :disabled="shouldDisable || characteristicDetails.selection === null" class="pl-3" @click="showDeleteForm">
                <v-icon>mdi-delete-forever</v-icon>
                {{ languageStrings.pages.characteristics.actions.delete }}
              </v-btn>
            </v-toolbar-items>
          </template>
        </PromisedTable>
      </v-col>
      <v-col sm="12" md="6" cols="12">
        <v-card v-if="computedSelection === null" elevation="0">
          <v-card-title>{{ languageStrings.pages.characteristics.detailsPanel.title }}</v-card-title>
          <v-card-text>
            {{ languageStrings.pages.characteristics.errorMessages.selectCharacteristicToEdit }}
          </v-card-text>
        </v-card>
        <UpdateProductCharacteristicForm
          v-else
          :language-strings="languageStrings"
          :title="languageStrings.pages.characteristics.detailsPanel.title"
          :characteristic.sync="computedSelection"
        />
      </v-col>
    </v-row>
    <CreateProductCharacteristicForm
      :language-strings="languageStrings"
      :visible.sync="createFormVisible"
      @created="performRefresh"
    />
    <ProgressDialog
      :language-strings="languageStrings"
      :prompt="formatPlural(languageStrings.dialogs.deleteProductCharacteristicConfirmation, characteristicDetails.selection)"
      :activity-promise-factory="deleteCharacteristicPromiseFactory"
      :confirm-button-text="languageStrings.actions.confirm"
      :cancel-button-text="languageStrings.actions.cancel"
    />
  </v-sheet>
</template>

<script>
import { mapActions } from 'vuex'
import PromisedTable from '~/components/PromisedTable.vue'
import UpdateProductCharacteristicForm from '~/components/UpdateProductCharacteristicForm.vue'
import CreateProductCharacteristicForm from '~/components/CreateProductCharacteristicForm.vue'

export default {
  components: {
    PromisedTable,
    UpdateProductCharacteristicForm,
    CreateProductCharacteristicForm
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      characteristicListHeaders: [
        { text: this.languageStrings.pages.characteristics.headers.name, value: 'name', sortable: true, filterable: true },
        { text: this.languageStrings.pages.characteristics.headers.description, value: 'description', sortable: true, filterable: true },
        { text: this.languageStrings.pages.characteristics.headers.enabled, value: 'enabled', sortable: true, filterable: true }
      ],
      characteristicDetails: {
        selection: []
      },
      deleteCharacteristicPromiseFactory: null,
      createFormVisible: false
    }
  },
  computed: {
    computedSelection: {
      get () {
        return (this.characteristicDetails.selection === null || this.characteristicDetails.selection.length === 0) ? null : this.characteristicDetails.selection[0]
      },
      set (value) {
        if (this.characteristicDetails.selection.length === 0) {
          this.characteristicDetails.push(value)
        } else {
          // This is to update the existing table records without having to perform a disruptive refresh
          // We're updating a specific reference, rather than replacing it with another one entirely
          Object.getOwnPropertyNames(value)
            .forEach((prop) => {
              this.characteristicDetails.selection[0][prop] = value[prop]
            })
        }
      }
    }
  },
  methods: {
    ...mapActions({
      listCharacteristics: 'product-characteristic/list',
      deleteCharacteristic: 'product-characteristic/delete'
    }),
    formatPlural (languageString, collection) {
      const replacementPlural = collection.length === 1 ? '' : 's'
      const replacementThis = collection.length === 1 ? 'this' : 'these'

      return languageString.replace('{thisPlural}', replacementThis).replace('{plural}', replacementPlural)
    },
    performRefresh () {
      this.$refs.characteristicList.refresh()
    },
    updateSelection (value) {
      this.characteristicDetails.selection = value
    },
    showCreateForm () {
      this.createFormVisible = true
    },
    showDeleteForm () {
      const selectedCharacteristics = this.characteristicDetails.selection
      const deleteSingleCharacteristic = (ch) => {
        return this.deleteCharacteristic(ch.id)
      }

      this.deleteCharacteristicPromiseFactory = () => {
        return Promise.all(selectedCharacteristics.map(deleteSingleCharacteristic))
      }
    }
  },
  meta: {
    auth: { required: true }
  },
  head () {
    return {
      title: this.languageStrings.pages.characteristics.title,
      meta: [
        { hid: 'og:title', content: this.languageStrings.pages.characteristics.title + ' - Jibberwock Admin' },
        { hid: 'apple-mobile-web-app-title', content: this.languageStrings.pages.characteristics.title + ' - Jibberwock Admin' }
      ]
    }
  }
}
</script>
