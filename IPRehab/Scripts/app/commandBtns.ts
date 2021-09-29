/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

//don't need use strict because the script is loaded as module which by default is executed in strict mode
//'use strict';

import { MDCRipple } from "../../node_modules/@material/ripple/component";


//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
  $('.rehabAction').each(function () {
    let $thisButton = $(this);
    //call closure
    //commandBtnController.addRipple($this);

    $thisButton.click(function () {
      //call closure
      //commandBtnController.makeRequest($this);
      commandBtnController.makeRequestUsingFormAction($thisButton);
    })
  });
});

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

let commandBtnController = (function () {
  /* private function */
  function addRipple(el) {
    /* addMaterial Design ripple effect to all .rehabAction buttons */
    //no need to retain a reference to the ripple instance, there's no need to assign it to a variable.
    //new MDCRipple(document.querySelector(el));
  }

  /* private function */
  function makeRequest($this) {
    //get formaction attribute which is created by Tag Helper

    let thisUrl: string = $this.prop('formAction');;

    /* data-attribute are all in lower case by covention */
    let controller: string = $this.data('controller');
    let action: string = $this.data('action');
    let stage: string = $this.data('stage');
    let stageLowerCase: string = stage.toLowerCase()
    let patientID: string = $this.data('patientid');
    let patientName: string = $this.data('patientname');
    let episodeID: string = $this.data('episodeid');
    let searchCriteria: string = $this.data('searchcriteria');
    let orderBy: string = $this.data('orderby')
    let pageNumber: string = $this.data('pagenumber')

    if (stageLowerCase.indexOf('patientlist') != -1) {
      stageLowerCase = 'patient;'
    }

    console.log('stageLowerCase', stageLowerCase);
    //get queryparameter. this is not suitable if the querystring is encrypted
    //const urlParams = new URLSearchParams(window.location.search);
    //console.log('urlParams', urlParams);
    //const param_x = urlParams.get('stage');
    //console.log('param_x', param_x);

    //if (param_x == stageLowerCase || (stageLowerCase == 'Full' && param_x == '')) {
    //  alert('You are already in it');
    //  $('.spinnerContainer').hide();
    //}
    //else {
    //  location.href = thisUrl;
    //}

    const pageTitleLowerCase = $(document).prop('title').toLowerCase();
    console.log('pageTitleLowerCase', pageTitleLowerCase);

    if (pageTitleLowerCase.indexOf(stageLowerCase) != -1) {
      alert('You are already in it');
      $('.spinnerContainer').hide();
    }
    else {
      thisUrl = location.protocol + '//' + location.host + '/' + controller + '/' + action + '?stage=' + stage + '&patientID=' + patientID + '&patientName=' + patientName + '&episodeID=' + episodeID + '&searchCriteria=' + searchCriteria + '&orderBy=' + orderBy + '&pageNumber=' + pageNumber;
      console.log('thisUrl', thisUrl);
      location.href = thisUrl;
    }
  }

  function makeRequestUsingFormAction($thisButton) {

    let stage: string = $thisButton.data('stage');
    let stageLowerCase: string = stage.toLowerCase();
    if (stageLowerCase.indexOf('patientlist') != -1) {
      stageLowerCase = 'patient;'
    }

    console.log('stageLowerCase', stageLowerCase);
    //get queryparameter. this is not suitable if the querystring is encrypted
    //const urlParams = new URLSearchParams(window.location.search);
    //console.log('urlParams', urlParams);
    //const param_x = urlParams.get('stage');
    //console.log('param_x', param_x);

    //if (param_x == stageLowerCase || (stageLowerCase == 'Full' && param_x == '')) {
    //  alert('You are already in it');
    //  $('.spinnerContainer').hide();
    //}
    //else {
    //  location.href = thisUrl;
    //}

    const pageTitleLowerCase = $(document).prop('title').toLowerCase();
    console.log('pageTitleLowerCase', pageTitleLowerCase);

    if (stageLowerCase.indexOf('followup') != -1)
      stageLowerCase = 'follow up';

    if (stageLowerCase == '')
      stageLowerCase = 'full';


    if (pageTitleLowerCase.indexOf(stageLowerCase) != -1) {
      $('#dialog')
        .text('You are already in it')
        .dialog();
      $('.spinnerContainer').hide();
    }
    else {
      let submitButton: any = $('#ajaxPost');
      if (submitButton.length == 0) {
        //submit doesn't exist on this page
        //get formaction attribute for this button and navigate away
        let thisUrl: string = $thisButton.prop('formAction');
        location.href = thisUrl;
      }
      else {
        //submit exists on this page
        //if submit is not disabled then the form is dirty
        if (!submitButton.is(":disabled")) {

          $('#dialog')
            .text('Data is not saved. Click Cancel then click the Save button to save the data. Click OK to not save the data and go to the ' + stage + ' page')
            .dialog({
              resizable: true,
              height: "auto",
              width: 400,
              modal: true,
              stack: true,
              //sticky: true,
              buttons: {
                Ok: function () {
                  var thisUrl = $thisButton.prop('formAction');
                  //navigate away
                  location.href = thisUrl;
                },
                Cancel: function () {
                  $(this).dialog("close");
                }
              }
            });
          $('.spinnerContainer').hide();
        }
        else {
          let thisUrl: string = $thisButton.prop('formAction');
          location.href = thisUrl;
        }
      }
    }
  }

  /****************************************************************************
   * public functions exposing addRipple() and makeRequest() to outside of the closure
  ****************************************************************************/
  return {
    'addRipple': addRipple,
    'makeRequest': makeRequest,
    'makeRequestUsingFormAction': makeRequestUsingFormAction
  }
})();


