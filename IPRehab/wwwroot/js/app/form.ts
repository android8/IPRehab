/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../../node_modules/@types/jqueryui/index.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />

import { IUserAnswer, AjaxPostbackModel } from "../appModels/IUserAnswer";
import { MDCTextField } from "../../node_modules/@material/textfield/index";
import { isNumeric, post } from "jquery";
import { InteractionTrigger } from "../../node_modules/@material/chips/deprecated/trailingaction/constants";
//import { MDCRipple } from "../../node_modules/@material/ripple/index";

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html
export { formController };

$(function () {
  $('.persistable').change(function () {
    const $this = $(this);
    let onsetDate: Date = new Date($(".persistable[data-questionkey^='Q23']").val().toString());
    let admissionDate: Date = new Date($(".persistable[data-questionkey^='Q12']").val().toString());

    if (formController.isDate(onsetDate) && formController.isDate(admissionDate)) {
      $('#ajaxPost').removeAttr('disabled');
      //$('#mvcPost').removeAttr('disabled');

      //let theScope: any = $('#userAnswerForm');
      //let stageName: string = $('#stage', theScope).val().toString();
      //const patientID: string = $('#patientID', theScope).val().toString();
      //const patientName: string = $('#patientName', theScope).val().toString();
      //let episodeID: number = +$('#episodeID', theScope).val();

      //if ($this.prop('id').indexOf('Q12') === -1 && $this.prop('id').indexOf('Q23') === -1) {
      //  formController.checkChanges($this, episodeID, stageName, patientID, patientName);
      //}
    }
  });

  $('select').each(function () {
    const $this = $(this);
    $this.change(function () {
      console.log($this.prop('id') + ' changed triggering breakLongSentence()')
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
    const $this = $(this);
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
    if ($('#userAnswerForm').validate()) {
      $('.spinnerContainer').show();
      const thisPostBtn: any = $(this);
      const theScope: any = $('#userAnswerForm');
      const patientID: string = $('#patientID', theScope).val().toString();
      const patientName: string = $('#patientName', theScope).val().toString();
      //let stage: string = $('#stage', theScope).val().toString();
      const stage: string = $('.pageTitle').data('systitle');
      let episodeID: number = +$('#episodeID', theScope).val();
      if (stage.toLowerCase() == 'new')
        episodeID = -1;
      formController.submitTheForm($('.persistable', theScope), stage, patientID, patientName, episodeID, thisPostBtn);
    }
  });

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
  let oldAnswers: Array<IUserAnswer> = new Array<IUserAnswer>();
  let newAnswers: Array<IUserAnswer> = new Array<IUserAnswer>();
  let updatedAnswers: Array<IUserAnswer> = new Array<IUserAnswer>();
  enum enumCRUD { C = 'Insert', U = 'Update', D = 'Delete', N = 'NoChange' };

  /* private function */
  function isEmpty($this: any): boolean {
    if (typeof $this.val() !== 'undefined' && $this.val())
      return false;
    else
      return true;
  }

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
  function breakLongSentence(thisSelectElement: any) {
    console.log('----------- begin breakLongSentence -----------');
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
    console.log('----------- end breakLongSentence -----------');
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
  function checkChanges($thisPersistable: any, episodeID: number, stage: string, patientID: string, patientName: string): void {
    console.log('$thisPersistable', $thisPersistable);

    let thisAnswer: IUserAnswer = <IUserAnswer>{};

    //a question may show several input fields for multiple stages,
    //so we have to use the data-questionkey at the field level, not the stage hidden input set at the form level
    let onsetDate: any = $(".persistable[data-questionkey^='Q23']").val();
    let admissionDate: any = $(".persistable[data-questionkey^='Q12']").val();

    //if not a concatenation, a '+' in front of a variable converts it to a number
    //ToDo: make this closure available to other modules to avoid code duplication in commandBtns.ts

    let currentValue: string = $thisPersistable.val();
    /* for non-radio non-checkbox, non-select elements, the loaded value when DOM is ready before changed */
    let defaultValue: string = $thisPersistable[0].defaultValue ? $thisPersistable[0].defaultValue : '';
    let CRUD: enumCRUD;

    switch ($thisPersistable.prop('type')) {
      case 'select-one': {
        defaultValue = $thisPersistable.data('codesetid');
        CRUD = optionComparison(currentValue, defaultValue);

        console.log($thisPersistable.prop('id') + ' ' + CRUD + ' current=' + (+currentValue.toString()) + ' default=' + (+defaultValue.toString()));
        break;
      }

      case 'checkbox':
      case 'radio': {
        let currentChecked: boolean = Boolean($thisPersistable.prop('checked'));
        let defaultChecked: boolean = Boolean($thisPersistable[0].defaultChecked);
        CRUD = booleanComparison(currentChecked, defaultChecked);
        console.log($thisPersistable.prop('id') + ' ' + CRUD + ' current=' + currentChecked + ' default=' + defaultChecked);
        break;
      }

      case 'date': {
        CRUD = dateComparison(currentValue, defaultValue);
        console.log($thisPersistable.prop('id') + ' ' + CRUD + ' current=' + (+currentValue.toString()) + ' default=' + (+defaultValue.toString()));
        break;
      }

      default: {
        CRUD = stringComparison(currentValue, defaultValue);
        console.log($thisPersistable.prop('id') + ' ' + CRUD + ' current=' + (+currentValue.toString()) + ' default=' + (+defaultValue.toString()));
        break;
      }
    }

    if (CRUD !== enumCRUD.N) {
      let answerID: string = $thisPersistable.data('answerid'); /* -1 if no answer */
      thisAnswer.AnswerID = +answerID;
      thisAnswer.PatientName = patientName.toString();
      thisAnswer.PatientID = patientID;
      thisAnswer.EpisodeID = episodeID;
      thisAnswer.AdmissionDate = admissionDate;
      thisAnswer.OnsetDate = onsetDate;
      thisAnswer.QuestionID = +$thisPersistable.data('questionid');
      thisAnswer.QuestionKey = $thisPersistable.data('questionkey');

      //12/6/2021 per stakeholder that all Q series questions (formerly BASE stage) should be shared across the stages
      if ((stage.toLowerCase() === 'base') && thisAnswer.QuestionKey.indexOf('Q') === 0)
        thisAnswer.StageID = 421  /* BASE stage ID*/;
      else
        thisAnswer.StageID = +$thisPersistable.data('stageid');

      /* the currently selected stage name*/
      thisAnswer.StageName = stage;

      let thisInputType: string = $thisPersistable.prop('type');
      thisAnswer.AnswerCodeSetID = +currentValue;
      thisAnswer.AnswerCodeSetDescription = $thisPersistable.data('codesetdescription');

      switch (thisInputType) {
        case 'select-one':
        case 'radio':
        case 'checkbox':
          break;
        default:
          /* all other input are text based, so in addition to the answer codesetID we need to save the current control text in the .Descrition field  */
          /* number type is only used in therapy minutes (O0401 series) and number of wound (M series)] */
          thisAnswer.Description = currentValue;
          break;
      }

      if ($thisPersistable.data('answersequencenumber'))
        thisAnswer.AnswerSequenceNumber = +$thisPersistable.data('answersequencenumber');

      thisAnswer.AnswerByUserID = $thisPersistable.data('userid');
      thisAnswer.LastUpdate = new Date();

      /* we are doing one time change checking at submit(), so the following lines which used for .persistable.change() events are obsolete. */
      /* since the answer state can be changed from one to another before submit, so we need to find previous element in all arraries and clear them then re-push the action to the proper array */
      //let inNew: IUserAnswer = newAnswers.find(e => e.QuestionKey == thisAnswer.QuestionKey && e.StageID == thisAnswer.StageID);
      //let inOld: IUserAnswer = oldAnswers.find(e => e.QuestionKey == thisAnswer.QuestionKey && e.StageID == thisAnswer.StageID);
      //let inUpdated: IUserAnswer = updatedAnswers.find(e => e.QuestionKey == thisAnswer.QuestionKey && e.StageID == thisAnswer.StageID);
      //if (inNew)
      //  newAnswers.splice(newAnswers.indexOf(inNew), 1);
      //if (inOld)
      //  oldAnswers.splice(newAnswers.indexOf(inNew), 1);
      //if (inUpdated)
      //  updatedAnswers.splice(newAnswers.indexOf(inNew), 1);

      switch (CRUD) {
        case enumCRUD.C:
          newAnswers.push(thisAnswer);
          console.log('push ' + $thisPersistable.prop('id') + ' to newAnswers');
          console.log('newAnswers', newAnswers);
          break;
        case enumCRUD.U:
          updatedAnswers.push(thisAnswer);
          console.log('push ' + $thisPersistable.prop('id') + ' to updatedAnswers');
          console.log('updatedAnswers', updatedAnswers);

          break;
        case enumCRUD.D:
          oldAnswers.push(thisAnswer);
          console.log('push ' + $thisPersistable.prop('id') + ' to oldAnswers');
          console.log('updatedAnswers', oldAnswers);
          break;
      }
    }
  }

  /* private function */
  function submitTheForm(persistables: any, stage: string, patientID: string, patientName: string, episodeID: number, thisPostBtn: any): void {
    /* reset the selected in case the previous post failed*/
    newAnswers = [];
    oldAnswers = [];
    updatedAnswers = [];

    $('.persistable').map(function () {
      checkChanges($(this), episodeID, stage, patientID, patientName);
    });

    console.log('newAnswers', newAnswers);
    console.log('oldAnswers', oldAnswers);
    console.log('updatedAnswers', updatedAnswers);

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
      if (!isNaN(thisControlIntValue) && thisControlIntValue <= 0) {
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

  function optionComparison(currentValue: string, defaultValue: string): enumCRUD {
    if (+currentValue === +defaultValue)
      return enumCRUD.N;
    if (+currentValue > 0 && +defaultValue <= 0)
      return enumCRUD.C;
    if (+currentValue <= 0 && +defaultValue > 0)
      return enumCRUD.D;
    if (+currentValue > 0 && +defaultValue > 0)
      return enumCRUD.U;
  }

  function booleanComparison(currentChecked: boolean, defaultChecked: boolean): enumCRUD {
    if (currentChecked === defaultChecked) {
      return enumCRUD.N;
    }

    if (currentChecked && !defaultChecked) {
      return enumCRUD.C;
    }

    if (!currentChecked && defaultChecked) {
      return enumCRUD.D;
    }
  }

  function dateComparison(currentValue: string, defaultValue: string): enumCRUD {
    if ((currentValue === defaultValue) || new Date(currentValue) === new Date(defaultValue) || !isDate(new Date(currentValue)) === !isDate(new
      Date(defaultValue))) {
      return enumCRUD.N;
    }
    if (isDate(new Date(currentValue)) && !isDate(new Date(defaultValue))) {
      return enumCRUD.C;
    }
    if (isDate(new Date(currentValue)) && isDate(new Date(defaultValue))) {
      return enumCRUD.U;
    }
    if (currentValue === '' && !isDate(new Date(currentValue)) && isDate(new Date(defaultValue))) {
      return enumCRUD.D;
    }
  }

  function stringComparison(currentValue: string, defaultValue: string): enumCRUD {
    if (currentValue === defaultValue) {
      return enumCRUD.N;
    }
    if (currentValue !== '' && defaultValue === '')
      return enumCRUD.C;
    if (currentValue === '' && defaultValue !== '')
      return enumCRUD.D;
    if (currentValue !== '' && defaultValue !== '')
      return enumCRUD.U;
  }

  function Score_GG0170_Except_GG0170R_GG0170S(): number {
    let A_to_P_score: number = 0;
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
        if (thisControlID.indexOf(valueFactoringFields[i]) !== -1) {
          switch (true) {
            case thisControlScore >= 7:
              if (valueFactoringFields[i] == 'I_') {
                //7,9,10, or 88 don't score per customer 12/8/2021
                updateScore($thisControl, 0);
              }
              else {
                updateScore($thisControl, 1);
                A_to_P_score++;
              }
              break;
            case thisControlScore > 0 && thisControlScore <= 6:
              //btw 1 and 6 add value point 
              updateScore($thisControl, thisControlScore);
              A_to_P_score += thisControlScore;
              break;
            default:
              updateScore($thisControl, 0);
              break;
          }

          //exit for() loop
          break;
        }
        else {
          updateScore($thisControl, 0);
        }
      }
    });

    return A_to_P_score;
  }

  function Score_GG0170R_GG0170S(): number {
    let R_Score: number = 0, S_Score: number = 0;
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
    switch (true) {
      case (GG0170IAdmissionChoice >= 7 || GG0170IDischargeChoice >= 7):
        multiplier = 2;
        break;
      case ((GG0170IAdmissionChoice > 0 && GG0170IAdmissionChoice <= 6) ||
        (GG0170IDischargeChoice > 0 && GG0170IDischargeChoice <= 6)):
        multiplier = 0;
        break;
    }

    if (GG0170RAdmissionScore > 0) {
      updateScore(GG0170R_Admission, GG0170RAdmissionScore * multiplier);
      R_Score += GG0170RAdmissionScore * multiplier;
    }
    else {
      updateScore(GG0170R_Admission, 0);
    }

    if (GG0170SAdmissionScore > 0) {
      updateScore(GG0170S_Admission, GG0170SAdmissionScore * multiplier);
      S_Score += GG0170SAdmissionScore * multiplier;
    }
    else {
      updateScore(GG0170S_Admission, 0);
    }

    return R_Score + S_Score;
  }

  function GG0170I_Admission_Choice(GG0170I_Admission: any): number {
    let GG0170IAdmissionChoice: number = 0;

    /* there will only be one match from the selector per stage, so each() only loop once*/
    GG0170I_Admission.each(function () {
      let GG0170I_Admission_Control: any = $(this);
      let thisControlValueInt: number = parseInt(GG0170I_Admission_Control.prop('value'));

      if (!isNaN(thisControlValueInt) && thisControlValueInt > 0) {
        let GG0170I_Admission_ControlType: string = GG0170I_Admission_Control.prop('type');
        switch (GG0170I_Admission_ControlType) {
          case "select-one": {
            //true score is the selected option text
            let selectedOption: string = $('#' + GG0170I_Admission_Control.prop('id') + ' option:selected').text();
            let selectedOptionInt: number = parseInt(selectedOption);
            if (!isNaN(selectedOptionInt) && selectedOptionInt > 0) {
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
              if (!isNaN(thisLableInt) && thisLableInt > 0) {
                GG0170IAdmissionChoice = thisLableInt;
              }
            }
            break;
          }
          case "text": {
            let thisInputValue: string = GG0170I_Admission_Control.val().toString();
            let thisInputValueInt: number = parseInt(thisInputValue);
            if (!isNaN(thisInputValueInt) && thisInputValueInt > 0) {
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
      if (!isNaN(thisControlValueInt) && thisControlValueInt > 0) {
        let GG0170I_Discharge_ControlType: string = GG0170I_Discharge_Control.prop('type');
        switch (GG0170I_Discharge_ControlType) {
          case "select-one": {
            //true score is the selected option text
            let selectedOption: any = $('#' + GG0170I_Discharge_Control.prop('id') + ' option:selected').text();
            let selectedOptionInt: number = parseInt(selectedOption);
            if (!isNaN(selectedOptionInt) && selectedOptionInt > 0) {
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
              if (!isNaN(thisLabelInt) && thisLabelInt > 0) {
                GG0170IDischargeChoice = thisLabelInt;
              }
            }
            break;
          }
          case "text": {
            //true score is the entered text
            let thisInputValue: string = GG0170I_Discharge_Control.val().toString();
            let thisInputValueInt: number = parseInt(thisInputValue);
            if (!isNaN(thisInputValueInt) && thisInputValueInt > 0) {
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
      let selectedOptionInt: number = 0;
      let GG0170R_Admission_Control: any = $(this);
      let thisControlValueInt: number = parseInt(GG0170R_Admission_Control.prop('value'));
      if (thisControlValueInt > 0) {
        let GG0170R_Admission_ControlType: string = GG0170R_Admission_Control.prop('type');
        switch (GG0170R_Admission_ControlType) {
          case "select-one": {
            //true score is the selected option text
            let selectedOption: any = $('#' + GG0170R_Admission_Control.prop('id') + ' option:selected').text();
            selectedOptionInt = parseInt(selectedOption);
            break;
          }
          case "checkbox":
          case "radio": {
            //true score is the checked label
            if (GG0170R_Admission_Control.prop('checked')) {
              let thisLabel = GG0170R_Admission_Control.closest('label').text();
              /* always NaN because currently there is no numeric data to go by for checkbox and radio controls */
              selectedOptionInt = parseInt(thisLabel);
            }
            break;
          }
          case "text": {
            //true score is the entered text
            let thisInputValue: string = GG0170R_Admission_Control.val().toString();
            selectedOptionInt = parseInt(thisInputValue);
            break;
          }
        }
        switch (true) {
          case (!isNaN(selectedOptionInt) && (selectedOptionInt > 0 && selectedOptionInt <= 6)):
            GG0170RAdmissionScore = selectedOptionInt;
            break;
          case (!isNaN(selectedOptionInt) && selectedOptionInt >= 7):
            GG0170RAdmissionScore = 1;
            break;
        }
      }
    });
    return GG0170RAdmissionScore;
  }

  function GG0170S_Admission_Score(GG0170S_Admission: any): number {
    let GG0170SAdmissionScore: number = 0;

    /* there will only be one match from the selector per stage, so each() only loop once */
    GG0170S_Admission.each(function () {
      let selectedOptionInt: number = 0;
      let GG0170S_Admission_Control: any = $(this);
      let thisControlValueInt: number = parseInt(GG0170S_Admission_Control.prop('value'));
      if (!isNaN(thisControlValueInt) && thisControlValueInt > 0) {
        let GG0170S_Admission_ControlType: string = GG0170S_Admission_Control.prop('type');
        switch (GG0170S_Admission_ControlType) {
          case "select-one": {
            //true score is the selected option text
            let selectedOption: any = $('#' + GG0170S_Admission_Control.prop('id') + ' option:selected').text();
            selectedOptionInt = parseInt(selectedOption);
            break;
          }
          case "checkbox":
          case "radio": {
            //true score is the checked label
            if (GG0170S_Admission_Control.prop('checked')) {
              let thisLabel: string = GG0170S_Admission_Control.closest('label').text();
              /* always NaN because currently there is no numeric data to go by for checkbox and radio controls */
              selectedOptionInt = parseInt(thisLabel);
            }
            break;
          }
          case "text": {
            //true score is the entered text
            let thisInputValue: string = GG0170S_Admission_Control.val().toString();
            selectedOptionInt = parseInt(thisInputValue);
            break;
          }
        }
        switch (true) {
          case (!isNaN(selectedOptionInt) && selectedOptionInt > 0 && selectedOptionInt <= 6):
            GG0170SAdmissionScore = selectedOptionInt;
            break;
          case (!isNaN(selectedOptionInt) && selectedOptionInt >= 7):
            GG0170SAdmissionScore = 1;
            break;
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
    'isDate': isDate,
    'scrollToAnchor': scrollToAnchor,
    'setRehabBtns': setRehabBtns,
    'resetRehabBtns': resetRehabBtns,
    'breakLongSentence': breakLongSentence,
    'getTextPixels': getTextPixels,
    'submitTheForm': submitTheForm,
    'validateForm': validateForm,
    'selfCareScore': selfCareScore,
    'mobilityScore': mobilityScore,
    'updateScore': updateScore,
    'checkChanges': checkChanges
  }
})();