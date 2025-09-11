// en general
let aside = document.getElementById("menu-abierto");

    document.addEventListener("click", function(e){
        if(e.target.classList.contains("menu")) { // busca por clase
            aside.classList.toggle("visible");
        } 
        else{
            aside.classList.remove("visible");
        }
    })


// en vista index
let modal = document.querySelector(".modal");
    document.querySelector("#cerrar").addEventListener("click", function(){
        modal.style.display="none";
    });

let crear = document.querySelector("#aggTaller").addEventListener("click", function(){
        modal.style.display="block";
    });