import { Field } from 'vee-validate';
import Vue from 'vue';

const validator = {
    getMessage: (field: string, args: Array<any>) => {
        const mid = args[0];
        const vee = (window as any).dryv.vee;
        const error = vee[mid + "_e"];
        return error;
    },

    validate: (value: any, args: Array<any>) => {
        let error: string = null;
        const mid = args[0];
        const vee = (window as any).dryv.vee;
        const val = vee[mid];
        if (!val.m) {
            return true;
        }
        if (val.r) {
            val.r();
        }
        for (let func of val.v) {
            error = func(val.m);
            if (error) {
                break;
            }
        }
        vee[mid + "_e"] = error;
        return error == null;;
    }
};

const refesh = (form: HTMLFormElement, val: any) => {
    const obj = {};
    for (let i = 0; i < form.childElementCount; i++) {
        updateModelFromElement(form[i] as HTMLInputElement, i, obj, this.$validator);
    }
    val.m = obj;
};

const updateField = (obj: any, name: string, value: any) => {
    if (!name) {
        return;
    }

    const names = name.replace(/^\w|\.\w/g, m => m.toLowerCase()).split(".");
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
            obj[Number(index)] = value;
        }
        else {
            parent[field] = value;
        }
    }
};

function prepare(compomnent: Vue, form: HTMLFormElement) {
    const obj = {};
    for (let i = 0; i < form.childElementCount; i++) {
        const element = form[i] as HTMLInputElement;
        const field = updateModelFromElement(element, i, obj, compomnent.$validator);
        const mid = element.getAttribute("dryv-v");
        if (mid) {
            const val = (window as any).dryv.vee[mid];
            if (val) {
                val.m = obj;
                val.r = refesh.bind(this, val, form);
                field.update({ bails: false });
            }
        }
    }
}

function updateModelFromElement(element: HTMLInputElement, i: number, obj: any, validator: any): Field {
    const field = validator.fields.find({ name: element.name });
    const value = field ? field.value : element.value;
    updateField(obj, element.name, value);

    return field;
}

export { validator, prepare };
export default { validator, prepare };


