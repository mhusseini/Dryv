import Vue from "vue/dist/vue.esm.browser";
import Dryvue from "@softwareproduction/dryvue";
import axios from "axios";

Vue.use(Dryvue,
    {
        get: axios.get,
        post: axios.post
    });

Vue.component('form-input',
    {
        props: ['value', 'type', 'name', 'label'],
        template: `<div class="form-item" v-dryv>
            <div>
                <div class="form-input-label">
                    <label>{{ label }}</label>
                </div>
                <div class="form-input-control">
                    <input v-bind:value="value" v-on:input="$emit('input', $event.target.value)">
                </div>
            </div>
            <div class="form-item-warning" v-if="warning">{{ warning }}</div>
            <div class="form-item-error" v-if="error">{{ error }}</div>
        </div>`,
        data() {
            return {
                error: null,
                warning: null
            };
        }
    });

const app = new Vue({
    el: '#app',
    data: {
        isValid: null,
        lastValidationWarning: null,
        model: {
            billingEqualsShipping: true,
            person: {
                firstName: null,
                lastName: null
            },
            shippingAddress: {
                zipCode: null,
                city: null
            },
            billingAddress: {
                zipCode: null,
                city: null
            }
        }
    },
    mounted() {
    },
    methods: {
        async validateForm(e) {
            e.preventDefault();
            this.isValid = true;
            const validation = await this.$dryv.validate();

            if (!validation.hasErrors && (!validation.hasWarnings || validation.warningHash === this.lastValidationWarning)) {
                const result = await axios.post(e.srcElement.action, this.model);
                this.isValid = this.$dryv.setValidationResult(result.data.messages);
                this.lastValidationWarning = null;
            } else {
                this.isValid = false;
                this.lastValidationWarning = validation.warningHash;
            }
        }
    }
});