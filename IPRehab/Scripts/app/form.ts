﻿/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../../node_modules/@types/jqueryui/index.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />

import { IUserAnswer, AjaxPostbackModel } from "../appModels/IUserAnswer";
import { MDCTextField } from "../../node_modules/@material/textfield/index";
import { post } from "jquery";
//import { MDCRipple } from "../../node_modules/@material/ripple/index";

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
  $('.persistable').change(function () {
    $('#submit').removeAttr('disabled');
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

  /* collect all persistable input values */
  $('#submit').click(function () {
    $('.spinnerContainer').show();
    let theScope = $('#userAnswerForm');
    formController.searlizeTheForm($('.persistable',theScope));
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
      let newTitle: string = $this.attr('title').replace(/Edit/g, 'Create');
      let newHref: string = $this.attr('href').replace(/Edit/g, 'Create');
      $this.attr('title', newTitle);
      $this.attr('href', newHref);
      currentIdx++;
      let newClass: string = $this.attr('class') + ' createActionCmd' + currentIdx.toString();;
      $this.attr('class', newClass);
    });
  }

  /* private function */
  function resetRehabBtns(targetScope: any) {
    let cmdBtns: string[] = ['primary', 'info', 'secondary', 'success', 'warning'];
    let currentIdx: number = 0;
    $.each($('.rehabAction', targetScope), function () {
      let $this = $(this);
      let newTitle: string = $this.attr('title').replace(/Create/g, 'Edit');
      let newHref: string = $this.attr('href').replace(/Create/g, 'Edit');
      $this.attr('title', newTitle);
      $this.attr('href', newHref);
      let resetClass: string = '';
      resetClass = 'badge badge-' + cmdBtns[currentIdx] + ' rehabAction';
      currentIdx++;
      $this.attr('class', resetClass);
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
      $('#Q44D_').attr('disabled', 'true');
      $('#Q45_').attr('disabled', 'true');
    }
    if (q44c_is_1 && q44d_is_1) {
      /*Q44C = 1 and Q44D = 1*/
      $('#Q45_').attr('disabled', 'false');
    }
    else {
      if (q44c_is_0) {
        $('#Q44D_').attr('disabled', 'false');
        $('#Q46_').focus();
      }
    }

    /* interrupted */
    let q42_is_interrupted: boolean = $('#Q42-INTRRUPT_86').prop('checked');

    if (q42_is_interrupted) {
      $('#Q43_').attr('disabled', 'false');
      $('#Q43_').focus();
    }
  }

  /* private function */
  function validate() {
    //$('form#userAnswerForm').validate({
    //  rules: {

    //  }
    //})
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
  function searlizeTheForm(persistables: any): void {
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
    let oldAnswersJson: string;
    const newAnswers: Array<IUserAnswer> = new Array<IUserAnswer>();;
    let newAnswersJson: string;
    const updatedAnswers: Array<IUserAnswer> = new Array<IUserAnswer>();
    let updatedAnswersJson: string;

    //ToDo: make this closure available to other modules to avoid code duplication in commandBtns.ts
    let CRUD: string;
    persistables.map(function () {
      let $thisPersistable: any = $(this);

      let currentValue = $thisPersistable.val();
      let oldValue = $thisPersistable.attr('data-oldvalue');
      let answerID = $thisPersistable.attr('data-answerid');

      if ($thisPersistable.type == 'select-one' && currentValue == -1 && oldValue == '') {
        return false; //won't break the .map, but skip the item and contintue mapping
      }
      if (currentValue == oldValue) {
        return false;
      }

      let thisAnswer: IUserAnswer = <IUserAnswer>{};

      if ($thisPersistable.attr('data-answerid'))
        thisAnswer.AnswerID = $thisPersistable.attr('data-answerid');

      if ($thisPersistable.attr('data-episodeid'))
        thisAnswer.EpisodeID = $thisPersistable.attr('data-episodeid');

      thisAnswer.QuestionID = $thisPersistable.attr('data-questionid');
      thisAnswer.StageID = $thisPersistable.attr('data-stageid');
      thisAnswer.AnswerCodeSetID = $thisPersistable.value;

      if ($thisPersistable.attr('data-answersequencenumber'))
        thisAnswer.AnswerSequenceNumber = $thisPersistable.attr('data-answersequencenumber');

      let thisInputType: string = $thisPersistable.attr('type');
      if (thisInputType == 'text' || thisInputType == 'date' || thisInputType == 'number' || thisInputType == 'textarea') {
        /* save extra description of the wide range of date, text, or number */
        thisAnswer.AnswerCodeSetID = $thisPersistable.attr('data-codesetid');
        thisAnswer.Description = $thisPersistable.value;
      }

      thisAnswer.AnswerByUserID = $thisPersistable.attr('data-userid');
      thisAnswer.LastUpdate = new Date();

      if (currentValue != '' && answerID == '') {
        CRUD = 'C';
      }
      else if (currentValue == '' && answerID != '') {
        CRUD = 'D';
      }
      else {
        CRUD = "U";
      }

      switch (CRUD) {
        case 'C':
          newAnswers.push(thisAnswer);
          newAnswersJson += JSON.stringify(newAnswers);
          break;
        case 'U':
          updatedAnswers.push(thisAnswer);
          updatedAnswersJson += JSON.stringify(updatedAnswers);
          break;
        case 'D':
          oldAnswers.push(thisAnswer);
          oldAnswersJson += JSON.stringify(oldAnswers);
          break;
      }
    });

    $("#dialog").text(newAnswersJson + '<br/>' + updatedAnswersJson + '<br/>' + oldAnswersJson);

    $('.spinnerContainer').hide();

    $("#dialog").dialog({
      resizable: false,
      height: "auto",
      width: 400,
      modal: true,
      buttons: {
        "Save": function () {
          let thisUrl: string = $('#submit').attr('formaction');
          //ToDo: ajax post here
          let postBackModel: AjaxPostbackModel;
          postBackModel.NewAnswers = newAnswers;
          postBackModel.OldAnswers = oldAnswers;
          postBackModel.UpdatedAnswers = updatedAnswers;
          alert('ToDo: sending ajax postBackModel to ' + thisUrl);
        },
        Cancel: function () {
          $(this).dialog("close");
        }
      }
    });
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