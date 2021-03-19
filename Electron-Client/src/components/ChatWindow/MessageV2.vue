<template>
  <div>
    <!--<div class="chatbox_date">
      {{ genDate(ts) }}
    </div>-->
    <div class="chatbox_serverMsg" v-if="uname == ''">
      {{ text }}
    </div>
    <div
      v-else
      class="chatbox__messageContainer"
      :class="out ? 'chatbox__messageContainer--right' : ''"
    >
      <div
        class="im_initials_avatar im_dialog_item_photo chatbox_avatar"
        :style="{
          background: '#' + this.getBackgroundColor(uname),
          color: this.getTextColor(this.getBackgroundColor(uname), true)
        }"
        v-if="!out"
      >
        {{ uname[0] }}
        <div class="im_dialog_item_online" v-if="online"></div>
      </div>

      <div
        class="chatbox__message"
        :style="{
          'background-color': out ? 'rgb(0, 153, 255)' : ''
        }"
      >
        <div class="author">
          {{ !out ? uname + ":" : "You:" }}
        </div>
        {{ text }}
        <div class="chatbox__time" :class="out ? 'chatbox__time--left' : ''">
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
    getBackgroundColor(name) {
      // via https://stackoverflow.com/questions/3426404/create-a-hexadecimal-colour-based-on-a-string-with-javascript
      function hashCode(str) {
        // java String#hashCode
        var hash = 0;
        for (var i = 0; i < str.length; i++) {
          hash = str.charCodeAt(i) + ((hash << 5) - hash);
        }
        return hash;
      }

      function intToRGB(i) {
        var c = (i & 0x00ffffff).toString(16).toUpperCase();

        return "00000".substring(0, 6 - c.length) + c;
      }

      return intToRGB(hashCode(name));
    },
    // via https://stackoverflow.com/questions/11867545/change-text-color-based-on-brightness-of-the-covered-background-area
    getTextColor(hex) {
      // https://stackoverflow.com/questions/21646738/convert-hex-to-rgba
      let c = hex.split("");
      if (c.length == 3) {
        c = [c[0], c[0], c[1], c[1], c[2], c[2]];
      }
      c = "0x" + c.join("");

      let rgb = [(c >> 16) & 255, (c >> 8) & 255, c & 255]

      // http://www.w3.org/TR/AERT#color-contrast
      const brightness = Math.round(
        (parseInt(rgb[0]) * 299 +
          parseInt(rgb[1]) * 587 +
          parseInt(rgb[2]) * 114) /
          1000
      );
      return (brightness > 125 ? "black" : "white");
    },
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
