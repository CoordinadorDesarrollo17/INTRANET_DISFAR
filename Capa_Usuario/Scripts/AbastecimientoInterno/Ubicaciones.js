function mostrarCamposUbicacion(accion) {
    if (accion == 'SI') {
        document.querySelector('#div_codigos').style.display = 'none'
        document.querySelector('#CodigoUbicacion').value = 'UBI-SISTEMA'
        resetValoresVacios([ "rackBloque", "posicion", "nivel"])
    } else {
        document.querySelector('#div_codigos').style.display = 'block'
        resetValoresVacios(["CodigoUbicacion"])
    }
}

let codigoUbicacion = ["", "", "", ""]

function generarCodigoUbicacion(codigo, posicion) {
    codigo = codigo.trim()

    if (codigo === "" || (posicion === 3 && isNaN(codigo))) {
        codigoUbicacion[posicion] = ""
    } else {
        codigoUbicacion[posicion] = codigo.toUpperCase()
    }

    document.querySelector('#CodigoUbicacion').value = codigoUbicacion.join("-")
}

function llenarItemCode(elemento) {
    const selectedOption = elemento.options[elemento.selectedIndex]
    const dataAttributes = selectedOption.dataset
    document.querySelector('#ItemCode').value = dataAttributes.itemcode
}