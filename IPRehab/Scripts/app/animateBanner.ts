//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

const sliderController = (function () {
    const bannerHeight: string = getMediaWidth();
    const bannerScrollOffY: string = '-' + bannerHeight;
    const footerHeight: string = $(".hoverFooter").css("height")
    const footerScrollOffY: string = '-' + footerHeight;
    let bannerFooterVisible: boolean = false;

    //$('.hoverHeader').css({ 'top': bannerScrollOffY, 'z-index': '100', 'transition-duration': '2s' });
    //$('.hoverFooter').css({ 'bottom': footerScrollOffY, 'z-index': '100', 'transition-duration': '2s' });
    //const vw = Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0);
    //console.log('view port width', vw);
    //const vh = Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0);
    //console.log('view port height', vw);

    function getMediaWidth() {
        let y: MediaQueryList = window.matchMedia("(max-width:576px)");
        let bannerCssHeight: string = $(".hoverHeader").css("height");
        let bannerHeight: number = parseInt(bannerCssHeight);

        if (!isNaN(bannerHeight)) {
            //bannerHeight -= 5;
            bannerCssHeight = bannerHeight + 'px';
        }

        if (y.matches) {
            console.log('screen is <= 576 pixels wide');
            return bannerCssHeight;
        }

        y = window.matchMedia("(max-width:768px)");
        if (y.matches) {
            console.log('screen is <= 768 pixels wide');
            return bannerCssHeight;
        }
        else {
            console.log('screen is >= 768 pixels wide');
            return bannerCssHeight;
        }
    }

    /* private function */
    function widnowResize($this) {
        //bannerScrollOffY = '-' + getMediaWidth();
        //footerScrollOffY = '-' + $(".hoverFooter").css("height");

        //$('.hoverHeader').css({ 'top': bannerScrollOffY, 'z-index': '100'/*, 'transition-duration': '0s'*/ });
        //$('.hoverFooter').css({ 'bottom': footerScrollOffY, 'z-index': '100'/*, 'transition-duration': '0s'*/ });

        console.log('windows resized, hide banner and footer');
        $('.hoverHeader, .hoverFooter').hide();
        bannerFooterVisible = false;
    }

    /* private function */
    function clickPulldown($this) {
        //let visible: boolean = $('.hoverHeader').is(":visible");
        //const bannerPosition = $('.hoverHeader').position();

        if (bannerFooterVisible) {
            //alert('hide banner');
            $('#logo').hide();
            $(".pulldown").css({ 'top': '0px' });
            $('.hoverHeader').css({ 'top': bannerScrollOffY, 'z-index': '100' /*, 'transition-duration': '1s' */ }).hide();
            $('.hoverFooter').css({ 'bottom': footerScrollOffY, 'z-index': '100' /*, 'transition-duration': '1s' */}).hide();
        }
        else {
            //alert('show banner');
            $('#logo').show();
            $(".pulldown").css({ 'top': bannerHeight });
            $('.hoverHeader').css({ 'top': '0px', 'z-index': '100', 'transition-duration': '1s' }).show();
            $('.hoverFooter').css({ 'bottom': '0px', 'z-index': '100', 'transition-duration': '1s' }).show();
        }
        bannerFooterVisible = !bannerFooterVisible;
    }

    /* private function */
    function slideOnLeaveElement($this) {
        $('#logo').hide();

        //console.log('mouseleave bannerScrollOffY', bannerScrollOffY);
        //console.log('mouseleave footerScrollOffY', footerScrollOffY);

        $('.hoverHeader').css({ 'top': bannerScrollOffY, 'z-index': '100', 'transition-duration': '1s' });
        $('.hoverFooter').css({ 'bottom': footerScrollOffY, 'z-index': '100', 'transition-duration': '1s' });
    }
    /****************************************************************************
     * public function exposing slide() outside of the closure
    ****************************************************************************/
    return {
        'widnowResize': widnowResize,
        'clickPulldown': clickPulldown
    }
})();

$(function () {
    $('.hoverHeader, .hoverFooter').hide();

    //listeners
    $(window).on('resize', function () { sliderController.widnowResize(this) });
    $(".pulldown").on('click', function () { sliderController.clickPulldown(this) });
});

