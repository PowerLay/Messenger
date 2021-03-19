module.exports = {
  transpileDependencies: ["vuetify"]
};

module.exports = {
  pluginOptions: {
    electronBuilder: {
      builderOptions: {
        "win":{
          "icon": "./src/assets/icons/icon.ico"
        }
      }
    }
  }
}