<template>
  <div :class="{ message: 1, own: out }">
    <div class="content">
      <div class="overline mb-0 ">
        <b>{{ uname + " | " + genDate(ts) }}</b>
      </div>
      <div class="text">{{ text }}</div>
    </div>
  </div>
</template>

<style scoped>
  .text {
    float:left
  }
</style>

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
