document.addEventListener('DOMContentLoaded', function () {
    // Código para el historial
    document.getElementById('ig2').addEventListener('click', function (event) {
        event.stopPropagation();
        var historial = document.querySelector('.historial');
        if (historial) {
            historial.classList.toggle('historial-activo');
        }
    });

    document.addEventListener('click', function (event) {
        var historial = document.querySelector('.historial');
        if (historial && historial.classList.contains('historial-activo') && !historial.contains(event.target) && event.target.id !== 'ig2') {
            historial.classList.remove('historial-activo');
        }
    });

    // Código para el menú principal
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

    // Botón para ir a la lista de alumnos
    const verListaButton = document.querySelector(".verLista");
    if (verListaButton) {
        verListaButton.addEventListener("click", function () {
            window.location.href = "/Profe/Profe/Alumnos";
        });
    }

    // Botón de volver en alumnos
    const regButton = document.querySelector(".reg");
    if (regButton) {
        regButton.addEventListener("click", function () {
            window.location.href = "Index.html";
        });
    }
});







// crear taller
// en vista index
const modal = document.querySelector(".modal");
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

