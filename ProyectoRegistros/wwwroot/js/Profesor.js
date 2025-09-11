

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

/*MENU PRINCIPAL*/

let aside = document.getElementById("menu-abierto");

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