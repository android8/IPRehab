/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />

//don't need use strict because the script is loaded as module which by default is executed in strict mode
//'use strict';

import { MDCRipple } from "../../node_modules/@material/ripple/component";
import { IUserAnswer, AjaxPostbackModel } from "../appModels/IUserAnswer";

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
  $('.rehabAction').each(function () {
    let $this = $(this);
    //call closure
    //commandBtnController.addRipple($this);

   $this.click(function () {
      //call closure
      commandBtnController.makeRequest($this);
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
    let thisUrl = $this.attr('formaction');
    console.log('thisUrl', thisUrl);

    //get data-stage attribute
    let buttonStage = $this.data('stage');
    if (buttonStage.indexOf('patientList') != -1) {
      buttonStage = 'patient';
    }
    console.log('buttonStage', buttonStage);

    //get queryparameter. this is not suitable if the querystring is encrypted
    //const urlParams = new URLSearchParams(window.location.search);
    //console.log('urlParams', urlParams);
    //const param_x = urlParams.get('stage');
    //console.log('param_x', param_x);

    //if (param_x == buttonStage || (buttonStage == 'Full' && param_x == '')) {
    //  alert('You are already in it');
    //  $('.spinnerContainer').hide();
    //}
    //else {
    //  location.href = thisUrl;
    //}

    const pageTitle = $(document).attr('title').toLowerCase();
    console.log('pageTitle', pageTitle);

    if (pageTitle.indexOf(buttonStage) != -1) {
      alert('You are already in it');
      $('.spinnerContainer').hide();
    }
    else {
      location.href = thisUrl;
    }
  }

  /****************************************************************************
   * public functions exposing addRipple() and makeRequest() to outside of the closure
  ****************************************************************************/
  return {
    'addRipple': addRipple,
    'makeRequest': makeRequest
  }
})();


