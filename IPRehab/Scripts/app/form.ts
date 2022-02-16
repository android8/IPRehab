﻿/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
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
    let onsetDate: Date = new Date($(".persistable[data-questionkey^='Q23']").val().toString());
    let admissionDate: Date = new Date($(".persistable[data-questionkey^='Q12']").val().toString());
    if (formController.isDate(onsetDate) && formController.isDate(admissionDate)) {
      $('#ajaxPost').removeAttr('disabled');
      //$('#mvcPost').removeAttr('disabled');
    }
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
      let anchorId: string = $this.data("anchorid");
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
      let stageName: string = $('#stage', theScope).val().toString();
      const patientID: string = $('#patientID', theScope).val().toString();
      const patientName: string = $('#patientName', theScope).val().toString();
      let episodeID: number = +$('#episodeID', theScope).val();
      if (stageName.toLowerCase() == "new")
        episodeID = -1;

      formController.submitTheForm($('.persistable', theScope), stageName, patientID, patientName, episodeID, thisPostBtn);
    }
  });

  /* on load */
  formController.selfCareScore();

  /* on change */
  $('.persistable[id^=GG0130]:not([id*=Discharge_Goal])').each(function () {
    const $this: any = $(this);
    $this.change(function () {
      formController.selfCareScore();
    });
  });

  /* mobility score on load */
  formController.mobilityScore();

  /* mobility score on change */
  $('.persistable[id^=GG0170]:not([id*=Discharge_Goal])').each(function () {
    $(this).change(function () {
      formController.mobilityScore();
    })
  });
});

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

let formController = (function () {
  /* private function */
  function isEmpty($this: any): boolean {
    if (typeof $this.val() !== 'undefined' && $this.val())
      return false;
    else
      return true;
  }

  /* internal function */
  function isDate(aDate: Date): boolean {
    return aDate instanceof Date && !isNaN(aDate.valueOf());
  }

  /* private function */
  function scrollToAnchor(anchorId: string) {
    console.log('scroll to ' + anchorId);
    let anchor: any = $('#' + anchorId);
    $('html,body').animate({ scrollTop: anchor.offset().top }, 'fast');
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
  function breakLongSentence(thisSelectElement) {
    console.log('thisSelectElement', thisSelectElement);
    let maxLength: number = 50;
    let longTextOptionDIV = thisSelectElement.next('div.longTextOption');
    console.log('longTextOptionDIV', longTextOptionDIV);
    let thisSelectWidth = thisSelectElement[0].clientWidth;
    let thisScope: any = thisSelectElement;
    let selectedValue: number = parseInt(thisSelectElement.prop('value'));
    if (selectedValue <= 0) {
      longTextOptionDIV.text('');
    }
    else {
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
  function submitTheForm(persistables: any, stageName: string, patientID: string, patientName: string, episodeID: number, thisPostBtn: any): void {
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

    let thisAnswer: IUserAnswer = <IUserAnswer>{};
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
      // !oldValue yields true only when the value is undefined or NaN, then skip the current item and exit the map() 
      if ($thisPersistable.prop('type') == 'select-one' && (!(+currentValue) && !oldValue)) {
        return false;
      }

      // !oldValue yields true only when the value is not acceptible, then skip the current item and exit the map() 
      if (($thisPersistable.prop('type') == 'checkbox' || $thisPersistable.prop('type') == 'radio')
        && (!$thisPersistable.prop('checked') && !oldValue)) {
        return false;
      }

      if (currentValue === oldValue)
      {
        return false;
      }

      //determine CRUD operation
      switch (true) {
        case (+currentValue > 0 && +oldValue <= 0):
          console.log('Insert currentValue '+ (+currentValue).toString() + 'oldValue ' + (+oldValue).toString());
          CRUD = 'C';
          break;
        case (+currentValue <= 0 && +oldValue > 0):
          console.log('Delete oldValue ' + (+oldValue).toString());
          CRUD = 'D';
          thisAnswer.AnswerID = +answerID;
          break;
        default:
          console.log('Update oldValue ' + (+oldValue).toString() + ' whith currentValue ' + (+currentValue));
          CRUD = "U";
          thisAnswer.AnswerID = +answerID;
          break;
      }

      thisAnswer.PatientName = patientName.toString();
      thisAnswer.PatientID = patientID;
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
      thisAnswer.StageName = stageName;

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

      if ($thisPersistable.data('answersequencenumber'))
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

  /* internal function */
  function getControlValue($this: any): number {
    let thisControlType: string = $this.prop('type');
    let thisValue: number = 0;
    switch (thisControlType) {
      case "select-one": {
        //true score is the selected option text because it starts with 1 to 6, 7, 9, 10 and 88
        let selectedOption: string = $('#' + $this.prop('id') + ' option:selected').text();
        thisValue = parseInt(selectedOption);
        break;
      }

      case "checkbox":
      case "radio": {
        if ($this.prop('checked')) {
          thisValue = 1;
        }
        break;
      }

      case "text": {
        let thisInputValue: string = $this.val().toString();
        if (parseInt(thisInputValue) > 0) {
          thisValue = 1;
        }
        break;
      }
    }
    return thisValue;
  }

  /* private function */
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

  /* private function */
  function selfCareScore(): void {
    let GG0130_Admission_Performance: number = 0;
    let GG0130_Discharge_Performance: number = 0;
    $('.persistable[id^=GG0130]:not([id*=Discharge_Performance]):not([id*=Discharge_Goal])').each(function () {
      const $this = $(this);
      const $thisValue = getControlValue($this);
      switch (true) {
        case ($thisValue >= 7): // greater than 7,9,10,88
          {
            updateScore($this, 1);
            GG0130_Admission_Performance += 1;
            break;
          }
        case ($thisValue > 0 && $thisValue <= 6): // between 1 and 6
          {
            updateScore($this, $thisValue);
            GG0130_Admission_Performance += $thisValue;
            break;
          }
        default:
          {
            updateScore($this, 0);
            break;
          }
      }
    });

    $('.persistable[id^=GG0130]:not([id*=Admission_Performance]):not([id*=Discharge_Goal])').each(function () {
      const $this = $(this);
      const $thisValue = getControlValue($this);
      switch (true) {
        case ($thisValue >= 7): // greater than 7,9,10,88
          {
            updateScore($this, 1);
            GG0130_Discharge_Performance += 1;
            break;
          }
        case ($thisValue > 0 && $thisValue <= 6): // between 1 and 6
          {
            updateScore($this, $thisValue);
            GG0130_Discharge_Performance += $thisValue;
            break;
          }
        default:
          {
            updateScore($this, 0);
            break;
          }
      }
    });

    $('#Self_Care_Aggregate_Score_Admission_Performance').text(GG0130_Admission_Performance);
    $('#Self_Care_Aggregate_Score_Discharge_Performance').text(GG0130_Discharge_Performance);
    $('#Self_Care_Aggregate_Score').text(GG0130_Admission_Performance + GG0130_Discharge_Performance);
  }

  /* private function */
  function mobilityScore(): void {
    let mobilityScore_Admission_Performance: number = 0, mobilityScore_Discharge_Performance: number = 0;

    mobilityScore_Admission_Performance += Score_GG0170AtoP_Admission_Performance();
    mobilityScore_Admission_Performance += Score_GG0170RandS_Admission_Performance();

    mobilityScore_Discharge_Performance += Score_GG0170AtoP_Discharge_Performance();
    mobilityScore_Discharge_Performance += Score_GG0170RandS_Discharge_Performance();

    $('#Self_Care_Aggregate_Score_Admission_Performance').text(mobilityScore_Admission_Performance);
    $('#Self_Care_Aggregate_Score_Discharge_Performance').text(mobilityScore_Discharge_Performance);
    $('#Mobility_Aggregate_Score').text(mobilityScore_Admission_Performance + mobilityScore_Discharge_Performance);
  }

  /* internal function */
  function Score_GG0170AtoP_Admission_Performance(): number {
    let GG0170_AtoP_Admission_Performance: number = 0;
    /* select only GG0170 Admission Performance excluding Q, R and S */
    $('.persistable[id^=GG0170]:not([id*=Discharge_Performance]):not([id*=Discharge_Goal]):not([id*=GG0170Q]):not([id*=GG0170R]):not([id*=GG0170S])').each(function () {
      let $thisControl = $(this);
      let thisControlScore: number = getControlValue($thisControl);
      let thisControlID: string = $thisControl.prop('id');

      switch (true) {
        case (thisControlScore >= 7): {
          if (thisControlID.indexOf('GG0170I') >= 0) {
            //7,9,10, or 88 don't score per customer 12/8/2021
            updateScore($thisControl, 0);
          }
          else {
            updateScore($thisControl, 1);
            GG0170_AtoP_Admission_Performance += 1;
          }
          break;
        }
        case (thisControlScore > 0 && thisControlScore <= 6): {
          //btw 1 and 6 add value point 
          updateScore($thisControl, thisControlScore);
          GG0170_AtoP_Admission_Performance += thisControlScore;
          break;
        }
        default: {
          updateScore($thisControl, 0);
          break;
        }
      }
    });

    return GG0170_AtoP_Admission_Performance;
  }

  /* internal function */
  function Score_GG0170RandS_Admission_Performance(): number {
    let admissionMultiplier: number = 1;

    /* use GG0170I to determine the multipliers for GG0170R and GG0170S */
    let GG0170I_Admission_Performance: any = $('.persistable[id^=GG0170I_Admission_Performance]');
    let GG0170I_Admission_Performance_Value: number = getControlValue(GG0170I_Admission_Performance);

    if (GG0170I_Admission_Performance_Value >= 7)
      admissionMultiplier = 2;
    else
      admissionMultiplier = 0;

    let R_Admission_Performance: number = 0;
    let S_Admission_Performance: number = 0;

    let GG0170R_Admission_Performance: any = $('.persistable[id^=GG0170R_Admission_Performance]');
    let GG0170R_Admission_Performance_Value: number = getControlValue(GG0170R_Admission_Performance);
    if (GG0170R_Admission_Performance_Value > 0) {
      updateScore(GG0170R_Admission_Performance, GG0170R_Admission_Performance_Value * admissionMultiplier);
      R_Admission_Performance += GG0170R_Admission_Performance_Value * admissionMultiplier;
    }
    else
      updateScore(GG0170R_Admission_Performance, 0);

    let GG0170S_Admission_Performance: any = $('.persistable[id^=GG0170S_Admission_Performance]');
    let GG0170S_Admission_Performance_Value: number = getControlValue(GG0170S_Admission_Performance);
    if (GG0170S_Admission_Performance_Value > 0) {
      updateScore(GG0170S_Admission_Performance, GG0170S_Admission_Performance_Value * admissionMultiplier);
      S_Admission_Performance += GG0170S_Admission_Performance_Value * admissionMultiplier;
    }
    else
      updateScore(GG0170S_Admission_Performance, 0);

    return R_Admission_Performance +  S_Admission_Performance;
  }

  /* internal function */
  function Score_GG0170AtoP_Discharge_Performance(): number {
    let GG0170_AtoP_Discharge_Performance: number = 0;
    /* select only GG0170 Discharge Performance excluding Q, R and S */
    $('.persistable[id^=GG0170]:not([id*=Admission_Performance]):not([id*=Discharge_Goal]):not([id*=GG0170Q]):not([id*=GG0170R]):not([id*=GG0170S])').each(function () {
      let $thisControl = $(this);
      let thisControlScore: number = getControlValue($thisControl);
      let thisControlID: string = $thisControl.prop('id');

      switch (true) {
        case thisControlScore >= 7:
          if (thisControlID.indexOf('GG0170I') >= 0) {
            //7,9,10, or 88 don't score per customer 12/8/2021
            updateScore($thisControl, 0);
          }
          else {
            updateScore($thisControl, 1);
            GG0170_AtoP_Discharge_Performance += 1;
          }
          break;
        case thisControlScore > 0 && thisControlScore <= 6:
          //btw 1 and 6 add value point 
          updateScore($thisControl, thisControlScore);
          GG0170_AtoP_Discharge_Performance += thisControlScore;
          break;
        default:
          updateScore($thisControl, 0);
          break;
      }
    });

    return GG0170_AtoP_Discharge_Performance;
  }

  /* internal function */
  function Score_GG0170RandS_Discharge_Performance(): number {
    let dischargeMultiplier: number = 1;

    /* use GG0170I to determine the multipliers for GG0170R and GG0170S */
    let GG0170I_Discharge_Performance: any = $('.persistable[id^=GG0170I_Discharge_Performance]');
    let GG0170I_Discharge_Performance_Value: number = getControlValue(GG0170I_Discharge_Performance);

    if (GG0170I_Discharge_Performance_Value >= 7)
      dischargeMultiplier = 2;
    else
      dischargeMultiplier = 0;

    let R_Discharge_Performance: number = 0;
    let S_Discharge_Performance: number = 0;

    let GG0170R_Discharge_Performance: any = $('.persistable[id^=GG0170R_Discharge_Performance]');
    let GG0170R_Discharge_Performance_Value: number = getControlValue(GG0170R_Discharge_Performance);
    if (GG0170R_Discharge_Performance_Value > 0) {
      updateScore(GG0170R_Discharge_Performance, GG0170R_Discharge_Performance_Value * dischargeMultiplier);
      R_Discharge_Performance += GG0170R_Discharge_Performance_Value * dischargeMultiplier;
    }
    else
      updateScore(GG0170R_Discharge_Performance, 0);

    let GG0170S_Discharge_Performance: any = $('.persistable[id^=GG0170S_Discharge_Performance]');
    let GG0170S_Discharge_Performance_Value: number = getControlValue(GG0170S_Discharge_Performance);
    if (GG0170S_Discharge_Performance_Value > 0) {
      updateScore(GG0170S_Discharge_Performance, GG0170S_Discharge_Performance_Value * dischargeMultiplier);
      S_Discharge_Performance += GG0170S_Discharge_Performance_Value * dischargeMultiplier;
    }
    else
      updateScore(GG0170S_Discharge_Performance, 0);

    return R_Discharge_Performance + S_Discharge_Performance;
  }

  /****************************************************************************
   * public functions exposing the private functions to outside of the closure
  ***************************************************************************/
  return {
    'isEmpty': isEmpty,
    'isDate': isDate,
    'scrollToAnchor': scrollToAnchor,
    'setRehabBtns': setRehabBtns,
    'resetRehabBtns': resetRehabBtns,
    'breakLongSentence': breakLongSentence,
    'getTextPixels': getTextPixels,
    'submitTheForm': submitTheForm,
    'validate': validateForm,
    'selfCareScore': selfCareScore,
    'mobilityScore': mobilityScore,
    'updateScore': updateScore
  }
})();