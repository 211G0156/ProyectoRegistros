//const { Alert } = require("../lib/bootstrap/dist/js/bootstrap.esm");

/* -------------------- BOTONES PRINCIPALES -------------------- */

// Botón para ir a lista de alumnos
const verListaButton = document.querySelector(".verLista");
if (verListaButton) {
    verListaButton.addEventListener("click", function () {
        window.location.href = "/Admin/Home/Alumnos";
    });
}

// Regresar de la lista de alumnos
const regresar = document.querySelector("#ig1");
if (regresar) {
    regresar.addEventListener("click", function () {
        window.location.href = "/Admin";
    });
}

/* -------------------- MENÚ PRINCIPAL -------------------- */
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

/* -------------------- MENÚ HISTORIAL -------------------- */
document.getElementById('ig2')?.addEventListener('click', function (event) {
    event.stopPropagation();
    var historial = document.querySelector('.historial');
    historial.classList.toggle('historial-activo');
});

document.addEventListener('click', function (event) {
    var historial = document.querySelector('.historial');
    if (historial && historial.classList.contains('historial-activo') && !historial.contains(event.target) && event.target.id !== 'ig2') {
        historial.classList.remove('historial-activo');
    }
});

/* -------------------- SELECT PERSONALIZADO -------------------- */
document.addEventListener('DOMContentLoaded', function () {
    var customSelects = document.querySelectorAll('.custom-select');

    customSelects.forEach(function (customSelect) {
        var selectHeader = customSelect.querySelector('.select-header');
        selectHeader?.addEventListener('click', function (event) {
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

/* -------------------- CERRAR MODALES -------------------- */
document.querySelectorAll(".cerrar").forEach(btnCerrar => {
    btnCerrar.addEventListener("click", function () {
        const modal = btnCerrar.closest(".modal");
        if (modal) modal.style.display = "none";
    });
});

/* -------------------- TALLERES -------------------- */

// EDITAR TALLER
document.querySelectorAll(".btneditar").forEach(boton => {
    if (boton.classList.contains("usuario")) return;
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
    const labelRojo = deleteModal?.querySelector("label");

    document.querySelectorAll(".btneliminar").forEach(botonImg => {
        if (botonImg.classList.contains("usuario")) return;
        botonImg.addEventListener("click", () => {
            const boton = botonImg.closest("button");
            inputDeleteId.value = boton.dataset.id;
            labelRojo.textContent = `${boton.dataset.nombre}`;
            deleteModal.style.display = "block";
        });
    });

    deleteModal?.querySelector(".cerrar")?.addEventListener("click", () => {
        deleteModal.style.display = "none";
    });

    if (deleteForm) {
        deleteForm.addEventListener("submit", async function (e) {
            e.preventDefault();
            const id = inputDeleteId.value;
            if (!id) return alert("No se encontró el ID del taller.");

            try {
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

/* -------------------- USUARIOS -------------------- */

document.addEventListener('DOMContentLoaded', function () {
    // EDITAR USUARIO
    document.querySelectorAll(".btneditar.usuario").forEach(boton => {
        boton.addEventListener("click", async () => {
            const id = boton.dataset.id;
            try {
                const response = await fetch(`/Admin/Usuarios/GetUsuario?id=${id}`);
                if (!response.ok) throw new Error("Error al obtener usuario");

                const data = await response.json();
                document.getElementById("EditUsuarioId").value = data.id;
                document.getElementById("editNombreUsu").value = data.nombre;
                document.getElementById("editCorreo").value = data.correo;
                document.getElementById("editTel").value = data.numTel;
                document.getElementById("editContraseña").value = data.contraseña;

                const rolSelect = document.getElementById("editRol");
                if (rolSelect && data.idRol) rolSelect.value = data.idRol;

                document.getElementById("modal-EditUsuario").style.display = "block";
            } catch (err) {
                console.error(err);
                alert("Error al cargar datos del usuario");
            }
        });
    });

    // ELIMINAR USUARIO
    document.querySelectorAll(".btneliminar.usuario").forEach(boton => {
        boton.addEventListener("click", function () {
            const id = boton.dataset.id;
            const nombre = boton.dataset.nombre;

            const inputDelete = document.getElementById("DeleteUsuarioId");
            const label = document.querySelector("#modal-DeleteUsuario label");
            const modalDel = document.getElementById("modal-DeleteUsuario");

            if (inputDelete) inputDelete.value = id;
            if (label) label.textContent = nombre;
            if (modalDel) modalDel.style.display = "block";
        });
    });

    // CONFIRMAR ELIMINAR USUARIO
    const formDeleteUsuario = document.getElementById("formDeleteUsuario");
    if (formDeleteUsuario) {
        formDeleteUsuario.addEventListener("submit", async function (e) {
            e.preventDefault();

            const id = document.getElementById("DeleteUsuarioId").value;
            if (!id) return alert("No se encontró el ID del usuario.");

            try {
                const formData = new FormData();
                formData.append("id", id);

                const response = await fetch(`/Admin/Usuarios/EliminarUsuario`, {
                    method: "POST",
                    body: formData
                });

                const text = await response.text();
                if (response.ok) {
                    alert(text);
                    window.location.reload();
                } else {
                    alert(text || "No se pudo eliminar el usuario.");
                }
            } catch (err) {
                console.error(err);
                alert("Error al eliminar el usuario.");
            }
        });
    }
});

// AGREGAR USUARIO
document.getElementById("aggUsuario")?.addEventListener("click", function () {
    const modal = document.getElementById("modal-AddUsuario");
    if (modal) modal.style.display = "block";
});

// AGREGAR TALLER
document.getElementById("aggTaller")?.addEventListener("click", function () {
    const modal = document.getElementById("modal-AddTaller");
    if (modal) modal.style.display = "block";
});

/* -------------------- RECIBO -------------------- */
let recibo = document.getElementById("modal-recibo");
const finalizar = document.querySelector("#finalizar");
if (recibo && finalizar) {
    finalizar.addEventListener("click", function () {
        recibo.style.display = "block";
        console.log("pipippip");
    });
}


document.addEventListener('DOMContentLoaded', function () {
    
    // filtrado por edad, en registro
    const edadInput = document.getElementById('edadAlumno');  //10
    const contenedorTalleres = document.getElementById('contenedor-talleres');

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

    

// para capturar datos del taller en atencion psic.
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

            checkbox.checked = true;
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