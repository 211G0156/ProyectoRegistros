/*MENU PRINCIPAL*/

let aside = document.getElementById("menu-abierto");

document.addEventListener("click", function (event) {
    if (event.target.tagName === "I" && event.target.classList.contains("menu")) {
        aside.classList.toggle("visible");
    } else if (aside.classList.contains("visible") && !aside.contains(event.target) && event.target !== document.querySelector(".menu")) {
        aside.classList.remove("visible");
    }
});

document.getElementById("ig2").addEventListener("click", function (event) {
    event.stopPropagation();
    aside.classList.remove("visible");
});




/*MENU DE HISTORIAL*/


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


/*SELECCION CON SPAN Y CHECKBOX*/


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

// en vista index
let modal = document.querySelector(".modal");
document.querySelector("#cerrar").addEventListener("click", function () {
    modal.style.display = "none";
});

let crear = document.querySelector("#aggTaller").addEventListener("click", function () {
    modal.style.display = "block";
});