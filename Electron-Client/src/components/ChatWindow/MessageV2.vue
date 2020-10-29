<template>
  <div>
    <!--<div class="chatbox_date">
      {{ genDate(ts) }}
    </div>-->
    <div
      class="chatbox__messageContainer"
      :class="out ? 'chatbox__messageContainer--right' : ''"
      :style="{
        //'margin-top': out ? '12px' : '4px',
        //'margin-bottom': !out ? '12px' : '4px'
      }"
    >
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
