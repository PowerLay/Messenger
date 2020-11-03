<template>
  <v-form style="width:100%" @submit="sendMessage">
    <v-text-field
      class="ma-0"
      v-model="message"
      NOT:append-icon="marker ? 'mdi-map-marker' : 'mdi-map-marker-off'"
      append-outer-icon="mdi-send"
      prepend-icon="mdi-emoticon"
      filled
      clear-icon="mdi-close-circle"
      clearable
      label="Message"
      type="text"
      @click:append-outer="sendMessage"
      @click:prepend="toggleEmojiDialog"
      @click:clear="clearMessage"
      @keypress.enter="sendMessage"
    ></v-text-field>
    <VEmojiPicker
      v-show="showEmojiDialog"
      labelSearch="Search"
      lang="ru-RU"
      @select="onSelectEmoji"
      :style="{ position: 'absolute', bottom: '80px' }"
    />
  </v-form>
</template>

<style >
#app
  > div
  > main
  > div
  > div
  > footer
  > form
  > div
  > div.v-input__control
  > div.v-text-field__details {
  display: none;
}
</style>


<script>
import store from "../../store";
import { VEmojiPicker, emojisDefault, categoriesDefault } from "v-emoji-picker";

export default {
  components: { VEmojiPicker },
  props:['bus'],
  data: () => ({
    store,
    showEmojiDialog: false,
    show: false,
    message: "Hey!",
    marker: true,
    iconIndex: 0,
    icons: [
      "mdi-emoticon",
      "mdi-emoticon-cool",
      "mdi-emoticon-dead",
      "mdi-emoticon-excited",
      "mdi-emoticon-happy",
      "mdi-emoticon-neutral",
      "mdi-emoticon-sad",
      "mdi-emoticon-tongue"
    ],
  }),

  computed: {
    icon() {
      return this.icons[this.iconIndex];
    },
    settings() {
      return this.store.state.settings;
    }
  },

  methods: {
    onSelectEmoji(e) {
      this.message += e.data;
    },
    toggleEmojiDialog() {
      this.showEmojiDialog = !this.showEmojiDialog;
    },
    hideEmojiDialog() {
      this.showEmojiDialog = false;
    },
    toggleMarker() {
      this.marker = !this.marker;
    },
    sendMessage(e) {
      e.preventDefault();
      this.hideEmojiDialog();
      if (!this.message) {
        return false;
      }
      const msg = {
        Name: this.settings.name,
        Text: this.message
      };
      const job = fetch("http://127.0.0.1:5000/api/Chat", {
        method: "POST",
        headers: {
          "Content-Type": "application/json;charset=utf-8",
          Authorization: "Bearer " + this.settings.token
        },
        body: JSON.stringify(msg)
      });
      //this.resetIcon();
      this.clearMessage();
    },
    clearMessage() {
      this.message = "";
    },
    resetIcon() {
      //this.iconIndex = 0;
    },
    changeIcon() {
      this.iconIndex === this.icons.length - 1
        ? (this.iconIndex = 0)
        : this.iconIndex++;
    }
  },
  mounted() {
    const settings = store.state.settings;
    this.uid = settings.name;
    this.name = settings.name;
  }
};
</script>