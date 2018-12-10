import Vue from "vue";
import VeeValidate from 'vee-validate';
import DryvVee from "dryv-vee-validate";

Vue.use(VeeValidate);
DryvVee.init(VeeValidate);

const app = new Vue({
    el: "#app",
    mixins: [DryvVee.mixin],
    data: {}
});

export default app;