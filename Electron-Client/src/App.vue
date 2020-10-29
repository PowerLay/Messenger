<template>
  <v-app>
    <v-main>
      <!-- <input :value='currentComponent'>
      <input v-model="currentComponent">-->
      <component :is="currentComponent" @changeView="changeView"></component>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import Vue from "vue";
import Entrance from "./components/Entrance/Entrance.vue";
//import ChatWindow from "./components/ChatWindow/ChatWindow.vue";
import ChatWindowV2 from "./components/ChatWindow/ChatWindowV2.vue"; // via https://codepen.io/karolsw3/pen/pBxpjZ
import store from './store'

export default Vue.extend({
  name: "App",

  components: {
    Entrance,
    ChatWindowV2
  },
  data: () => ({
    store,
    currentComponent: "Entrance"
  }),
  methods: {
    changeView(a: string) {
      this.currentComponent = a;
    }
  },
  created(){
    store.commit('init')
  }
});
</script>

<style scoped>
*{
    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -khtml-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
}
</style>