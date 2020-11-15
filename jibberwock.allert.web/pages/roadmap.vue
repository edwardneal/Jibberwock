<template>
  <v-container fill-height fluid class="pa-0 blue darken-3">
    <v-container fill-height fluid class="blue darken-4 rounded-br-pill">
      <v-stepper value="Q4" non-linear class="full-page-component single-header-stepper">
        <v-stepper-header>
          <v-stepper-step step="Q4" editable complete edit-icon="mdi-weather-snowy">
            December 2020
          </v-stepper-step>
          <v-divider />
          <v-stepper-step step="Q1" editable complete edit-icon="mdi-weather-rainy">
            March 2021
          </v-stepper-step>
          <v-divider />
          <v-stepper-step step="Q2" editable complete edit-icon="mdi-weather-sunny">
            June 2021
          </v-stepper-step>
          <v-divider />
          <v-stepper-step step="Q3" editable complete edit-icon="mdi-weather-windy-variant">
            September 2021
          </v-stepper-step>
        </v-stepper-header>

        <v-stepper-content step="Q4" class="pa-0">
          <v-card flat color="rgba(0, 0, 0, 0)">
            <v-card-title class="text-h5">
              December 2020
            </v-card-title>
            <v-card-text class="text-body-1">
              <p>
                By the end of the year, it should be possible to create a tenant, log in and manage it. This functionality should practically end up being shared between Allert and any
                future projects, so will need to be &quot;component-ised&quot; for future use. There should hopefully also be a way to manage the Allert-specific components.
              </p>
              <p>
                I'm thus expecting to have the following functionality by the end of the year:
              </p>
              <ul>
                <li>
                  Tenant sign-up page. This requires two possible scenarios: creating a brand-new tenant from scratch, or adding an Allert subscription to an existing tenant.
                </li>
                <li>
                  Tenant management: contact details, and the ability to migrate from one tier of Allert to another.
                </li>
                <li>
                  Tenant security management. This means that roles can be created, permissions can be assigned to them and users can be placed into them.
                </li>
                <li>
                  Basic Allert management. This is mainly the ability to create alert definitions and alert definition groups.
                </li>
                <li>
                  Creating a background task to compile the alert definitions to DLLs, digitally sign these and update the database to reference them.
                </li>
              </ul>
            </v-card-text>
          </v-card>
        </v-stepper-content>
        <v-stepper-content step="Q1">
          <v-card flat color="rgba(0, 0, 0, 0)">
            <v-card-title class="text-h5">
              March 2021
            </v-card-title>
            <v-card-text class="text-body-1">
              <p>
                March 2021 should be the initial release here. This'll be limited to real-time functionality only, for the time being.
              </p>
              <p>
                This means that I'll be working on:
              </p>
              <ul>
                <li>
                  Tenant management: generating an API key and allowing it to be used for authentication.
                </li>
                <li>
                  Core API functionality: the ability to pass a JSON object to the API, authenticating using API keys and rate-limiting based upon the subscription tier.
                </li>
                <li>
                  Core API functionality: API call to get the currently-active alarms.
                </li>
              </ul>
            </v-card-text>
          </v-card>
        </v-stepper-content>
        <v-stepper-content step="Q2">
          <v-card flat color="rgba(0, 0, 0, 0)">
            <v-card-title class="text-h5">
              June 2021
            </v-card-title>
            <v-card-text class="text-body-1">
              <p>
                June 2021 will largely just be about incremental changes, enabling additional data formats:
              </p>
              <ul>
                <li>
                  XML: enabling access to all attributes and elements.
                </li>
                <li>
                  Plain text: passing the input through as-is.
                </li>
                <li>
                  Form submission: providing the form properties as top-level properties (either as scalar properties or as an array.)
                </li>
                <li>
                  CSV: simply providing nested arrays of lines and fields.
                </li>
                <li>
                  Binary: providing a MemoryStream, but limiting the maximum file size based upon the subscription tier.
                </li>
              </ul>
            </v-card-text>
          </v-card>
        </v-stepper-content>
        <v-stepper-content step="Q3">
          <v-card flat color="rgba(0, 0, 0, 0)">
            <v-card-title class="text-h5">
              September 2021
            </v-card-title>
            <v-card-text class="text-body-1">
              <p>
                If everything goes to plan, I'm hoping to provide a way to &quot;flip&quot; the direction of Allert's data access.
                Rather than a third-party client POSTing data to the API as required, Allert would run in the background and make HTTP requests to a web service on a scheduled basis,
                then generate alerts based on the response.
              </p>
              <p>
                Making a HTTP request also implies credentials, and I'm going to need to encrypt those. I'll use the opportunity to enable
                encryption on alert data too.
              </p>
            </v-card-text>
          </v-card>
        </v-stepper-content>
      </v-stepper>
    </v-container>
  </v-container>
</template>

<style>
  ol { margin-bottom: 16px; }
  p { text-align: justify; }

  .link-container { margin-right: -16px; }
  .full-page-component { height: 100%; width: 100% }

  .v-stepper.single-header-stepper {
      background-color: rgba(0, 0, 0, 0);
      box-shadow: none;
  }
  .v-stepper.single-header-stepper > .v-stepper__header {
      background-color: #303030;
      border-radius: 4px;
  }
  .v-stepper.single-header-stepper > .v-stepper__header > .v-stepper__step {
      border-radius: 0px;
  }
  .v-stepper.single-header-stepper > .v-stepper__header > .v-stepper__step:first-child {
      border-bottom-left-radius: 4px;
  }
  .v-stepper.single-header-stepper > .v-stepper__header > .v-stepper__step:last-child {
      border-bottom-right-radius: 4px;
  }
</style>

<script>
export default {
  components: {
  },
  props: {
    languageStrings: {
      type: Object,
      required: true
    }
  },
  head () {
    return {
      title: this.languageStrings.pages.roadmap.title
    }
  }
}
</script>
