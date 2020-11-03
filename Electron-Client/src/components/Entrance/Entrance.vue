<template>
  <v-container fluid fill-height>
    <v-row justify="center" align="center" width=100%>
        <LRForm  @changeView="changeView" />
    </v-row>
  </v-container>
</template>

<script>
import Vue from "vue";
import LRForm from "./LoginRegisterForm.vue";

export default Vue.extend({
  name: "Entrance",
  components: {
    LRForm
  },
  data: () => ({}),
  methods:{
    changeView(a){
      this.$emit("changeView", a);
    }
  }
});
</script>