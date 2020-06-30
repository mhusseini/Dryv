const dryv = window.dryv || (window.dryv = {});

function hashCode(text) {
    var hash = 0, i, chr;
    for (i = 0; i < text.length; i++) {
        chr = text.charCodeAt(i);
        hash = ((hash << 5) - hash) + chr;
        hash |= 0; // Convert to 32bit integer
    }
    return hash;
}

async function validate(component, dryv) {
    const formValidators = component.$dryv.formValidators;
    if (!formValidators) {
        return true;
    }

    const disablers = dryv.disablers;
    const disabledFields = [];

    if (disablers) {
        for (let field of Object.keys(disablers)) {
            const disabler = disablers[field];
            if (disabler && await disabler(component.$data)) {
                disabledFields.push(field + ".");
            }
        }
    }

    let errors = "";
    let warnings = "";

    for (let v of formValidators) {
        const result = await v.validate(disabledFields);
        if (!result) {
            continue;
        }

        switch (typeof result) {
            case "object":
                switch (result.type) {
                    case "error":
                        errors += `${v.path}=${result.text};`;
                        break;
                    case "warning":
                        warnings += `${v.path}=${result.text};`;
                        break;
                }
                break;
            case "string":
                errors += `${v.path}=${result};`;
                break;
        }
    }

    return {
        hasErrors: errors.length > 0,
        errorHash: hashCode(errors),
        hasWarnings: warnings.length > 0,
        warningHash: hashCode(warnings)
    };
}

function setValidationResult(component, results) {
    if (component.$dryv.formValidators) {
        component.$dryv.formValidators.forEach(v => v.setResults(results));
    }

    return !results || results.length === 0;
}

const dryvSetDirective = "dryv-set";
const dryvFieldDirective = "dryv";

function findFormComponent(vnode) {
    let component = vnode.context;

    while (component) {
        if (component._vnode && component._vnode.data &&
            component._vnode.data.directives &&
            component._vnode.data.directives.filter(dryv => dryv.name === dryvSetDirective).length > 0) {
            return component;
        }

        component = component.$parent;
    }

    return null;
}

function findeModelExpression(vnode) {
    let n = vnode;

    while (n) {
        if (n.data && n.data.model && n.data.model.expression) {
            return n.data.model.expression;
        }
        n = n.parent;
    }

    return null;
}

function initializeFormComponent(component, name, options) {
    if (!component.$dryv) {
        component.$dryv = {};
    }

    const d = component.$dryv;

    if (!d.v) {
        const validationSet = options.dryv.v[name];
        if (!validationSet) {
            throw `No validation set with name '${name}' was found on the Dryv object supplied with the options.`;
        }

        d.v = validationSet;
    }

    if (!d.formValidators) {
        d.formValidators = [];
    }

    if (!d.validate) {
        d.validate = validate.bind(component, component, d.v);
    }

    if (!d.setValidationResult) {
        d.setValidationResult = setValidationResult.bind(component, component);
    }
}

function handleValidationResult(Vue, component, result, errorField, warningField) {
    let error;
    let warning;

    if (result) {
        switch (typeof result) {
            case "object":
                switch (result.type) {
                    case "error":
                        error = result.text;
                        warning = null;
                        break;
                    case "warning":
                        error = null;
                        warning = result.text;
                        break;
                    default:
                        error = null;
                        warning = null;
                }
                break;
            case "string":
                error = result;
                warning = null;
                break;
            default:
                error = null;
                warning = null;
        }
    } else {
        error = null;
        warning = null;
    }

    Vue.set(component, errorField, error);
    Vue.set(component, warningField, warning);

    return result;
}

const Dryvue = {
    install(Vue, options) {
        if (!options) options = {};
        if (!options.get) options.get = axios.get;
        if (!options.post) options.post = axios.post;
        if (!options.errorField) options.errorField = "error";
        if (!options.warningField) options.warningField = "warning";
        if (!options.dryv) options.dryv = window.dryv;

        dryv.validateAsync = async function (baseUrl, method, data) {
            const isGet = method === "GET";
            const url = isGet ? baseUrl + "?" + Object.keys(data).map(k => `${k}=${encodeURIComponent(data[k])}`).join('&') : baseUrl;
            const response = isGet
                ? await options.get(url)
                : await options.post(url, data);

            return response.data;
        };

        Vue.component("dryv-group",
            {
                props: ['groups'],
                data() {
                    return {};
                },
                created() {
                    debugger;
                    const groups = this.groups;
                    for (let group in groups) {
                        const field = groups[group];
                        Vue.set(this, field, null);
                    }
                }
            });

        Vue.directive(dryvFieldDirective,
            {
                inserted: function (el, binding, vnode) {
                    const component = vnode.context;
                    if (!component) {
                        throw `The '${dryvFieldDirective}' directive can only be applied to components.`;
                    }

                    const formComponent = findFormComponent(vnode);
                    if (!formComponent) {
                        Vue.util.warn(`No component found with a ${dryvSetDirective} directive.`);
                        return;
                    }

                    let path;
                    let errorField = options.errorField;
                    let warningField = options.warningField;

                    switch (typeof binding.value) {
                        case "object":
                            errorField = binding.value.errorField || options.errorField;
                            warningField = binding.value.warningField || options.warningField;
                            path = binding.value.path;
                            break;
                        case "string":
                            path = binding.value;
                            break;
                    }

                    if (!path) {
                        path = findeModelExpression(vnode);
                    }

                    if (!path) {
                        throw `The property path is missing. Please specify a value for the ${dryvFieldDirective} attribute or use the ${dryvFieldDirective} directive in combination with 'v-model'. Example value: 'firstName' or 'child.firstName'.`;
                    }

                    let validator = undefined;

                    Vue.set(component, errorField, null);

                    if (!formComponent.$dryv) {
                        formComponent.$dryv = {
                            formValidators: []
                        };
                    }

                    formComponent.$dryv.formValidators.push({
                        path,
                        validate: async (disabledFields) => {
                            if (validator === undefined) {
                                validator = formComponent.$dryv.v.validators[path];
                            }

                            if (!validator) {
                                return null;
                            }

                            const result = (!disabledFields || disabledFields.filter(f => path.indexOf(f) >= 0).length === 0) && await validator(formComponent.$data);
                            return handleValidationResult(Vue, component, result, errorField, warningField);
                        },
                        setResults: results => {
                            if (!validator) {
                                return null;
                            }
                            const result = results && results[path];
                            return handleValidationResult(Vue, component, result, errorField, warningField);
                        }
                    });
                }
            });

        Vue.directive(dryvSetDirective, {
            inserted: function (el, binding, vnode) {
                const component = vnode.context;
                if (!component) {
                    throw `The ${dryvSetDirective} directive can only be applied to components.`;
                }

                let name;
                switch (typeof binding.value) {
                    case "object":
                        name = binding.value.name;
                        break;
                    case "string":
                        name = binding.value;
                        break;
                    default:
                        name = null;
                }

                if (!name) {
                    throw `Form name is missing. Please specify a value for the ${dryvSetDirective} attribute.`;
                }

                initializeFormComponent(component, name, options);
            }
        });
    }
};