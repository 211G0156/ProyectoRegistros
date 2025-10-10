
//const { Alert } = require("../lib/bootstrap/dist/js/bootstrap.esm");

// Boton de para ir a lista alumnos
const verListaButton = document.querySelector(".verLista");
if (verListaButton) {
    verListaButton.addEventListener("click", function () {
        window.location.href = "/Admin/Home/Alumnos";
    });
}

//IG 1 regresar de la lista de alumnos
const regresar = document.querySelector("#ig1");
if (regresar) {
    regresar.addEventListener("click", function () {
        window.location.href = "/Admin";
    });
}
// Boton de volver en alumnos
//const regButton = document.querySelector(".reg");
//if (regButton) {
//    regButton.addEventListener("click", function () {
//        window.location.href = "Index.html";
//    });
//}


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



//var deleteForm = document.querySelector('#modal-DeleteTaller form');
//if (deleteForm) {
//    deleteForm.addEventListener('submit', function (e) {
//        var val = document.getElementById('DeleteId')?.value;
//        console.log('Enviando formulario EliminarTaller, id =', val);
//    });
//}


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


// EDITAR TALLER
document.querySelectorAll(".btneditar").forEach(boton => {
    boton.addEventListener("click", async () => {
        const id = boton.dataset.id;

        try {
            const response = await fetch(`/Admin/Home/GetTaller/${id}`);
            if (!response.ok) throw new Error("Error al obtener taller");

            const data = await response.json();

            document.getElementById("EditId").value = data.id;
            document.getElementById("editNombre").value = data.nombre;
            document.getElementById("editDias").value = data.dias;
            document.getElementById("editEspacios").value = data.lugaresDisp;
            document.getElementById("editHoraInicio").value = data.horaInicio;
            document.getElementById("editHoraFinal").value = data.horaFinal;
            document.getElementById("editEdadMin").value = data.edadMin;
            document.getElementById("editEdadMax").value = data.edadMax;
            document.getElementById("editCosto").value = data.costo;
            document.getElementById("editProfesor").value = data.idUsuario;

            document.getElementById("modal-EditTaller").style.display = "block";
        } catch (err) {
            console.error(err);
            alert("Error al cargar datos del taller");
        }
    });
});


// ELIMINAR TALLER
document.addEventListener("DOMContentLoaded", function () {
    const deleteModal = document.getElementById("modal-DeleteTaller");
    const deleteForm = deleteModal?.querySelector("form");
    const inputDeleteId = document.getElementById("DeleteId");
    const labelRojo = deleteModal.querySelector("label");

    document.querySelectorAll(".btneliminar").forEach(botonImg => {
        botonImg.addEventListener("click", () => {
            const boton = botonImg.closest("button");
            inputDeleteId.value = boton.dataset.id;
            labelRojo.textContent = `${boton.dataset.nombre}`;
            deleteModal.style.display = "block";
        });
    });

    deleteModal.querySelector(".cerrar")?.addEventListener("click", () => {
        deleteModal.style.display = "none";
    });

    if (deleteForm) {
        deleteForm.addEventListener("submit", async function (e) {
            e.preventDefault();
            const id = inputDeleteId.value;
            if (!id) {
                alert("No se encontró el ID del taller.");
                return;
            }

            try {
                const confirmDelete = confirm(
                    "Si el taller tiene alumnos registrados, se aplicará baja lógica. ¿Desea continuar?"
                );
                if (!confirmDelete) return;

                const response = await fetch(`/Admin/Home/EliminarTaller/${id}`, { method: "POST" });
                const text = await response.text();

                if (response.ok) {
                    alert(text);
                    window.location.reload();
                } else {
                    alert(text || "No se pudo eliminar el taller.");
                }
            } catch (err) {
                console.error(err);
                alert("Error al eliminar el taller.");
            }
        });
    }
});



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

//// modal de crear taller
//document.querySelector("#aggTaller").addEventListener("click", function () {
//    modAgregar.style.display = "block";
//});



 //en vista index

document.querySelectorAll(".cerrar").forEach(btnCerrar => {
    btnCerrar.addEventListener("click", function () {
        const modal = btnCerrar.closest(".modal");
        if (modal) {
            modal.style.display = "none";
        }
    });
});

const aggButton = document.querySelector("#aggUsuario, #aggTaller");
if (aggButton) {
    aggButton.addEventListener("click", function () {
        const modAgregar = document.querySelector("#modal-AddTaller");
        modAgregar.style.display = "block";
    });
}
////Abrir modal de editar e ingresa datos
//$(document).on("click", ".btneditar", function (e) {
//    e.preventDefault();
//    let id = $(this).data("id");

//    $.get("/Admin/Home/GetTaller/" + id, function (data) {
//        $("#edit-Id").val(data.id);
//        $("#edit-Nombre").val(data.nombre);
//        $("#edit-Dias").val(data.dias);
//        $("#edit-Espacios").val(data.espacios);
//        $("#edit-HoraInicio").val(data.horaInicio);
//        $("#edit-HoraFinal").val(data.horaFinal);
//        $("#edit-EdadMin").val(data.edadMin);
//        $("#edit-EdadMax").val(data.edadMax);
//        $("#edit-Costo").val(data.costo);
//        $("#edit-Profesor").val(data.idUsuario); // profesor seleccionado

//        $("#modal-EditTaller").fadeIn();
//    });
//});

//// Guardar cambios
//$("#formEditTaller").on("submit", function (e) {
//    e.preventDefault();

//    let taller = {
//        Id: $("#edit-Id").val(),
//        Nombre: $("#edit-Nombre").val(),
//        Dias: $("#edit-Dias").val(),
//        LugaresDisp: $("#edit-Espacios").val(),
//        HoraInicio: $("#edit-HoraInicio").val(),
//        HoraFinal: $("#edit-HoraFinal").val(),
//        EdadMin: $("#edit-EdadMin").val(),
//        EdadMax: $("#edit-EdadMax").val(),
//        Costo: $("#edit-Costo").val(),
//        IdUsuario: $("#edit-Profesor").val()
//    };

//    $.ajax({
//        url: "/Admin/Home/EditarTaller",
//        type: "POST",
//        contentType: "application/json",
//        data: JSON.stringify(taller),
//        success: function (resp) {
//            if (resp.success) {
//                alert("Taller actualizado correctamente");
//                location.reload();
//            }
//        }
//    });
//});



    //// Botón cerrar
    //$(document).on("click", "#modal-EditTaller .cerrar", function () {
    //    $("#modal-EditTaller").fadeOut();
    //});



// this is forrrrr el recibo q sale despues de registrar alumno
let recibo = document.getElementById("modal-recibo")
document.querySelector("#finalizar").addEventListener("click", function () {
    recibo.style.display = "block";
    console.log("pipippip");
});

