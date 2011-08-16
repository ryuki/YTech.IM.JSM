/// <reference path="jquery-1.5-vsdoc.js" />

// Numeric only control handler
jQuery.fn.ForceNumericOnly =
function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            alert(e.keyCode);
            var key = e.charCode || e.keyCode || 0;
            // allow backspace, tab, delete, arrows, numbers and keypad numbers ONLY
            return (
                key == 8 ||
                key == 9 ||
                key == 46 ||
                (key >= 37 && key <= 40) ||
                (key >= 48 && key <= 57) ||
                (key >= 96 && key <= 105));
        })
    })
};

$(function () {
    $('.btn').append($('<span />').addClass('helper'));
});

function RemoveAttribute(ctlId, attr) {
   // alert($.browser.msie);
   // alert($.browser.version);
    if ($.browser.msie && $.browser.version == '6.0') {
       // alert('this ie6 and ie');
        //ie6 bugs
        document.getElementById(ctlId).setAttribute(attr, "");
    }
    else {
        $("#" + ctlId).removeAttr(attr);
    }
}
