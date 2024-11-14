//https://www.typescriptlang.org/docs/handbook/asp-net-core.html
/****************************************************************************
 * javaScript closure
 ***************************************************************************/
const sliderController = (function () {
    const sFooterHeight = $(".footer").css("height");
    const sBannerHeight = $("banner").css("height");
    const iBannerHeight = parseInt(sBannerHeight);
    let bannerFooterVisible;
    //$('.banner').css({ 'top': bannerScrollOffY, 'z-index': '100', 'transition-duration': '2s' });
    //$('.footer').css({ 'bottom': footerScrollOffY, 'z-index': '100', 'transition-duration': '2s' });
    //const vw = Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0);
    //console.log('view port width', vw);
    //const vh = Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0);
    //console.log('view port height', vw);
    /* public function */
    function getMediaWidth() {
        let y;
        if (!isNaN(iBannerHeight)) {
            if (window.matchMedia("(max-width:576px)").matches) {
                console.log('screen is <= 576 pixels wide');
            }
            if (window.matchMedia("(max-width:768px)").matches) {
                console.log('screen is <= 768 pixels wide');
            }
            else {
                console.log('screen is >= 768 pixels wide');
            }
        }
    }
    /* public function */
    function repositionDOM() {
        if (bannerFooterVisible) {
            $('.banner').css({ 'top': '-' + sBannerHeight });
            $('.footer').css({ 'bottom': '-' + sFooterHeight });
            $('.pulldown').css({ 'top': '0px' });
            $('.search').css({ 'top': '0px' });
            $('.patients').css({ 'top': sBannerHeight, 'position': 'relative' });
            $('article').css({ 'top': '0px' });
        }
        else {
            $('.banner').css({ 'top': '0px', 'transition-duration': '1s' });
            $('.footer').css({ 'bottom': '0px', 'transition-duration': '1s' });
            $('.pulldown').css('top', sBannerHeight);
            $('.search').css('top', sBannerHeight);
            $('.patients').css({ 'top': sBannerHeight, 'position': 'relative' });
            $('article').css({ 'top': sBannerHeight });
        }
        bannerFooterVisible = !bannerFooterVisible;
    }
    /****************************************************************************
     * public function exposing slide() outside of the closure
    ****************************************************************************/
    return {
        'repositionDOM': repositionDOM,
        'getMediaWidth': getMediaWidth
    };
})();
$(function () {
    sliderController.getMediaWidth();
    sliderController.repositionDOM();
    //listeners
    $(window).on('resize', function () { sliderController.repositionDOM(); });
    $(".pulldown").on('click', function () { sliderController.repositionDOM(); });
});
//# sourceMappingURL=animateBanner.js.map