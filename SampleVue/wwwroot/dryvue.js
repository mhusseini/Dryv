//
// Diese Datei wird beizeiten durch ein NPM-Paket ausgetauscht.
//
const dryvRoot = window.dryv || (window.dryv = {});

function hashCode(text) {
    var hash = 0, i, chr;
    for (i = 0; i < text.length; i++) {
        chr = text.charCodeAt(i);
        hash = (hash << 5) - hash + chr;
        hash |= 0; // Convert to 32bit integer
    }
    return hash;
}

async function validate(component, clientContext) {
    const $dryv = component.$dryv;
    const context = Object.assign({ dryv: $dryv }, clientContext);
    const formValidators = $dryv.formValidators;
    if (!formValidators) {
        return true;
    }

    if ($dryv.groupComponents) {
        $dryv.groupComponents.forEach(c => c.clear());
    }

    const disablers = $dryv.v.disablers;
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

    $dryv._lastDisabledFields = disabledFields || null;
    $dryv._lastContext = context;

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

function initializeFieldComponent(formComponent, component, path) {
    if (component.$dryv) {
        return;
    }

    component.$dryv = { path, formValidators: [] };
    formComponent.$watch(path, (newValue, oldValue) => {
        const d = formComponent.$dryv;
        if (d._lastDisabledFields === undefined) {
            return;
        }

        component.$dryv.formValidators
            .filter(v => !v.isValidating && v.path === path)
            .forEach(v => v.validate(d._lastDisabledFields, d._lastContext, true));
    });
}
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
function copyRules($dryv, name) {
    const validationSet = dryvRoot.v[name];
    if (!validationSet) {
        if ($dryv.v) {
            return;
        }

        throw `No validation set with name '${name}' was found on the Dryv object supplied with the options.`;
    }

    $dryv.v = validationSet;
    $dryv.params = validationSet.parameters;
}

function initializeFormComponent(component, name, path, options) {
    if (!component.$dryv) {
        const $dryv = Object.assign({}, { fieldValidators: [], namedValidators: {} }, options);
        component.$dryv = $dryv;
        (component.methods || (component.methods = {}))["$dryvParam"] = () => $dryv.params || {};
    }

    const d = component.$dryv;
    d.path = path;
    copyRules(d, name);

    if (!d.formValidators) {
        d.formValidators = [];
    }

    if (!d.validate) {
        d.validate = validate.bind(component, component);
    }

    if (!d.setValidationResult) {
        d.setValidationResult = setValidationResult.bind(component, component);
    }
}

function handleValidationResult(Vue, component, result, errorField, warningField, hasGroupErrorField, groupComponent) {
    let type = null;
    let group = null;
    let text = null;

    if (result) {
        switch (typeof result) {
            case "object":
                type = result.type;
                group = result.group;
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

    if (group && groupComponent) {
        error && groupComponent.addError(error, group);
        warning && groupComponent.addWarning(warning, group, this);
        Vue.set(component, errorField, null);
        Vue.set(component, warningField, null);
        Vue.set(component, hasGroupErrorField, true);
    } else {
        Vue.set(component, errorField, error);
        Vue.set(component, warningField, warning);
        Vue.set(component, hasGroupErrorField, false);
    }

    return text && { type, text, group };
}

function runValidation(v, m, context) {
    return v.reduce(function (promiseChain, currentTask) {
        return promiseChain.then(function (r) {
            return r || currentTask(m, context);
        });
    }, Promise.resolve());
}

function valueOfDate(value, locale, format) {
    throw "Please specify the 'valueOfDate' field of the Dryvue options.";
}

const defaultOptions = {
    valueOfDate: valueOfDate,
    get: fetch,
    post: (url, data) => fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json; charset=UTF-8' },
        body: JSON.stringify(data),
    }),
    errorField: "error",
    warningField: "warning",
    hasGroupErrorField: "hasError"
};

const Dryvue = {
    install(Vue, o) {
        const options = Object.assign({}, defaultOptions, o);
        options.callServer = async function (baseUrl, method, data) {
            const isGet = method === "GET";
            const url = isGet ? baseUrl + "?" + Object.keys(data).map(k => `${k}=${encodeURIComponent(data[k])}`).join('&') : baseUrl;
            const response = isGet
                ? await options.get(url)
                : await options.post(url, data);

            return response.data;
        };

        function addResultItem(items, text, group) {
            if (!items[group]) {
                Vue.set(items, group, []);
            }

            const texts = items[group];
            if (texts.indexOf(text) < 0) {
                texts.push(text);
            }
        }

        function flattenItems(items) {
            return [].concat.apply([], Object.keys(items).map(k => items[k]));
        }

        function clearItems(items, group) {
            Object.keys(items).filter(k => !!group || k === group).map(k => items[k]).forEach(l => l.splice(0));
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
                    clear(group) {
                        clearItems(this.errors, group);
                        clearItems(this.warnings, group);
                    },
                    addError(text, group) {
                        addResultItem(this.errors, text, group);
                    },
                    addWarning(text, group) {
                        addResultItem(this.warnings, text, group);
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
                        formComponent.$dryv = Object.assign({
                            formValidators: [],
                            namedValidators: {}
                        }, options);
                        
                        const directive = formComponent._vnode.data.directives.filter(d => d.name === dryvSetDirective)[0].value;

                        if (typeof directive === "object") {
                            formComponent.$dryv.path = directive.path;
                        }
                        else if (typeof directive === "string") {
                            formComponent.$dryv.path = directive;
                        }

                        copyRules(formComponent.$dryv, formComponent.$dryv.path);
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
                    let hasGroupErrorField = options.hasGroupErrorField;

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

                    initializeFieldComponent(formComponent, component, path);

                    const validators = $dryv.v.validators[path];
                    if (!validators) {
                        return;
                    }

                    const v1 = validators.map(v => v.annotations);
                    v1.splice(0, 0, {});
                    const annotations = v1.reduce((t, s) => Object.assign(t, s));

                    for (let annotationName in annotations) {
                        if (!component.$data.hasOwnProperty(annotationName)) {
                            continue;
                        }

                        Vue.set(component, annotationName, annotations[annotationName]);
                    }

                    if (component.$data.hasOwnProperty(errorField)) {
                        Vue.set(component, errorField, null);
                    }
                    else {
                        throw new `Please specify the data property ${errorField} on the form input component.`;
                    }

                    if (component.$data.hasOwnProperty(warningField)) {
                        Vue.set(component, warningField, null);
                    }
                    else {
                        throw new `Please specify the data property ${warningField} on the form input component.`;
                    }

                    const fieldValidator = {
                        isValidating: false,
                        path,
                        validate: async (disabledFields, context, validateRelated) => {
                            if (validators === undefined) {
                                return null;
                            }

                            if (!validators) {
                                return null;
                            }

                            fieldValidator.isValidating = true;

                            try {
                                let data = formComponent.$data;

                                if ($dryv.path) {
                                    $dryv.path.split(".").forEach(p => data = data[p]);
                                }

                                const context2 = Object.assign({}, context);
                                context2.component = formComponent;

                                let result = null;
                                const isEnabled = !disabledFields || disabledFields.filter(f => path.indexOf(f) >= 0).length === 0;
                                if (isEnabled) {
                                    if (validateRelated) {
                                        const groups = validators.map(v => v.group).filter(g => !!g);
                                        groups.forEach(g => groupComponent.clear(g));
                                    }
                                    const validationFunctions = validators.map(v => v.validate);
                                    result = await runValidation(validationFunctions, data, context2);
                                    if (validateRelated) {
                                        const related = [].concat.apply([], validators.map(v => v.related));
                                        related.forEach(path => $dryv.namedValidators[path].validate(disabledFields, context));
                                    }
                                }

                                return handleValidationResult(Vue, component, result, errorField, warningField, hasGroupErrorField, groupComponent);
                            }
                            finally {
                                fieldValidator.isValidating = false;
                            }
                        },
                        setResults: results => {
                            const result = results && results[path];
                            return handleValidationResult(Vue, component, result, errorField, warningField, hasGroupErrorField, groupComponent);
                        }
                    };

                    component.$dryv.formValidators.push(fieldValidator);
                    $dryv.namedValidators[path] = fieldValidator;
                    $dryv.formValidators.push(fieldValidator);
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

                initializeFormComponent(component, name, path, options, Vue);
            }
        });
    }
};

Dryvue.mixin = {
    data() {
        return { dryv: { params: {} } };
    },
    mounted() {
        if (this.$dryv) {
            this.$set(this.$data, "dryv", { params: this.$dryv.params });
        }
    }
};
