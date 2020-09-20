<template>
    <v-container fill-height fluid style="padding: 0px 5% 0px 5%; align-items: normal;">
        <v-row align="start" align-content="start" justify="start">
            <v-col align="start">
                <h1 class="text-h4 font-weight-light"><v-chip class="mb-1"><v-icon left>mdi-timer-sand</v-icon> WIP</v-chip> Allert</h1>
                <p>
                    Jibberwock Allert is a cloud alerting engine which runs user scripts against input to determine whether or not an alert has occurred.
                    A list of active alerts is stored in Azure Table Storage, and can be retrieved at a later date.
                </p>
            </v-col>
        </v-row>
        <v-row class="d-flex">
            <v-col class="align-self-start">
                <v-img height="318" contain src="/images/allert-architecture.png"></v-img>
            </v-col>
            <v-col class="align-self-start">
                <v-row>
                    <v-col align="left"><h2 class="text-h4 font-weight-light">Architecture</h2></v-col>
                </v-row>
                <v-row>
                    <v-col>
                        <p>
                            The high-level architecture of Allert involves two API calls. The first API call pushes data into an Alert Definition Group, receiving a list of alerts which this data has triggered.
                            The second API call lists the alerts which are currently active in an Alert Definition Group.
                        </p>
                        <p>
                            The process is shown in the diagram opposite:
                        </p>
                        <ol>
                            <li>A system makes the initial HTTP request to the REST API, passing a JSON object.</li>
                            <li>Every C# or VB.Net script in the Alert Definition Group is run in parallel.</li>
                            <li>The list of alerts returned from each of the scripts is collated.</li>
                            <li>The full list of the currently-active alerts in the Alert Definition Group is updated.</li>
                            <li>The list of alerts from step 3 is returned to the system from step 1.</li>
                            <li>Another client can request the full list of the currently-active alerts in the Alert Definition Group.</li>
                        </ol>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
        <v-row class="d-flex">
            <v-col class="align-self-start">
                <v-row>
                    <v-col align="left"><h2 class="text-h4 font-weight-light">Use Cases</h2></v-col>
                </v-row>
                <v-row>
                    <v-col>
                        <p>
                            Allert refactors the alert detection logic from a monitoring system into a separate service, enabling checks to be run against data from any data source.
                        </p>
                        <h3 class="text-h5 font-weight-light">Complex, testable alerting logic</h3>
                        <p>
                            Many existing monitoring systems simply have a list of limits - Very Low, Low, High, Very High - or a custom scripting language to determine if a specific alert triggered. This logic can only be tested when data starts to flow into the system. 
                            By using standard scripting languages (C# and VB.Net), Allert eliminates the need to learn and use any custom scripting languages, while also enabling scripts to be more complex. The decoupling between the data's source and the alerting logic also means that this alerting logic can be mocked and unit tested prior to deployment.
                        </p>
                        <p>
                            A script can also return multiple instances of a specific alert, which allows you to generate alerts from aggregated data (such as result sets from an on-premise database, or metrics from a web service.)
                        </p>
                        <h3 class="text-h5 font-weight-light">Consolidated alerting</h3>
                        <p>
                            Typically, monitoring will involve pulling data out of a client and storing it into a database. This can cause problems when it comes to handling notifications and alerts from cloud-based architectures, since there has to be either a separate alerting path to receive ad-hoc notifications and inject it into the monitoring system, or an agent which stores these notifications and allows the monitoring system to receive them on the next poll.
                        </p>
                        <p>
                            Allert is designed to have data pushed into it from the outset, which means that it handles web hooks natively. This in turn means that you can generate alerts from Atlassian Status (aka statuspage.io) notifications, Stripe payments/refunds/disputes and so on. 
                            A slight variant of this would be to use Amazon SNS, Google Cloud Monitoring or an Azure LogicApp to push data to the Allert API as a web hook; while doing so won't necessarily centralise your alerting logic, it will consolidate your alerts into a &quot;single pane of glass.&quot;
                        </p>
                        <p>
                            Another alternative approach is to use multiple monitoring systems to gather data from proprietary hardware or software, (or from multiple sites, departments or clients) then push a snapshot of each system's current values to Allert. 
                            This simplifies the monitoring system configuration, enables more complex alerting logic and &quot;decloaks&quot; alerts and root causes which might not have been visible or accessible by bringing them together into one place.
                        </p>
                    </v-col>
                </v-row>
            </v-col>
            <v-col class="align-self-center">
                <v-img height="387" contain src="/images/allert-use-cases.png"></v-img>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
    export default {
        head: {
            title: 'Allert - Jibberwock',
            meta: [
                { hid: 'description', name: 'description', content: 'Jibberwock Allert is a cloud alerting engine which evaluates data from any source and runs user-specified rules to generate alerts.' },
                { hid: 'og-title', property: 'og:title', content: 'Allert - Jibberwock' },
                { hid: 'og-url', property: 'og:url', content: 'https://www.jibberwock.com/allert' },
            ]
        },

        data: () => ({
        })
    }
</script>
