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

    // filtrado por edad, en registro
    const contenedorTalleres = document.getElementById('contenedor-talleres');
    const edadInput = document.getElementById('edadAlumno');

    edadInput.addEventListener('input', () => {
        const edad = parseInt(edadInput.value);  //10

        const talleres = contenedorTalleres.querySelectorAll('.op-taller');
        if (isNaN(edad) || edad === 0) {
            talleres.forEach(taller => taller.style.display = 'none');
            contenedorTalleres.style.display = 'none';
            return;
        }
        contenedorTalleres.style.display = 'block';
        talleres.forEach(taller => {
            const edadMin = parseInt(taller.getAttribute('data-edadmin')) || 0;
            const edadMaxAttr = taller.getAttribute('data-edadmax');
            const edadMax = edadMaxAttr && !isNaN(parseInt(edadMaxAttr)) ? parseInt(edadMaxAttr) : null;
            const cumpleEdad = edad >= edadMin && (edadMax === null || edad <= edadMax);
            if (cumpleEdad) {
                taller.style.display = '';
            } else {
                taller.style.display = 'none';
            }
        });
    });

});

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

    document.querySelectorAll('.btneditar').forEach(btnEditar => {
        btnEditar.addEventListener('click', function () {
            let img = this;

            document.getElementById("editID").value = img.dataset.id;
            document.getElementById("editNombre").value = img.dataset.nombre;
            document.getElementById("editTutor").value = img.dataset.tutor;
            document.getElementById("editTel").value = img.dataset.numcontacto;
            document.getElementById("editTel2").value = img.dataset.numsecundario;
            document.getElementById("editPadecim").value = img.dataset.padecimientos;

            modEditar.style.display = "block";
        });
    });

    // modal de eliminar YA FUNCIONA :D

    document.querySelectorAll(".btneliminar").forEach(boton => {
        boton.addEventListener("click", async function () {
            const tbody = document.querySelector("#talleresAlumno");
            const alumnoId = this.dataset.id;
            tbody.innerHTML = "";

            document.querySelector("#deleteID").value = alumnoId;

            try {
                //cargar los talleres por alumno
                const talleres = await FuncionTraerTalleres(alumnoId);
                if (talleres.length === 0) {
                    tbody.innerHTML = `<tr><td colspan="2">Sin talleres</td></tr>`;
                }

                else {
                    talleres.forEach(taller => {
                        const row = document.createElement("tr");
                        // en name va el parametro q pusimos en el controller sip
                        row.innerHTML = `<td><input type="checkbox" name="TalleresEliminar" value="${taller.id}"/></td>
                                    <td>${taller.nombre}</td>`;
                        console.log(taller.nombre);
                        tbody.appendChild(row);
                    });
                }
                modEliminar.style.display = "block";


                document.querySelector("#eliminarAlumno").onclick = async () => {
                    const checkboxes = document.querySelectorAll('input[name="TalleresEliminar"]:checked');
                    const talleresEliminar = Array.from(checkboxes).map(checkbox => parseInt(checkbox.value));

                    if (talleres.length === 0) {
                        alert("Selecciona al menos un taller para eliminar.");
                        return;
                    }

                    const response = await fetch('/Profe/Profe/eliminarTallerDelAlumno', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            Id: parseInt(alumnoId),
                            TalleresEliminar: talleresEliminar
                        })
                    });

                    if (response.ok) {
                        alert("Talleres eliminados correctamente");
                        location.reload();
                    } else {
                        alert("Error al eliminar los talleres");
                    }
                };
            }
            catch (error) {
                console.error("Error", error);
            }
        });

    });


    async function FuncionTraerTalleres(alumnoId) {
        try {
            const response = await fetch(`/Profe/FuncionTraerTalleres/${alumnoId}`);
            if (!response.ok) {
                throw new Error("No se puede");
            }
            return await response.json();
        }
        catch (error) {
            throw error;
        }
    }



// para capturar datos del taller atencion psic.
const buscarTexto = "atencion psicopedagogica";
const limpiar = t => t.normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().trim(); // para problemas con acentos

document.querySelectorAll(".op-taller").forEach(op => {
    op.addEventListener("click", e => {
        e.stopPropagation();
        const checkbox = op.querySelector("input[type='checkbox']");
        const nombre = limpiar(op.querySelector(".nombreTaller").textContent);
        const esAtencion = nombre.includes(limpiar(buscarTexto));

        cerrarInputs();

        if (!esAtencion) {
            checkbox.checked = !checkbox.checked;
            return;
        }

        checkbox.checked = !checkbox.checked;
        ["dias", "horas"].forEach(tipo => {
            const label = op.querySelector(`.label-${tipo}`);
            const inputHidden = op.querySelector(`input[name='${tipo.charAt(0).toUpperCase() + tipo.slice(1)}_${checkbox.value}']`);

            let input = document.createElement("input");
            input.type = "text";
            input.className = `input-${tipo}`;
            input.value = inputHidden ? inputHidden.value : label.textContent.trim() || `Sin ${tipo}`;
            label.style.display = "none";
            label.after(input);

            input.addEventListener("click", ev => ev.stopPropagation());
            if (inputHidden) {
                input.addEventListener("input", () => {
                    inputHidden.value = input.value;
                });
            }
        });
    });
});

document.addEventListener("click", cerrarInputs);

function cerrarInputs() {
    document.querySelectorAll(".op-taller").forEach(op => {
        ["dias", "horas"].forEach(tipo => {
            const input = op.querySelector(`.input-${tipo}`);
            const label = op.querySelector(`.label-${tipo}`);
            if (input) {
                label.textContent = input.value.trim() || `Sin ${tipo}`;
                input.remove();
                label.style.display = "inline-block";
            }
        });
    });
}



});