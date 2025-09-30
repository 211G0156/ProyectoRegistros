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

    //IG 1 regresar de la lista de alumnos
    const regresar = document.querySelector("#ig1");
    if (regresar) {
        regresar.addEventListener("click", function () {
            window.location.href = "/Profe/Profe/Index";
        });
    }







    // crear taller
    // en vista index
    const modal = document.querySelector(".modal");
    const modEditar = document.querySelector("#modal-EditAlumno");
    const modEliminar = document.querySelector("#modal-DeleteAlumno");




    document.querySelectorAll(".cerrar").forEach(btnCerrar => {
        btnCerrar.addEventListener("click", function () {
            // this isssss para encontrar el modal mas cercano
            const modal = btnCerrar.closest(".modal");
            if (modal) {
                modal.style.display = "none";
            }
        });
    });


//EDITAR

    document.querySelectorAll('.btneditar').forEach(btn => {
        btn.addEventListener('click', function () {
            let td = this.parentElement;

            document.getElementById("editID").value = td.dataset.id;
            document.getElementById("editNombre").value = td.dataset.nombre;
            document.getElementById("editTutor").value = td.dataset.tutor;
            document.getElementById("editTel").value = td.dataset.numcontacto;
            document.getElementById("editTel2").value = td.dataset.numsecundario;
            document.getElementById("editPadecim").value = td.dataset.padecimientos;

            modEditar.style.display = "block";
        });
    });

    // modal de eliminar NO FUNCIONA :C

   // document.querySelectorAll(".btneliminar").forEach(boton => {
        //boton.addEventListener("click", function () {
            
        //    const alumnoId = this.dataset.id;

        //    const inputId = document.getElementById("deleteID");
        //    inputId.value = alumnoId;

        //    const talleresDelAlumno = allTalleres.filter(t => t.IdAlumno == alumnoId);

        //    let html = "";
        //    talleresDelAlumno.forEach(t => {
        //        html += `
        //        <tr>
        //            <td>${t.IdTallerNavigation.Nombre}</td>
        //            <td><input type="checkbox" name="TalleresEliminar" value="${t.IdTaller}" /></td>
        //        </tr>`;
        //    });

        //    document.getElementById("talleresAlumno").innerHTML = html;
        //    modEliminar.style.display = "block";
            
       // });
});