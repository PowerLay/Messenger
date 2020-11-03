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
              <v-form ref="loginForm" v-model="lvalid" lazy-validation>
                <v-row>
                  <v-col cols="12">
                    <v-text-field
                      v-model="login.username"
                      label="Username"
                      :rules="[
                        rules.required,
                        rules.spc,
                        v => !errors.username || errors.username
                      ]"
                      required
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12">
                    <v-text-field
                      v-model="login.password"
                      :append-icon="pwdShow ? 'eye' : 'eye-off'"
                      :rules="[
                        rules.required,
                        rules.min,
                        v => !errors.password || errors.password
                      ]"
                      type="password"
                      label="Password"
                      hint="At least 8 characters"
                      counter
                      @click:append="pwdShow = !pwdShow"
                    ></v-text-field>
                  </v-col>
                  <v-col class="d-flex" cols="12" sm="6" xsm="12"> </v-col>
                  <v-spacer></v-spacer>
                  <v-col class="d-flex" cols="12" sm="3" xsm="12" align-end>
                    <v-btn
                      x-large
                      block
                      :disabled="!lvalid"
                      :loading="loading"
                      color="success"
                      @click="validatelogin"
                    >
                      Login
                    </v-btn>
                  </v-col>
                </v-row>
              </v-form>
            </v-card-text>
          </v-card>
        </v-tab-item>
        <v-tab-item>
          <v-card class="px-4">
            <v-card-text>
              <v-form ref="registerForm" v-model="rvalid" lazy-validation>
                <v-row>
                  <v-col cols="12">
                    <v-text-field
                      v-model="register.username"
                      label="Username"
                      maxlength="20"
                      :rules="[
                        rules.required,
                        rules.spc,
                        v => !errors.username || errors.username
                      ]"
                      required
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12">
                    <v-text-field
                      v-model="register.password"
                      :append-icon="pwdShow ? 'mdi-eye' : 'mdi-eye-off'"
                      :rules="[rules.required, rules.min]"
                      :type="pwdShow ? 'text' : 'password'"
                      label="Password"
                      hint="At least 8 characters"
                      counter
                      @click:append="pwdShow = !pwdShow"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12">
                    <v-text-field
                      block
                      v-model="register.verifyPwd"
                      :append-icon="pwdShow ? 'mdi-eye' : 'mdi-eye-off'"
                      :rules="[rules.required, passwordMatch]"
                      :type="pwdShow ? 'text' : 'password'"
                      label="Confirm Password"
                      counter
                      @click:append="pwdShow = !pwdShow"
                    ></v-text-field>
                  </v-col>
                  <v-spacer></v-spacer>
                  <v-col class="d-flex ml-auto" cols="12" sm="3" xsm="12">
                    <v-btn
                      x-large
                      block
                      :disabled="!rvalid"
                      color="success"
                      :loading="loading"
                      @click="validateregister"
                      >Register</v-btn
                    >
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
export default {
  name: "EntranceCardV2",
  computed: {
    passwordMatch() {
      return () =>
        this.register.password === this.register.verifyPwd ||
        "Password must match";
    },
    settings() {
      return store.state.settings;
    }
  },
  watch:{
    tab(){
      this.resetErrors()
    },
  },
  methods: {
    async checkUserExists(username) {
      const t = this.globalThis;
      t.loading = true;
      let result = false;
      let hasError = true;
      await this.request(
        "http://127.0.0.1:5000/api/Login?username=" + username,
        "GET",
        {},
        null,
        function(r) {
          r = JSON.parse(r);
          result = r.response;
          t.loading = false;
          hasError = false;
        },
        function(e) {
          t.loading = false;
          null;
        }
      );
      if (!hasError) {
        return result;
      } else {
        return true; // needed to display error in handler higher than this
      }
    },
    Register(username, pass) {
      this.Login(username, pass) // lol its same endpoint just kiddin
    },
    Login(username, pass) {
      const t = this.globalThis;
      t.loading = true;
      const user = {
        Username: username,
        Password: pass
      };
      this.request(
        "http://127.0.0.1:5000/api/Login",
        "POST",
        {
          "Content-Type": "application/json;charset=utf-8"
        },
        JSON.stringify(user),
        function(r) {
          r = JSON.parse(r);
          if (r.token) {
            t.settings.username = username;
            t.settings.token = r.token;
            store.commit("saveLocal");
            t.$emit("changeView", "ChatWindowV2");
          } else {
            t.displayLoginError("Server error (unable to read token)");
            setTimeout(t.resetErrors, 3000);
          }
          t.loading = false;
        },
        function(e) {
          if (e == 401) {
            t.displayPasswordError("Invalid password");
            t.loading = false;
          } else if (e == 1337) {
            // connection problem
            t.displayLoginError(
              "Unable to resolve host (check your internet connection)"
            );
            t.loading = false;
            setTimeout(t.resetErrors, 3000);
          }
        }
      );
    },
    async request(url, method, headers, body, callback, error) {
      try {
        const response = await fetch(url, {
          headers: headers,
          method: method,
          body: body
        });
        if (response.status != 200) {
          error(response.status);
        } else {
          const resp = await response.text();
          callback(resp);
        }
      } catch (e) {
        error(1337);
      }
    },
    async validatelogin() {
      if (this.$refs.loginForm.validate()) {
        const exist = await this.checkUserExists(this.login.username);
        if (exist) {
          this.Login(this.login.username, this.login.password);
        } else {
          this.displayLoginError("User doesnt exists");
        }
      }
    },
    async validateregister() {
      if (this.$refs.registerForm.validate()) {
        const exist = await this.checkUserExists(this.register.username);
        if (!exist) {
          this.Register(this.register.username, this.register.password);
        } else {
          this.displayLoginError("User already exists");
        }
      }
    },
    resetErrors() {
      this.errors.username = "";
      this.errors.password = "";
      this.$refs.loginForm?.validate();
      this.$refs.registerForm?.validate();
    },
    displayLoginError(txt) {
      this.errors.username = txt;
      window.test = this
      this.$refs.loginForm?.validate();
      this.$refs.registerForm?.validate();
      this.errors.username = "";
    },
    displayPasswordError(txt) {
      this.errors.password = txt;
      this.$refs.loginForm?.validate();
      this.$refs.registerForm?.validate();
      this.errors.password = "";
    },
    onEnter: function() {
      const n = Object.keys(this.$refs)
      if (n[this.tab] == 'loginForm'){
        this.validatelogin()
      } else if (n[this.tab] == 'registerForm'){
        this.validateregister()
      }
    }
  },
  mounted() {
    this.globalThis = this; // trick to bypass eslynt shit
    this.rvalid = false;
    this.lvalid = false;
  },
  data: () => ({
    dialog: true,
    tab: 0,
    tabs: [
      { name: "Login", icon: "mdi-account" },
      { name: "Register", icon: "mdi-account-outline" }
    ],
    rvalid: false,
    lvalid: false,
    loading: false,
    errors: {
      username: "",
      password: ""
    },
    login: {
      password: "",
      username: "",
    },
    register: {
      username: "",
      password: "",
      verifyPwd: "",
    },
    pwdShow: false,
    rules: {
      required: value => !!value || "Required.",
      min: v => (v && v.length >= 8) || "Min 8 characters",
      spc: v => v.replace(/\s/gi, "") == v || "Must not contain spaces"
    }
  })
};
</script>