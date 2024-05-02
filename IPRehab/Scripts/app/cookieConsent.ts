interface JQuery {
  cookie: any;
}

$(function () {
  const btnConsent: any = $("button.acceptConsent");
  const consentContainer: any = $('div.cookieConsent');
  btnConsent.click(function () {

    //jquery way
    //($ as any).cookie({ 'AspNet.Consent': btnConsent.data("cookie-string") });

    document.cookie = btnConsent.data("cookie-string");
    consentContainer.fadeOut("fast");
  });
});