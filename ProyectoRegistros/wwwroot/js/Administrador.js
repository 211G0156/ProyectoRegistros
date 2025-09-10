document.getElementById('ig2').addEventListener('click', function (event) {
    event.stopPropagation();
    var historial = document.querySelector('.historial');
    historial.classList.toggle('historial-activo');
});

document.addEventListener('click', function (event) {
    var historial = document.querySelector('.historial');
    if (historial.classList.contains('historial-activo') && !historial.contains(event.target) && event.target.id !== 'ig2') {
        historial.classList.remove('historial-activo');
    }
});




document.addEventListener('DOMContentLoaded', function () {
    var customSelects = document.querySelectorAll('.custom-select');

    customSelects.forEach(function (customSelect) {
        var selectHeader = customSelect.querySelector('.select-header');

        selectHeader.addEventListener('click', function (event) {
            event.stopPropagation();

            customSelects.forEach(function (otherSelect) {
                if (otherSelect !== customSelect) {
                    otherSelect.classList.remove('show');
                }
            });

            customSelect.classList.toggle('show');
        });

    });

    document.addEventListener('click', function (event) {
        if (!event.target.closest('.custom-select')) {
            customSelects.forEach(function (customSelect) {
                customSelect.classList.remove('show');
            });
        }
    });
});