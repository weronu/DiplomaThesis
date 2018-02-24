function blockUI() {
    var msg = "<div class=\"page-loader\"></div>";
    if (!Modernizr.cssanimations) {
        msg = "<h3 class=\"center-horizontal-50-down text-center\">Loading...</h3";
    }

    $.blockUI({
        message: msg,
        css: {
            background: "rgba(255,255,255, 0.9)",
            border: "1px solid white",
            borderRadius: "1%"
        },
        overlayCSS: {
            opacity: 0.6
        },
        baseZ: 1051
    });
}

function unblockUI() {
    $.unblockUI();
}

function OnError() {
    alert("Error !");
}