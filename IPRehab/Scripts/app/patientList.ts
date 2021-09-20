/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />

import { IUserAnswer, AjaxPostbackModel } from "../appModels/IUserAnswer";

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
  $('#searchBtn').on('click', function () {
    patientListController.search();
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
  function search() {
    /* get criteria from input */
    let searchCriteria: string = $('#searchCriteria').val().toString();
    let thisHref: string = '';

    /* create href conditionally on localhost or not */
    let host: string = location.host;
    if (host.indexOf('localhost') != -1) {
      thisHref = '/Patient/Index?searchCriteria=' + searchCriteria;
    }
    else {
      thisHref = '/IPRehabMetrics/Patient/Index?searchCriteria=' + searchCriteria;
    }

    $('#recordCount').text('');
    location.href = thisHref;
  }

  /****************************************************************************
   * public functions exposing getSearchCriteriaFromUrl() and search() to outside of the closure
  ***************************************************************************/
  return {
    'getSearchCriteriaFromUrl': getSearchCriteriaFromUrl,
    'search': search
  }
})();