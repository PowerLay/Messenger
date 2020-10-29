<template>
  <v-container fluid fill-height>
    <v-row justify="center" align="center" width=100%>
        <LRForm  @changeView="changeView" />
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import EntranceCard from "./EntranceCard.vue";
import LRForm from "./LoginRegisterForm.vue";

export default Vue.extend({
  name: "Entrance",
  components: {
    LRForm//EntranceCard
  },
  data: () => ({}),
  methods:{
    changeView(a: string){
      this.$emit("changeView", a);
    }
  }
});
</script>