<template>
    <v-container fill-height fluid style="padding: 0px 5% 0px 5%;">
        <v-row align="center" justify="center">
            <v-col class="align-self-center">
                <v-img height="256" id="logo" contain src="/images/urban-cityscape.png" position="center right" class="mr-8" translate="false"></v-img>
            </v-col>
            <v-col class="align-self-start">
                <v-row>
                    <v-col align="left"><h1 class="text-h2 font-weight-medium">Jibberwock</h1></v-col>
                </v-row>
                <v-row>
                    <v-col align="left" class="pt-0"><h2 class="text-h5 text--secondary text-left">A collection of various projects and libraries.</h2></v-col>
                </v-row>
                <v-row class="text-left" style="max-width: 594px">
                    <v-card v-for="product in products" :key="product.name" style="padding-top: 16px; margin-bottom: 2px" min-width="150" max-width="198" width="30%" :href="product.url" tile>
                        <v-img height="96" contain :src="product.logoPath"></v-img>
                        <v-card-title style="word-break: initial;">{{ product.name }}</v-card-title>
                        <v-card-text>{{ product.description }}</v-card-text>
                    </v-card>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<style>
    /* v-responsive__content comes from the size of the original PNG, with comes to 884px. I want this to be much smaller, to make sure that the picture doesn't force the v-col into its own line */
    #logo .v-responsive__content {
        width: 442px !important;
    }
</style>

<script>
    import products from '@/static-data/products.js';

    export default {
        head: {
            title: 'Homepage - Jibberwock',
            meta: [
                { hid: 'description', name: 'description', content: 'Jibberwock is a site to display a number of open-source projects.' },
                { hid: 'og-title', property: 'og:title', content: 'Homepage - Jibberwock' },
                { hid: 'og-url', property: 'og:url', content: 'https://www.jibberwock.com' },
            ]
        },

        data: () => ({
            products: products
        })
    }
</script>
