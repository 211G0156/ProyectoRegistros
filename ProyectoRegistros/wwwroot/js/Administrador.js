
//const { Alert } = require("../lib/bootstrap/dist/js/bootstrap.esm");

// Boton de para ir a lista alumnos
const verListaButton = document.querySelector(".verLista");
if (verListaButton) {
    verListaButton.addEventListener("click", function () {
        window.location.href = "Alumnos.html";
    });
}

// Boton de volver en alumnos
const regButton = document.querySelector(".reg");
if (regButton) {
    regButton.addEventListener("click", function () {
        window.location.href = "Index.html";
    });
}


/*MENU PRINCIPAL*/

let aside = document.getElementById("menu-abierto");
if (aside) {
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
}
/*MENU HISTORIAL*/


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



// crear taller
// en vista index
const modal = document.querySelector(".modal");
const modAgregar = document.querySelector("#modal-AddTaller");
const modEditar = document.querySelector("#modal-EditTaller");
const modEliminar = document.querySelector("#modal-DeleteTaller");




document.querySelectorAll(".cerrar").forEach(btnCerrar => {
    btnCerrar.addEventListener("click", function () {
        // this isssss para encontrar el modal mas cercano
        const modal = btnCerrar.closest(".modal");
        if (modal) {
            modal.style.display = "none";
        }
    });
});

// modal de crear taller
document.querySelector("#aggTaller").addEventListener("click", function () {
    modAgregar.style.display = "block";
});

// modal de editar
document.querySelectorAll(".btneditar").forEach(boton => {
    boton.addEventListener("click", function () {
        modEditar.style.display = "block";
    });

    // modal de eliminar
    document.querySelectorAll(".btneliminar").forEach(boton => {
        boton.addEventListener("click", function () {
            modEliminar.style.display = "block";
        });
    });

});




 //en vista index


document.querySelector(".cerrar").addEventListener("click", function () {
    modal.style.display = "none";

});



let crear = document.querySelector("#aggTaller").addEventListener("click", function () {
    modal.style.display = "block";
});


