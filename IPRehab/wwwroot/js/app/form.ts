/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../../node_modules/@types/jqueryui/index.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />

import { IUserAnswer, AjaxPostbackModel } from "../appModels/IUserAnswer";
import { MDCTextField } from "../../node_modules/@material/textfield/index";
import { isNumeric, post } from "jquery";
import { InteractionTrigger } from "../../node_modules/@material/chips/deprecated/trailingaction/constants";
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
      let episodeID: number = +$('#episodeID', theScope).val();
      if (stageName.toString().toLowerCase() == "new")
        episodeID = -1;

      formController.submitTheForm($('.persistable', theScope), stageName, patientID, patientName, episodeID, thisPostBtn);
    }
  });

  const stage: string = $('.pageTitle').text().replace(' ', '_');

  /* on load */
  formController.selfCareScore();

  /* on change */
  $('.persistable[id^=GG0130]:not([id*=Discharge])').each(function () {
    $(this).change(function () {
      formController.selfCareScore();
    });
  });

  /* on load */
  formController.mobilityScore();

  /* on change */
  $('[id^=GG0170]').each(function () {
    $(this).change(function () {
      formController.mobilityScore();
    })
  });
});

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

let formController = (function () {
  function isEmpty($this: any): boolean {
    if (typeof $this.val() !== 'undefined' && $this.val())
      return false;
    else
      return true;
  }

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
      let newClass: string = $this.prop('class') + ' createActionCmd' + currentIdx.toString();
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
  function submitTheForm(persistables: any, stageName: any, patientID: any, patientName: any, episodeID: number, thisPostBtn: any): void {
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

      //return false doesn't break the .map, but skips the current item and continues mapping the next persistable
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

      thisAnswer.EpisodeID = episodeID;

      //both of admission date and onset date are rendered with MaterialInputDate view template with the same class
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

    let dialogOptions: any = {
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
          //when post to MVC (not WebAPI) controller, the antiforerytoken must be named 'RequestVerificationToken' in the header
          'RequestVerificationToken': $('input[name="X-CSRF-TOKEN-IPREHAB"]').val().toString(),
          'Accept': 'application/json',
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

        if (error.statusText == "OK" || error.statusText == "Ok") {
          $('#dialog')
            .text('Data is saved.')
            .dialog(dialogOptions)
        }
        else {
          $('#dialog')
            .text('Data is not saved. ' + error.responseText)
            .dialog(dialogOptions)
        }
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
          $('#dialog')
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

  /* private function */
  function selfCareScore(): void {
    let selfCareScore: number = 0;
    $('.persistable[id^=GG0130]:not([id*=Discharge])').each(function () {
      let $thisControl: any = $(this);
      let thisControlID: string = $thisControl.prop('id');
      let thisControlIntValue: number = parseInt($thisControl.prop('value'));
      let thisControlType: string = $thisControl.prop('type');

      let thisControlScore: number = 0;
      if (thisControlIntValue !== NaN && thisControlIntValue <=0) {
        updateScore($thisControl, 0);
      }
      else {
        switch (thisControlType) {
          case "select-one": {
            //true score is the selected option text
            let selectedOption: any = $('#' + thisControlID + ' option:selected').text();
            thisControlScore = parseInt(selectedOption);
            break;
          }
          case "checkbox":
          case "radio": {
            if ($thisControl.prop('checked')) {
              thisControlScore = 1;
            }
            break;
          }
          case "text": {
            let thisValue: string = $thisControl.val().toString();
            thisControlScore = parseInt(thisValue);
            if (thisControlScore > 0) {
              thisControlScore = 1;
            }
            break;
          }
        }

        if (thisControlScore <= 6) { //between 1 and 6
          updateScore($thisControl, thisControlScore);
          selfCareScore += thisControlScore;
        }
        else if (thisControlScore >= 7) { // greater than 7,9,10,88
          updateScore($thisControl, 1);
          selfCareScore++;
        }
        else {
          updateScore($thisControl, 0);
        }
      }
      $('#Self_Care_Aggregate_Score').text(selfCareScore);
    });
  }

  /* private function */
  function mobilityScore(): void {
    let mobilityScore: number = 0;

    /* handel all GG0170 except GG0170R and GG0170S */
    mobilityScore += Score_GG0170_Except_GG0170R_GG0170S();

    /* handle GG0170R and GG0170S together */
    mobilityScore += Score_GG0170R_GG0170S();

    $('#Mobility_Aggregate_Score').text(mobilityScore);
  }

  function updateScore(thisControl: any, newScore: number) {
    let theScoreEl: any;
    theScoreEl = $(thisControl.siblings('i.score'));

    if (newScore <= 0) {
      if (theScoreEl.length > 0) {
        theScoreEl.remove();
      }
    }
    else {
      if (theScoreEl.length == 0) {
        {
          thisControl.parent().closest('div').append("<i class='score'>score: " + newScore + "<i>");
        }
      }
      else {
        theScoreEl.text('score: ' + newScore);
      }
    }
  }

  function Score_GG0170_Except_GG0170R_GG0170S(): number {
    let mobilityScore: number = 0;
    /* select only GG0170 inputs including RR and SS but excluding R, S, and Discharge */
    $('.persistable[id^=GG0170]:not([id*=Discharge]):not([id*=GG0170Q]):not([id*=GG0170R]):not([id*=GG0170S])').each(function () {
      let thisControlScore: number = 0;
      let $thisControl = $(this);
      let thisControlID: string = $thisControl.prop('id');
      let thisControType: string = $thisControl.prop('type');
      switch (thisControType) {
        case "select-one": {
          //true score is the selected option text
          let selectedOption: any = $('#' + thisControlID + ' option:selected').text();
          thisControlScore = parseInt(selectedOption);
          break;
        }
        case "checkbox":
        case "radio": {
          if ($thisControl.prop('checked')) {
            /* always NaN because currently there is no numeric data to go by for checkbox and radio controls */
            let thisLabel = $thisControl.closest('label').text();
            thisControlScore = parseInt(thisLabel);
          }
          break;
        }
        case "text": {
          /* only if the input contains numeric string */
          let thisValue: string = $thisControl.val().toString();
          thisControlScore = parseInt(thisValue);
          break;
        }
      }

      const valueFactoringFields: string[] = ['A_', 'B_', 'C_', 'D_', 'E_', 'F_', 'G_', 'I_', 'J_', 'K_', 'L_', 'M_', 'N_', 'O_', 'P_'];

      for (var i = 0; i < valueFactoringFields.length; i++) {
        if (thisControlID.indexOf(valueFactoringFields[i]) != -1) {
          switch (true) {
            case thisControlScore >= 7:
              //7,9,10, or 88 add 1 point
              //one point score
              updateScore($thisControl, 1);
              mobilityScore++;
              break;
            case thisControlScore > 0 && thisControlScore <= 6:
              //btw 1 and 6 add value point 
              updateScore($thisControl, thisControlScore);
              mobilityScore += thisControlScore;
              break;
            default:
              updateScore($thisControl, 0);
              break;
          }

          //exit for() loop
          break;
        }
      }
    });

    return mobilityScore;
  }

  function Score_GG0170R_GG0170S(): number {
    let mobilityScore: number = 0;
    /* only one element in the following selector will be matched per stage form */
    const GG0170I_Admission: any = $('#GG0170I_Admission_Performance_0, #GG0170I_Interim_Performance_0, #GG0170I_Admission_Goal_0, #GG0170I_Follow_Up_Performance_0');
    let GG0170IAdmissionChoice: number = GG0170I_Admission_Choice(GG0170I_Admission);

    const GG0170I_Discharge: any = $('#GG0170I_Discharge_Goal_0, #GG0170I_Discharge_Performance_0');
    let GG0170IDischargeChoice: number = GG0170I_Discharge_Choice(GG0170I_Discharge);

    const GG0170R_Admission: any = $('#GG0170R_Admission_Performance_0, #GG0170R_Interim_Performance_0, #GG0170R_Admission_Goal_0, #GG0170R_Follow_Up_Performance_0');
    let GG0170RAdmissionScore: number = GG0170R_Admission_Score(GG0170R_Admission);

    const GG0170S_Admission: any = $('#GG0170S_Admission_Performance_0, #GG0170S_Interim_Performance_0, #GG0170S_Admission_Goal_0, #GG0170S_Follow_Up_Performance_0');
    let GG0170SAdmissionScore: number = GG0170S_Admission_Score(GG0170S_Admission);

    let multiplier: number = 1;
    if (GG0170IAdmissionChoice >= 7 && GG0170IDischargeChoice >= 7) {
      multiplier = 2;
    }

    if (GG0170RAdmissionScore > 0) {
      updateScore(GG0170R_Admission, GG0170RAdmissionScore * multiplier);
      mobilityScore += GG0170RAdmissionScore * multiplier;
    }
    else {
      updateScore(GG0170R_Admission, 0);
    }

    if (GG0170SAdmissionScore > 0) {
      updateScore(GG0170S_Admission, GG0170SAdmissionScore * multiplier);
      mobilityScore += GG0170SAdmissionScore * multiplier;
    }
    else {
      updateScore(GG0170S_Admission, 0);
    }

    return mobilityScore;
  }

  function GG0170I_Admission_Choice(GG0170I_Admission: any): number {
    let GG0170IAdmissionChoice: number = 0;

    /* there will only be one match from the selector per stage, so each() only loop once*/
    GG0170I_Admission.each(function () {
      let GG0170I_Admission_Control: any = $(this);
      let thisControlValueInt: number = parseInt(GG0170I_Admission_Control.prop('value'));

      if (thisControlValueInt > 0) {
        let GG0170I_Admission_ControlType: string = GG0170I_Admission_Control.prop('type');
        switch (GG0170I_Admission_ControlType) {
          case "select-one": {
            //true score is the selected option text
            let selectedOption: string = $('#' + GG0170I_Admission_Control.prop('id') + ' option:selected').text();
            let selectedOptionInt: number = parseInt(selectedOption);
            if (selectedOptionInt > 0) {
              GG0170IAdmissionChoice = selectedOptionInt;
            }
            break;
          }
          case "checkbox":
          case "radio": {
            if (GG0170I_Admission_Control.prop('checked')) {
              let thisLabel: string = GG0170I_Admission_Control.closest('label').text();
              let thisLableInt: number = parseInt(thisLabel);

              /* always NaN because currently there is no numeric data to go by for checkbox and radio controls */
              if (thisLableInt > 0) {
                GG0170IAdmissionChoice = thisLableInt;
              }
            }
            break;
          }
          case "text": {
            let thisInputValue: string = GG0170I_Admission_Control.val().toString();
            let thisInputValueInt: number = parseInt(thisInputValue);
            if (thisInputValueInt > 0) {
              GG0170IAdmissionChoice = thisInputValueInt;
            }
            break;
          }
        }
      }
    });
    return GG0170IAdmissionChoice;
  }

  function GG0170I_Discharge_Choice(GG0170I_Discharge: any): number {
    let GG0170IDischargeChoice: number = 0;

    /* there will only be one match from the selector per stage, so each() only loop once */
    GG0170I_Discharge.each(function () {
      let GG0170I_Discharge_Control: any = $(this);
      let thisControlValueInt: number = parseInt(GG0170I_Discharge_Control.prop('value'));
      if (thisControlValueInt > 0) {
        let GG0170I_Discharge_ControlType: string = GG0170I_Discharge_Control.prop('type');
        switch (GG0170I_Discharge_ControlType) {
          case "select-one": {
            //true score is the selected option text
            let selectedOption: any = $('#' + GG0170I_Discharge_Control.prop('id') + ' option:selected').text();
            let selectedOptionInt: number = parseInt(selectedOption);
            if (selectedOptionInt > 0) {
              GG0170IDischargeChoice = selectedOptionInt;
            }
            break;
          }
          case "checkbox":
          case "radio": {
            //true score is the checked label
            if (GG0170I_Discharge_Control.prop('checked')) {
              let thisLabel: string = GG0170I_Discharge_Control.closest('label').text();
              let thisLabelInt: number = parseInt(thisLabel);

              /* always NaN because currently there is no numeric data to go by for checkbox and radio controls */
              if (thisLabelInt > 0) {
                GG0170IDischargeChoice = thisLabelInt;
              }
            }
            break;
          }
          case "text": {
            //true score is the entered text
            let thisInputValue: string = GG0170I_Discharge_Control.val().toString();
            let thisInputValueInt: number = parseInt(thisInputValue);
            if (thisInputValueInt > 0) {
              GG0170IDischargeChoice = thisInputValueInt;
            }
            break;
          }
        }
      }
    });
    return GG0170IDischargeChoice;
  }

  function GG0170R_Admission_Score(GG0170R_Admission: any): number {
    let GG0170RAdmissionScore: number = 0;

    /* there will only be one match from the selector per stage, so each() only loop once */
    GG0170R_Admission.each(function () {
      let GG0170R_Admission_Control: any = $(this);
      let thisControlValueInt: number = parseInt(GG0170R_Admission_Control.prop('value'));
      if (thisControlValueInt > 0) {
        let GG0170R_Admission_ControlType: string = GG0170R_Admission_Control.prop('type');
        switch (GG0170R_Admission_ControlType) {
          case "select-one": {
            //true score is the selected option text
            let selectedOption: any = $('#' + GG0170R_Admission_Control.prop('id') + ' option:selected').text();
            let selectedOptionInt: number = parseInt(selectedOption);
            if (selectedOptionInt > 0) {
              GG0170RAdmissionScore = selectedOptionInt;
            }
            break;
          }
          case "checkbox":
          case "radio": {
            //true score is the checked label
            if (GG0170R_Admission_Control.prop('checked')) {
              let thisLabel = GG0170R_Admission_Control.closest('label').text();
              let thisLabelInt: number = parseInt(thisLabel);

              /* always NaN because currently there is no numeric data to go by for checkbox and radio controls */
              if (thisLabelInt > 0) {
                GG0170RAdmissionScore = 1;
              }
            }
            break;
          }
          case "text": {
            //true score is the entered text
            let thisInputValue: string = GG0170R_Admission_Control.val().toString();
            let thisInputValueInt: number = parseInt(thisInputValue);
            if (thisInputValueInt > 0) {
              GG0170RAdmissionScore = 1;
            }
            break;
          }
        }
      }
    });
    return GG0170RAdmissionScore;
  }

  function GG0170S_Admission_Score(GG0170S_Admission: any): number {
    let GG0170SAdmissionScore: number = 0;

    /* there will only be one match from the selector per stage, so each() only loop once */
    GG0170S_Admission.each(function () {
      let GG0170S_Admission_Control: any = $(this);
      let thisControlValueInt: number = parseInt(GG0170S_Admission_Control.prop('value'));
      if (thisControlValueInt > 0) {
        let GG0170S_Admission_ControlType: string = GG0170S_Admission_Control.prop('type');
        switch (GG0170S_Admission_ControlType) {
          case "select-one": {
            //true score is the selected option text
            let selectedOption: any = $('#' + GG0170S_Admission_Control.prop('id') + ' option:selected').text();
            let selectedOptionInt: number = parseInt(selectedOption);
            if (selectedOptionInt > 0) {
              GG0170SAdmissionScore = selectedOptionInt;
            }
            break;
          }
          case "checkbox":
          case "radio": {
            //true score is the checked label
            if (GG0170S_Admission_Control.prop('checked')) {
              let thisLabel: string = GG0170S_Admission_Control.closest('label').text();
              let thisLabelInt: number = parseInt(thisLabel);

              /* always NaN because currently there is no numeric data to go by for checkbox and radio controls */
              if (thisLabelInt > 0) {
                GG0170SAdmissionScore = 1;
              }
            }
            break;
          }
          case "text": {
            //true score is the entered text
            let thisInputValue: string = GG0170S_Admission_Control.val().toString();
            let thisInputValueInt: number = parseInt(thisInputValue);
            if (thisInputValueInt > 0) {
              GG0170SAdmissionScore = 1;
            }
            break;
          }
        }
      }
    });
    return GG0170SAdmissionScore;
  }

  /****************************************************************************
   * public functions exposing the private functions to outside of the closure
  ***************************************************************************/
  return {
    'isEmpty': isEmpty,
    'scrollToAnchor': scrollToAnchor,
    'setRehabBtns': setRehabBtns,
    'resetRehabBtns': resetRehabBtns,
    'breakLongSentence': breakLongSentence,
    'getTextPixels': getTextPixels,
    'submitTheForm': submitTheForm,
    'validate': validateForm,
    'selfCareScore': selfCareScore,
    'mobilityScore': mobilityScore
  }
})();