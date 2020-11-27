<template>
  <div>
    <span class="text-h6">Alert Priorities</span>
    <v-simple-table>
      <thead>
        <tr>
          <th class="text-left">
            Priority
          </th>
          <th class="text-left">
            Name
          </th>
          <th class="text-left">
            Description
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(prop, propIdx) in [0, 1, 2, 3, 4, 5]" :key="propIdx">
          <td>
            {{ prop }}
          </td>
          <td>
            <v-text-field v-model="productConfiguration.defaultProductConfiguration.configuration.alertPriorityNames[prop].name" hide-details solo flat dense />
          </td>
          <td>
            <v-text-field v-model="productConfiguration.defaultProductConfiguration.configuration.alertPriorityNames[prop].description" hide-details solo flat dense />
          </td>
        </tr>
      </tbody>
    </v-simple-table>
  </div>
</template>

<script>
export default {
  components: {
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    },
    productConfiguration: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
    }
  }
}
</script>
