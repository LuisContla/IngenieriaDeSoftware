document.addEventListener("DOMContentLoaded", function () {
    const toggleBtn = document.getElementById('modoToggle');
    const icono = document.getElementById('modoIcono');
    const root = document.documentElement;

    function aplicarTemaOscuro() {
        root.setAttribute('data-theme', 'dark');
        localStorage.setItem('tema', 'oscuro');
        if (icono) icono.textContent = '🌞';
    }

    function aplicarTemaClaro() {
        root.setAttribute('data-theme', 'light');
        localStorage.setItem('tema', 'claro');
        if (icono) icono.textContent = '🌙';
    }

    // Aplicar tema guardado
    const tema = localStorage.getItem('tema');
    if (tema === 'oscuro') {
        aplicarTemaOscuro();
    } else {
        aplicarTemaClaro();
    }

    if (toggleBtn) {
        toggleBtn.addEventListener('click', () => {
            const actual = root.getAttribute('data-theme');
            if (actual === 'dark') {
                aplicarTemaClaro();
            } else {
                aplicarTemaOscuro();
            }
        });
    }
});
