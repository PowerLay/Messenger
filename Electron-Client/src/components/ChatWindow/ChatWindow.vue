<template>
  <div>
    <v-row justify="center">
      <br />
      <v-col sm="10" class="chat-container">
        <Message
          v-for="msg in messages"
          :text="msg.text"
          OLDout="msg.user_id == settings.uid"
          :out="msg.name == settings.name"
          :uname="msg.name"
          :ts="msg.ts"
          :key="msg.ts + '_' + Math.random()"
        />
        <div id="anchor"></div>
      </v-col>
      <v-col cols="12" sm="10" md="8" lg="6"> </v-col>
    </v-row>
    <br />
    <br />
    <v-footer fixed class="font-weight-medium">
      <MessageInput />
    </v-footer>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import MessageInput from "./MessageInput.vue";
import Message from "./Message.vue";
import store from "../../store";

export default Vue.extend({
  name: "ChatWindow",
  components: {
    Message,
    MessageInput
  },
  data: () => ({
    store,
    messages: []
  }),
  computed: {
    settings() {
      return this.store.state.settings;
    }
  },
  methods: {
    async subscribe() {
      try {
        const response = await fetch("http://localhost:5000/api/Chat");
        if (response.status != 200) {
          await new Promise(resolve => setTimeout(resolve, 1000)); // sleep(1sec)
          await this.subscribe();
        } else {
          // Get and show the message
          const message = await response.text();
          this.messages = JSON.parse(message);
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
    /*const settings = JSON.parse(
      localStorage.getItem("settings") || '{"uid":-1,"name":"error"}'
    );
    this.uid = settings.name;
    this.name = settings.name;*/
    this.subscribe();
  }
});
</script>

<style>
#anchor {
  /* allow the final child to be selected as an anchor node */
  overflow-anchor: auto;

  /* anchor nodes are required to have non-zero area */
  height: 1px;
}
.message {
  margin-bottom: 3px;
}
.message.own {
  text-align: right;
}
.message.own .content {
  background-color: lightskyblue;
}
.chat-container .username {
  font-size: 18px;
  font-weight: bold;
}
.chat-container {
  overflow-anchor: none;
}
.chat-container .content {
  padding: 8px;
  background-color: lightgreen;
  border-radius: 10px;
  display: inline-block;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.2), 0 1px 1px 0 rgba(0, 0, 0, 0.14),
    0 2px 1px -1px rgba(0, 0, 0, 0.12);
  max-width: 50%;
  word-wrap: break-word;
}
</style>