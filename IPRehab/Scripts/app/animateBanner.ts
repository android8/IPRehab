
/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
  pageLoad();
});

function pageLoad(): void {
  /*hide header and footer after 2 seconds*/
  let topScrollY: string;

  function getMediaWidth() {
    let x: any = window.matchMedia("(max-width:768px)");
    if (x.matches) {
      return "-70px";
    }
    else {
      return "-60px";
    }
  }

  topScrollY = getMediaWidth();
  $('.hoverHeader').css({ 'top': topScrollY, 'z-index': '100', 'transition-duration': '2s' });
  $('.hoverFooter').css({ 'bottom': '-15px', 'z-index': '100', 'transition-duration': '2s' });

  $(".hoverHeader, .hoverFooter").hover(
    function () {
      $('.hoverHeader').css({ 'top': '0px', 'z-index': '100', 'transition-duration': '1s' });
      $('.hoverFooter').css({ 'bottom': '0px', 'z-index': '100', 'transition-duration': '1s' });
    },
    function () {
      topScrollY = getMediaWidth();
      $('.hoverHeader').css({ 'top': topScrollY, 'z-index': '100', 'transition-duration': '1s' });
      $('.hoverFooter').css({ 'bottom': '-15px', 'z-index': '100', 'transition-duration': '1s' });
    }
  );
}