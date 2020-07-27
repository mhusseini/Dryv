//
// Diese Datei wird beizeiten durch ein NPM-Paket ausgetauscht.
//
const dryv = window.dryv || (window.dryv = {});

function hashCode(text) {
    var hash = 0, i, chr;
    for (i = 0; i < text.length; i++) {
        chr = text.charCodeAt(i);
        hash = (hash << 5) - hash + chr;
        hash |= 0; // Convert to 32bit integer
    }
    return hash;
}

async function validate(component, dryv, context) {
    const $dryv = component.$dryv;
    const formValidators = $dryv.formValidators;
    if (!formValidators) {
        return true;
    }

    if ($dryv.groupComponents) {
        $dryv.groupComponents.forEach(c => c.clear());
    }

    const disablers = dryv.disablers;
    const disabledFields = [];

    if (disablers) {
        for (let field of Object.keys(disablers)) {
            const disabler = disablers[field];
            if (!disabler) {
                continue;
            }

            var validationFunctions = disabler.filter(v => v.validate(component.$data));

            if (validationFunctions.length) {
                disabledFields.push(field + ".");
            }
        }
    }

    let errors = "";
    let warnings = "";

    for (let v of formValidators) {
        const result = await v.validate(disabledFields, context);
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

const dryvGroupTag = "dryv-group";
const dryvSetDirective = "dryv-set";
const dryvFieldDirective = "dryv";

function findFormComponent(vnode) {
    let component = vnode.context;
    let formComponent = null;
    let groupComponent = null;

    while (component) {
        if (!formComponent &&
            component._vnode && component._vnode.data &&
            component._vnode.data.directives &&
            component._vnode.data.directives.filter(d => d.name === dryvSetDirective).length > 0) {
            formComponent = component;
        }
        else if (!groupComponent && component.$vnode && component.$vnode.componentOptions.tag === dryvGroupTag) {
            groupComponent = component;
        }

        if (groupComponent && formComponent) {
            break;
        }

        component = component.$parent;
    }

    return { groupComponent, formComponent };
}

function findModelExpression(vnode) {
    let n = vnode;

    while (n) {
        if (n.data && n.data.model && n.data.model.expression) {
            return n.data.model.expression;
        }
        n = n.parent;
    }

    return null;
}

function initializeFormComponent(component, name, path, options) {
    if (!component.$dryv) {
        component.$dryv = {};
    }

    const d = component.$dryv;
    d.path = path;

    if (!d.v) {
        const validationSet = options.dryv.v[name];
        if (!validationSet) {
            throw `No validation set with name '${name}' was found on the Dryv object supplied with the options.`;
        }

        d.v = validationSet;
        d.parameters = d.v.parameters;
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

function handleValidationResult(Vue, component, result, errorField, warningField, groupComponent) {
    let type = null;
    let groupName = null;
    let text = null;

    if (result) {
        switch (typeof result) {
            case "object":
                type = result.type;
                groupName = result.groupName;
                text = result.text;
                break;
            case "string":
                type = "error";
                text = result;
                break;
        }
    }

    const error = type === "error" && text;
    const warning = type === "warning" && text;

    if (groupName && groupComponent) {
        error && groupComponent.addError(error, groupName);
        warning && groupComponent.addWarning(warning, groupName);
        Vue.set(component, errorField, null);
        Vue.set(component, warningField, null);
    } else {
        Vue.set(component, errorField, error);
        Vue.set(component, warningField, warning);
    }

    return text && { type, text, groupName };
}

function runValidation(v, m, context) {
    return v.reduce(function (promiseChain, currentTask) {
        return promiseChain.then(function (r) {
            return r || currentTask(m, context);
        });
    }, Promise.resolve());
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

        function addResultItem(items, text, groupName) {
            if (!items[groupName]) {
                Vue.set(items, groupName, []);
            }

            const texts = items[groupName];
            if (texts.indexOf(text) < 0) {
                texts.push(text);
                // Vue.set(texts, texts.length, text);
            }
        }

        function flattenItems(items) {
            return [].concat.apply([], Object.keys(items).map(g => items[g]));
        }

        function clearItems(items) {
            Object.keys(items).map(g => items[g]).forEach(l => l.splice(0));
        }

        Vue.component(dryvGroupTag,
            {
                data() {
                    return {
                        errors: {},
                        warnings: {}
                    };
                },
                computed: {
                    allErrors() {
                        return flattenItems(this.errors);
                    },
                    allWarnings() {
                        return flattenItems(this.warnings);
                    }
                },
                methods: {
                    clear() {
                        clearItems(this.errors);
                        clearItems(this.warnings);
                    },
                    addError(text, groupName) {
                        addResultItem(this.errors, text, groupName);
                    },
                    addWarning(text, groupName) {
                        addResultItem(this.warnings, text, groupName);
                    }
                },
                template: "<div><slot :errors='errors' :warnings='warnings' :allErrors='allErrors' :allWarnings='allWarnings'></slot></div>"
            });

        Vue.directive(dryvFieldDirective,
            {
                inserted: function (el, binding, vnode) {
                    const component = vnode.context;
                    if (!component) {
                        throw `The '${dryvFieldDirective}' directive can only be applied to components.`;
                    }

                    const components = findFormComponent(vnode);
                    const formComponent = components.formComponent;

                    if (!formComponent) {
                        Vue.util.warn(`No component found with a ${dryvSetDirective} directive.`);
                        return;
                    }

                    if (!formComponent.$dryv) {
                        formComponent.$dryv = {
                            formValidators: []
                        };

                        const directive = formComponent._vnode.data.directives.filter(d => d.name === dryvSetDirective)[0].value;

                        if (typeof directive === "object") {
                            formComponent.$dryv.path = directive.path;
                        }
                    }

                    const $dryv = formComponent.$dryv;

                    const groupComponent = components.groupComponent;
                    if (groupComponent) {
                        if (!$dryv.groupComponents) {
                            $dryv.groupComponents = [];
                        }
                        if ($dryv.groupComponents.indexOf(groupComponent) < 0) {
                            $dryv.groupComponents.push(groupComponent);
                        }
                    }

                    let path = null;
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
                        path = findModelExpression(vnode);
                    }

                    if (!path) {
                        throw `The property path is missing. Please specify a value for the ${dryvFieldDirective} attribute or use the ${dryvFieldDirective} directive in combination with 'v-model'. Example value: 'firstName' or 'child.firstName'.`;
                    }

                    if ($dryv.path) {
                        path = path.substr($dryv.path.length + 1);
                    }

                    Vue.set(component, errorField, null);
                    Vue.set(component, warningField, null);

                    let validator = undefined;
                    let lastDisabledFields = undefined;

                    const fieldValidator = {
                        isValidating: false,
                        path,
                        validate: async (disabledFields, context) => {
                            if (validator === undefined) {
                                validator = $dryv.v.validators[path];
                            }

                            if (!validator) {
                                return null;
                            }

                            fieldValidator.lastContext = context;
                            fieldValidator.isValidating = true;

                            try {
                                lastDisabledFields = disabledFields || null;
                                let data = formComponent.$data;

                                if ($dryv.path) {
                                    $dryv.path.split(".").forEach(p => data = data[p]);
                                }

                                const context2 = Object.assign({}, context);
                                context2.component = formComponent;

                                let result = null;
                                const isEnabled = !disabledFields || disabledFields.filter(f => path.indexOf(f) >= 0).length === 0;
                                if (isEnabled) {
                                    const validationFunctions = validator.map(v => v.validate);
                                    result = await runValidation(validationFunctions, data, context2);
                                }
                                return handleValidationResult(Vue, component, result, errorField, warningField, groupComponent);
                            }
                            finally {
                                fieldValidator.isValidating = false;
                            }
                        },
                        setResults: results => {
                            const result = results && results[path];
                            return handleValidationResult(Vue, component, result, errorField, warningField, groupComponent);
                        }
                    };

                    $dryv.formValidators.push(fieldValidator);

                    formComponent.$watch(path, (newValue, oldValue) => {
                        if (lastDisabledFields !== undefined && !fieldValidator.isValidating) {
                            fieldValidator.validate(lastDisabledFields, fieldValidator.lastContext);
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
                let path = null;

                switch (typeof binding.value) {
                    case "object":
                        name = binding.value.name;
                        path = binding.value.path;
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

                initializeFormComponent(component, name, path, options);
            }
        });
    }
};
