document.addEventListener("DOMContentLoaded", function () {
    const Usuario = "@User.Identity.Name"; // da error
    let talleres = [];

    const modalRecibo = document.getElementById("modal-recibo");
    const txtPadecimientos = document.getElementById("txtpadecimientos");
    const lblPadecimientos = modalRecibo.querySelector("#padecimientos");
    const form = document.getElementById("datos-alumno");
    const chbPadecimiento = document.getElementById("chbpadecimiento");
    // convierte hora a minutos 
    function aMinutos(hora) {
        const [h, m] = hora.split(":").map(Number);
        return h * 60 + m;
    }

    txtPadecimientos.addEventListener("input", function () {
        lblPadecimientos.textContent = txtPadecimientos.value.trim() || "Ninguno";
    });

    function verificarHorarios(e) {
        if (!e.target.matches('input[name="TalleresSeleccionados"]')) return;

        const seleccionados = [];
        document.querySelectorAll('input[name="TalleresSeleccionados"]:checked').forEach(input => {
            const taller = input.closest(".op-taller");
            const dias = taller.querySelector(".label-dias").textContent.split(",").map(d => d.trim().toLowerCase());
            const [inicio, fin] = taller.querySelector(".label-horas").textContent.split("-").map(h => h.trim());
            seleccionados.push({ dias, inicioMin: aMinutos(inicio), finMin: aMinutos(fin) });
        });

        document.querySelectorAll('input[name="TalleresSeleccionados"]').forEach(input => {
            const taller = input.closest(".op-taller");
            const diasTaller = taller.querySelector(".label-dias").textContent.split(",").map(d => d.trim().toLowerCase());
            const [inicio, fin] = taller.querySelector(".label-horas").textContent.split("-").map(h => h.trim());
            const inicioMin = aMinutos(inicio);
            const finMin = aMinutos(fin);

            const choca = seleccionados.some(s => {
                const mismoDia = s.dias.some(d => diasTaller.includes(d));
                return mismoDia && (inicioMin < s.finMin && finMin > s.inicioMin);
            });

            const isChecked = input.checked;
            input.disabled = choca && !isChecked;
            taller.style.opacity = choca && !isChecked ? "0.5" : "1";
        });
    }

    document.addEventListener("change", verificarHorarios);
    const btnFinalizar = document.getElementById("finalizar");
    if (btnFinalizar) {
        btnFinalizar.addEventListener("click", function (e) {
            e.preventDefault();

            if (!form.checkValidity()) {
                form.reportValidity();
                return;
            }
            const formData = new FormData(form);
            fetch('/Profe/Profe/RegistroForm', {
                method: 'POST',
                body: formData
            }).then(async response => {
                const text = await response.text(); 
                console.log("Respuesta del servidor:", text); 

            try {
                const result = JSON.parse(text); 
                if (!result.ok) {
                    alert(result.mensaje);
                    return; 
                }

                alert(result.mensaje);
                modalRecibo.style.display = "block";
            } catch (err) {
                console.error("La respuesta NO era JSON válido:", err);
            }

            // modalRecibo.style.display = "block";
           

            modalRecibo.querySelector("#nombre").textContent = document.getElementById("Alumno_Nombre").value;
            modalRecibo.querySelector("#fechaCumple").textContent = document.getElementById("Alumno_FechaCumple").value;
            modalRecibo.querySelector("#direccion").textContent = document.getElementById("Alumno_Direccion").value;
            modalRecibo.querySelector("#numContacto").textContent = document.getElementById("Alumno_NumContacto").value;
            modalRecibo.querySelector("#padecimientos").textContent = txtPadecimientos.value.trim() || "Ninguno";
            modalRecibo.querySelector("#tutor").textContent = document.getElementById("Alumno_Tutor").value;
            modalRecibo.querySelector("#email").textContent = document.getElementById("Alumno_Email").value;
            modalRecibo.querySelector("#numSecundario").textContent = document.getElementById("Alumno_NumSecundario").value;


            const padecimientos = chbPadecimiento.checked ? (txtPadecimientos.value.trim() || "Ninguno") : "Ninguno";
            lblPadecimientos.textContent = padecimientos;
            
            let total = 0;
            talleres = [];

            document.querySelectorAll('input[name="TalleresSeleccionados"]:checked').forEach(input => {
                const taller = input.closest(".op-taller");
                const nombre = taller.querySelector(".nombreTaller").textContent;
                const precio = taller.querySelector(".precioTaller").textContent;

                const inputDias = taller.querySelector("input[name^='Dias_']");
                const inputHoraInicio = taller.querySelector("input[name^='HoraInicio_']");
                const inputHoraFinal = taller.querySelector("input[name^='HoraFinal_']");
                
                const dias = inputDias ? inputDias.value : taller.querySelector(".label-dias").textContent;
                const horaInicio = inputHoraInicio ? inputHoraInicio.value : "";
                const horaFinal = inputHoraFinal ? inputHoraFinal.value : "";

                const precioNum = parseFloat(precio.replace(/[^0-9.-]+/g, "")) || 0;
                total += precioNum;
                talleres.push(`${nombre} ${dias} - ${horaInicio} a ${horaFinal}`);
            });

            modalRecibo.querySelector("#talleres").innerHTML = talleres.length > 0 ? talleres.join("<br>") : "Ninguno";
            modalRecibo.querySelector("#donativo-total").textContent = `Total: $${total.toFixed(2)}`;
        });
        
        })
    };
    // autorellenado
    const nombreInput = document.getElementById("Alumno_Nombre");
    if (nombreInput) {
        nombreInput.addEventListener("blur", function () {
            const nombre = nombreInput.value.trim();
            if (nombre.length < 3) return; 

            fetch(`/Profe/Profe/BuscarAlumno?nombre=${encodeURIComponent(nombre)}`)
                .then(r => r.json())
                .then(data => {
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
                })
                .catch(err => console.error("Error al buscar alumno:", err));
        });
    }


    const btnCancelar = modalRecibo.querySelector("#cancelar");
    if (btnCancelar) {
        btnCancelar.addEventListener("click", function () {
            modalRecibo.style.display = "none";
        });
    }

    // Checkbox donativo
    const chkDonativo = document.getElementById("chkDonativo");
    if (chkDonativo) {
        chkDonativo.addEventListener("change", function () {
            const isPagado = this.checked;
            document.getElementById("PagadoHidden").value = isPagado ? "true" : "false";
            const label = this.nextElementSibling;
            if (label) label.textContent = isPagado ? "Pagado" : "No pagado";

            const fechaPago = modalRecibo.querySelector("#fechaPago");
            fechaPago.textContent = isPagado ? "Fecha de pago: " + new Date().toLocaleDateString("es-MX") : "";
        });
    }

    const btnAceptar = document.getElementById("aceptarRecibo");
    if (btnAceptar) {
        btnAceptar.addEventListener("click", function () {
            const isPagado = chkDonativo.checked;
            document.getElementById("PagadoHidden").value = isPagado ? "true" : "false";
            modalRecibo.style.display = "none";

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

            form.submit();
        });
    };
     

    
});