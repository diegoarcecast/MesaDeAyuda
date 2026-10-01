(function (window, $) {
    "use strict";

    var pendingRequests = 0;
    var showTimer;
    var slowTimer;

    function loader() {
        return document.getElementById("globalLoader");
    }

    function setLoaderText(title, message) {
        var titleElement = document.getElementById("globalLoaderTitle");
        var messageElement = document.getElementById("globalLoaderMessage");
        if (titleElement && title) { titleElement.textContent = title; }
        if (messageElement && message) { messageElement.textContent = message; }
    }

    function showLoader(title, message) {
        var element = loader();
        if (!element) { return; }
        setLoaderText(title || "Cargando…", message || "Estamos preparando la información.");
        element.className = "global-loader is-visible";
        element.setAttribute("aria-hidden", "false");
        document.body.setAttribute("aria-busy", "true");
    }

    function hideLoader() {
        var element = loader();
        window.clearTimeout(showTimer);
        window.clearTimeout(slowTimer);
        if (!element) { return; }
        element.className = "global-loader";
        element.setAttribute("aria-hidden", "true");
        document.body.removeAttribute("aria-busy");
        setLoaderText("Cargando…", "Estamos preparando la información.");
    }

    function updateNetworkStatus() {
        var status = document.getElementById("networkStatus");
        if (!status) { return; }
        if (window.navigator.onLine) {
            status.setAttribute("hidden", "hidden");
        } else {
            status.removeAttribute("hidden");
            hideLoader();
        }
    }

    window.INAMU = window.INAMU || {};
    window.INAMU.loading = { show: showLoader, hide: hideLoader };

    $(function () {
        updateNetworkStatus();
        $(window).on("online offline", updateNetworkStatus);

        $(document).ajaxSend(function () {
            pendingRequests += 1;
            window.clearTimeout(showTimer);
            showTimer = window.setTimeout(function () { showLoader(); }, 250);
            window.clearTimeout(slowTimer);
            slowTimer = window.setTimeout(function () {
                if (pendingRequests > 0) {
                    showLoader("La solicitud está tardando más de lo esperado", "Seguimos esperando una respuesta del servidor.");
                }
            }, 10000);
        });

        $(document).ajaxComplete(function () {
            pendingRequests = Math.max(0, pendingRequests - 1);
            if (pendingRequests === 0) { hideLoader(); }
        });

        $(document).ajaxError(function (event, xhr) {
            if (xhr && xhr.status === 0 && !window.navigator.onLine) { updateNetworkStatus(); }
        });

        $("form").on("submit", function () {
            if (!$(this).attr("onsubmit") && this.checkValidity && this.checkValidity()) {
                showLoader("Procesando…", "Estamos guardando la información de forma segura.");
            }
        });

        window.addEventListener("beforeunload", function () {
            showLoader("Cargando pantalla…", "Estamos preparando la siguiente vista.");
        });
    });
}(window, window.jQuery));
