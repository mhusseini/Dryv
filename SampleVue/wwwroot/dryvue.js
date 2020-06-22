const dryv = window.dryv || (window.dryv = {});

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

    let errors = 0;

    for (let v of formValidators) {
        if (await v.validate(disabledFields)) {
            errors++;
        }
    }

    return !errors;
}

function setValidationResult(component, errors) {
    if (component.$dryv.formValidators) {
        component.$dryv.formValidators.forEach(v => v.setError(errors));
    }

    return !errors;
}

function findFormComponent(vnode) {
    let component = vnode.context;

    while (component) {
        if (component._vnode && component._vnode.data &&
            component._vnode.data.directives &&
            component._vnode.data.directives.filter(dryv => dryv.name === "dryv").length > 0) {
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

const Dryvue = {
    install(Vue, options) {
        if (!options) options = {};
        if (!options.get) options.get = axios.get;
        if (!options.post) options.post = axios.post;
        if (!options.errorField) options.errorField = "error";
        if (!options.dryv) options.dryv = window.dryv;

        dryv.validateAsync = async function (baseUrl, method, data) {
            const isGet = method === 'GET';
            const url = isGet ? baseUrl + "?" + Object.keys(data).map(k => `${k}=${encodeURIComponent(data[k])}`).join('&') : baseUrl;
            const response = isGet
                ? await options.get(url)
                : await options.post(url, data);

            return response.data && response.data.text;
        };

        Vue.directive('dryv-field',
            {
                inserted: function (el, binding, vnode) {
                    const component = vnode.context;
                    if (!component) {
                        throw "The 'v-dryv-field' directive can only be applied to components.";
                    }

                    const formComponent = findFormComponent(vnode);
                    if (!formComponent) {
                        Vue.util.warn("No component found with a 'v-dryv' directive.");
                        return;
                    }

                    let path;
                    let errorField = options.errorField;

                    switch (typeof binding.value) {
                        case "object":
                            errorField = binding.value.errorField || options.errorField;
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
                        throw "Property path is missing. Please specify a value for the 'v-dryv-field' attribute or use the 'v-drvy-field' directive in combination with 'v-model'. Example value: 'firstName' or 'child.firstName'.";
                    }

                    //const validator = options.dryv.validators[path];
                    //if (!validator) {
                    //    return;
                    //}

                    let validator = undefined;

                    Vue.set(component, errorField, null);

                    if (!formComponent.$dryv) {
                        formComponent.$dryv = {
                            formValidators: []
                        };
                    }

                    formComponent.$dryv.formValidators.push({
                        validate: async (disabledFields) => {
                            if (validator === undefined) {
                                validator = formComponent.$dryv.v.validators[path];
                            }
                            if (!validator) {
                                return null;
                            }
                            const error = (!disabledFields || disabledFields.filter(f => name.indexOf(f) >= 0).length === 0) && await validator(formComponent.$data);
                            Vue.set(component, errorField, error);
                            return error;
                        },
                        setError: errors => {
                            if (!validator) {
                                return null;
                            }
                            const error = errors && errors[name];
                            Vue.set(component, errorField, error);
                            return error;
                        }
                    });
                }
            });

        Vue.directive('dryv', {
            inserted: function (el, binding, vnode) {
                const component = vnode.context;
                if (!component) {
                    throw "The 'v-dryv' directive can only be applied to components.";
                }

                let name;
                switch (typeof binding.value) {
                    case "object":
                        name = binding.value.name;
                        break;
                    case "string":
                        name = binding.value;
                        break;
                }

                if (!name) {
                    throw "Form name is missing. Please specify a value for the 'v-dryv' attribute.";
                }

                initializeFormComponent(component, name, options);
            }
        });
    }
};