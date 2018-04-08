(function () {
    var regex = /(\w+)(\[(\d)\])?/;
    var createFormHandler = function (form) {
        var handler = function () { return form.data("dryv-object", null); };
        form.data("dryv-handler", handler)
            .on("invalid-form", handler);
    };
    var updateField = function (element, obj) {
        var el = $(element);
        var names = el.attr("name").replace(/^\w|\.\w/g, function (m) { return m.toLowerCase(); }).split(".");
        var max = names.length - 1;
        for (var i = 0; i < names.length; i++) {
            var name_1 = names[i];
            var m = regex.exec(name_1);
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
                obj[Number(index)] = el.val();
            }
            else {
                parent_1[field] = el.val();
            }
        }
    };
    var createObject = function (context) {
        var form = $(context.currentForm);
        form.data("dryv-handler") || createFormHandler(form);
        var obj = {};
        $("input, select, textarea", form).each(function (_, element) { return updateField(element, obj); });
        form.data("dryv-object", obj);
        return obj;
    };
    var getObject = function (context) {
        var existing;
        var obj = (existing = $(context.currentForm).data("dryv-object"))
            || createObject(context);
        obj.isNew = !existing;
        return obj;
    };
    $.validator.addMethod("dryv", function (_, element, functions) {
        var obj = getObject(this);
        if (!obj.isNew) {
            updateField(element, obj);
        }
        var e = $(element);
        e.data("msgDryv", null);
        for (var _i = 0, functions_1 = functions; _i < functions_1.length; _i++) {
            var fn = functions_1[_i];
            var error = fn(obj);
            if (error) {
                e.data("msgDryv", error.message || error);
                return false;
            }
        }
        return true;
    });
    $.validator.unobtrusive.adapters.add("dryv", function (options) {
        try {
            options.rules["dryv"] = eval(options.message);
        }
        catch (ex) {
            console.error("Failed to parse Dryv validation: " + ex + ".\nThe expression that was parsed is:\n" + options.message);
        }
    });
})();
