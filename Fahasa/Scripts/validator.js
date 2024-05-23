function Validator(option) {
    var selectorRules = {};

    function getParent(element, selector) {
        while (element.parentElement) {
            if (element.parentElement.matches(selector)) return element.parentElement
            element = element.parentElement
        }
    }
    //Hàm thực hiện validate
    function validate(intputElement, rule, errorElement) {
        var errorMessage;
        var rules = selectorRules[rule.selector];

        for (var i = 0; i < rules.length; i++) {
            errorMessage = rules[i](intputElement.value);
            if (errorMessage) break;
        }

        if (errorMessage) {
            errorElement.classList.add("invalid");
            errorElement.querySelector(option.formMessage).innerText = errorMessage;
        } else {
            errorElement.classList.remove("invalid");
            errorElement.querySelector(option.formMessage).innerText = "";
        }
        return !errorMessage;
    }

    const formElement = document.querySelector(option.formElement);
    if (formElement) {
        formElement.onsubmit = function (e) {
            e.preventDefault();
            var isFormValid = true;

            option.rules.forEach((rule) => {
                var intputElement = formElement.querySelector(rule.selector);
                var errorElement = getParent(intputElement, option.formGroup);
                var isValid = validate(intputElement, rule, errorElement);
                if (!isValid) isFormValid = false;
            });

            if (isFormValid) {
                if (typeof option.onSubmit === "function") {
                    var enableInput = formElement.querySelectorAll("input[name]");
                    var formValue = Array.from(enableInput).reduce(function (values, input) {
                        switch (input.type) {
                            case "radio":
                                values[input.name] = formElement.querySelector(
                                    'input[name="' + input.name + '"]:checked'
                                ).value;
                                break;
                            case "checkbox":
                                if (!input.matches(":checked")) {
                                    values[input.name] = "";
                                    return values;
                                }
                                if (!Array.isArray(values[input.name])) {
                                    values[input.name] = [];
                                }
                                values[input.name].push(input.value);
                                break;
                            case "file":
                                values[input.name] = input.files;
                                break;
                            default:
                                values[input.name] = input.value;
                        }
                        return values;
                    }, {});
                    option.onSubmit(formValue);
                } else {
                    formElement.submit();
                }
            }
        };

        option.rules.forEach(function (rule) {
            if (Array.isArray(selectorRules[rule.selector])) {
                selectorRules[rule.selector].push(rule.test);
            } else {
                selectorRules[rule.selector] = [rule.test];
            }

            //Lấy element form cần validate
            var intputElement = formElement.querySelectorAll(rule.selector);

            Array.from(intputElement).forEach(function (intputElement, errorElement) {
                var errorElement = getParent(intputElement, option.formGroup);

                intputElement.addEventListener("blur", function () {
                    validate(intputElement, rule, errorElement);
                });

                intputElement.addEventListener("input", function () {
                    errorElement.classList.remove("invalid");
                    errorElement.querySelector(option.formMessage).innerText = "";
                });
            });
        });
    }
}

Validator.isEmail = function (selector, message) {
    const regex =
        /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
    return {
        selector: selector,
        test: function (value) {
            return regex.test(value) ? undefined : message || "Trường này là email";
        },
    };
};
Validator.isRequired = function (selector, message) {
    return {
        selector: selector,
        test: function (value) {
            var el = document.querySelector(selector)
            if (['checkbox', 'radio'].includes(el.type))
                el.checked ? value = '1' : value = ''
            return value.trim() ? undefined : message || "Vui lòng nhập trường này";
        },
    };
};
Validator.minLength = function (selector, minlength, message) {
    return {
        selector: selector,
        test: function (value) {
            return value.length >= minlength ? undefined : message || `Trường này tồi thiểu ${minlength} ký tự`;
        },
    };
};
Validator.isConfirmed = function (selector, getSelector, message) {
    return {
        selector: selector,
        test: function (value) {
            return value === getSelector() ? undefined : message || "Mật khẩu nhập lại không chinh xác"
        }
    }
}
