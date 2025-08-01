//para ejecutar un metodo de c# en js

function a() {
    console.log("Hola")
    DotNet.invokeMethodAsync("BlazorPeliculas.Client", "ObtenerCurrentCount")
        //si el metodo retorna algo
        .then(resultado => {
            console.log("conteo js: " + resultado);
        })
}


function pruebaPuntoNetInstancia(dotnetHelper) {
    console.log("HOla desde el metodo 2")
    dotnetHelper.invokeMethodAsync("IncrementCount");
}

function timerInactivo(dotnetHelper) {
    var timer;
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer);
        //timer = setTimeout(logout, 3*1000) //3 seg
    }

    function logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }
}