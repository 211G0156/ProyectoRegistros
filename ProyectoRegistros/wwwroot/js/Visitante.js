let aside = document.getElementById("menu-abierto");

document.addEventListener("click", function (e) {
    if (e.target.classList.contains("menu")) { // busca por clase
        aside.classList.toggle("visible");
    }
    else {
        aside.classList.remove("visible");
    }
})

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