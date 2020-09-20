<template>
    <v-container fill-height fluid style="padding: 0px 5% 0px 5%; align-items: normal;">
        <v-row align="start" align-content="start" justify="start">
            <v-col align="start">
                <h1 class="text-h4 font-weight-light"><v-chip class="mb-1"><v-icon left>mdi-timer-sand</v-icon> WIP</v-chip> ProxyVirt</h1>
                <p>
                    ProxyVirt is designed to provide dynamic layer-five routing from an Azure IoT Edge container to a third-party server (whether running elsewhere in an intranet, or on the Internet.)
                    This is designed to make it easier to develop ad-hoc IoT deployments, but it also makes it much easier to build a highly-available REST client because the TCP-level retry logic can be handled by ProxyVirt.
                </p>
            </v-col>
        </v-row>
        <v-row class="d-flex">
            <v-col class="align-self-start">
                <v-row>
                    <v-col align="left"><h2 class="text-h4 font-weight-light">Architecture</h2></v-col>
                </v-row>
                <v-row>
                    <v-col>
                        <p>
                            ProxyVirt runs as a single Azure IoT Edge module: the proxy forwarder. This module exposes a standard SOCKS proxy on a static port (62000), and all other modules will use [proxyvirt:62000] as their proxy.
                            This will direct all HTTP/HTTPS traffic into ProxyVirt (the &quot;data plane&quot;)
                        </p>
                        <p>
                            In parallel to this, ProxyVirt modules can be configured to advertise their Internet connectivity in some way, and recognise the advertisements of ProxyVirt modules running around them.
                            This advertisement publishing/discovery might take some of the following forms:
                        </p>
                        <ul>
                            <li><a href="https://www.consul.io/" target="_blank" rel="noopener">Consul</a></li>
                            <li><a href="https://etcd.io/" target="_blank" rel="noopener">etcd</a></li>
                            <li><a href="https://cassandra.apache.org/" target="_blank" rel="noopener">Cassandra</a></li>
                            <li>DNS lookups (such as WPAD)</li>
                            <li>Network broadcasts</li>
                            <li>&quot;Special&quot; protocols which define no proxy, or a static corporate proxy</li>
                        </ul>
                        <p>
                            Each ProxyVirt module will need to have a measure of configuration which describes what kind of priority each discovery mechanism might have, as well as mechanism-specific configuration.
                        </p>
                        <p>
                            In a sense, ProxyVirt becomes a layer-five equivalent to an IP routing protocol such as BGP: the discovery mechanism priorities might be considered the equivalent of routing table metrics, a ProxyVirt module will discover and advertise itself on the network, and the results of the discovery process will control how traffic is routed.
                        </p>
                        <p>
                            Once a new upstream proxy has been discovered and verified, it will sit in the proxy forwarder's list of candidates. The proxy forwarder can then select an upstream proxy from that list, open a TCP connection and start forwarding data to it.
                            This process of selecting an upstream forwarder can be quite flexible, given that the proxy knows how well the upstream forwarder is performing, how many connection failures have occurred and how much traffic has already been sent to it (for upstream metered connections.)
                        </p>
                        <p>
                            The net result of this should be that TCP connections will &quot;Just Work&quot; as long as there's some kind of upstream connectivity nearby. The proxy forwarder will automatically try to fail over to a new upstream proxy if a connection fails, which introduces a degree of high availability and ensures that IoT connection layers can remain quite simple.
                            In one situation, there might be a large number Azure IoT Edge devices which are passively gathering and aggregating air quality data from a sensor. The ProxyVirt modules on them could be configured to try to use one another as upstream proxies. If one of these devices was plugged in, all of the devices would be able to transparently upload their data.
                        </p>
                    </v-col>
                </v-row>
            </v-col>
            <v-col class="align-self-start">
                <v-row>
                    <v-col align="left"><h2 class="text-h4 font-weight-light">ProxyVirt vs. Transparent IoT Edge gateway</h2></v-col>
                </v-row>
                <v-row>
                    <v-col>
                        <p>
                            Microsoft offer a very similar service in the form of an IoT Edge gateway, and documentation for this is available <a href="https://docs.microsoft.com/en-us/azure/iot-edge/iot-edge-as-gateway" target="_blank" rel="noopener">here</a>.
                            The closest equivalent is a Transparent gateway, where the downstream devices are registered as devices in IoT Hub in their own right; the Azure IoT Edge runtime acts as a layer-seven proxy from those devices to the IoT Hub.
                            There are three main disadvantages to this:
                        </p>
                        <ol>
                            <li>It isn't currently possible to have one IoT Edge act as a transparent gateway for another IoT Edge device. This means that while we might well have any number of non-Edge devices polling a sensor, a device running something like an Azure Blob Storage module needs to have its own transparent gateway</li>
                            <li>The configuration for a transparent gateway is fairly static. The gateway needs its own certificate chain, and the connection string for a downstream device needs to include the gateway name. While this works for a simple, static configuration, something more dynamic rapidly becomes difficult</li>
                            <li>While a transparent IoT Edge gateway functions well enough as a proxy for IoT Hub devices, it doesn't provide a HTTP proxy. As a result, more complex scenarios where custom applications want to access an Internet-facing API directly are impossible without using a SOCKS proxy</li>
                        </ol>
                        <p>
                            Both systems could be used in parallel; we could quite easily imagine a central IoT Hub with multiple IoT Edge devices acting as transparent gateways and &quot;grouping&quot; devices in the same part of the building together to let them communicate with one another while offline, but then have ProxyVirt providing reliable connectivity to the IoT Hub when they're next available.
                        </p>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
    export default {
        head: {
            title: 'ProxyVirt - Jibberwock',
            meta: [
                { hid: 'description', name: 'description', content: 'ProxyVirt virtualises a standard web proxy, allowing client traffic to be dynamically routed between networks.' },
                { hid: 'og-title', property: 'og:title', content: 'ProxyVirt - Jibberwock' },
                { hid: 'og-url', property: 'og:url', content: 'https://www.jibberwock.com/proxyvirt' },
            ]
        },

        data: () => ({
        })
    }
</script>
