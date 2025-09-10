let aside = document.getElementById("menu-abierto");

    document.addEventListener("click", function(e){
        if(e.target.classList.contains("menu")) { // busca por clase
            aside.classList.toggle("visible");
        } 
        else{
            aside.classList.remove("visible");
        }
    })