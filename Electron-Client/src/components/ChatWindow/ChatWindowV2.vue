<template>
  <div style="width:100%;height:100%">
    <div class="chatbox" @click="hideEmojiDialog">
      <div class="chatbox__container">
        <div class="chatbox__chat">
          <div class="chatbox__messages" id="scroller">
            <Message
              v-for="msg in messages"
              :text="msg.text"
              :out="msg.name == settings.username"
              :uname="msg.name"
              :ts="msg.ts"
              :key="msg.ts + '_' + Math.random()"
            />
            <div id="anchor" ref="anchorScroll"></div>
          </div>
        </div>
      </div>
    </div>
    <v-footer fixed class="font-weight-medium">
      <MessageInput ref="minput"/>
    </v-footer>
  </div>
</template>

<script>
import MessageInput from "./MessageInput.vue";
import Message from "./MessageV2.vue";
import store from "../../store";
import lodash from "lodash";

export default {
  name: "ChatWindow",
  components: {
    Message,
    MessageInput
  },
  computed: {
    settings() {
      return this.store.state.settings;
    }
  },
  watch: {
    rawMessages(n, o) {
      // find difference between new and old
      // then call addMessage() function to apply
      // * may be u should create some temporary messages store
      // * just not to update source object fully every time
      // * only to add new messages
      const diff = this.lodash.differenceWith(n, o, this.lodash.isEqual);
      if (diff.length) {
        //console.info(diff);
        this.messages = [...this.messages, ...diff];
      }
    }
  },
  methods: {
    hideEmojiDialog(){
      this.$refs.minput.hideEmojiDialog()
    },
    async subscribe() {
      try {
        const response = await fetch("http://localhost:5000/api/Chat");
        if (response.status != 200) {
          await new Promise(resolve => setTimeout(resolve, 1000)); // sleep(1sec)
          await this.subscribe();
        } else {
          // Get and show the message
          const message = await response.text();
          this.rawMessages = JSON.parse(message);
          // Call subscribe() again to get the next message
          await new Promise(resolve => setTimeout(resolve, 200)); // sleep(0.2sec)
          await this.subscribe();
        }
      } catch (e) {
        await new Promise(resolve => setTimeout(resolve, 1000)); // sleep(1sec)
        await this.subscribe();
      }
    }
  },
  mounted() {
    this.globalThis = this; // trick to bypass eslynt shit
    const t = this.globalThis;
    this.subscribe();

    const w = setInterval(function() {
      // sure there's no 'onRendered' event, so we cant catch it other way
      const el = t.$refs.anchorScroll;
      if (el.id == "anchor") {
        el.scrollIntoView(false);
        clearInterval(w);
      }
    }, 100);
  },
  data: () => ({
    lodash,
    store,
    messages: [],
    rawMessages: [],
  })
};
</script>

<style>
@import url("https://fonts.googleapis.com/css?family=Open+Sans:400,400i,700");
* {
  box-sizing: border-box;
}

#scroller * {
  overflow-anchor: none;
}

#anchor {
  overflow-anchor: auto;
  height: 1px;
}
html {
  overflow: hidden;
}
html,
body,
#app,
.v-main {
  width: 100%;
  height: 100%;
}
*:focus {
  outline: none;
}
p {
  margin: 0;
}
html,
body {
  margin: 0;
  font-size: 13px;
  height: 100%;
  font-family: "Open Sans", sans-serif;
}
.chatbox {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  width: 100%;
  height: 100%;
}
.chatbox__messages {
  margin: 0 auto;
  height: 100%;
  padding: 20px;
  padding-top: 0px;
  padding-bottom: 0;
  overflow-y: auto;
}
.chatbox__container {
  position: relative;
  width: 100%;
  height: 100%;
}
.chatbox__info,
.chatbox__navigation {
  align-self: flex-start;
  width: 100%;
  border-bottom: 1px solid #f2f2f2;
}
.chatbox__info {
  padding: 10px 10px;
  display: flex;
  align-items: center;
  text-transform: capitalize;
  justify-content: space-between;
}
.chatbox__info img {
  cursor: pointer;
}
.chatbox__navigation {
  display: flex;
  height: auto;
  align-items: center;
  text-align: left;
  padding: 8px 10px;
}

.chatbox__chat {
  position: relative;
  height: calc(100% - 78px);
}

.chatbox__messageContainer {
  width: 100%;
  display: flex;
  align-items: center;
  margin: 6px 0;
  justify-content: flex-start;
}
.chatbox__messageContainer--right {
  justify-content: flex-end;
}
.chatbox__messageContainer--right .chatbox__message {
  color: #fff;
  border: none;
}
.chatbox__message {
  position: relative;
  max-width: 400px;
  padding: 6px 15px;
  text-align: left;
  border-radius: 20px;
  font-size: 12px;
  color: #000;
  word-break: break-word;
  background-color: #f4f4f4;
}
.chatbox__message:hover .chatbox__time {
  display: block;
}
.author {
  opacity: 0.7;
}
.chatbox__time {
  display: none;
  position: absolute;
  color: #ccc;
  right: -50px; /*-38px;*/
  top: calc(50% - 9px);
}
.chatbox__time--left {
  left: -50px; /*no seconds: -38px;*/
  right: auto;
}
.chatbox__date {
  width: 100%;
  height: 20px;
  text-align: center;
  margin: 13px 0 10px;
  font-size: 12px;
  color: #ccc;
}
.chatbox__date:nth-child(1) {
  margin-top: 0;
}
.chatbox__popupMenu,
.chatbox__colorPalette,
.chatbox__userMenu {
  position: absolute;
  right: 10px;
  top: 40px;
  background: #fff;
  width: 200px;
  border-radius: 5px;
  box-shadow: 0 0 3px rgba(0, 0, 0, 0.2);
  z-index: 2;
}
.chatbox__popupMenu button,
.chatbox__colorPalette button,
.chatbox__userMenu button {
  width: 100%;
  padding: 8px 10px;
  cursor: pointer;
  background-color: #fff;
  border: none;
}
.chatbox__popupMenu button:hover,
.chatbox__colorPalette button:hover,
.chatbox__userMenu button:hover {
  background: #f4f4f4;
}
.chatbox__popupMenu button:last-child,
.chatbox__colorPalette button:last-child,
.chatbox__userMenu button:last-child {
  color: #f00;
}
.chatbox__popupMenu button:disabled,
.chatbox__colorPalette button:disabled,
.chatbox__userMenu button:disabled,
.chatbox__popupMenu button button[disabled],
.chatbox__colorPalette button button[disabled],
.chatbox__userMenu button button[disabled] {
  cursor: default;
  color: #bbb;
}
.chatbox__popupMenu button:disabled:hover,
.chatbox__colorPalette button:disabled:hover,
.chatbox__userMenu button:disabled:hover,
.chatbox__popupMenu button button[disabled]:hover,
.chatbox__colorPalette button button[disabled]:hover,
.chatbox__userMenu button button[disabled]:hover {
  background: #fff;
}
.chatbox__colorPalette {
  width: auto;
  padding: 9px;
  display: grid;
  grid-template: 1fr 1fr 1fr/1fr 1fr 1fr 1fr;
  grid-gap: 5px;
}
.chatbox__userMenu {
  right: auto;
  left: 10px;
}
.chatbox__color {
  width: 25px;
  height: 25px;
  border-radius: 30px;
  cursor: pointer;
}
.chatbox__color:hover {
  opacity: 0.9;
}
.chatbox__color:nth-child(1) {
  background-color: #ff7ca8;
}
.chatbox__color:nth-child(2) {
  background-color: #247ba0;
}
.chatbox__color:nth-child(3) {
  background-color: #70c1b3;
}
.chatbox__color:nth-child(4) {
  background-color: #b2dbbf;
}
.chatbox__color:nth-child(5) {
  background-color: #ff1654;
}
.chatbox__color:nth-child(6) {
  background-color: #ffba08;
}
.chatbox__color:nth-child(7) {
  background-color: #3f88c5;
}
.chatbox__color:nth-child(8) {
  background-color: #23bf73;
}
.chatbox__color:nth-child(9) {
  background-color: #ff0f80;
}
.chatbox__color:nth-child(10) {
  background-color: #fe4e00;
}
.chatbox__color:nth-child(11) {
  background-color: #f19a3e;
}
.chatbox__color:nth-child(12) {
  background-color: #09f;
}
.darkMode {
  color: #fff;
  background: #222;
}
.darkMode footer,
.darkMode .chatbox__info,
.darkMode .chatbox__navigation,
.darkMode .chatbox__channelSwitchButton,
.darkMode .chatbox__channelNewButton,
.darkMode .chatbox__colorPalette,
.darkMode .chatbox__userMenu {
  color: #fff;
  background: #222;
}
.darkMode .chatbox__contacts,
.darkMode .chatbox__contact,
.darkMode .chatbox__info,
.darkMode .chatbox__navigation {
  border-color: #444;
}
.darkMode .chatbox__message {
  background: #444;
  color: #fff;
}
.darkMode .chatbox__messageInput {
  background: #444;
  color: #fff;
}
.darkMode .chatbox__channelNewButton img,
.darkMode .chatbox__info img,
.darkMode .chatbox__navigation img {
  filter: invert(100%);
}
@media screen and (max-width: 600px) {
  .chatbox__container {
    width: 100%;
  }
}
@media screen and (max-width: 640px) {
  .chatbox__messageInput {
    width: 290px;
  }
}
</style>