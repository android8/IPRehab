/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../../node_modules/@types/jqueryui/index.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />

import { IUserAnswer, AjaxPostbackModel } from "../appModels/IUserAnswer";
import { MDCTextField } from "../../node_modules/@material/textfield/index";
import { post } from "jquery";
//import { MDCRipple } from "../../node_modules/@material/ripple/index";

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
  $('.persistable').change(function () {
    $('#ajaxPost').removeAttr('disabled');
    $('#mvcPost').removeAttr('disabled');
  });

  $('select').each(function () {
    let $this = $(this);
    $this.change(function () {
      formController.breakLongSentence($this);
    });
  });

  /* section nav */
  $('#questionTab').hover(
    function () { /* slide into the viewing area*/
      $('#questionTab').css({ 'left': '0px', 'transition-duration': '1s' });
    },
    function () { /* slide out of the viewing area */
      $('#questionTab').css({ 'left': '-230px', 'transition-duration': '1s' });
    }
  );

  /* jump to section anchor */
  $('.gotoSection').each(function () {
    let $this = $(this);
    $this.click(function () {
      let anchorId = $this.data("anchorid");
      formController.scrollToAnchor(anchorId);
    });
  });

  /* traditional form post */
  //$('#mvcPost').click(function () {
  //  $('form').submit();
  //});

  /* ajax post form */
  $('#ajaxPost').click(function () {
    if (formController.validate) {
      const thisPostBtn: any = $(this);
      $('.spinnerContainer').show();
      let theScope: any = $('#userAnswerForm');
      let stageName: any = $('#stage', theScope).val();
      const patientID: any = $('#patientID', theScope).val();
      const patientName: any = $('#patientName', theScope).val();
      const episodeID: any = $('#episodeID', theScope).val();
      formController.searlizeTheForm($('.persistable', theScope), stageName, patientID, patientName, episodeID, thisPostBtn);
    }
  });

  formController.checkRules();
});

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

let formController = (function () {
  /* private function */
  function scrollToAnchor(anchorId: string) {
    let aTag: any = $('a[name="' + anchorId + '"]');
    $('html,body').animate({ scrollTop: aTag.offset().top - 15 }, 'fast');
  }

  /* private function */
  function setRehabBtns(targetScope: any) {
    let currentIdx: number = 0;
    $.each($('.rehabAction', targetScope), function () {
      let $this = $(this);
      let newTitle: string = $this.prop('title').replace(/Edit/g, 'Create');
      let newHref: string = $this.prop('href').replace(/Edit/g, 'Create');
      $this.prop('title', newTitle);
      $this.prop('href', newHref);
      currentIdx++;
      let newClass: string = $this.prop('class') + ' createActionCmd' + currentIdx.toString();;
      $this.prop('class', newClass);
    });
  }

  /* private function */
  function resetRehabBtns(targetScope: any) {
    let cmdBtns: string[] = ['primary', 'info', 'secondary', 'success', 'warning'];
    let currentIdx: number = 0;
    $.each($('.rehabAction', targetScope), function () {
      let $this = $(this);
      let newTitle: string = $this.prop('title').replace(/Create/g, 'Edit');
      let newHref: string = $this.prop('href').replace(/Create/g, 'Edit');
      $this.prop('title', newTitle);
      $this.prop('href', newHref);
      let resetClass: string = '';
      resetClass = 'badge badge-' + cmdBtns[currentIdx] + ' rehabAction';
      currentIdx++;
      $this.prop('class', resetClass);
    });
  }

  /* private function */
  function checkRules() {
    let q44c_is_1: boolean = $('#Q44C_86').prop("checked");
    let q44c_is_0: boolean = $('#Q44C_87').prop("checked");
    let q44d_is_1: boolean = $('#Q44D_').val() == '1';
    let q46: any = $('#Q46_').val();

    if (!q44c_is_1 && !q44c_is_0) {
      /* Q44c is not answered */
      $('#Q44D_').prop('disabled', 'true');
      $('#Q45_').prop('disabled', 'true');
    }
    if (q44c_is_1 && q44d_is_1) {
      /*Q44C = 1 and Q44D = 1*/
      $('#Q45_').prop('disabled', 'false');
    }
    else {
      if (q44c_is_0) {
        $('#Q44D_').prop('disabled', 'false');
        $('#Q46_').focus();
      }
    }

    /* interrupted */
    let q42_is_interrupted: boolean = $('#Q42-INTRRUPT_86').prop('checked');

    if (q42_is_interrupted) {
      $('#Q43_').prop('disabled', 'false');
      $('#Q43_').focus();
    }
  }

  /* private function */
  function validate() {
    $('form#userAnswerForm').validate({
    //  rules: {

    //  }
    })
  }

  /* private function */
  function breakLongSentence(thisSelectElement) {
    console.log('thisSelectElement', thisSelectElement);
    let maxLength: number = 50;
    let longTextOptionDIV = thisSelectElement.next('div.longTextOption');
    console.log('longTextOptionDIV', longTextOptionDIV);
    let thisSelectWidth = thisSelectElement[0].clientWidth;
    let thisScope: any = thisSelectElement;
    $.each($('option:selected', thisScope), function () {
      let $thisOption = $(this);

      let regX = new RegExp("([\\w\\s]{" + (maxLength - 2) + ",}?\\w)\\s?\\b", "g")
      let oldText: string = $thisOption.text();
      let font = $thisOption.css('font');
      let oldTextInPixel = getTextPixels(oldText, font);

      console.log('oldTextInPixel', oldTextInPixel);
      console.log('thisSelectWidth', thisSelectWidth);
      longTextOptionDIV.text('');
      if (oldTextInPixel > thisSelectWidth) {
        let newStr = oldText.replace(regX, "$1\n");
        newStr = newStr.trim();
        let startWithNumber = $.isNumeric(newStr.substring(0, 1));
        if (startWithNumber) {
          newStr = newStr.substring(newStr.indexOf(" ") + 1);
        }
        console.log('old ->', oldText);
        console.log('new ->', newStr);
        longTextOptionDIV.text(newStr);
        longTextOptionDIV.removeClass("invisible");
      }
    });
  }

  /* private function */
  function getTextPixels(someText: string, font: any) {
    let canvas = document.createElement('canvas');
    let context = canvas.getContext("2d");
    context.font = font;
    let width = context.measureText(someText).width;
    return Math.ceil(width);
  }

  /* private function */
  function searlizeTheForm(persistables: any, stageName: any, patientID: any, patientName: any, episodeID: any, thisPostBtn: any): void {
    //const inputAnswerArray: any[] = $('input[value!=""].persistable', theForm).serializeArray();
    //console.log("serialized input", inputAnswerArray);

    //const selectAnswerArray: any[] = [];
    //$.map($('select', theForm), function () {
    //  let $thisDropdown = $(this);
    //  $('option', $thisDropdown).each(function () {
    //    let $thisOption = $(this);
    //    if ($thisOption.is(':selected')) {

    //    }
    //  })
    //});

    const oldAnswers: Array<IUserAnswer> = new Array<IUserAnswer>();
    const newAnswers: Array<IUserAnswer> = new Array<IUserAnswer>();
    const updatedAnswers: Array<IUserAnswer> = new Array<IUserAnswer>();

    //get the key answers. these must be done outside of the .map() 
    //because each answer in .map() will use the same episode onset date and admission date
    let onsetDate: any = $(".persistable[data-questionkey^='Q23']").val();
    let admissionDate: any = $(".persistable[data-questionkey^='Q12']").val();

    //ToDo: make this closure available to other modules to avoid code duplication in commandBtns.ts
    persistables.map(function () {
      let $thisPersistable: any = $(this);
      let currentValue: string = $thisPersistable.val();
      let oldValue: string = $thisPersistable.data('oldvalue');
      let answerID: string = $thisPersistable.data('answerid');
      let CRUD: string;

      //return false doesn't break the .map, but skips the current item and continues mapping
      if ($thisPersistable.prop('type') == 'select-one' && (+currentValue == -1 && !oldValue)) {
        return false;
      }
      if (($thisPersistable.prop('type') == 'checkbox' || $thisPersistable.prop('type') == 'radio')
        && (!$thisPersistable.prop('checked') && !oldValue)) {
        return false;
      }
      if (currentValue == oldValue //both are not undefined and with the same empty or non-empty strings, ie ''=='', 'xyx'=='xyz'
        || (currentValue == '' && !oldValue) //currentValue is blank and oldValue is undefined
        || (!currentValue && !oldValue))  //both are undefineed
      {
        return false;
      }

      //determine CRUD operation
      if (currentValue != '' && (!answerID || +answerID == -1)) {
        CRUD = 'C';
      }
      else if (currentValue == '' && (answerID || +answerID != -1)) {
        CRUD = 'D';
      }
      else {
        CRUD = "U";
      }

      let thisAnswer: IUserAnswer = <IUserAnswer>{};
      thisAnswer.PatientName = patientName.toString();
      thisAnswer.PatientID = patientID.toString();

      if (answerID)
        thisAnswer.AnswerID = +answerID;

      thisAnswer.EpisodeID = episodeID.toString();

      //both of these answers are rendered with MaterialInputDate view template with the same class
      //so use id to determine to which the data-codesetdescription property belong
      thisAnswer.AdmissionDate = admissionDate;
      thisAnswer.OnsetDate = onsetDate;

      //+ in front of string convert it to number
      thisAnswer.QuestionID = +$thisPersistable.data('questionid');
      thisAnswer.QuestionKey = $thisPersistable.data('questionkey');

      //a question may show several input fields for multiple stages,
      //so we have to use the data-stagid at the field level, not the stage hidden input set at the form level
      thisAnswer.StageID = +$thisPersistable.data('stageid');
      thisAnswer.StageName = stageName.toString();

      let thisInputType: string = $thisPersistable.prop('type');
      switch (thisInputType) {
        case 'text':
        case 'date':
        case 'textarea':
        case 'number': /* only used for therapy minutes */
          /* save extra description */
          thisAnswer.AnswerCodeSetID = +$thisPersistable.data('codesetid');
          thisAnswer.Description = currentValue;
          break;
        default: /* dropdown and radio */
          thisAnswer.AnswerCodeSetID = +currentValue;
          break;
      }

      thisAnswer.AnswerCodeSetDescription = $thisPersistable.data('codesetdescription');

      if ($thisPersistable.prop('data-answersequencenumber'))
        thisAnswer.AnswerSequenceNumber = +$thisPersistable.data('answersequencenumber');

      thisAnswer.AnswerByUserID = $thisPersistable.data('userid');
      thisAnswer.LastUpdate = new Date();


      switch (CRUD) {
        case 'C':
          newAnswers.push(thisAnswer);
          break;
        case 'U':
          updatedAnswers.push(thisAnswer);
          break;
        case 'D':
          oldAnswers.push(thisAnswer);
          break;
      }
    });

    $('.spinnerContainer').hide();

    const antiForgeryToken: string = $('input[name="CSRF-TOKEN-IPREHAB"]').val().toString();

    let dialogOptions: any = {
      resizable: true,
      //height: ($(window).height() - 200),
      //width: '90%',
      classes: { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' },
      modal: true,
      stack: true,
      sticky: true,
      position: { my: 'center', at: 'center' , of: window },
      buttons: [{
        //    "Save": function () {
        //      //do something here
        //      let thisUrl: string = $('form').prop('action');
        //      let postBackModel: AjaxPostbackModel = <AjaxPostbackModel>{};
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

    let postBackModel: AjaxPostbackModel = <AjaxPostbackModel>{};
    postBackModel.EpisodeID = episodeID;
    postBackModel.NewAnswers = newAnswers;
    postBackModel.OldAnswers = oldAnswers;
    postBackModel.UpdatedAnswers = updatedAnswers;

    thisPostBtn.attr('disabled', 'false');
    let apiBaseUrl = thisPostBtn.data('apibaseurl');
    let apiController = thisPostBtn.data('controller');
    let thisUrl: string = apiBaseUrl + '/api/' + apiController;
    //thisUrl = $('form').prop('action');
    $('.spinnerContainer').show();

    jQueryAjax();
    //onPost();

    //use jquery ajax
    function jQueryAjax() {
      $.ajax({
        type: "POST",
        url: thisUrl,
        data: JSON.stringify(postBackModel),
        headers: {
        //  //when post to MVC (not WebAPI) controller, the antiforerytoken must be named 'RequestVerificationToken' in the header
        //  'RequestVerificationToken': antiForgeryToken,
        //  'Accept': 'application/json',
        },
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        crossDomain: true,
        //xhrFields: {
        //  withCredentials: true
        //}
      }).done(function (result) {
        thisPostBtn.attr('disabled', 'true');
        $('.spinnerContainer').hide();
        console.log('postback result', result.message);
        console.log('inserted entities', result.insetedEntities);
        dialogOptions.title = 'Success';
        $('#dialog')
          .text('Data is saved.')
          .dialog(dialogOptions);
        $('.rehabAction').removeAttr('disabled');
      }).fail(function (error) {
        thisPostBtn.attr('disabled', 'false');
        console.log('postback error', error);
        $('.spinnerContainer').hide();
        dialogOptions.title = error.statusText;
        dialogOptions.classes = { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' }
        $('#dialog')
          .text('Data is not saved. ' + error.responseText)
          .dialog(dialogOptions)
      });

    }

    //use fetch api
    function onPost() {
      const url = thisUrl;
      var headers = {}

      fetch(url, {
        method: "POST",
        mode: 'cors',
        headers: headers
      })
        .then((response) => {
          if (!response.ok) {
            throw new Error(response.text.toString())
          }
          return response.json();
        })
        .then(data => {
-          $('#dialog')
            .text('Data is saved.')
            .dialog(dialogOptions);
          $('.rehabAction').removeAttr('disabled');

        })
        .catch(function (error) {
          thisPostBtn.attr('disabled', 'false');
          console.log('postback error', error);
          $('.spinnerContainer').hide();
          dialogOptions.title = error.statusText;
          dialogOptions.classes = { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' }
          $('#dialog')
            .text('Data is not saved. ' + error)
            .dialog(dialogOptions)
        });
    }
  }

  /* private function */
  function validateForm(theForm: any): void {
    theForm.validate();
  }

  /****************************************************************************
   * public functions exposing the private functions to outside of the closure
  ***************************************************************************/
  return {
    'scrollToAnchor': scrollToAnchor,
    'setRehabBtns': setRehabBtns,
    'resetRehabBtns': resetRehabBtns,
    'checkRules': checkRules,
    'breakLongSentence': breakLongSentence,
    'getTextPixels': getTextPixels,
    'searlizeTheForm': searlizeTheForm,
    'validate': validateForm
  }
})();