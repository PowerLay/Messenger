<template>
  <div style="width:100%;height:100%">
    <div class="chatbox" @click="hideEmojiDialog">
      <div class="chatbox__container">
        <div class="chatbox__chat">
          <div class="chatbox__messages" id="scroller">
            <Message
              v-for="msg in messages"
              :text="msg.text + ' ' +onlineUsers[msg.name]"
              :out="msg.name == settings.username"
              :online="onlineUsers[msg.name]"
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
      <MessageInput ref="minput" />
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
      // then call something like addMessage() function to apply
      // * may be u should create some temporary messages store
      // * just not to update source object fully every time
      // * only to add new messages
      const diff = this.lodash.differenceWith(n, o, this.lodash.isEqual);
      if (diff.length) {
        //console.info(diff);
        this.messages = [...this.messages, ...diff];
      }
    },
    rawOnlineUsers(n, o) {
      const diff = this.lodash.differenceWith(n, o, this.lodash.isEqual);
      console.info(o,n)
      if (diff.length) {
        console.info(111)
        this.onlineUsers = [...this.onlineUsers, ...diff];
      } 
    }
  },
  methods: {
    hideEmojiDialog() {
      this.$refs.minput.hideEmojiDialog();
    },
    async subscribe(url, method, auth, done, timeout = 200) {
      try {
        const a = {
          Authorization: "Bearer " + this.settings.token
        };
        const response = await fetch(url, {
          method: method,
          headers: auth ? a : {},
          body: method == "POST" ? {} : null
        });
        if (response.status != 200) {
          await new Promise(resolve => setTimeout(resolve, 1000)); // sleep(1 sec)
          await this.subscribe(...arguments);
        } else {
          // Get and show the message
          const resp = await response.text();
          done(resp);
          // Call subscribe() again to get the next message
          await new Promise(resolve => setTimeout(resolve, timeout)); // sleep(n msec)
          await this.subscribe(...arguments);
        }
      } catch (e) {
        console.error(e);
        await new Promise(resolve => setTimeout(resolve, 1000)); // sleep(1sec)
        await this.subscribe(...arguments);
      }
    },
    async subscribeToMessages() {
      const t = this.globalThis;
      this.subscribe("http://localhost:5000/api/Chat", "GET", false, function(
        resp
      ) {
        t.rawMessages = JSON.parse(resp);
      });
    },

    async subscribeToOnlinePost() {
      this.subscribe(
        "http://localhost:5000/api/Online",
        "POST",
        true,
        function() {},
        500
      );
    },
    async subscribeToOnlineGet() {
      const t = this.globalThis;
      this.subscribe(
        "http://localhost:5000/api/Online",
        "GET",
        true,
        function(d) {
          t.rawOnlineUsers = JSON.parse(d);
        },
        400
      );
    }
  },
  mounted() {
    this.globalThis = this; // trick to bypass eslynt shit
    const t = this.globalThis;
    //this.subscribeToMessages();
    this.subscribeToOnlinePost();
    this.subscribeToOnlineGet();

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
    onlineUsers: [],
    rawOnlineUsers: []
  })
};
</script>

<style>
/* позаимствовано из клиента вк */
.im_initials_avatar.-color1 {
    background-image: radial-gradient(circle at center 0px, #FFC247, #FFA21F);
}
.im_initials_avatar {
    border-radius: 50%;
    text-align: center;
    text-transform: uppercase;
    user-select: none;
}
.chatbox_avatar {
  width: 44px; height: 44px; font-size: 22px; line-height: 44px; letter-spacing: 0.4px;
}
.im_dialog_item_online {
    content: '';
    position: relative;
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background: #71cb50;
    border: 2px #71cb50 solid;
    bottom: 9px;
    right: 1px;
}

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
.chatbox_serverMsg {
  text-align: center;
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