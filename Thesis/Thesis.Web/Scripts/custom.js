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

function OnError(xhr, errorType, exception) {
    var responseText;
    $("#dialog").html("");
    try {
        responseText = jQuery.parseJSON(xhr.responseText);
        $("#dialog").append("<div><b>" + errorType + " " + exception + "</b></div>");
        $("#dialog").append("<div><u>Exception</u>:<br /><br />" + responseText.ExceptionType + "</div>");
        $("#dialog").append("<div><u>StackTrace</u>:<br /><br />" + responseText.StackTrace + "</div>");
        $("#dialog").append("<div><u>Message</u>:<br /><br />" + responseText.Message + "</div>");
    } catch (e) {
        responseText = xhr.responseText;
        $("#dialog").html(responseText);
    }
    $("#dialog").dialog({
        title: "jQuery Exception Details",
        width: 700,
        buttons: {
            Close: function () {
                $(this).dialog('close');
            }
        }
    });
}