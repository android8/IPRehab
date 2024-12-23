﻿//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
    $('#searchBtn').on('click', function () {
        const searchButton = $(this)
        $('.spinnerContainer').show();
        patientListController.search(searchButton);
    });
});

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

let patientListController = (function () {
    /* private function */
    function getSearchCriteriaFromUrl() {
        const urlParams: any = new URLSearchParams(window.location.search.substring(1));

        //get queryparameter. this is not suitable if the querystring is encrypted
        const previousCriteria: string = urlParams.get('criteria');

        //get criteria from the input ellement
        const previousSearchInputValue: string = $("#searchCriteria").val().toString();
        //$('#previousCriteria').text(previousCriteria);

        //only set the val of the input when the input has no value set by the server
        if (previousSearchInputValue == '')
            $('#searchCriteria').val(previousCriteria);
    }

    /* private function */
    function search(searchButton) {
        /* get criteria from input */
        let searchCriteria: string = $('#searchCriteria').val().toString();
        let formAction: string = searchButton.attr("formaction");
        if (formAction.indexOf('&searchcriteria') === -1) {
            formAction += '&searchcriteria=' + searchCriteria;
        }
        else {
            formAction.replace('&searchcriteria=', '&searchcriteria=' + searchCriteria);
        }
        let thisHref: string = formAction;

        /* create href conditionally on localhost or not */
        //let host: string = location.host;
        //if (host.indexOf('localhost') !== -1) {
        //  thisHref = '/Patient/Index?searchCriteria=' + searchCriteria;
        //}
        //else {
        //  thisHref = '/IPRehabMetrics/Patient/Index?searchCriteria=' + searchCriteria;
        //}

        $('#recordCount').text('');
        location.href = thisHref;
    }

    /* private function */
    function getFullSSN($this) {
        alert($this);
        console.log($this);
    }

    /****************************************************************************
     * public functions exposing getSearchCriteriaFromUrl() and search() to outside of the closure
    ***************************************************************************/
    return {
        'getSearchCriteriaFromUrl': getSearchCriteriaFromUrl,
        'search': search,
        'getFullSSN': getFullSSN
    }
})();

$('.blurry').on('click', function () { patientListController.getFullSSN(this) });