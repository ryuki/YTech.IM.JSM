// For Non-Supported Browsers
//Start
var Accept = AccepatbleBrowserVersion();
if (!Accept)
//    alert("Syncfusion MVC controls do not support this browser version. Supported browser versions are:\n\t1. Internet Explorer 7+\n\t2. Mozilla 2.0+\n\t3. Safari 3.0+\n\t4. Opera 9+\n\t5. Chrome 1+\nPlease upgrade to the appropriate browser version for better results. ");
function AccepatbleBrowserVersion() {
    var userAgent = navigator.userAgent.toLowerCase();
    var chromeBrowser = /chrome/.test(navigator.userAgent.toLowerCase());
    var version = 0;

    // Is this a version of IE?
    if ($.browser.msie) {
        userAgent = $.browser.version;
        userAgent = userAgent.substring(0, userAgent.indexOf('.'));
        version = userAgent;
        if (version < 7.0)
            return false;
    }

    // Is this a version of Chrome?
    if (chromeBrowser) {
        userAgent = userAgent.substring(userAgent.indexOf('chrome/') + 7);
        userAgent = userAgent.substring(0, userAgent.indexOf('.'));
        version = userAgent;
        // If it is chrome then jQuery thinks it's safari so we have to tell it it isn't
        $.browser.safari = false;
        if (version < 1.0)
            return false;
    }

    // Is this a version of Safari?
    if ($.browser.safari) {
        userAgent = userAgent.substring(userAgent.indexOf('version/') + 8);
        userAgent = userAgent.substring(0, userAgent.indexOf('.'));
        version = userAgent;
        if (version < 3.0)
            return false;
    }

    // Is this a version of Mozilla?
    if ($.browser.mozilla) {
        //Is it Firefox?
        if (navigator.userAgent.toLowerCase().indexOf('firefox') != -1) {
            userAgent = userAgent.substring(userAgent.indexOf('firefox/') + 8);
            userAgent = userAgent.substring(0, userAgent.indexOf('.'));
            version = userAgent;
            if (version < 2.0)
                return false;
        }
    }

    // Is this a version of Opera?
    if ($.browser.opera) {
        userAgent = userAgent.substring(userAgent.indexOf('opera ') + 6);
        userAgent = userAgent.substring(0, userAgent.indexOf('.'));
        version = userAgent;
        if (version < 9.0)
            return false;
    }
    return true;
}
/* Launching Other Products*/
var iisPrefixLink = null;
function viewdemo(product) {
    if (iisPrefixLink != null) {
        window.location = String.format(iisPrefixLink, product);
    }
    else {
        var text = '<%= ResolveUrl("~/StartDevServer.ashx") %>';
        $.get(text + "?product=" + product + ".MVC", null, function(data) {
            window.location = data;
        });
    }

}
//End



function ShowWaitingPopup(obj) {
            $find("Accordion_Overlay").ShowPopUp();
}
function HideWaitingPopup(obj) {
            $find("Accordion_Overlay").HidePopUp();
}
function showPopup(obj) {
    var target;
    try {
    target = obj.get_updateTarget();
    }
    catch (E) {
    target = obj;
    }
    var elementId = target.id;
    var elementBounds = Sys.UI.DomElement.getBounds(target);
    var popupDiv = document.createElement('div');
    $(popupDiv).addClass('Popup').width($("#" + elementId).width()).height($("#" + elementId).height()).appendTo(document.body).attr('id', elementId + '_Waitingpopup').css('opacity', .8);
    Sys.UI.DomElement.setLocation(popupDiv, elementBounds.x, elementBounds.y);
}

function hidePopup(obj) {
    //$.fn.sfEvalScripts(obj.get_data());
    var target;
    try {
        target = obj.get_updateTarget();
    }
    catch (E) {
        target = obj;
    }
    var elementId = target.id;
    $($get(elementId + '_Waitingpopup')).remove();
}
$(document).ready(function() {
    if (!$.browser.safari) {
        $("#sampleTab1 ul:first").append("<li id=\"copycode\" title=\"Copy code to clipboard.\"><a>Copy Code</a></li>");
        $("#copycode").click(function() {
            CopyCode($(this).parent().siblings(":visible").find("div>div")[0]);
        });
    }
});
function CopyCode(element) {
    var obj = element, txt, isie = document.all ? true : false;
    if (isie)
        txt = obj.innerText;
    else
        txt = obj.textContent;
    if (window.clipboardData)
        window.clipboardData.setData("Text", txt);
    else if (window.netscape) {
        try {
            netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect")
        }
        catch (e) {
            alert("Access Denied! Please set signed.applets.codebase_principal_support to true at page about:config!")
        }
        var clip = Components.classes["@mozilla.org/widget/clipboard;1"].createInstance(Components.interfaces.nsIClipboard);
        if (!clip)
            return;
        var trans = Components.classes["@mozilla.org/widget/transferable;1"].createInstance(Components.interfaces.nsITransferable);
        if (!trans)
            return;
        trans.addDataFlavor("text/unicode");
        var str = {}, len = {}, str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString), copytext = txt;
        str.data = copytext;
        trans.setTransferData("text/unicode", str, copytext.length * 2);
        var clipid = Components.interfaces.nsIClipboard;
        if (!clip)
            return false;
        clip.setData(trans, null, clipid.kGlobalClipboard);
    };
}     