<template>
  <v-container fluid fill-height>
    <v-row justify="center" align="center" width="100%">
      <component :is="entranceView" @changeView="changeView"></component>
    </v-row>
  </v-container>
</template>

<script>
import Vue from "vue";
import LRForm from "./LoginRegisterForm.vue";
import SettingsForm from "./Settings.vue";

export default Vue.extend({
  name: "Entrance",
  components: {
    LRForm,
    SettingsForm
  },
  data: () => ({
    entranceView: "SettingsForm"
  }),
  methods: {
    changeView(a) {
      if (a.includes('ChatWindow')){
        this.$emit("changeView", a);
      } else {
        this.entranceView=a
      }
    }
  }
});
</script>