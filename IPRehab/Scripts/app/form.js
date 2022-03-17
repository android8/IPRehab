/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../../node_modules/@types/jqueryui/index.d.ts" />
//import { MDCRipple } from "../../node_modules/@material/ripple/index";
import { Utility } from "./utility.js";
import { UserAnswer } from "./userAnswer.js";
import { AjaxPostbackModel } from "./AjaxPostbackModel.js";
//https://www.typescriptlang.org/docs/handbook/asp-net-core.html
$(function () {
    $('.persistable').change(function () {
        const onsetDate = new Date($(".persistable[data-questionkey^='Q23']").val().toString());
        const admissionDate = new Date($(".persistable[data-questionkey^='Q12']").val().toString());
        const commonUtility = new Utility();
        //if (formController.isDate(onsetDate) && formController.isDate(admissionDate)) {
        if (commonUtility.isDate(onsetDate) && commonUtility.isDate(admissionDate)) {
            $('#ajaxPost').removeAttr('disabled');
            //$('#mvcPost').removeAttr('disabled');
        }
    });
    $('select').each(function () {
        const $this = $(this);
        $this.change(function () {
            formController.breakLongSentence($this);
        });
    });
    /* section nav */
    $('#questionTab').hover(function () {
        $('#questionTab').css({ 'left': '0px', 'transition-duration': '1s' });
    }, function () {
        $('#questionTab').css({ 'left': '-230px', 'transition-duration': '1s' });
    });
    /* jump to section anchor */
    $('.gotoSection').each(function () {
        const $this = $(this);
        $this.click(function () {
            const $fromThis = $(this);
            const toAnchor = $('#' + $this.data("anchorid"));
            if (toAnchor != null)
                formController.scrollToAnchor($fromThis, toAnchor);
        });
    });
    /* show hide section */
    $('.section-title').each(function () {
        const thisTitle = $(this);
        thisTitle.click(function () {
            let sectionContent = thisTitle.next();
            let sectionContentHidden = sectionContent.attr('hidden');
            sectionContent.attr('hidden', !sectionContentHidden);
        });
    });
    /* show hide individual question */
    $('.child1').each(function () {
        const thisKey = $(this);
        thisKey.click(function () {
            const thisQuestion = thisKey.next();
            console.log('thisQuestion', thisQuestion);
            const thisQuestionHidden = thisQuestion.attr('hidden');
            console.log('thisQuestion hidden', thisQuestionHidden);
            thisQuestion.attr('hidden', !thisQuestionHidden);
        });
    });
    /* traditional form post */
    //$('#mvcPost').click(function () {
    //  $('form').submit();
    //});
    /* ajax post form */
    $('#ajaxPost').click(function () {
        if (formController.validate) {
            formController.submitTheForm($(this));
        }
    });
    /* self care score on load */
    formController.selfCareScore();
    /* self care scorce on change */
    $('.persistable[id^=GG0130]:not([id*=Discharge_Goal])').each(function () {
        const $this = $(this);
        $this.change(function () {
            formController.selfCareScore();
            formController.grandTotal();
        });
    });
    /* mobility score on load */
    formController.mobilityScore();
    /* mobility score on change */
    $('.persistable[id^=GG0170]:not([id*=Discharge_Goal])').each(function () {
        $(this).change(function () {
            formController.mobilityScore();
            formController.grandTotal();
        });
    });
    /* grand total on load */
    formController.grandTotal();
});
/****************************************************************************
 * javaScript closure
 ***************************************************************************/
let formController = (function () {
    const commonUtility = new Utility();
    /* private function */
    function scrollToAnchor(from, to) {
        /* https://arnavzedion.medium.com/the-difference-between-offsettop-scrolltop-clienttop-36cf52b733ca#:~:text=offsetTop%20is%20read-only%2C%20while%20scrollTop%20is%20read%2Fwrite.%20As,dependent%20variable%20or%20offset%20position%20to%20scroll%20independently
         */
        var position = to.offset().top
            - from.offset().top
            + from.scrollTop();
        $('html,body').animate({ scrollTop: position }, 'fast');
    }
    /* private function */
    function setRehabBtns(targetScope) {
        let currentIdx = 0;
        $.each($('.rehabAction', targetScope), function () {
            let $this = $(this);
            let newTitle = $this.prop('title').replace(/Edit/g, 'Create');
            let newHref = $this.prop('href').replace(/Edit/g, 'Create');
            $this.prop('title', newTitle);
            $this.prop('href', newHref);
            currentIdx++;
            let newClass = $this.prop('class') + ' createActionCmd' + currentIdx.toString();
            $this.prop('class', newClass);
        });
    }
    /* private function */
    function resetRehabBtns(targetScope) {
        let cmdBtns = ['primary', 'info', 'secondary', 'success', 'warning'];
        let currentIdx = 0;
        $.each($('.rehabAction', targetScope), function () {
            let $this = $(this);
            let newTitle = $this.prop('title').replace(/Create/g, 'Edit');
            let newHref = $this.prop('href').replace(/Create/g, 'Edit');
            $this.prop('title', newTitle);
            $this.prop('href', newHref);
            let resetClass = '';
            resetClass = 'badge badge-' + cmdBtns[currentIdx] + ' rehabAction';
            currentIdx++;
            $this.prop('class', resetClass);
        });
    }
    /* private function */
    function breakLongSentence(thisSelectElement) {
        //console.log('thisSelectElement', thisSelectElement);
        let maxLength = 50;
        let longTextOptionDIV = thisSelectElement.next('div.longTextOption');
        //console.log('longTextOptionDIV', longTextOptionDIV);
        let thisSelectWidth = thisSelectElement[0].clientWidth;
        let thisScope = thisSelectElement;
        let selectedValue = parseInt(thisSelectElement.prop('value'));
        if (selectedValue <= 0) {
            longTextOptionDIV.text('');
        }
        else {
            $.each($('option:selected', thisScope), function () {
                let $thisOption = $(this);
                let regX = new RegExp("([\\w\\s]{" + (maxLength - 2) + ",}?\\w)\\s?\\b", "g");
                let oldText = $thisOption.text();
                let font = $thisOption.css('font');
                let oldTextInPixel = commonUtility.getTextPixels(oldText, font);
                //console.log('oldTextInPixel', oldTextInPixel);
                //console.log('thisSelectWidth', thisSelectWidth);
                longTextOptionDIV.text('');
                if (oldTextInPixel > thisSelectWidth) {
                    let newStr = oldText.replace(regX, "$1\n");
                    newStr = newStr.trim();
                    let startWithNumber = $.isNumeric(newStr.substring(0, 1));
                    if (startWithNumber) {
                        newStr = newStr.substring(newStr.indexOf(" ") + 1);
                    }
                    //console.log('old ->', oldText);
                    //console.log('new ->', newStr);
                    longTextOptionDIV.text(newStr);
                    longTextOptionDIV.removeClass("invisible");
                }
            });
        }
    }
    /* private function */
    function submitTheForm(thisPostBtn) {
        $('.spinnerContainer').show();
        const oldAnswers = new Array();
        const newAnswers = new Array();
        const updatedAnswers = new Array();
        const theScope = $('#userAnswerForm');
        const stageName = $('#stage', theScope).val().toString();
        const facilityID = $('#facilityID', theScope).val().toString();
        const patientID = $('#patientID', theScope).val().toString();
        const patientName = $('#patientName', theScope).val().toString();
        let episodeID = +($('#episodeID', theScope).val());
        //get the key answers. these must be done outside of the .map() 
        //because each answer in .map() will use the same episode onset date and admission date
        let onsetDate = new Date($(".persistable[data-questionkey^='Q23']").val().toString());
        let admissionDate = new Date($(".persistable[data-questionkey^='Q12']").val().toString());
        //ToDo: make this closure available to other modules to avoid code duplication in commandBtns.ts
        const persistables = $('.persistable');
        let counter = 0;
        if (stageName.toLowerCase() == "new")
            episodeID = -1;
        persistables.each(function () {
            counter++;
            const $thisPersistable = $(this);
            const questionKey = $thisPersistable.data('questionkey');
            let thisAnswer = new UserAnswer();
            let oldValue = $thisPersistable.data('oldvalue');
            let currentValue = '';
            currentValue = commonUtility.getControlValue($thisPersistable, 'default');
            //!undefined or !NaN yield true
            if (+currentValue <= 0)
                currentValue = '';
            let equalityResult = commonUtility.isSameAnswer($thisPersistable, oldValue, currentValue);
            if (equalityResult != '') {
                console.log('(' + counter + ') exit ' + questionKey + ' ' + equalityResult);
                return;
            }
            console.log('--------- continue ---------');
            const questionId = +$thisPersistable.data('questionid');
            const controlType = $thisPersistable.prop('type');
            const answerId = $thisPersistable.data('answerid');
            const measureId = +$thisPersistable.data('measureid');
            const measureDescription = $thisPersistable.data('measuredescription');
            const codesetDescription = $thisPersistable.data('codesetdescription');
            const answerSequence = +$thisPersistable.data('answersequence');
            const userID = $thisPersistable.data('userid');
            thisAnswer.PatientName = patientName;
            thisAnswer.PatientID = patientID;
            thisAnswer.EpisodeID = episodeID;
            thisAnswer.OnsetDate = onsetDate;
            thisAnswer.AdmissionDate = admissionDate;
            thisAnswer.QuestionID = questionId;
            thisAnswer.QuestionKey = questionKey;
            //possible problem with stage id or measure id
            thisAnswer.MeasureID = measureId;
            thisAnswer.MeasureName = measureDescription;
            thisAnswer.OldAnswerCodeSetID = +oldValue;
            thisAnswer.AnswerCodeSetDescription = codesetDescription;
            if (answerSequence)
                thisAnswer.AnswerSequenceNumber = answerSequence;
            thisAnswer.AnswerByUserID = userID;
            thisAnswer.LastUpdate = new Date();
            switch (controlType) {
                case 'text':
                case 'date':
                case 'textarea':
                case 'number':
                    /* store in the description the free text because all inputs derived from text type have the same codeset id */
                    thisAnswer.AnswerCodeSetID = +$thisPersistable.data('codesetid');
                    thisAnswer.Description = currentValue;
                    break;
                default: /* dropdown and radio */
                    thisAnswer.AnswerCodeSetID = +currentValue;
                    break;
            }
            console.log('(' + counter + ') ' + questionKey, thisAnswer);
            switch (true) {
                case (currentValue && !oldValue):
                    console.log('(C)reate new value = ' + currentValue + ', old value = blank');
                    newAnswers.push(thisAnswer);
                    break;
                case (oldValue && !currentValue):
                    console.log('(D)elete old value = ' + oldValue + ', new value = blank');
                    thisAnswer.AnswerID = +answerId;
                    oldAnswers.push(thisAnswer);
                    break;
                case ((currentValue && oldValue) && (currentValue !== oldValue)):
                    console.log('(U)pdate old value = ' + oldValue + ', new value = ' + currentValue);
                    thisAnswer.AnswerID = +answerId;
                    updatedAnswers.push(thisAnswer);
                    break;
            }
        });
        $('.spinnerContainer').hide();
        let dialogOptions = {
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
        let postBackModel = new AjaxPostbackModel();
        postBackModel.EpisodeID = episodeID;
        postBackModel.NewAnswers = newAnswers;
        postBackModel.OldAnswers = oldAnswers;
        postBackModel.UpdatedAnswers = updatedAnswers;
        console.log('postBackModel', postBackModel);
        thisPostBtn.attr('disabled', 'false');
        let apiBaseUrl = thisPostBtn.data('apibaseurl');
        let apiController = thisPostBtn.data('controller');
        let thisUrl = apiBaseUrl + '/api/' + apiController;
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
            })
                .done(function (result) {
                thisPostBtn.attr('disabled', 'true');
                $('.spinnerContainer').hide();
                console.log('postback result', result.message);
                console.log('inserted entities', result.insetedEntities);
                dialogOptions.title = 'Success';
                $('#dialog')
                    .text('Data is saved.')
                    .dialog(dialogOptions);
                $('.rehabAction').removeAttr('disabled');
            })
                .fail(function (error) {
                thisPostBtn.attr('disabled', 'false');
                console.log('postback error', error);
                $('.spinnerContainer').hide();
                dialogOptions.title = error.statusText;
                dialogOptions.classes = { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' };
                if (error.statusText == "OK" || error.statusText == "Ok") {
                    $('#dialog')
                        .text('Data is saved.')
                        .dialog(dialogOptions);
                }
                else {
                    $('#dialog')
                        .text('Data is not saved. ' + error.responseText)
                        .dialog(dialogOptions);
                }
            });
        }
        //use fetch api
        function onPost() {
            const url = thisUrl;
            var headers = {};
            fetch(url, {
                method: "POST",
                mode: 'cors',
                headers: headers
            })
                .then((response) => {
                if (!response.ok) {
                    throw new Error(response.text.toString());
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
                dialogOptions.classes = { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' };
                $('#dialog')
                    .text('Data is not saved. ' + error)
                    .dialog(dialogOptions);
            });
        }
    }
    /* private function */
    function validateForm(theForm) {
        theForm.validate();
    }
    /* private function */
    function updateScore(thisControl, newScore) {
        let theScoreEl;
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
    function selfCareScore() {
        let selfCareAdmissionPerformance = 0, selfCareDischargePerformance = 0, selfCarePerformance = 0 /* Interim Performance or Discharge Performance */;
        $('.persistable[id^=GG0130]:not([id*=Discharge_Goal])').each(function () {
            const $this = $(this);
            const $thisID = $(this).prop('id');
            const $thisValue = commonUtility.getControlValue($this);
            switch (true) {
                case ($thisValue >= 7): // greater than 7,9,10,88
                    {
                        updateScore($this, 1);
                        if ($thisID.indexOf('Admission') >= 0)
                            selfCareAdmissionPerformance += 1;
                        else if ($thisID.indexOf('Discharge') >= 0)
                            selfCareDischargePerformance += 1;
                        else
                            selfCarePerformance += 1;
                        break;
                    }
                case ($thisValue > 0 && $thisValue <= 6): // between 1 and 6
                    {
                        updateScore($this, $thisValue);
                        if ($thisID.indexOf('Admission') >= 0)
                            selfCareAdmissionPerformance += $thisValue;
                        else if ($thisID.indexOf('Discharge') >= 0)
                            selfCareDischargePerformance += $thisValue;
                        else
                            selfCarePerformance += $thisValue;
                        break;
                    }
                default:
                    {
                        updateScore($this, 0);
                        break;
                    }
            }
        });
        if ($('#Self_Care_Aggregate_Score_Admission_Performance').length > 0 && $('#Self_Care_Aggregate_Score_Discharge_Performance').length > 0) {
            $('#Self_Care_Aggregate_Score_Admission_Performance').text(selfCareAdmissionPerformance);
            $('#Self_Care_Aggregate_Score_Discharge_Performance').text(selfCareDischargePerformance);
            $('#Self_Care_Aggregate_Score_Total_Change').text(selfCareDischargePerformance - selfCareAdmissionPerformance);
        }
        else {
            /* interim performance or follow up performance */
            $('#Self_Care_Aggregate_Score').text(selfCarePerformance);
        }
    }
    /* private function */
    function mobilityScore() {
        if ($('#Mobility_Aggregate_Score_Admission_Performance').length > 0 && $('#Mobility_Aggregate_Score_Discharge_Performance').length > 0) {
            let mobilityAdmissionPerformance = 0, mobilityDischargePerformance = 0;
            mobilityAdmissionPerformance += Score_GG0170AtoP_Performance();
            mobilityAdmissionPerformance += Score_GG0170RandS_Performance('Base');
            mobilityDischargePerformance += Score_GG0170AtoP_Discharge_Performance();
            mobilityDischargePerformance += Score_GG0170RandS_Discharge_Performance();
            $('#Mobility_Aggregate_Score_Admission_Performance').text(mobilityAdmissionPerformance);
            $('#Mobility_Aggregate_Score_Discharge_Performance').text(mobilityDischargePerformance);
            $('#Mobility_Aggregate_Score_Total_Change').text(mobilityDischargePerformance - mobilityAdmissionPerformance);
        }
        else {
            let mobilityPerformance = 0;
            /* Interim Performance or Follow Up Performance reuse Score_GG0170x_Performance() since the element selector will pickup only GG0170x */
            mobilityPerformance += Score_GG0170AtoP_Performance();
            mobilityPerformance += Score_GG0170RandS_Performance('Interim or Follow Up');
            $('#Mobility_Aggregate_Score').text(mobilityPerformance);
        }
    }
    /* private function */
    function grandTotal() {
        let grandTotal = 0;
        if ($('#Self_Care_Aggregate_Score_Admission_Performance').length > 0 && $('#Mobility_Aggregate_Score_Admission_Performance').length > 0) {
            /* Admission Performance */
            const Self_Care_Admission_Performance = $('#Self_Care_Aggregate_Score_Admission_Performance').text();
            const selfCareAdmissionPerformance = parseInt(Self_Care_Admission_Performance);
            const Mobility_Admission_Performance = $('#Mobility_Aggregate_Score_Admission_Performance').text();
            const mobilityAdmissionPerformance = parseInt(Mobility_Admission_Performance);
            const admissionPerformanceGrandTotal = selfCareAdmissionPerformance + mobilityAdmissionPerformance;
            if ($('#Admission_Performance_Grand_Total').length > 0)
                $('#Admission_Performance_Grand_Total').text(admissionPerformanceGrandTotal);
            /* Discharge Performance */
            const Self_Care_Discharge_Performance = $('#Self_Care_Aggregate_Score_Discharge_Performance').text();
            const selfCareDischargePerformance = parseInt(Self_Care_Discharge_Performance);
            const Mobility_Discharge_Performance = $('#Mobility_Aggregate_Score_Discharge_Performance').text();
            const mobilityDischargePerformance = parseInt(Mobility_Discharge_Performance);
            const dischargePerformanceGrandTotal = selfCareDischargePerformance + mobilityDischargePerformance;
            if ($('#Discharge_Performance_Grand_Total').length > 0)
                $('#Discharge_Performance_Grand_Total').text(dischargePerformanceGrandTotal);
            grandTotal = dischargePerformanceGrandTotal - admissionPerformanceGrandTotal;
        }
        else {
            /* Interim Performance or Follow Up Performance */
            const Self_Care_Aggregate = $('#Self_Care_Aggregate_Score').text();
            const selfCareAggregate = parseInt(Self_Care_Aggregate);
            const Mobility_Aggregate = $('#Mobility_Aggregate_Score').text();
            const mobilityAggregate = parseInt(Mobility_Aggregate);
            grandTotal = selfCareAggregate + mobilityAggregate;
        }
        if ($('#Grand_Total').length > 0)
            $('#Grand_Total').text(grandTotal);
    }
    /* internal function */
    function Score_GG0170AtoP_Performance() {
        let GG0170_AtoP_Performance = 0;
        /* select only GG0170 Admission Performance excluding Q, R and S */
        $('.persistable[id^=GG0170]:not([id*=Discharge_Performance]):not([id*=Discharge_Goal]):not([id*=GG0170Q]):not([id*=GG0170R]):not([id*=GG0170S])').each(function () {
            let $thisControl = $(this);
            let thisControlScore = commonUtility.getControlValue($thisControl);
            let thisControlID = $thisControl.prop('id');
            switch (true) {
                case (thisControlScore >= 7): {
                    if (thisControlID.indexOf('GG0170I') >= 0) {
                        //7,9,10, or 88 don't score per customer 12/8/2021
                        updateScore($thisControl, 0);
                    }
                    else {
                        updateScore($thisControl, 1);
                        GG0170_AtoP_Performance += 1;
                    }
                    break;
                }
                case (thisControlScore > 0 && thisControlScore <= 6): {
                    //btw 1 and 6 add value point 
                    updateScore($thisControl, thisControlScore);
                    GG0170_AtoP_Performance += thisControlScore;
                    break;
                }
                default: {
                    updateScore($thisControl, 0);
                    break;
                }
            }
        });
        return GG0170_AtoP_Performance;
    }
    /* internal function */
    function Score_GG0170RandS_Performance(stage) {
        let multiplier = 1, R_Performance = 0, S_Performance = 0;
        let GG0170I, GG0170R, GG0170S;
        let GG0170I_Value = 0, GG0170R_Value = 0, GG0170S_Value = 0;
        if (stage == 'Base') {
            /* GG0170I determines the multiplier for GG0170R and GG0170S */
            GG0170I = $('.persistable[id^=GG0170I_Admission_Performance]');
            GG0170R = $('.persistable[id^=GG0170R_Admission_Performance]');
            GG0170S = $('.persistable[id^=GG0170S_Admission_Performance]');
        }
        else {
            GG0170I = $('.persistable[id^=GG0170I_Interim_Performance], .persistable[id^=GG0170I_Follow_Up_Performance]');
            GG0170R = $('.persistable[id^=GG0170R_Interim_Performance], .persistable[id^=GG0170R_Follow_Up_Performance]');
            GG0170S = $('.persistable[id^=GG0170S_Interim_Performance], .persistable[id^=GG0170S_Follow_Up_Performance]');
        }
        GG0170I_Value = commonUtility.getControlValue(GG0170I);
        if (GG0170I_Value >= 7)
            multiplier = 2;
        if (GG0170I_Value <= 6)
            multiplier = 0;
        if (isNaN(GG0170I_Value))
            multiplier = 1; //when GG0170I is not answered score R and S as is
        GG0170R_Value = commonUtility.getControlValue(GG0170R);
        if (GG0170R_Value > 0) {
            updateScore(GG0170R, GG0170R_Value * multiplier);
            R_Performance += GG0170R_Value * multiplier;
        }
        else
            updateScore(GG0170R, 0);
        GG0170S_Value = commonUtility.getControlValue(GG0170S);
        if (GG0170S_Value > 0) {
            updateScore(GG0170S, GG0170S_Value * multiplier);
            S_Performance += GG0170S_Value * multiplier;
        }
        else
            updateScore(GG0170S, 0);
        return R_Performance + S_Performance;
    }
    /* internal function */
    function Score_GG0170AtoP_Discharge_Performance() {
        let GG0170_AtoP_Discharge_Performance = 0;
        /* select only GG0170 Discharge Performance excluding Q, R and S */
        $('.persistable[id^=GG0170]:not([id*=Admission_Performance]):not([id*=Discharge_Goal]):not([id*=GG0170Q]):not([id*=GG0170R]):not([id*=GG0170S])').each(function () {
            let $thisControl = $(this);
            let thisControlScore = commonUtility.getControlValue($thisControl);
            let thisControlID = $thisControl.prop('id');
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
    function Score_GG0170RandS_Discharge_Performance() {
        let multiplier = 1, R_Performance = 0, S_Performance = 0;
        /* use GG0170I to determine the multipliers for GG0170R and GG0170S */
        let GG0170I = $('.persistable[id^=GG0170I_Discharge_Performance]');
        let GG0170I_Value = commonUtility.getControlValue(GG0170I);
        if (GG0170I_Value >= 7)
            multiplier = 2;
        if (GG0170I_Value <= 6)
            multiplier = 0;
        if (isNaN(GG0170I_Value))
            multiplier = 1; //when GG0170I is not answered score R and S as is
        let GG0170R = $('.persistable[id^=GG0170R_Discharge_Performance]');
        let GG0170R_Value = commonUtility.getControlValue(GG0170R);
        if (GG0170R_Value > 0) {
            updateScore(GG0170R, GG0170R_Value * multiplier);
            R_Performance += GG0170R_Value * multiplier;
        }
        else
            updateScore(GG0170R, 0);
        let GG0170S = $('.persistable[id^=GG0170S_Discharge_Performance]');
        let GG0170S_Value = commonUtility.getControlValue(GG0170S);
        if (GG0170S_Value > 0) {
            updateScore(GG0170S, GG0170S_Value * multiplier);
            S_Performance += GG0170S_Value * multiplier;
        }
        else
            updateScore(GG0170S, 0);
        return R_Performance + S_Performance;
    }
    /****************************************************************************
     * public functions exposing the private functions to outside of the closure
    ***************************************************************************/
    return {
        'scrollToAnchor': scrollToAnchor,
        'setRehabBtns': setRehabBtns,
        'resetRehabBtns': resetRehabBtns,
        'breakLongSentence': breakLongSentence,
        'submitTheForm': submitTheForm,
        'validate': validateForm,
        'selfCareScore': selfCareScore,
        'mobilityScore': mobilityScore,
        'updateScore': updateScore,
        'grandTotal': grandTotal
    };
})();
//# sourceMappingURL=form.js.map