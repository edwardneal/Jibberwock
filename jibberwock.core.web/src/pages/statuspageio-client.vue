<template>
    <v-container fill-height fluid style="padding: 0px 5% 0px 5%; align-items: normal;">
        <v-row align="start" align-content="start" justify="start">
            <v-col align="start">
                <h1 class="text-h4 font-weight-light">statuspage.io Client</h1>
                <p>
                    <a href="https://www.statuspage.io" target="_blank" rel="noopener">statuspage.io</a> (later <a href="https://techcrunch.com/2016/07/14/atlassian-acquires-statuspage" target="_blank" rel="noopener">acquired by Atlassian</a> and renamed Atlassian Status) is a fairly common service which provides a hosted status page.
                    Part of this service is a publicly-accessible REST API, which enables people and services to get the current status, any current incidents and scheduled maintenance events.
                    While there are a few clients which enable .NET Framework applications to access the private REST API (which enables them to push statuses into statuspage.io), there are very few simple ways to deal with the public REST API. This library meets that need.
                    Its source is also available on <a href="https://github.com/edwardneal/StatusPageClient" target="_blank" rel="noopener">GitHub</a>.
                </p>
            </v-col>
        </v-row>
        <v-row class="d-flex">
            <v-col class="align-self-start">
                <v-row>
                    <v-col align="left"><h2 class="text-h4 font-weight-light">Use Cases &amp; Caveats</h2></v-col>
                </v-row>
                <v-row>
                    <v-col>
                        <p>
                            Since Atlassian Status is used so widely, being able to read data from its API allows us to monitor the status of a large number of &quot;infrastructure&quot; services, including GitHub, Cloudflare, SendGrid, Dropbox and Atlassian Status itself.
                            We might just want to record the current status to help build root cause analyses if there's an outage, or we could do more complex work and queue up background tasks until the service is available, or fail over to a secondary provider.
                            We could also expose this information using ASP.NET's existing health check infrastructure (at which point a layer seven load balancer might be able to divert users to a static page informing them of the downtime.)
                        </p>
                        <p>
                            Whatever your use case though, the APIs are rate-limited. Repeated calls to these APIs isn't advisable. A Windows Service, Azure Function App, Lambda Function or similar background task would be a better fit here, polling the APIs for their current status and writing it into a local data store.
                        </p>
                    </v-col>
                </v-row>
            </v-col>
            <v-col class="align-self-start">
                <v-row>
                    <v-col align="left"><h2 class="text-h4 font-weight-light">Getting Started</h2></v-col>
                </v-row>
                <v-row>
                    <v-col>
                        <p>
                            The Atlassian Status client is available in the <a href="https://www.nuget.org/packages/StatusPageIOClient/" target="_blank" rel="noopener">NuGet Gallery</a>. You can install it by running one of the commands below:
                        </p>
                        <p>Package Manager: <kbd>Install-Package StatusPageIOClient</kbd></p>
                        <p>.NET CLI: <kbd>dotnet add package StatusPageIOClient</kbd></p>
                        <p>
                            Then, reference the <code>StatusPageClient</code> class in code as below:
                        </p>
                        <p>
                            <pre class="pl-6">
var client = new StatusPageClient("<var>PageId</var>")
    {
        RetrieveAllMaintenanceEvents = true,
        RetrieveAllIncidents = true
    };

await client.RefreshAsync();
</pre>
                        </p>
                        <p>
                            You can now access <code>client.StatusPage</code> to access the results of the API calls.
                        </p>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
        <v-row class="d-flex">
            <v-col class="align-self-start">
                <v-row>
                    <v-col align="left"><h2 class="text-h4 font-weight-light">Data Model</h2></v-col>
                </v-row>
                <v-row>
                    <v-col>
                        <p>There are four key items in the data model: Pages, Components, Incidents, Incident Updates and Scheduled Maintenances.</p>
                        <p>
                            A <strong>Page</strong> lies at the root of the Atlassian Status data model. It refers to a single status page, and contains many <strong>Components</strong>, <strong>Incidents</strong> and <strong>Scheduled Maintenances</strong>. 
                            You can access this using the <code>StatusPage</code> property, which returns an instance of the <code>StatusPageClient.Models.StatusPage</code> class.
                        </p>
                        <p>
                            Each <strong>Page</strong> has a hierarchy of <strong>Component</strong>s. Each item in the hierarchy has a status, name and description. A <strong>Component</strong> is represented by an instance of the <code>StatusPageClient.Models.Component</code> class.
                        </p>
                        <p>
                            An <strong>Incident</strong> is the most complex item in the data model. It has a name (and a direct link to the Atlassian Status web interface), a description of the impact, the start/resolution dates, a list of <strong>Incident Updates</strong>, the current status and a list of impacted <strong>Components</strong>.
                            Each <strong>Incident</strong> is represented by an instance of the <code>StatusPageClient.Models.Incident</code> class.
                        </p>
                        <p>
                            <strong>Incident</strong>s also have many <strong>Incident Updates</strong>. Each of these updates has a piece of text, a status, and an optional link to a Twitter tweet. It is represented by an instance of the <code>StatusPage.Models.IncidentUpdate</code> class.
                        </p>
                        <p>
                            Finally, a <strong>Page</strong> also has many <strong>Scheduled Maintenance</strong>s. These are actually variants of <strong>Incident</strong>s, and have all their fields, plus two additional items which contain the scheduled start/end dates.
                            A <strong>Scheduled Maintenance</strong> is represented by an instance of the <code>StatusPageClient.Models.ScheduledMaintenance</code> class.
                        </p>
                    </v-col>
                </v-row>
            </v-col>
            <v-col class="align-self-start">
                <v-row>
                    <v-col align="left"><h2 class="text-h4 font-weight-light">Common Page IDs</h2></v-col>
                </v-row>
                <v-row>
                    <v-col>
                        <p>
                            These Page IDs can be passed to the constructor of <code>StatusPageClient</code>.
                        </p>
                        <v-simple-table>
                            <thead>
                                <tr>
                                    <th class="text-left">Page ID</th>
                                    <th class="text-left">Product</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="pg in statusPages" :key="pg.id">
                                    <td>{{ pg.id }}</td>
                                    <td><a :href="pg.link" target="_blank" rel="noopener">{{ pg.name }}</a></td>
                                </tr>
                            </tbody>
                        </v-simple-table>
                        <p>
                            You'll note that many of these status pages look very similar. In general though, add <code>/api</code> to the end of the URL; alternatively, hit <kbd>F12</kbd> and refresh the page, then look for a web request to <code>status.json</code>. 
                            In the response, look at the <code>page</code> property's <code>id</code> property. This will typically be a twelve-character alphanumeric string.
                        </p>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
    import statusPages from '@/static-data/status-pages.js';

    export default {
        name: 'StatusPageIOClient',
        title: 'statuspage.io Client - Jibberwock',
        head: {
            title: 'statuspage.io Client - Jibberwock',
            meta: [
                { hid: 'description', name: 'description', content: 'The statuspage.io client allows .NET and PowerShell clients to access data from a public statuspage.io site.' },
                { hid: 'og-title', property: 'og:title', content: 'statuspage.io Client - Jibberwock' },
                { hid: 'og-url', property: 'og:url', content: 'https://www.jibberwock.com/statuspageio-client' },
            ]
        },

        data: () => ({
            statusPages: statusPages
        })
    }
</script>
