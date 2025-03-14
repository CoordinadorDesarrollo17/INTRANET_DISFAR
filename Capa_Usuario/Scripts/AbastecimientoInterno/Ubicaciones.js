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

async function registrarDatos(actionController, listado) {
    const form = document.querySelector('#frm_registroUbicacion');

    if (!form) {
        console.log('Error: El formulario no se encontró.');
        return;
    }

    // Crear FormData y convertirlo a un objeto JSON
    const formData = new FormData(form);
    const jsonData = Object.fromEntries(formData.entries());

    try {
        const response = await fetch(`/AbastecimientoInterno/${actionController}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(jsonData) // Enviar los datos como JSON
        });

        if (!response.ok) {
            throw new Error(`Error en la solicitud: ${response.statusText}`);
        }

        const data = await response.json();

        if (data.Icono == 'success') {
            resetValoresVacios(["CodigoUbicacion", "rackBloque", "posicion", "nivel"])
            if (typeof codigoUbicacion !== 'undefined' && codigoUbicacion.length > 0) { codigoUbicacion = ["", "", "", ""] }        // Limpiar variable local
        }

        Swal.fire({
            title: data.Titulo,
            html: Array.isArray(data.Mensajes) && data.Mensajes.length > 0
                ? data.Mensajes.join('<br>')
                : data.Mensajes || "",
            icon: data.Icono,
            allowEscapeKey: false,
            allowOutsideClick: false,
            showConfirmButton: true
        }).then((result) => {
            if (result.isConfirmed) {
                $('#div_listado').load(`/AbastecimientoInterno/${listado}`).fadeIn("fast")
            }
        });


    } catch (error) {
        console.error('Error:', error);
    }
}