/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

//don't need use strict because the script is loaded as module which by default is executed in strict mode
//'use strict';

//import { Utility } from "./commonImport.js";

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

const commandBtnController = (function () {
  const dialogOptions: any = {
    resizable: true,
    //height: ($(window).height() - 200),
    //width: '90%',
    classes: { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' },
    modal: true,
    stack: true,
    sticky: true,
    position: { my: 'center', at: 'center', of: window },
    buttons: [{
      //    "Save": function () {
      //      //do something here
      //      let thisUrl: string = $('form').prop('action');
      //      let postBackModel: AjaxPostbackModel = new AjaxPostbackModel();
      //      postBackModel.NewAnswers = newAnswers;
      //      postBackModel.OldAnswers = oldAnswers;
      //      postBackModel.UpdatedAnswers = updatedAnswers;
      //      alert('ToDo: sending ajax postBackModel to ' + thisUrl);
      //    },
      text: "Close",
      //icon: "ui-icon-close",
      click: function () {
        $(this).dialog("close");
      }
    }]
  };

  /* private function */
  function addRipple(el) {
    /* addMaterial Design ripple effect to all .rehabAction buttons */
    //no need to retain a reference to the ripple instance, there's no need to assign it to a variable.
    //new MDCRipple(document.querySelector(el));
  }

  /* private function */
  function makeRequest($this: any) {
    //get formaction attribute which is created by Tag Helper

    let thisUrl: string = $this.prop('formAction');;

    /* data-attribute are all in lower case by covention */
    const controller: string = $this.data('controller');
    const action: string = $this.data('action');
    const stage: string = $this.data('stage');
    const patientID: string = $this.data('patientid');
    const patientName: string = $this.data('patientname');
    const episodeID: string = $this.data('episodeid');
    const searchCriteria: string = $this.data('searchcriteria');
    const orderBy: string = $this.data('orderby')
    const pageNumber: string = $this.data('pagenumber')
    let stageLowerCase: string = stage.toLowerCase()

    if (stageLowerCase.indexOf('patientlist') !== -1) {
      stageLowerCase = 'patient;'
    }

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

    const pageTitleLowerCase: string = $(".pageTitle").data('systitle').toLowerCase();
    if (stageLowerCase === pageTitleLowerCase)
    {
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

    const stage: string = $thisButton.data('stage');
    let stageLowerCase: string = stage.toLowerCase();
    if (stageLowerCase.indexOf('patientlist') !== -1) {
      stageLowerCase = 'patient;'
    }

    if (stageLowerCase.indexOf('followup') !== -1)
      stageLowerCase = 'follow up';

    if (stageLowerCase === '')
      stageLowerCase = 'full';

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

    const pageTitleLowerCase: string = $(".pageTitle").data('systitle').toLowerCase();
    if (stageLowerCase === pageTitleLowerCase) {
      $('.spinnerContainer').hide();
      $('#dialog')
        .text('You are already in it')
        .dialog(dialogOptions, {
          title: 'Warning'
        });
    }
    else {
      const submitButton: any = $('#ajaxPost');
      if (submitButton.length === 0) {
        //submit doesn't exist on this page
        //get formaction attribute for this button and navigate away
        const thisUrl: string = $thisButton.prop('formAction');
        location.href = thisUrl;
      }
      else {
        //submit exists on this page
        //if submit is not disabled then the form is dirty
        if (!submitButton.is(":disabled")) {
          $('.spinnerContainer').hide();
          $('#dialog')
            .text('Data is not saved. To save it, click Cancel to close this dialog window, then click the purple Save button on the upper left edge of the browser tab. To abandon the changes, click OK to continue going to the ' + stage + ' page')
            .dialog(dialogOptions, {
              title: 'Warning',
              buttons:
                [{
                  text: "Ok",
                  click: function () {
                    $(this).dialog("close");
                    $('.spinnerContainer').show();
                    const thisUrl = $thisButton.prop('formAction');
                    $('.spinnerContainer').show();
                    //navigate away
                    location.href = thisUrl;
                  }
                },
                {
                  text: "Cancel",
                  click: function () {
                    $('.spinnerContainer').hide();
                    $(this).dialog("close");
                  }
                }]
            })
        }
        else {
          const thisUrl: string = $thisButton.prop('formAction');
          location.href = thisUrl;
        }
      }
    }
  }

  function slideCommands(triggerContainer: any, hidden: boolean) {
    const siblingContainers: any[] = [];
    triggerContainer.siblings().each(function () {
      const siblingContainer = $(this); //should be <div class="mdc-touch-target-wrapper">
      const el: any = {};
      el.h = siblingContainer;
      el.width = el.h.children().eq(0).width(); //should be <button>
      el.h.width(0);
      siblingContainers.push(el);
    });

    for (let i = 0; i < siblingContainers.length; i++) {
      const thisCommandbtn: any = siblingContainers[i];
      const target: string = hidden ? thisCommandbtn.width + "px" : "0px";
      thisCommandbtn.h.animate({ width: target });
    }
  }
  /****************************************************************************
   * public functions exposing addRipple() and makeRequest() to outside of the closure
  ****************************************************************************/
  return {
    'addRipple': addRipple,
    'makeRequest': makeRequest,
    'makeRequestUsingFormAction': makeRequestUsingFormAction,
    'slideCommands': slideCommands
  }
})();
/****************************** end of closure *******************************/

$(function () {
  $('.rehabAction').each(function () {
    const $thisButton = $(this);
    //call closure
    //commandBtnController.addRipple($this);

    $thisButton.click(function () {
      //call closure
      //commandBtnController.makeRequest($this);
      commandBtnController.makeRequestUsingFormAction($thisButton);
    })
  });

  $('.commandTrigger').click(function () {
    const $this: any = $(this);
    const thisContainer: any = $this.parent(); //should be <div class="mdc-touch-target-wrapper">

    const hidden: boolean = $this.data('hidden');
    commandBtnController.slideCommands(thisContainer, hidden);
    $this.data('hidden', !hidden);
    if (hidden)
      $this.prop('title', 'Show Commands');
    else
      $this.prop('title', 'Hide Commands');
  })
});

