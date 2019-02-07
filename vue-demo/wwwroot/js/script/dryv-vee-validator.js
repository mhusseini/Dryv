"use strict";
var _this = this;
Object.defineProperty(exports, "__esModule", { value: true });
var validator = {
    getMessage: function (field, args) {
        var mid = args[0];
        var vee = window.dryv.vee;
        var error = vee[mid + "_e"];
        return error;
    },
    validate: function (value, args) {
        var error = null;
        var mid = args[0];
        var vee = window.dryv.vee;
        var val = vee[mid];
        if (!val.m) {
            return true;
        }
        if (val.r) {
            val.r();
        }
        for (var _i = 0, _a = val.v; _i < _a.length; _i++) {
            var func = _a[_i];
            error = func(val.m);
            if (error) {
                break;
            }
        }
        vee[mid + "_e"] = error;
        return error == null;
        ;
    }
};
exports.validator = validator;
var refesh = function (form, val) {
    var obj = {};
    for (var i = 0; i < form.childElementCount; i++) {
        updateModelFromElement(form[i], i, obj, _this.$validator);
    }
    val.m = obj;
};
var updateField = function (obj, name, value) {
    if (!name) {
        return;
    }
    var names = name.replace(/^\w|\.\w/g, function (m) { return m.toLowerCase(); }).split(".");
    var max = names.length - 1;
    for (var i = 0; i < names.length; i++) {
        var name_1 = names[i];
        var m = /(\w+)(\[(\d)\])?/.exec(name_1);
        var field = m[1];
        var index = m[3];
        var parent_1 = obj;
        obj = obj[field];
        if (i < max) {
            if (!obj) {
                obj = index ? [] : {};
                parent_1[field] = obj;
            }
            if (index) {
                var idx = Number(index);
                if (obj[idx]) {
                    obj = obj[idx];
                }
                else {
                    obj = obj[idx] = {};
                }
            }
        }
        else if (index) {
            if (!obj) {
                obj = parent_1[field] = [];
            }
            obj[Number(index)] = value;
        }
        else {
            parent_1[field] = value;
        }
    }
};
function prepare(compomnent, form) {
    var obj = {};
    for (var i = 0; i < form.childElementCount; i++) {
        var element = form[i];
        var field = updateModelFromElement(element, i, obj, compomnent.$validator);
        var mid = element.getAttribute("dryv-v");
        if (mid) {
            var val = window.dryv.vee[mid];
            if (val) {
                val.m = obj;
                val.r = refesh.bind(this, val, form);
                field.update({ bails: false });
            }
        }
    }
}
exports.prepare = prepare;
function updateModelFromElement(element, i, obj, validator) {
    var field = validator.fields.find({ name: element.name });
    var value = field ? field.value : element.value;
    updateField(obj, element.name, value);
    return field;
}
exports.default = { validator: validator, prepare: prepare };
//# sourceMappingURL=dryv-vee-validator.js.map