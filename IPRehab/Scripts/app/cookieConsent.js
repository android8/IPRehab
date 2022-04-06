/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
$(function () {
    const btnConsent = $("button.acceptConsent");
    const consentContainer = $('div.cookieConsent');
    btnConsent.click(function () {
        //jquery way
        //($ as any).cookie({ 'AspNet.Consent': btnConsent.data("cookie-string") });
        document.cookie = btnConsent.data("cookie-string");
        consentContainer.fadeOut("fast");
    });
});
//# sourceMappingURL=cookieConsent.js.map