/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

interface JQuery {
  cookie: any;
}
$(function () {
  let btnConsent: any = $("#btnConcent");
  const cookieConsent: any = $('.cookieConsent');
  btnConsent.click(function () {
    //jquery way
    ($ as any).cookie({ 'AspNet.Consent': btnConsent.data("data-cookie-string") });

    cookieConsent.fadeOut("fast");
  });
});