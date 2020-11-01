<template>
  <div>
    <!--<div class="chatbox_date">
      {{ genDate(ts) }}
    </div>-->
    <div class='chatbox_serverMsg' v-if="uname==''">
      {{ text }}
    </div>
    <div v-else
      class="chatbox__messageContainer"
      :class="out ? 'chatbox__messageContainer--right' : ''"
      :style="{
        //'margin-top': out ? '12px' : '4px',
        //'margin-bottom': !out ? '12px' : '4px'
      }"
      ><div class="im_initials_avatar -color1 im_dialog_item_photo chatbox_avatar" v-if="!out">
        {{ uname[0] }}
        <div class="im_dialog_item_online" v-if="online"></div>
      </div>
      
      <div
        class="chatbox__message"
        :style="{
          'background-color':
            out ? 'rgb(0, 153, 255)' : '',
        }"
      >
        <div class="author">
            {{ !out ? uname+':' : 'You:' }}
        </div>
        {{ text }}
        <div class="chatbox__time" :class="out ? 'chatbox__time--left':''">
          {{ genDate(ts) }}
        </div>
      </div>
    </div>
  </div>
</template>


<script>
export default {
  props: {
    uname: String,
    text: String,
    out: Boolean,
    online: Boolean,
    ts: Number
  },
  methods: {
    genDate(ts) {
      const date = new Date(ts * 1000);
      const hours = date.getHours();
      const minutes = "0" + date.getMinutes();
      const seconds = "0" + date.getSeconds();

      const formattedTime =
        hours + ":" + minutes.substr(-2) + ":" + seconds.substr(-2);

      return formattedTime;
    }
  }
};
</script>
