<template>
  <v-dialog v-model="dialog" persistent max-width="600px" min-width="360px">
    <div v-on:keyup.enter="onEnter">
      <v-tabs
        v-model="tab"
        show-arrows
        background-color="deep-purple accent-4"
        icons-and-text
        dark
        grow
      >
        <v-tabs-slider color="purple darken-4"></v-tabs-slider>
        <v-tab v-for="i in tabs" :key="tabs[i]">
          <v-icon large>{{ i.icon }}</v-icon>
          <div class="caption py-1">{{ i.name }}</div>
        </v-tab>
        <v-tab-item>
          <v-card class="px-4">
            <v-card-text>
              <v-form ref="settingsForm" v-model="svalid" lazy-validation>
                <v-row>Window:</v-row>
                <v-row>
                  <v-col cols="12">
                    <v-text-field
                      v-model.number="win.width"
                      label="Width"
                      type="number"
                      :rules="[rules.required, rules.number]"
                      required
                    ></v-text-field>
                    <v-text-field
                      v-model.number="win.height"
                      label="Height"
                      type="number"
                      :rules="[rules.required, rules.number]"
                      required
                    ></v-text-field>
                  </v-col>
                </v-row>
                <v-row>Server:</v-row>
                <v-row>
                  <v-col cols="12">
                    <v-text-field
                      v-model="server.host"
                      label="Host"
                      :rules="[rules.required]"
                      required
                    ></v-text-field>
                    <v-text-field
                      v-model.number="server.port"
                      label="Port"
                      type="number"
                      :rules="[rules.required, rules.number]"
                      required
                    ></v-text-field>
                  </v-col>
                  <v-col class="d-flex" cols="12" sm="6" xsm="12"> </v-col>
                  <v-spacer></v-spacer>
                  <v-col class="d-flex" cols="12" sm="3" xsm="12" align-end>
                    <v-btn
                      x-large
                      block
                      :disabled="!svalid"
                      color="success"
                      @click="validateSettings"
                    >
                      Apply
                    </v-btn>
                  </v-col>
                </v-row>
              </v-form>
            </v-card-text>
          </v-card>
        </v-tab-item>
      </v-tabs>
    </div>
  </v-dialog>
</template>

<script>
import store from "../../store";
const { ipcRenderer } = window.require("electron");
export default {
  name: "SettingsTab",
  computed: {
    settings() {
      return store.state.settings;
    }
  },
  watch: {},
  methods: {
    validateSettings() {
      if (this.$refs.settingsForm.validate()) {
        this.settings.win = this.win;
        localStorage.setItem("winDims", JSON.stringify(this.win));
        this.settings.server = this.server;
        this.mainWindow.send(
          "changeResolution",
          this.win.width,
          this.win.height
        );
        this.$emit("changeView", "LRForm");
      }
    },
    onEnter: function() {
      this.validateSettings();
    }
  },
  mounted() {
    this.globalThis = this; // trick to bypass eslynt shit
    if (localStorage.getItem("winDims")) {
      const win = JSON.parse(localStorage.getItem("winDims"));
      this.win = win;
      this.mainWindow.send("changeResolution", win.width, win.height);
    } else {
      this.win = {
        width: window.outerWidth,
        height: window.outerHeight
      };
    }
  },
  data: () => ({
    mainWindow: ipcRenderer,
    dialog: true,
    tab: 0,
    tabs: [{ name: "Settings", icon: "mdi-settings" }],
    svalid: true,
    win: {
      width: 800,
      height: 650
    },
    server: {
      host: "127.0.0.1",
      port: 5000
    },
    rules: {
      required: value => !!value || "Required.",
      number: v => Number.isInteger(v) || "Must be integer",
      min: v => (v && v.length >= 8) || "Min 8 characters",
      spc: v => v.replace(/\s/gi, "") == v || "Must not contain spaces"
    }
  })
};
</script>

<style>
/* Chrome, Safari, Edge, Opera */
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

/* Firefox */
input[type="number"] {
  -moz-appearance: textfield;
}
</style>