$(document).ready(function () {
    console.log("Script activo después de carga completa");

    const $modalRecibo = $('#modal-recibo');
    const $txtPadecimientos = $('#txtpadecimientos');
    const $lblPadecimientos = $('#modal-recibo #padecimientos');
    const $form = $('#datos-alumno');

    function aMinutos(hora) {
        const [h, m] = hora.split(':').map(Number);
        return h * 60 + m;
    }
    $('#chbpadecimiento').change(function () {
        const isChecked = $(this).is(':checked');
        $txtPadecimientos.toggle(isChecked).prop('disabled', !isChecked);
        if (!isChecked) {
            $txtPadecimientos.val('');
            $lblPadecimientos.text('Ninguno');
        }
    });

    $txtPadecimientos.on('input', function () {
        $lblPadecimientos.text($(this).val().trim() || 'Ninguno');
    });

    function verificarHorarios(e) {
        const $cambiado = $(e.target);
        console.log("Checkbox clickeado:", $cambiado.val());
        const seleccionados = [];

        $('input[name="TalleresSeleccionados"]:checked').each(function () {
            const $taller = $(this).closest('.op-taller');
            const dias = $taller.find('.label-dias').text().split(',').map(d => d.trim().toLowerCase());
            const [inicio, fin] = $taller.find('.label-horas').text().split('-').map(h => h.trim());
            seleccionados.push({ dias, inicioMin: aMinutos(inicio), finMin: aMinutos(fin) });
        });

        $('input[name="TalleresSeleccionados"]').each(function () {
            const $taller = $(this).closest('.op-taller');
            const diasTaller = $taller.find('.label-dias').text().split(',').map(d => d.trim().toLowerCase());
            const [inicio, fin] = $taller.find('.label-horas').text().split('-').map(h => h.trim());
            const inicioMin = aMinutos(inicio);
            const finMin = aMinutos(fin);

            const choca = seleccionados.some(s => {
                const mismoDia = s.dias.some(d => diasTaller.includes(d));
                return mismoDia && (inicioMin < s.finMin && finMin > s.inicioMin);
            });

            const isChecked = $(this).is(':checked');
            $(this).prop('disabled', choca && !isChecked);
            $taller.css('opacity', choca && !isChecked ? '0.5' : '1');
        });
    }

    $(document).on('change', 'input[name="TalleresSeleccionados"]', verificarHorarios);
    $('#finalizar').on('click', function (e) {
        e.preventDefault();

        if (!$form[0].checkValidity()) {
            $form[0].reportValidity();
            return;
        }
        $modalRecibo.show();
        $('#modal-recibo #nombre').text($('#Alumno_Nombre').val());
        $('#modal-recibo #fechaCumple').text($('#Alumno_FechaCumple').val());
        $('#modal-recibo #direccion').text($('#Alumno_Direccion').val());
        $('#modal-recibo #numContacto').text($('#Alumno_NumContacto').val());
        $('#modal-recibo #padecimientos').text($txtPadecimientos.val().trim() || 'Ninguno');
        $('#modal-recibo #tutor').text($('#Alumno_Tutor').val());
        $('#modal-recibo #email').text($('#Alumno_Email').val());
        $('#modal-recibo #numSecundario').text($('#Alumno_NumSecundario').val());

        let total = 0;
        const talleres = [];
        $('input[name="TalleresSeleccionados"]:checked').each(function () {
            const $taller = $(this).closest('.op-taller');
            const tallerNombre = $taller.find('.nombreTaller').text();
            const tallerPrecio = $taller.find('.precioTaller').text();
            const dias = $taller.find('.label-dias').text();
            const hora = $taller.find('.label-horas').text();

            const precioNumerico = parseFloat(tallerPrecio.replace(/[^0-9.-]+/g, ''));
            total += precioNumerico;
            talleres.push(`${tallerNombre} ${dias} - ${hora}`);
        });

        $('#modal-recibo #talleres').html(talleres.length > 0 ? talleres.join('<br>') : 'Ninguno');
        $('#modal-recibo #donativo-total').text(`Total: $${total.toFixed(2)}`);
    });


    ////////////


    $('#modal-recibo #cancelar').click(function () {
        $('#modal-recibo').css('display', 'none');
    });

    // Marcar pago de donativo
    $('#chkDonativo').off('change').on('change', function () {
        const isPagado = $(this).is(':checked');
        const idAlumno = $('#Alumno_Id').val();

        $('#PagadoHidden').val(isPagado ? 'true' : 'false');
        $('#chkDonativo').next('label').text(isPagado ? 'Pagado' : 'No pagado');
        if (isPagado) {
            $('#modal-recibo #fechaPago').text('Fecha de pago: ' + new Date().toLocaleDateString('es-MX'));
        } else {
            $('#modal-recibo #fechaPago').text('');
        }
    });
    // si fue pagado o no el total
    $('#aceptarRecibo').off('click').on('click', function () {
        const isPagado = $('#chkDonativo').is(':checked');
        $('#PagadoHidden').val(isPagado ? 'true' : 'false');
        $('#modal-recibo').hide();
        $('#datos-alumno')[0].submit();
    });





    $('#aceptarRecibo').on('click', function () {
        const tutor = $('#Alumno_Tutor').val();
        const total = $('#donativo-total').text().replace('Total: ', '');
        const fecha = new Date().toLocaleDateString('es-MX');
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
                                Recibimos de <strong>${tutor}</strong>, la cantidad de
                                <strong>${total}</strong> por concepto de <strong>inscripción a talleres</strong>,
                                recibido por <strong>Centro Cultural Lili y Edilberto Montemayor Seguy</strong>,
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
        const nuevaVentana = window.open('', '_blank');
        nuevaVentana.document.write(htmlRecibo);
        nuevaVentana.document.close();
        nuevaVentana.focus();
        nuevaVentana.print();
    });



});