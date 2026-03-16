document.addEventListener("DOMContentLoaded", function () {
    var decimalFields = document.querySelectorAll("[data-decimal-field]");

    function normalizeDecimalValue(field) {
        field.value = field.value.replace(",", ".");
    }

    decimalFields.forEach(function (field) {
        field.addEventListener("input", function () {
            normalizeDecimalValue(field);
        });

        field.addEventListener("blur", function () {
            normalizeDecimalValue(field);
        });
    });

    document.querySelectorAll("form").forEach(function (form) {
        form.addEventListener("submit", function () {
            decimalFields.forEach(function (field) {
                normalizeDecimalValue(field);
            });
        });
    });
});
