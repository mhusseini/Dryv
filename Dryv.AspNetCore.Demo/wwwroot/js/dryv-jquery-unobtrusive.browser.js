///<amd-module name="dryv-jquery-unobtrusive"/>
///<reference types="jquery" />

(function () {
    // const createFormHandler = (form: any) => {
    //     const handler = () => form.data("dryv-object", null);
    //     form.data("dryv-handler", handler)
    //         .on("invalid-form", handler);
    // };

    const convert = (value, type) => {
        switch (type) {
            case "number": return Number(value);
            case "boolean": value.toLowerCase() === "true" || !!value;
            default: return value;
        }
    };
    const getValue = ($el: JQuery<HTMLElement>) => {
        const type = $el.attr("type").toLowerCase();
        switch (type) {
            case "checkbox":
            case "radio":
                return $el[0]["checked"];
            default:
                return convert($el.val(), $el.attr("data-val-dryv-type"));
        }
    };

    const updateField = (element, obj) => {
        const el = $(element);
        const names = el.attr("name").replace(/^\w|\.\w/g, m => m.toLowerCase()).split(".");
        const max = names.length - 1;
        for (let i = 0; i < names.length; i++) {
            const name = names[i];
            const m = /(\w+)(\[(\d)\])?/.exec(name);
            const field = m[1];
            const index = m[3];
            const parent = obj;
            obj = obj[field];
            if (i < max) {
                if (!obj) {
                    obj = index ? [] : {};
                    parent[field] = obj;
                }

                if (index) {
                    const idx = Number(index);
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
                    obj = parent[field] = [];
                }
                obj[Number(index)] = getValue(el);
            }
            else {
                parent[field] = getValue(el);
            }
        }
    };

    const createObject = ($form) => {
        const obj = {};
        $("input, select, textarea", $form).each((_, element) => updateField(element, obj));
        $form.data("dryv-object", obj);
        return obj;
    };

    const getObject = ($form) => {
        let existing;
        const obj = (existing = $form.data("dryv-object"))
            || createObject($form);
        obj.isNew = !existing;
        return obj;
    };

    $.validator.addMethod("dryv", function (_, element, functions) {
        const obj = getObject($(this.currentForm));
        if (!obj.isNew) {
            updateField(element, obj);
        }
        const e = $(element);
        e.data("msgDryv", null);
        for (let fn of functions) {
            const error = fn(obj);
            if (error) {
                e.data("msgDryv", error.message || error);
                return false;
            }
        }

        return true;
    });

    $.validator.unobtrusive.adapters.add("dryv", options => {
        const form = options.form;
        const $form = $(form);
        if (!$form.data("dryv-init")) {
            $form.data("dryv-init", true);
            $form.bind("submit", function () { $(this).data("dryv-object", null); })

            $("input:not([data-val-dryv]), textarea:not([data-val-dryv])", $form)
                .each((i, el) => {
                    if (el["type"] === "hidden" &&
                        $("input[type=checkbox][name='" + el["name"] + "']", $form).length) {
                        return;
                    }

                    $(el).change(function () {
                        const obj = getObject($form);
                        updateField(this, obj);
                    });
                });
        }
        try {
            options.rules["dryv"] = eval(options.message);
        } catch (ex) {
            console.error(`Failed to parse Dryv validation: ${ex}.\nThe expression that was parsed is:\n${options.message}`);
        }
    });
})();