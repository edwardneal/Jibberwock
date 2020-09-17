<template>
    <v-container fill-height fluid style="padding: 0px 5% 0px 5%; align-items: normal;">
        <v-row align="start" align-content="start" justify="start">
            <v-col align="start">
                <h1 class="text-h4 font-weight-light">About</h1>
                <p>
                    Jibberwock is a site to display a number of open-source projects, both completed and in-progress.
                    <router-link to="/allert">Allert</router-link> is a cloud alerting engine which runs user scripts against input to determine whether or not an alert has occurred.
                    A <router-link to="/statuspageio-client">statuspage.io client</router-link> provides a way for .NET applications and PowerShell scripts to access
                    any <a href="https://www.atlassian.com/software/statuspage" target="_blank" rel="noopener">statuspage.io</a> (aka Atlassian Statuspage) pages to find their status and historical outages.
                    <router-link to="/proxyvirt">ProxyVirt</router-link> is an Azure IoT Hub Edge Module which acts as a standard SOCKS proxy, but dynamically determines a path to the destination server.
                </p>
                <h1 class="text-h4 font-weight-light">Privacy</h1>
                <p>Your privacy is important to us, and will be respected regarding any information we may collect from you across our website and other sites we own and operate.</p>
                <p>We only ask for personal information when we truly need it to provide a service to you. We collect it by fair and lawful means, with your knowledge and consent. We also let you know why we're collecting it and how it will be used.</p>
                <p>We only retain collected information for as long as necessary to provide you with your requested service. What data we store, we'll protect within commercially acceptable means to prevent loss and theft, as well as unauthorised access, disclosure, copying, use or modification.</p>
                <p>We don't share any personally identifying information publicly or with third-parties, except when required to by law.</p>
                <p>Our website may link to external sites that are not operated by us. Please be aware that we have no control over the content and practices of these sites, and cannot accept responsibility or liability for their respective privacy policies.</p>
                <p>You are free to refuse our request for your personal information, with the understanding that we will be unable to provide you with some of your desired services.</p>
                <p>Your continued use of our website will be regarded as acceptance of our practices around privacy and personal information. If you have any questions about how we handle user data and personal information, feel free to contact us.</p>
                <h2 class="text-h4 font-weight-light">Cookies</h2>
                <p>The <a href="https://www.cookiesandyou.com/">Cookies &amp; You</a> website explains what cookies are. The Jibberwock website uses cookies to improve performance and analyze website traffic. This may mean sharing site usage data with our analytics partners.</p>
                <p>At present, we gather data using Cloudflare, Google Analytics and Microsoft Application Insights in order to perform our performance and traffic analysis.</p>
                <h1 class="text-h4 font-weight-light">Attributions</h1>
                <p>The background image on the homepage was <a href="https://www.freepik.com" target="_blank" rel="noopener">designed by jemastock / Freepik</a>.</p>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
    export default {
        name: 'Privacy',
        title: 'Privacy - Jibberwock',

        data: () => ({
        })
    }
</script>
