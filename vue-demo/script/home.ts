import Vue from "vue";
import VeeValidate, { ValidationProvider } from 'vee-validate';
import dryv from './dryv-vee-validator';

Vue.use(VeeValidate, {
    events: 'input|blur',
    fastExit: false,
});

VeeValidate.Validator.extend('dryv', dryv.validator);

Vue.prototype.$dryv = {
    id: 0,
    forms: {}
};

const findForm = (el: HTMLInputElement, binding: any, vnode: any) => {
    let form = el.parentElement;
    while (form.tagName !== "FORM" && form.parentElement) {
        form = form.parentElement;
    }
    if (!form) {
        // TODO: output error
    }
    return form;
};

const getFormContext = (el: HTMLInputElement, binding: any, vnode: any) => {
    const form = findForm(el, binding, vnode);
    const dryv = (vnode.context as any).$dryv;
    let formid = Number(form.getAttribute("data-dryv-id"));
    if (isNaN(formid) || !formid) {
        formid = ++dryv.id;
        form.setAttribute("data-dryv-id", formid.toString());
        dryv.forms[formid] = {
            elCount: 0,
            model: {}
        };
    }

    return dryv.forms[formid];
};

Vue.directive('dryv', {
    bind(el: HTMLInputElement, binding: any, vnode: any) {
        const values = binding.value.split(':');
        if (values.length !== 2) {
            // TODO: output error
        }

        const type = values[0];
        const name = values[1];
        const value = (window as any)[name];
        if (!value) {
            // TODO: output error
        }
    },
    inserted(el: HTMLInputElement, binding, vnode) {
        debugger;
        const myDryv = getFormContext(el, binding, vnode);
        const parts = binding.value.split(":");
        const type = parts[0];
        const name = parts[1];
        myDryv.elCount++;
        myDryv.model[name] = el.value;
    },
    update(el:HTMLInputElement, binding, vnode) {
        const myDryv = getFormContext(el, binding, vnode);
        const parts = binding.value.split(":");
        const type = parts[0];
        const name = parts[1];
        myDryv.model[name] = el.value;
    },
    unbind(el, binding, ref) {
        debugger;
    }
});
const app = new Vue({
    el: "#app",
    data: {
        name: "hallo",
        age: 23
    },
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