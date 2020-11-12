<template>
  <v-sheet>
    <v-row justify="center" align="center">
      <v-col cols="12">
        {{ languageStrings.pages.products.instructions }}
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-col sm="12" md="6" cols="12">
        <PromisedTable
          ref="productList"
          :language-strings="languageStrings"
          :headers="productListHeaders"
          :populate-function="listProducts"
          @selection-changed="updateProductSelection"
        >
          <template v-slot:toolbar-actions="{ shouldDisable }">
            <v-toolbar-items>
              <v-btn depressed :disabled="shouldDisable" class="pl-3" @click="performProductRefresh">
                <v-icon>mdi-refresh</v-icon>
                {{ languageStrings.pages.products.actions.refresh }}
              </v-btn>
              <v-btn depressed :disabled="shouldDisable" class="pl-3" @click="showCreateProductForm">
                <v-icon>mdi-pencil</v-icon>
                {{ languageStrings.pages.products.actions.create }}
              </v-btn>
            </v-toolbar-items>
          </template>
        </PromisedTable>
        <CreateProductForm
          :language-strings="languageStrings"
          :visible.sync="createProductFormVisible"
          @created="performProductRefresh"
        />
      </v-col>
      <v-col sm="12" md="6" cols="12">
        <v-card v-if="computedSelection === null" elevation="0">
          <v-card-title>{{ languageStrings.pages.products.detailsPanel.title }}</v-card-title>
          <v-card-text>
            {{ languageStrings.pages.products.errorMessages.selectProductToEdit }}
          </v-card-text>
        </v-card>
        <UpdateProductForm
          v-else
          :language-strings="languageStrings"
          :title="languageStrings.pages.characteristics.detailsPanel.title"
          :product.sync="computedSelection"
        />
      </v-col>
    </v-row>
  </v-sheet>
</template>

<script>
import { mapActions } from 'vuex'
import PromisedTable from '~/components/PromisedTable.vue'
import CreateProductForm from '~/components/CreateProductForm.vue'
import UpdateProductForm from '~/components/UpdateProductForm.vue'

export default {
  components: {
    PromisedTable,
    CreateProductForm,
    UpdateProductForm
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      productListHeaders: [
        { text: this.languageStrings.pages.products.headers.name, value: 'name', sortable: true, filterable: true },
        { text: this.languageStrings.pages.products.headers.resourceIdentifier, value: 'resourceIdentifier', sortable: true, filterable: true },
        { text: this.languageStrings.pages.products.headers.visible, value: 'visible', sortable: true, filterable: true }
      ],
      productDetails: {
        selection: []
      },
      createProductFormVisible: false
    }
  },
  computed: {
    computedSelection: {
      get () {
        return (this.productDetails.selection === null || this.productDetails.selection.length === 0) ? null : this.productDetails.selection[0]
      },
      set (value) {
        if (this.productDetails.selection.length === 0) {
          this.productDetails.push(value)
        } else {
          // This is to update the existing table records without having to perform a disruptive refresh
          // We're updating a specific reference, rather than replacing it with another one entirely
          Object.getOwnPropertyNames(value)
            .forEach((prop) => {
              this.productDetails.selection[0][prop] = value[prop]
            })
        }
      }
    }
  },
  methods: {
    ...mapActions({
      listProducts: 'products/list'
    }),
    performProductRefresh () {
      this.$refs.productList.refresh()
    },
    updateProductSelection (value) {
      this.productDetails.selection = value
    },
    showCreateProductForm () {
      this.createProductFormVisible = true
    }
  },
  meta: {
    auth: { required: true }
  },
  head () {
    return {
      title: this.languageStrings.pages.products.title,
      meta: [
        { hid: 'og:title', content: this.languageStrings.pages.products.title + ' - Jibberwock Admin' },
        { hid: 'apple-mobile-web-app-title', content: this.languageStrings.pages.products.title + ' - Jibberwock Admin' }
      ]
    }
  }
}
</script>
