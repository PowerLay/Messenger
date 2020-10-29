import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    settings:{
      username:null,
      token:null,
    }
  },
  mutations: {
    init(state) {
      const s = localStorage.getItem('settings')
      if (s) {
        state.settings = JSON.parse(s);
      }
    },
    saveLocal(state){
      localStorage.setItem('settings', JSON.stringify(state.settings))
    }
  },
  actions: {},
  modules: {}
});
