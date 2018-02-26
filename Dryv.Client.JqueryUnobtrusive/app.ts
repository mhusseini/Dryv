module Dryv {
    declare const $;
    declare const console: any;

    const createFormHandler = (form: any) => {
        const handler = () => form.data("dryv-object", null);
        form.data("dryv-handler", handler)
            .on("invalid-form", handler);
    }

    const createObject = (context: any) => {
        const form = $(context.currentForm);
        form.data("dryv-handler") || createFormHandler(form);

        const obj = {};
        for (let element of context.currentElements) {
            const el = $(element);
            obj[el.attr("name")] = el.val();
        }
        form.data("dryv-object", obj);
        return obj;
    }

    const getObject = (context: any) =>
        $(context.currentForm).data("dryv-object") ||
        createObject(context);

    $((() => {
        $.validator.addMethod("dryv", function (_, element, functions) {
            const obj = getObject(this);
            const e = $(element);
            e.data("msgDryv", null);
            for (let fn of functions) {
                const error = fn(obj);
                if (error) {
                    e.data("msgDryv", error);
                    return false;
                }
            }

            return true;
        });

        $.validator.unobtrusive.adapters.add("dryv", options => {
            try {
                options.rules["dryv"] = eval(options.message);
            } catch (ex) {
                console.error(`Failed to parse Dryv validation: ${ex}.\nThe expression that was parsed is:\n${options.message}`);
            }
        });
    })());
}