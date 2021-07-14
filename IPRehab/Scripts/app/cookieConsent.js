/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
$(function () {
    let btnConsent = $("#btnConcent");
    const cookieConsent = $('.cookieConsent');
    btnConsent.click(function () {
        //jquery way
        $.cookie({ 'AspNet.Consent': btnConsent.data("data-cookie-string") });
        cookieConsent.fadeOut("fast");
    });
});
//# sourceMappingURL=cookieConsent.js.map