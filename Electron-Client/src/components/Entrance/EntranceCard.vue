<template>
  <v-form @submit="submit">
    <v-card ref="form">
      <v-card-text>
        <v-text-field
          ref="name"
          v-model="name"
          :rules="[() => !!name && !!name.trim()]"
          label="Отображаемое имя"
          placeholder="VaZZeLiN_1337"
          required
        ></v-text-field>
      </v-card-text>
      <v-divider class="mt-12"></v-divider>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" text @click="submit">
          Submit
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-form>
</template>

<script>
import store from "../../store";
export default {
  name: "EntranceCard",
  data: () => ({
    store,
    name: null,
    formHasErrors: false
  }),

  computed: {
    form() {
      return {
        name: this.name
      };
    }
  },

  methods: {
    submit(e) {
      e.preventDefault();
      this.formHasErrors = false;

      Object.keys(this.form).forEach(f => {
        if (!this.form[f]) this.formHasErrors = true;

        this.$refs[f].validate(true);
      });

      if (!this.formHasErrors) {
        //const settings = JSON.parse(localStorage.getItem("settings"));
        const settings = store.state.settings;
        settings.name = this.name;
        settings.uid = this.name;
        //localStorage.setItem("settings", JSON.stringify(settings));
        store.commit("saveLocal");
        this.$emit("changeView", "ChatWindowV2");
      }
    }
  },
  mounted() {
    const settings = store.state.settings; //localStorage.getItem("settings");

    this.name = settings.name;
    this.uid = settings.name;
  }
};
</script>