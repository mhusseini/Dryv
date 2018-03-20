(function () {
    var createFormHandler = function (form) {
        var handler = function () { return form.data("dryv-object", null); };
        form.data("dryv-handler", handler)
            .on("invalid-form", handler);
    };
    var createObject = function (context) {
        var form = $(context.currentForm);
        form.data("dryv-handler") || createFormHandler(form);
        var obj = {};
        $("input, select, textarea", form).each(function (_, element) {
            var el = $(element);
            obj[el.attr("name")] = el.val();
        });
        form.data("dryv-object", obj);
        return obj;
    };
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
