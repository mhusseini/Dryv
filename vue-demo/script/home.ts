import Vue from "vue";
import VeeValidate from 'vee-validate';
import dryv from './dryv-vee-validator';

Vue.use(VeeValidate, {
    events: 'input|blur',
    fastExit: false,
});

VeeValidate.Validator.extend('dryv', dryv.validator);

const app = new Vue({
    el: "#app",
    data: {},
    methods: {
        onSubmit(e: Event) {
            dryv.prepare(this, e.target as HTMLFormElement);

            this.$validator.validate().then((result: boolean) => {
                if (!result) {
                    e.preventDefault();
                }
            });
        }

    }
});

export default app;