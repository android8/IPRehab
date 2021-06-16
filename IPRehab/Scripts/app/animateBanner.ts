
/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
  pageLoad();
});

function pageLoad(): void {
  /*hide header and footer after 2 seconds*/
  let topScrollY: string;
  const vw = Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0);
  console.log('view port width', vw);
  const vh = Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0)
  console.log('view port height', vw);

  function getMediaWidth() {
    let x = window.matchMedia("(max-width:576px)");
    if (x.matches) {
      console.log('screen is <= 576 pixels wide');
      return "-30px";
    }

    x = window.matchMedia("(max-width:768px)")
    if (x.matches) {
      console.log('screen is <= 768 pixels wide');
      return "-65px";
    }
    else {
      console.log('screen is >= 768 pixels wide');
      return "-75px";
    }
  }

  topScrollY = getMediaWidth();
  console.log('documentReady topScrollY', topScrollY);
  $('.hoverHeader').css({ 'top': topScrollY, 'z-index': '100', 'transition-duration': '2s' });
  $('.hoverFooter').css({ 'bottom': '-15px', 'z-index': '100', 'transition-duration': '2s' });

  $(window).resize(function () {
    topScrollY = getMediaWidth();
    console.log('mouseOut topScrollY', topScrollY);
    $('.hoverHeader').css({ 'top': topScrollY, 'z-index': '100', 'transition-duration': '1s' });
    $('.hoverFooter').css({ 'bottom': '-15px', 'z-index': '100', 'transition-duration': '1s' });
  });

  $(".hoverHeader, .hoverFooter").hover(
    function () {
      $('.hoverHeader').css({ 'top': '0px', 'z-index': '100', 'transition-duration': '1s' });
      $('.hoverFooter').css({ 'bottom': '0px', 'z-index': '100', 'transition-duration': '1s' });
    },
    function () {
      topScrollY = getMediaWidth();
      console.log('mouseOut topScrollY', topScrollY);
      $('.hoverHeader').css({ 'top': topScrollY, 'z-index': '100', 'transition-duration': '1s' });
      $('.hoverFooter').css({ 'bottom': '-15px', 'z-index': '100', 'transition-duration': '1s' });
    }
  );
}