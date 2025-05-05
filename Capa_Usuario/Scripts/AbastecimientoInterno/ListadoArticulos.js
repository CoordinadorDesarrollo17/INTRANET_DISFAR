function validarTodosLosStocks() {
    const campos = document.querySelectorAll('.cls-stockSolicitado:not([hidden])');
    let todoEsValido = true;
    let primerCampoInvalido = null;

    campos.forEach(input => {
        const stockMax = parseFloat(input.getAttribute('data-stock-max') || "0");
        const valor = parseFloat(input.value || "0");

        if (input.value.trim() !== "") {
            if (valor > stockMax) {
                if (!primerCampoInvalido) {
                    primerCampoInvalido = input;
                }
                todoEsValido = false;
            }
        }
    });

    // Mostrar u ocultar botón guardar
    const btn = document.querySelector('#btn_guardarArticulos');
    if (btn) {
        btn.style.display = todoEsValido ? 'block' : 'none';
    }

    // Obtener inputs con .cls-cantidadManual
    const inputs = Array.from(document.querySelectorAll('.cls-cantidadManual'));

    // Encontrar el último `data-posicion` donde Master o Saldo tenga valor
    let maxFilaConValor = 0;
    const valoresPorFila = {};

    inputs.forEach(input => {
        const pos = parseInt(input.dataset.posicion, 10);
        const val = input.value.trim();

        if (!valoresPorFila[pos]) valoresPorFila[pos] = false;
        if (val !== '') {
            valoresPorFila[pos] = true;
            if (pos > maxFilaConValor) {
                maxFilaConValor = pos;
            }
        }
    });

    // Desbloquear siguiente fila completa
    inputs.forEach(input => {
        const pos = parseInt(input.dataset.posicion, 10);
        const liberar = (pos === maxFilaConValor + 1);

        if (!valoresPorFila[pos] && !liberar) {
            input.readOnly = true;
            input.classList.add('cls-readonly');
        } else {
            input.readOnly = false;
            input.classList.remove('cls-readonly');
        }
    });

    // Calcular salidas con posible multiplicación
    const salidas = {};

    inputs.forEach(input => {
        const target = input.dataset.idtotalsalida;
        if (!target) return;

        let valor = parseFloat(input.value) || 0;

        // Solo multiplicar si tiene atributo data-multiplicarpor
        const multiplicador = parseFloat(input.dataset.multiplicarpor || "1");

        // Detectar si el campo es tipo Master (según ID o nombre)
        if (input.id.includes("stockSolicitadoMaster")) {
            valor *= multiplicador;
        }

        if (!salidas[target]) salidas[target] = 0;
        salidas[target] += valor;
    });

    // Actualizar inputs de salida
    Object.entries(salidas).forEach(([id, total]) => {
        const salidaInput = document.getElementById(id);
        if (salidaInput) {
            salidaInput.value = total;
        }
    });

    // Mostrar alerta si hay campo inválido
    if (primerCampoInvalido) {
        Swal.fire({
            icon: 'warning',
            title: 'Cantidad inválida',
            text: 'La cantidad solicitada no puede ser mayor al stock disponible.',
            confirmButtonText: '<i class="bi bi-check-lg"></i> Entendido'
        }).then(() => {
            primerCampoInvalido.focus();
        });
    }
}
