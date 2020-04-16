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
        var names = el.attr("name").replace(/^\w|\.\w/g, function (m) { return m.toLowerCase(); }).split(".");
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
        var obj = (existing = $form.data("dryv-object"))
            || createObject($form);
        obj.isNew = !existing;
        return obj;
    };
    $.validator.addMethod("dryv", function (_, element, func) {
        var obj = getObject($(this.currentForm));
        if (!obj.isNew) {
            updateField(element, obj);
        }
        var e = $(element);
        e.data("msgDryv", null);
        var error = func(obj);
        if (error) {
            e.data("msgDryv", error.message || error);
            return false;
        }
        return true;
    });
    $.validator.unobtrusive.adapters.add("dryv", function (options) {
        var form = options.form;
        var $form = $(form);
        if (!$form.data("dryv-init")) {
            $form.data("dryv-init", true);
            $form.bind("submit", function () { $(this).data("dryv-object", null); });
            $("input:not([data-val-dryv]), textarea:not([data-val-dryv])", $form)
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
        var func = window.dryv[options.message];
        if (!func) {
            console.error("Cannot find Dryv validation function '" + options.message + "'.");
        }
        else {
            options.rules["dryv"] = func;
        }
    });
})();
