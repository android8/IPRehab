//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

const sliderController = (function () {

    /* private function */
    function slide() {
        //not visible outside the closure
        function getMediaWidth() {
            let y: MediaQueryList = window.matchMedia("(max-width:576px)");
            let bannerCssHeight: string = $(".hoverHeader").css("height");
            let bannerHeight: number = parseInt(bannerCssHeight);

            if (!isNaN(bannerHeight)) {
                bannerHeight -= 5;
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
        /*hide header and footer after 2 seconds*/
        let bannerScrollOffY: string = '-' + getMediaWidth();
        let footerScrollOffY: string = '-' + $(".hoverFooter").css("height");
        const bannerScrollInY: string = '0px';
        const foterScrollInY: string = '0px';
        //const vw = Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0);
        //console.log('view port width', vw);
        //const vh = Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0);
        //console.log('view port height', vw);

        console.log('documentReady bannerScrollOffY', bannerScrollOffY);
        console.log('documentReady footerScrollOffY', footerScrollOffY);

        $('.hoverHeader').css({ 'top': bannerScrollOffY, 'z-index': '100', 'transition-duration': '2s' });
        $('.hoverFooter').css({ 'bottom': footerScrollOffY, 'z-index': '100', 'transition-duration': '2s' });

        $(window).on('resize', function () {
            bannerScrollOffY = '-' + getMediaWidth();
            footerScrollOffY = '-' + $(".hoverFooter").css("height");

            console.log('windows resize bannerScrollOffY', bannerScrollOffY);
            console.log('windows resize footerScrollOffY', footerScrollOffY);

            $('.hoverHeader').css({ 'top': bannerScrollOffY, 'z-index': '100', 'transition-duration': '1s' });
            $('.hoverFooter').css({ 'bottom': footerScrollOffY, 'z-index': '100', 'transition-duration': '1s' });
        });

        $(".hoverHeader, .hoverFooter, .pulldown")
            .on('mouseenter', function () {
                $('#logo').show();
                console.log('mouseenter bannerScrollInY', bannerScrollInY);
                console.log('mouseenter foterScrollInY', foterScrollInY);

                $('.hoverHeader').css({ 'top': bannerScrollInY, 'z-index': '100', 'transition-duration': '1s' });
                $('.hoverFooter').css({ 'bottom': foterScrollInY, 'z-index': '100', 'transition-duration': '1s' });
            })
            .on('mouseleave', function () {
                $('#logo').hide();
                bannerScrollOffY = '-' + getMediaWidth();
                footerScrollOffY = '-' + $(".hoverFooter").css("height");

                console.log('mouseleave bannerScrollOffY', bannerScrollOffY);
                console.log('mouseleave footerScrollOffY', footerScrollOffY);

                $('.hoverHeader').css({ 'top': bannerScrollOffY, 'z-index': '100', 'transition-duration': '1s' });
                $('.hoverFooter').css({ 'bottom': footerScrollOffY, 'z-index': '100', 'transition-duration': '1s' });
            });
    }

    /****************************************************************************
     * public function exposing slide() outside of the closure
    ****************************************************************************/
    return {
        'slide': slide
    }
})();

$(function () {
    //call closure
    sliderController.slide();
});

