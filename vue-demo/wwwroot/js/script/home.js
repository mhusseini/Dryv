"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var vue_1 = require("vue");
var vee_validate_1 = require("vee-validate");
var dryv_vee_validator_1 = require("./dryv-vee-validator");
vue_1.default.use(vee_validate_1.default, {
    events: 'input|blur',
    fastExit: false,
});
vee_validate_1.default.Validator.extend('dryv', dryv_vee_validator_1.default.validator);
vue_1.default.prototype.$dryv = {
    id: 0,
    forms: {}
};
var findForm = function (el, binding, vnode) {
    var form = el.parentElement;
    while (form.tagName !== "FORM" && form.parentElement) {
        form = form.parentElement;
    }
    if (!form) {
        // TODO: output error
    }
    return form;
};
var getFormContext = function (el, binding, vnode) {
    var form = findForm(el, binding, vnode);
    var dryv = vnode.context.$dryv;
    var formid = Number(form.getAttribute("data-dryv-id"));
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
vue_1.default.directive('dryv', {
    bind: function (el, binding, vnode) {
        var values = binding.value.split(':');
        if (values.length !== 2) {
            // TODO: output error
        }
        var type = values[0];
        var name = values[1];
        var value = window[name];
        if (!value) {
            // TODO: output error
        }
    },
    inserted: function (el, binding, vnode) {
        debugger;
        var myDryv = getFormContext(el, binding, vnode);
        var parts = binding.value.split(":");
        var type = parts[0];
        var name = parts[1];
        myDryv.elCount++;
        myDryv.model[name] = el.value;
    },
    update: function (el, binding, vnode) {
        var myDryv = getFormContext(el, binding, vnode);
        var parts = binding.value.split(":");
        var type = parts[0];
        var name = parts[1];
        myDryv.model[name] = el.value;
    },
    unbind: function (el, binding, ref) {
        debugger;
    }
});
var app = new vue_1.default({
    el: "#app",
    data: {
        name: "hallo",
        age: 23
    },
    methods: {
        onSubmit: function (e) {
            dryv_vee_validator_1.default.prepare(this, e.target);
            this.$validator.validate().then(function (result) {
                if (!result) {
                    e.preventDefault();
                }
            });
        }
    }
});
exports.default = app;
//# sourceMappingURL=home.js.map