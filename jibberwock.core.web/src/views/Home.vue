<template>
    <v-container fill-height fluid style="padding: 0px 5% 0px 5%;">
        <v-row align="center" justify="center">
            <v-col class="align-self-center">
                <v-img height="256" id="logo" contain src="/images/urban-cityscape.png" position="center right" class="mr-8"></v-img>
            </v-col>
            <v-col class="align-self-start">
                <v-row class="text-h2 font-weight-medium">
                    <v-col align="left">Jibberwock</v-col>
                </v-row>
                <v-row class="text-h5 text--secondary text-left">
                    <v-col align="left" class="pt-0">A collection of various projects and libraries.</v-col>
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
    import products from '@/plugins/products.js';

    export default {
        name: 'Home',
        title: 'Homepage - Jibberwock',

        data: () => ({
            products: products
        })
    }
</script>
