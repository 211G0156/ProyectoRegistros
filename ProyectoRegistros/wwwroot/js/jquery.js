document.addEventListener("DOMContentLoaded", function () {
    //function convertirAPM(hora) {
    //    hora = hora.trim().toUpperCase();
    //    if (!hora.includes("AM") && !hora.includes("PM")) {
    //        return hora;
    //    }
    //    let [time, sufijo] = hora.split(" ");
    //    let [h, m] = time.split(":").map(Number);

    //    if (sufijo === "PM" && h !== 12) h += 12;
    //    if (sufijo === "AM" && h === 12) h = 0;
    //    return `${String(h).padStart(2, "0")}:${String(m).padStart(2, "0")}`;
    //}

    //function aMinutos(hora) {
    //    const [h, m] = hora.split(":").map(Number);
    //    return h * 60 + m;
    //}
    try {
        const contenedorTalleres = document.getElementById('contenedor-talleres');
        const listaEsperaOpciones = document.querySelectorAll(".seleccionar-opciones .opciones");
        const edadInput = document.getElementById('edadAlumno');
        if (edadInput && contenedorTalleres) {
            edadInput.addEventListener('input', () => {
                const edad = parseInt(edadInput.value);  //10

                const talleres = contenedorTalleres.querySelectorAll('.op-taller');
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
    /* ES PARA FILTRAR TALLERES EN BASE A LOS QUE YA ESCOGIO, EVITAR CHOQUE ENTRE HORAS no funciona */
    //function aplicarBloqueoPorHorario() {
    //    const checkboxes = document.querySelectorAll('input[name="TalleresSeleccionados"]');
    //    console.log("ola");
    //    // Lista de talleres ya seleccionados
    //    const seleccionados = Array.from(checkboxes).filter(c => c.checked).map(c => {
    //        const id = c.value;
    //        return {
    //            dias: document.querySelector(`input[name="Dias_${id}"]`).value.trim().toLowerCase(),
    //            inicio: aMinutos(convertirAPM(document.querySelector(`input[name="HoraInicio_${id}"]`).value.trim())),
    //            final: aMinutos(convertirAPM(document.querySelector(`input[name="HoraFinal_${id}"]`).value.trim()))
    //        };
    //    });

    //    checkboxes.forEach(chk => {
    //        const id = chk.value;
    //        const dias = document.querySelector(`input[name="Dias_${id}"]`).value.trim().toLowerCase();
    //        const inicio = aMinutos(convertirAPM(document.querySelector(`input[name="HoraInicio_${id}"]`).value.trim()));
    //        const final = aMinutos(convertirAPM(document.querySelector(`input[name="HoraFinal_${id}"]`).value.trim()));
    //        const taller = chk.closest('.op-taller');

    //        chk.disabled = false;
    //        taller.classList.remove("bloqueado");

    //        if (chk.checked) return;
    //        const choca = seleccionados.some(s =>
    //            s.dias === dias && (inicio < s.final && final > s.inicio)
    //        );
    //        if (choca) {
    //            chk.disabled = true;
    //            taller.classList.add("bloqueado");
    //        }
    //    });
    //}

    //const checkboxes = document.querySelectorAll('input[name="TalleresSeleccionados"]');
    //checkboxes.forEach(c => c.addEventListener("change", aplicarBloqueoPorHorario));

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
                const response = await fetch('/Profe/Profe/RegistroForm', {
                    method: 'POST',
                    body: formData
                });

                const text = await response.text();
                console.log("Respuesta del servidor:", text);

                let result;

                try {
                    result = JSON.parse(text);
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
            modalRecibo.querySelector("#talleres").innerHTML = talleres.length > 0 ? talleres.join("<br>") : "Ninguno";
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

            fetch(`/Profe/Profe/BuscarAlumno?nombre=${encodeURIComponent(nombre)}`).then(r => r.json()).then(data => {
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

    // Checkbox donativo
    //const chkDonativo = document.getElementById("chkDonativo");
    //const lblDonativo = document.getElementById("donativo-total");

    //chkDonativo.addEventListener("change", async function () {
    //    const isPagado = this.checked;
    //    lblDonativo.textContent = isPagado ? "Pagado" : "No pagado";
    //    const idAlumno = document.getElementById("Alumno_Id").value;
    //    try {
    //        await fetch(`/Profe/Profe/ActualizarPago?idAlumno=${idAlumno}&pagado=${isPagado}`, {
    //            method: 'POST'
    //        });
    //    } catch (err) {
    //        console.error("Error al actualizar pago:", err);
    //    }
    //});


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
            const talleresTexto = talleres.length > 0 ? talleres.join("<br>") : "Ninguno";

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

            //form.submit();
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