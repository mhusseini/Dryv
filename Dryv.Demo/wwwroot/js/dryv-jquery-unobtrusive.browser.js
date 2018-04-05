(function () {
    var createFormHandler = function (form) {
        var handler = function () { return form.data("dryv-object", null); };
        form.data("dryv-handler", handler)
            .on("invalid-form", handler);
    };
    var createObject = function (context) {
        var form = $(context.currentForm);
        form.data("dryv-handler") || createFormHandler(form);
        var regex = /(\w+)(\[(\d)\])?/;
        var obj = {};
        $("input, select, textarea", form).each(function (_, element) {
            var current = obj;
            var el = $(element);
            var names = el.attr("name").split(".");
            var max = names.length - 1;
            for (var i = 0; i < names.length; i++) {
                var name_1 = names[i];
                var g = name_1.charAt(0).toLowerCase() + name_1.substr(1);
                var m = regex.exec(g);
                var field = m[1];
                var index = m[3];
                var parent_1 = current;
                current = current[field];
                if (i < max) {
                    if (!current) {
                        current = index ? [] : {};
                        parent_1[field] = current;
                    }
                    if (index) {
                        var idx = Number(index);
                        if (current[idx]) {
                            current = current[idx];
                        }
                        else {
                            current = current[idx] = {};
                        }
                    }
                }
                else {
                    if (index) {
                        if (!current) {
                            current = parent_1[field] = [];
                        }
                        current[Number(index)] = el.val();
                    }
                    else {
                        parent_1[field] = el.val();
                    }
                }
            }
        });
        form.data("dryv-object", obj);
        return obj;
    };
    window["createObject"] = createObject;
    var getObject = function (context) {
        return $(context.currentForm).data("dryv-object") ||
            createObject(context);
    };
    $.validator.addMethod("dryv", function (_, element, functions) {
        var obj = getObject(this);
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
