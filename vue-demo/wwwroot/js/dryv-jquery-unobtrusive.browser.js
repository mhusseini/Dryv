(function () {
    var convert = function (value, type) {
        switch (type) {
            case "number": return Number(value);
            case "boolean": value.toLowerCase() === "true" || !!value;
            default: return value;
        }
    };
    var getValue = function ($el) {
        var type = $el.attr("type").toLowerCase();
        switch (type) {
            case "checkbox":
            case "radio":
                return $el[0]["checked"];
            default:
                return convert($el.val(), $el.attr("data-val-dryv-type"));
        }
    };
    var updateField = function (element, obj) {
        var el = $(element);
        if (el.data("dryv-ignore")) {
            return;
        }
        var name = el.attr("name");
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
                obj[Number(index)] = getValue(el);
            }
            else {
                parent_1[field] = getValue(el);
            }
        }
    };
    var createObject = function ($form) {
        var obj = {};
        $("input, select, textarea", $form).each(function (_, element) { return updateField(element, obj); });
        $form.data("dryv-object", obj);
        return obj;
    };
    var getObject = function ($form) {
        var existing;
        var obj = (existing = $form.data("dryv-object")) || createObject($form);
        obj.isNew = !existing;
        return obj;
    };
    $.validator.addMethod("dryv", function (_, element, message) {
        var func = window.dryv[message];
        if (!func) {
            throw "Cannot find Dryv validation function '" + message + "'.";
        }
        var obj = getObject($(this.currentForm));
        if (!obj.isNew) {
            updateField(element, obj);
        }
        var e = $(element);
        var error = func(obj);
        if (error) {
            e.data("msgDryv", error.message || error);
            if (error.type === "warning") {
                var lastWarning = e.data("dryvWarning");
                if (lastWarning === error.message) {
                    e.data("dryvWarningCanIgnore", true);
                    return true;
                }
                e.data("dryvWarning", error.message);
            }
            e.data("dryvWarningCanIgnore", null);
            return false;
        }
        else {
            e.data("dryvWarningCanIgnore", null);
            e.data("dryvWarning", null);
            e.data("msgDryv", null);
        }
        return true;
    });
    $.validator.unobtrusive.adapters.add("dryv", function (options) {
        var form = options.form;
        var $form = $(form);
        if (!$form.data("dryv-init")) {
            $form.data("dryv-init", true);
            $form.bind("submit", function () { $(this).data("dryv-object", null); });
            $("input:not([data-val-dryv]), textarea:not([data-val-dryv]), select:not([data-val-dryv]), datalist:not([data-val-dryv]), button:not([data-val-dryv])", $form)
                .each(function (i, el) {
                    if (el["type"] === "hidden" &&
                        $("input[type=checkbox][name='" + el["name"] + "']", $form).length) {
                        $(el).data("dryv-ignore", true);
                        return;
                    }
                    $(el).change(function () {
                        var obj = getObject($form);
                        updateField(this, obj);
                    });
                });
        }
        options.rules["dryv"] = options.message;
    });
    $.validator.setDefaults({
        highlight: function (element, errorClass, validClass) {
            $(element.form).find("*[data-valmsg-for=" + element.id + "]")
                .addClass(!$(element).data("dryvWarning") ? errorClass : this.settings.warningClass)
                .removeClass(validClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element.form).find("*[data-valmsg-for=" + element.id + "]")
                .removeClass(errorClass)
                .removeClass(this.settings.warningClass);
        }
    });
    var proto = $.validator.prototype;
    var originalShowErrors = proto.defaultShowErrors;
    proto.defaultShowErrors = function () {
        if (!this.settings.warningClass) {
            this.settings.warningClass = "field-validation-warning";
        }
        var form = $(this.currentForm);
        var successList = [];
        var removeFromErrorList = [];
        for (var _i = 0, _a = this.successList; _i < _a.length; _i++) {
            var element = _a[_i];
            var e = $(element);
            var message = e.data("dryvWarning");
            if (!message) {
                successList.push(element);
                continue;
            }
            var item = {
                message: message,
                element: element
            };
            this.errorList.push(item);
            if (e.data("dryvWarningCanIgnore")) {
                removeFromErrorList.push(item);
            }
            form.find("*[data-valmsg-for=" + element.id + "]")
                .addClass(this.settings.warningClass)
                .removeClass(this.settings.validClass);
        }
        this.successList = successList;
        originalShowErrors.call(this);
        for (var _b = 0, removeFromErrorList_1 = removeFromErrorList; _b < removeFromErrorList_1.length; _b++) {
            var item = removeFromErrorList_1[_b];
            this.errorList.splice(this.errorList.indexOf(item), 1);
        }
    };
})();
