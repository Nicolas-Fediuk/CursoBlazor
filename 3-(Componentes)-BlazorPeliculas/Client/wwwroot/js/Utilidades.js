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