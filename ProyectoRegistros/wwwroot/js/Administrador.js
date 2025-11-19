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
// CERRAR Y LIMPIAR MODALES
document.querySelectorAll(".cerrar").forEach(btnCerrar => {
    btnCerrar.addEventListener("click", function () {
        const modal = btnCerrar.closest(".modal");
        if (modal) {
            modal.style.display = "none";
            const form = modal.querySelector("form");
            if (form) form.reset();
        }
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

        botonImg.addEventListener("click", async () => {
            const boton = botonImg.closest("button");
            const id = boton.dataset.id;
            const nombre = boton.dataset.nombre;

            try {
                const resp = await fetch(`/Admin/Home/VerificarTaller/${id}`);
                const data = await resp.json();

                if (data.tieneAlumnos) {
                    const confirmacion = confirm(
                        `El taller "${nombre}" tiene alumnos registrados.\n¿Deseas eliminarlo de todos modos?`
                    );
                    if (!confirmacion) {
                        if (deleteModal) deleteModal.style.display = "none";
                        return;
                    }
                }

                inputDeleteId.value = id;
                labelRojo.textContent = nombre;
                deleteModal.style.display = "block";

            } catch (err) {
                console.error("Error al verificar taller:", err);
                alert("Error al verificar si el taller tiene alumnos.");
            }
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
                    if (text.includes("No puedes eliminar tu propio usuario")) {
                        alert("No puedes eliminar tu propio usuario.");
                    } else {
                        alert(text || "No se pudo eliminar el usuario.");
                    }
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
document.addEventListener("DOMContentLoaded", function () {
    const formAdd = document.querySelector("#modal-AddTaller form");

    if (formAdd) {
        formAdd.addEventListener("submit", async function (e) {
            e.preventDefault();

            const formData = new FormData(formAdd);

            try {
                const resp = await fetch("/Admin/Home/AgregarTaller", {
                    method: "POST",
                    body: formData
                });

                const data = await resp.json();

                alert(data.message);

                if (data.success) {
                    const modal = document.getElementById("modal-AddTaller");
                    if (modal) modal.style.display = "none";
                    formAdd.reset();
                    window.location.reload();
                }

            } catch (error) {
                console.error("Error al agregar taller:", error);
                alert("Ocurrió un error al intentar guardar el taller.");
            }
        });
    }
});


/* -------------------- RECIBO -------------------- */



document.addEventListener('DOMContentLoaded', function () {
    // filtrar por edad
    try {
        const contenedorTalleres = document.getElementById('contenedor-talleres');
        const listaEsperaOpciones = document.querySelectorAll(".seleccionar-opciones .opciones");
        const edadInput = document.getElementById('edadAlumno');
        if (edadInput && contenedorTalleres) {
            edadInput.addEventListener('input', () => {
                const edad = parseInt(edadInput.value);  //10
                // pa limpiar cada q cambie el valor
                const talleres = contenedorTalleres.querySelectorAll('.op-taller');    
                document.querySelectorAll('input[name="TalleresSeleccionados"]').forEach(c => c.checked = false);
                document.querySelectorAll('input[name="ListaEsperaSeleccionada"]').forEach(c => c.checked = false);
                document.querySelector('.seleccionar').textContent = "Seleccionar";
                if (isNaN(edad) || edad === 0) {
                    talleres.forEach(taller => taller.style.display = 'none');
                    listaEsperaOpciones.forEach(op => op.style.display = 'none');
                    contenedorTalleres.style.display = 'none';
                    return;
                }
                contenedorTalleres.style.display = 'block';
                talleres.forEach(taller => {
                    const edadMin = parseInt(taller.getAttribute('data-edadmin')) || 0;
                    const edadMaxAttr = taller.getAttribute('data-edadmax');
                    const edadMax = edadMaxAttr && !isNaN(parseInt(edadMaxAttr)) ? parseInt(edadMaxAttr) : null;
                    const cumpleEdad = edad >= edadMin && (edadMax === null || edad <= edadMax);
                    taller.style.display = cumpleEdad ? '' : 'none';
                });
                listaEsperaOpciones.forEach(op => {
                    const edadMin = parseInt(op.getAttribute('data-edadmin')) || 0;
                    const edadMaxAttr = op.getAttribute('data-edadmax');
                    const edadMax = edadMaxAttr && !isNaN(parseInt(edadMaxAttr)) ? parseInt(edadMaxAttr) : null;

                    const cumpleEdad = edad >= edadMin && (edadMax === null || edad <= edadMax);
                    op.style.display = cumpleEdad ? '' : 'none';
                });
            });
        }
    } catch {
        console.log("Error");
    }

    /* Aqui es para lo de lista de espera */ 
    const selectDisplay = document.querySelector(".seleccionar");
    const contOpciones = document.querySelector(".seleccionar-opciones");
    contOpciones.style.display = "none";
    selectDisplay.addEventListener("click", () => {
        contOpciones.style.display = contOpciones.style.display === "block" ? "none" : "block";
    });
    //cerrar
    document.addEventListener("click", (e) => {
        if (!selectDisplay.contains(e.target) && !contOpciones.contains(e.target)) {
            contOpciones.style.display = "none";
        }
    });

    document.querySelectorAll(".opciones input[type='checkbox']").forEach(chk => {
        chk.addEventListener("change", () => {
            const seleccionados = [...document.querySelectorAll(".opciones input:checked")].map(x => x.parentElement.textContent.trim());
            selectDisplay.textContent = seleccionados.length ? seleccionados.join(", ") : "Seleccionar talleres";
        });
    });



    let talleres = [];

    const modalRecibo = document.getElementById("modal-recibo");
    const txtPadecimientos = document.getElementById("txtpadecimientos");
    const lblPadecimientos = document.getElementById("padecimientos");   // del recibo
    const form = document.getElementById("datos-alumno");
    const chbPadecimiento = document.getElementById("chbpadecimiento");
    const btnFinalizar = document.getElementById("finalizar");


    if (chbPadecimiento) {
        chbPadecimiento.addEventListener("change", () => {
            if (chbPadecimiento.checked) {
                txtPadecimientos.disabled = false;
                txtPadecimientos.classList.add("mostrar");
                txtPadecimientos.focus();
            } else {
                txtPadecimientos.disabled = true;
                txtPadecimientos.value = "";
                txtPadecimientos.classList.remove("mostrar");
            }
        });
    };

    if (btnFinalizar) {
        btnFinalizar.addEventListener("click", async function (e) {
            e.preventDefault();

            if (!form.checkValidity()) {
                form.reportValidity();
                return;
            }
            const chkDonativo = document.getElementById("chkDonativo");
            document.getElementById("PagadoHidden").value = chkDonativo.checked ? "true" : "false";


            const formData = new FormData(form);

            const talleresSeleccionados = [];
            document.querySelectorAll('input[name="TalleresSeleccionados"]:checked').forEach(chk => talleresSeleccionados.push(chk.value));

            //talleresSeleccionados.forEach(val => {
            //    formData.append("TalleresSeleccionados", val);
            //});

            const listaEspera = [];
            const selectEspera = document.getElementById("ListaEsperaSeleccionada");

            if (selectEspera) {
                for (const option of selectEspera.options) {
                    if (option.selected) {
                        listaEspera.push(option.value);
                    }
                }
            }

            try {
                const response = await fetch('/Admin/Home/RegistroForm', {
                    method: 'POST',
                    body: formData
                });

                const text = await response.text();
                console.log("Respuesta del servidor:", text);

                let result;

                try {
                    result = JSON.parse(text);
                    document.getElementById("Alumno_Id").value = result.idAlumno; 
                } catch {
                    console.error("Respuesta no era JSON. Ignorada.");
                    return;
                }
                alert(result.mensaje);

                if (!result.ok) return;
                modalRecibo.style.display = "block";
                llenarModalRecibo();

            } catch (error) {
                console.error("Error en fetch:", error);
            }
        });
    }

    function llenarModalRecibo() {
        try {
            modalRecibo.querySelector("#nombre").textContent = Alumno_Nombre.value;
            modalRecibo.querySelector("#fechaCumple").textContent = Alumno_FechaCumple.value;
            modalRecibo.querySelector("#direccion").textContent = Alumno_Direccion.value;
            modalRecibo.querySelector("#numContacto").textContent = Alumno_NumContacto.value;
            modalRecibo.querySelector("#tutor").textContent = Alumno_Tutor.value;
            modalRecibo.querySelector("#email").textContent = Alumno_Email.value;
            modalRecibo.querySelector("#numSecundario").textContent = Alumno_NumSecundario.value;

            const padecimientos = chbPadecimiento.checked ? (txtPadecimientos.value.trim() || "Ninguno") : "Ninguno";
            lblPadecimientos.textContent = padecimientos;

            let total = 0;
            let talleres = [];
            let listaEspera = [];

            document.querySelectorAll('input[name="TalleresSeleccionados"]:checked').forEach(input => {
                const taller = input.closest(".op-taller");
                const nombre = taller.querySelector(".nombreTaller").textContent;
                const precio = (taller.querySelector(".precioTaller")?.textContent || "0");
                const precioNum = parseFloat(precio.replace(/[^0-9.-]+/g, "")) || 0;

                const dias = (taller.querySelector("input[name^='Dias_']")?.value) || taller.querySelector(".label-dias").textContent;
                const horaInicio = taller.querySelector("input[name^='HoraInicio_']")?.value || "";
                const horaFinal = taller.querySelector("input[name^='HoraFinal_']")?.value || "";
                total += precioNum;

                talleres.push(`${nombre} ${dias} - ${horaInicio} a ${horaFinal}`);
            });
           // con lista de espera
            document.querySelectorAll('input[name="ListaEsperaSeleccionada"]:checked').forEach(input => {
                const label = input.closest("label.opciones").textContent.trim();
                listaEspera.push(label);
            });


            modalRecibo.querySelector("#talleres").innerHTML = talleres.length > 0 ? talleres.join("<br>") : "Ninguno";

            const lblListaEspera = modalRecibo.querySelector("#talleres");
            if (lblListaEspera) {
                lblListaEspera.innerHTML = listaEspera.length > 0 ? listaEspera.join("<br>") : talleres; 
                modalRecibo.querySelector("#donativo-total").textContent = `Total: $`;
            }
            modalRecibo.querySelector("#donativo-total").textContent = `Total: $${total.toFixed(2)}`;


            txtPadecimientos.addEventListener("input", function () {
                lblPadecimientos.textContent = txtPadecimientos.value.trim() || "Ninguno";
            });
        } catch {
            console.log("puaj");
        }
    };
    // autorellenado
    const nombreInput = document.getElementById("Alumno_Nombre");
    if (nombreInput) {
        nombreInput.addEventListener("blur", function () {
            const nombre = nombreInput.value.trim();
            if (nombre.length < 3) return;

            fetch(`/Admin/Home/BuscarAlumno?nombre=${encodeURIComponent(nombre)}`).then(r => r.json()).then(data => {
                if (data) {
                    console.log("Alumno encontrado:", data);
                    document.querySelector('input[name="Alumno.FechaCumple"]').value = data.fechaCumple?.split('T')[0] || "";
                    document.querySelector('input[name="Alumno.Direccion"]').value = data.direccion || "";
                    document.querySelector('input[name="Alumno.Edad"]').value = data.edad || "";
                    document.querySelector('input[name="Alumno.NumContacto"]').value = data.numContacto || "";
                    document.querySelector('input[name="Alumno.Tutor"]').value = data.tutor || "";
                    document.querySelector('input[name="Alumno.Email"]').value = data.email || "";
                    document.querySelector('input[name="Alumno.NumSecundario"]').value = data.numSecundario || "";
                    document.querySelector('textarea[name="Alumno.Padecimientos"]').value = data.padecimientos || "";

                } else {
                    console.log(" No se encontró alumno con ese nombre.");
                }
            }).catch(err => console.error("Error al buscar alumno:", err));
        });
    }

    const btnCancelar = modalRecibo.querySelector("#cancelar");
    if (btnCancelar) {
        btnCancelar.addEventListener("click", function () {
            modalRecibo.style.display = "none";
        });
    }


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


       // Checkbox donativo
    chkDonativo.addEventListener("change", async function () {
    const isPagado = this.checked;
    const idAlumno = document.getElementById("Alumno_Id").value;

    if (!idAlumno) {
        return;
    }

    try {
        await fetch(`/Admin/Home/ActualizarPago?idAlumno=${idAlumno}&pagado=${isPagado}`, {
            method: 'POST'
        });
    } catch (err) {
        console.error("Error al actualizar pago:", err);
    }
});


    const btnAceptar = document.getElementById("aceptarRecibo");
    const btnLimpiar = document.getElementById("limpiar");

    /* RECIBO PARA IMPRIMIR */
    if (btnAceptar) {
        btnAceptar.addEventListener("click", function () {
            modalRecibo.style.display = "none";

            const isPagado = chkDonativo.checked;
            document.getElementById("PagadoHidden").value = isPagado ? "true" : "false";
            if (!isPagado) {
                return;
            }
            const nombre = document.getElementById("Alumno_Nombre").value;
            const total = document.getElementById("donativo-total").textContent.replace("Total: ", "");
            const fecha = new Date().toLocaleDateString("es-MX");
            const tal = Array.from(document.querySelectorAll('input[name="TalleresSeleccionados"]:checked')).map(input => input.nextElementSibling.textContent.trim());
            const talleresTexto = tal.length > 0 ? tal.join("<br>") : "Ninguno";

            const htmlRecibo = `
                <html>
                <head>
                <style>
                @media screen {
                    .recibo {
                        width:60%;
                    }
                    .encabezado img {
                            width:6vw !important;
                        }
                }
            // esta parte es la que se ve al imprimir
                .recibo {
                        color: lightslategray;
                        font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif;
                    }

                    fieldset {
                        border: 2px solid black;
                        padding: 20px;
                    }

                .encabezado {
                    display: inline-flex;
                    justify-content: space-between;
                    width: 100%;
                    height: 10vh;
                }

                    .encabezado img {
                        height: 5vh;
                        width:15vw;
                    }

                .cuerpo {
                    text-align: justify;
                    line-height: 3vh;
                }
                .pie-pagina {
                    text-align: center;
                    margin-top: 5vh;
                }
                </style>
                </head>
                <body>
                    <div class="recibo">
                        <fieldset>
                            <section class="encabezado">
                                <img src="/img/logo2.png">
                                <h2>RECIBO DE PAGO</h2>
                                <img src="/img/logo.png">
                            </section>
                            <section class="cuerpo">
                                <p>
                                    Recibimos de <strong>${nombre}</strong>, la cantidad de
                                    <strong>${total}</strong> por concepto de inscripción a los siguientes talleres:<br>
                                    ${talleresTexto}<br><br>
                                    Registro realizado por: <strong>${Usuario}</strong><br>
                                    Recibido por <strong>Centro Cultural Lili y Edilberto Montemayor Seguy</strong>,
                                    con dirección en <strong>Amador Chapa 186, Zona Centro, 26700 Sabinas, Coahuila</strong>
                                    y teléfono <strong>861 612 1225</strong> con fecha de <strong>${fecha}</strong>.
                                </p>
                            </section>
                            <section class="pie-pagina">
                                <hr size="1" width="40%">
                                <label>FIRMA</label>
                            </section>
                        </fieldset>
                    </div>
                </body>
                </html>`;

            const nuevaVentana = window.open("", "_blank");
            nuevaVentana.document.write(htmlRecibo);
            nuevaVentana.document.close();
            nuevaVentana.focus();
            nuevaVentana.print();

        });
    };
    /* LIMPIAR DATOS DEL FORM */
    btnLimpiar.addEventListener("click", () => {
        form.reset();
        //reset a lista talleres
        document.querySelectorAll('input[name="TalleresSeleccionados"]').forEach(c => c.checked = false);
        document.querySelector('.seleccionar').textContent = "";

        chbPadecimiento.checked = false;
        txtPadecimientos.disabled = true;
        txtPadecimientos.value = "";

        if (modalRecibo) {
            modalRecibo.querySelector("#talleres").innerHTML = "";
            modalRecibo.querySelector("#donativo-total").textContent = "Total: $0.00";
        }

    });
});

