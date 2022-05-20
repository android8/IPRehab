/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../../node_modules/@types/jqueryui/index.d.ts" />
import { Utility, UserAnswer, AjaxPostbackModel /*, EnumGetControlValueBehavior*/ } from "./commonImport.js";
//https://www.typescriptlang.org/docs/handbook/asp-net-core.html
const commonUtility = new Utility();
/****************************************************************************
 * javaScript closure
 ***************************************************************************/
const formController = (function () {
    let EnumGetControlValueBehavior;
    (function (EnumGetControlValueBehavior) {
        EnumGetControlValueBehavior[EnumGetControlValueBehavior["Elaborated"] = 0] = "Elaborated";
        EnumGetControlValueBehavior[EnumGetControlValueBehavior["Simple"] = 1] = "Simple";
    })(EnumGetControlValueBehavior || (EnumGetControlValueBehavior = {}));
    function scrollTo(thisElement) {
        let scrollAmount = thisElement.prop('offsetTop') + 15; //scroll up further by 15px
        if (thisElement.prop('id').indexOf('Q12') !== -1)
            scrollAmount = 0;
        console.log('scroll to ' + thisElement.prop('id') + ', amount ' + scrollAmount, thisElement);
        $('html,body').animate({ scrollTop: scrollAmount }, 'fast');
        thisElement.focus();
    }
    /* private function */
    function scrollToAnchor(anchorID) {
        const thisElement = $('#' + anchorID);
        //const scrollAmount: number = thisElement.prop('offsetTop');
        //$('html,body').animate({ scrollTop: scrollAmount }, 'fast');
        scrollTo(thisElement);
    }
    /* private function */
    function breakLongSentence(thisSelectElement) {
        //console.log('thisSelectElement', thisSelectElement);
        const maxLength = 50;
        const longTextOptionDIV = thisSelectElement.next('div.longTextOption');
        //console.log('longTextOptionDIV', longTextOptionDIV);
        const thisSelectWidth = thisSelectElement[0].clientWidth;
        const thisScope = thisSelectElement;
        const selectedValue = parseInt(thisSelectElement.prop('value'));
        if (selectedValue <= 0) {
            longTextOptionDIV.text('');
        }
        else {
            $.each($('option:selected', thisScope), function () {
                const $thisOption = $(this);
                const regX = new RegExp("([\\w\\s]{" + (maxLength - 2) + ",}?\\w)\\s?\\b", "g");
                const oldText = $thisOption.text();
                const font = $thisOption.css('font');
                const oldTextInPixel = commonUtility.getTextPixels(oldText, font);
                //console.log('oldTextInPixel', oldTextInPixel);
                //console.log('thisSelectWidth', thisSelectWidth);
                longTextOptionDIV.text('');
                if (oldTextInPixel > thisSelectWidth) {
                    let newStr = oldText.replace(regX, "$1\n");
                    newStr = newStr.trim();
                    const startWithNumber = $.isNumeric(newStr.substring(0, 1));
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
    function submitTheForm(thisPostBtn, dialogOptions) {
        var _a, _b, _c, _d;
        //use jquery ajax
        function jQueryAjax(thisUrl, postBackModel, episodeID) {
            $.ajax({
                type: "POST",
                url: thisUrl,
                data: JSON.stringify(postBackModel),
                headers: {
                    //when post to MVC (not WebAPI) controller, the antiforerytoken must be named 'RequestVerificationToken' in the header
                    'RequestVerificationToken': $('input[name=X-CSRF-TOKEN-IPREHAB]').val().toString(),
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
                $('.spinnerContainer').hide();
                console.log('postback result', result);
                const jsonResult = $.parseJSON(result);
                let dialogText;
                console.log('jsonResult', jsonResult);
                if (episodeID === -1) {
                    /* update the hidden fields in the form, without refreshing the screen and repost it will create duplicate record. */
                    $('#episodeID').val(jsonResult);
                    $('#stage').val('Base');
                    /* update on screen episode_legend and pageTitle */
                    $('#pageTitle').text('Episode of Care');
                    $('#episodeID_legend').text(jsonResult);
                    dialogText = '\Note: When in NEW mord and after the record is saved, refreshing the screen will only show another new form.  To dobule confirm the record just saved, go back to Patient list and select the Episode of Care ID corresponding shown on the upper right of the form.';
                }
                dialogOptions.title = 'Success';
                $('#dialog')
                    .text('Data is saved.' + dialogText)
                    .dialog(dialogOptions);
            })
                .fail(function (error) {
                $('.spinnerContainer').hide();
                thisPostBtn.attr('disabled', 'false');
                console.log('postback error', error);
                dialogOptions.title = error.statusText;
                if (error.statusText === "OK" || error.statusText === "Ok") {
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
        function onPost(thisUrl) {
            const url = thisUrl;
            const headers = {};
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
            })
                .catch(function (error) {
                thisPostBtn.attr('disabled', 'false');
                console.log('postback error', error);
                $('.spinnerContainer').hide();
                dialogOptions.title = error.statusText;
                $('#dialog')
                    .text('Data is not saved. ' + error)
                    .dialog(dialogOptions);
            });
        }
        thisPostBtn.attr('disabled', 'true');
        $('.spinnerContainer').show();
        const oldAnswers = new Array();
        const newAnswers = new Array();
        const updatedAnswers = new Array();
        const theScope = $('#userAnswerForm');
        const stage = (_a = $('#stage', theScope).val()) === null || _a === void 0 ? void 0 : _a.toString();
        const patientID = (_b = $('#patientID', theScope).val()) === null || _b === void 0 ? void 0 : _b.toString();
        const patientName = (_c = $('#patientName', theScope).val()) === null || _c === void 0 ? void 0 : _c.toString();
        let facilityID = (_d = $('#facilityID', theScope).val()) === null || _d === void 0 ? void 0 : _d.toString();
        let episodeID = +($('#episodeID', theScope).val());
        if (stage === 'New')
            episodeID = -1;
        //get the key answers. these must be done outside of the .map()
        //because each answer in .map() will use the same episode onset date and admission date
        const onsetDate = new Date($('.persistable[id^=Q23]').val().toString());
        const admissionDate = new Date($('.persistable[id^=Q12_]').val().toString());
        if (facilityID) {
            const tmp = facilityID.split('(')[2];
            const tmp2pos = tmp.indexOf(')');
            facilityID = tmp.substr(0, tmp2pos);
        }
        //ToDo: make this closure available to other modules to avoid code duplication in commandBtns.ts
        const persistables = $('.persistable');
        let counter = 0;
        persistables.each(function () {
            var _a, _b;
            counter++;
            const $thisPersistable = $(this);
            const questionKey = $thisPersistable.data('questionkey');
            const thisAnswer = new UserAnswer();
            const oldValue = (_a = $thisPersistable.data('oldvalue')) === null || _a === void 0 ? void 0 : _a.toString();
            let currentValue = '';
            //must use 'simple' to get straight val(), otherwise, the getControlValue() use more elaborated way to get the control value
            currentValue = (_b = commonUtility.getControlValue($thisPersistable, EnumGetControlValueBehavior.Simple)) === null || _b === void 0 ? void 0 : _b.toString();
            //!undefined or !NaN yield true
            if (+currentValue === -1)
                currentValue = '';
            if (commonUtility.isTheSame($thisPersistable, oldValue, currentValue)) {
                return;
            }
            console.log('--------- continue (not equal) ---------');
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
                    /* AnswerCodeSetID and OldAnswerCodeSetID don't change for these control types */
                    thisAnswer.OldAnswerCodeSetID = +$thisPersistable.data('codesetid');
                    ;
                    thisAnswer.Description = currentValue;
                    break;
                default: /* dropdown and radio */
                    thisAnswer.AnswerCodeSetID = +currentValue;
                    thisAnswer.OldAnswerCodeSetID = +oldValue;
                    break;
            }
            console.log('(' + counter + ') ' + questionKey, thisAnswer);
            const CRUD = commonUtility.getCRUD($thisPersistable, oldValue, currentValue);
            switch (CRUD) {
                case 'C':
                    newAnswers.push(thisAnswer);
                    break;
                case 'D1':
                case 'D2':
                    thisAnswer.AnswerID = +answerId;
                    oldAnswers.push(thisAnswer);
                    break;
                case 'U':
                    thisAnswer.AnswerID = +answerId;
                    updatedAnswers.push(thisAnswer);
                    break;
            }
        });
        if (newAnswers.length === 0 && oldAnswers.length === 0 && updatedAnswers.length === 0) {
            $('#dialog')
                .text('Nothing to save.  All fields seem be unchanged')
                .dialog(dialogOptions);
        }
        $('.spinnerContainer').hide();
        const postBackModel = new AjaxPostbackModel();
        postBackModel.EpisodeID = episodeID;
        postBackModel.FacilityID = facilityID;
        postBackModel.NewAnswers = newAnswers;
        postBackModel.OldAnswers = oldAnswers;
        postBackModel.UpdatedAnswers = updatedAnswers;
        console.log('postBackModel', postBackModel);
        const apiBaseUrl = thisPostBtn.data('apibaseurl');
        const apiController = thisPostBtn.data('controller');
        let thisUrl;
        if (episodeID === -1) {
            //post to different api when episode === -1
            thisUrl = apiBaseUrl + '/api/' + apiController + '/PostNewEpisode';
        }
        else {
            thisUrl = apiBaseUrl + '/api/' + apiController;
        }
        //thisUrl = $('form').prop('action');
        $('.spinnerContainer').show();
        jQueryAjax(thisUrl, postBackModel, episodeID);
        //onPost(thisUrl);
    }
    /* private function */
    function validateForm(theForm) {
        theForm.validate();
    }
    /* private function */
    function updateScore(thisControl, newScore) {
        const theScoreEl = $(thisControl.siblings('i.score'));
        if (newScore <= 0) {
            if (theScoreEl.length > 0) {
                theScoreEl.remove();
            }
        }
        else {
            if (theScoreEl.length === 0) {
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
        if ($('.scoreSection #self_care_aggregate_score_admission_performance').length > 0 && $('.scoreSection #self_care_aggregate_score_discharge_performance').length > 0) {
            $('.scoreSection #self_care_aggregate_score_admission_performance').text(selfCareAdmissionPerformance);
            $('.scoreSection #self_care_aggregate_score_discharge_performance').text(selfCareDischargePerformance);
            $('.scoreSection #self_care_aggregate_score_total_change').text(selfCareDischargePerformance - selfCareAdmissionPerformance);
            $('#slidingAggregator #self_care_admission_score').text(selfCareAdmissionPerformance);
            $('#slidingAggregator #self_care_discharge_score').text(selfCareDischargePerformance);
            $('#slidingAggregator #self_care_total').text(selfCareDischargePerformance - selfCareAdmissionPerformance);
        }
        else {
            /* interim performance or follow up performance */
            $('.scoreSection #self_care_aggregate_score').text(selfCarePerformance);
            $('#slidingAggregator #self_care_aggregate_score').text(selfCarePerformance);
        }
    }
    /* private function */
    function mobilityScore() {
        if ($('.scoreSection #mobility_aggregate_score_admission_performance').length > 0 && $('.scoreSection #mobility_aggregate_score_discharge_performance').length > 0) {
            let mobilityAdmissionPerformance = 0, mobilityDischargePerformance = 0;
            mobilityAdmissionPerformance += Score_GG0170AtoP_Performance();
            mobilityAdmissionPerformance += Score_GG0170RandS_Performance('Base');
            mobilityDischargePerformance += Score_GG0170AtoP_Discharge_Performance();
            mobilityDischargePerformance += Score_GG0170RandS_Discharge_Performance();
            $('.scoreSection #mobility_aggregate_score_admission_performance').text(mobilityAdmissionPerformance);
            $('.scoreSection #mobility_aggregate_score_discharge_performance').text(mobilityDischargePerformance);
            $('.scoreSection #mobility_aggregate_score_total_change').text(mobilityDischargePerformance - mobilityAdmissionPerformance);
            $('#slidingAggregator #mobility_admission_score').text(mobilityAdmissionPerformance);
            $('#slidingAggregator #mobility_discharge_score').text(mobilityDischargePerformance);
            $('#slidingAggregator #mobility_total').text(mobilityDischargePerformance - mobilityAdmissionPerformance);
        }
        else {
            let mobilityPerformance = 0;
            /* Interim Performance or Follow Up Performance reuse Score_GG0170x_Performance() since the element selector will pickup only GG0170x */
            mobilityPerformance += Score_GG0170AtoP_Performance();
            mobilityPerformance += Score_GG0170RandS_Performance('Interim or Follow Up');
            $('.scoreSection #mobility_aggregate_score').text(mobilityPerformance);
            $('#slidingAggregator #mobility_aggregate_score').text(mobilityPerformance);
        }
    }
    /* private function */
    function grandTotal() {
        let grandTotal = 0;
        if ($('.scoreSection #self_care_aggregate_score_admission_performance').length > 0 && $('.scoreSection #mobility_aggregate_score_admission_performance').length > 0) {
            /* Admission Performance */
            const self_care_admission_performance = $('.scoreSection #self_care_aggregate_score_admission_performance').text();
            const selfCareAdmissionPerformance = parseInt(self_care_admission_performance);
            const mobility_admission_performance = $('.scoreSection #mobility_aggregate_score_admission_performance').text();
            const mobilityAdmissionPerformance = parseInt(mobility_admission_performance);
            const admissionPerformanceGrandTotal = selfCareAdmissionPerformance + mobilityAdmissionPerformance;
            if ($('.scoreSection #admission_performance_grand_total').length > 0)
                $('.scoreSection #admission_performance_grand_total').text(admissionPerformanceGrandTotal);
            if ($('#slidingAggregator #admission_total').length > 0)
                $('#slidingAggregator #admission_total').text(admissionPerformanceGrandTotal);
            /* Discharge Performance */
            const self_care_discharge_performance = $('.scoreSection #self_care_aggregate_score_discharge_performance').text();
            const selfCareDischargePerformance = parseInt(self_care_discharge_performance);
            const mobility_discharge_performance = $('.scoreSection #mobility_aggregate_score_discharge_performance').text();
            const mobilityDischargePerformance = parseInt(mobility_discharge_performance);
            const dischargePerformanceGrandTotal = selfCareDischargePerformance + mobilityDischargePerformance;
            if ($('.scoreSection #discharge_performance_grand_total').length > 0)
                $('.scoreSection #discharge_performance_grand_total').text(dischargePerformanceGrandTotal);
            if ($('#slidingAggregator #discharge_total').length > 0)
                $('#slidingAggregator #discharge_total').text(dischargePerformanceGrandTotal);
            grandTotal = dischargePerformanceGrandTotal - admissionPerformanceGrandTotal;
        }
        else {
            /* Interim Performance or Follow Up Performance */
            const self_care_aggregate = $('.scoreSection #self_care_aggregate_score').text();
            const selfCareAggregate = parseInt(self_care_aggregate);
            const mobility_aggregate = $('.scoreSection #mobility_aggregate_score').text();
            const mobilityAggregate = parseInt(mobility_aggregate);
            grandTotal = selfCareAggregate + mobilityAggregate;
        }
        if ($('.scoreSection #grand_total').length > 0)
            $('.scoreSection #grand_total').text(grandTotal);
        if ($('#slidingAggregator #grand_total').length > 0)
            $('#slidingAggregator #grand_total').text(grandTotal);
    }
    /* internal function */
    function Score_GG0170AtoP_Performance() {
        let GG0170_AtoP_Performance = 0;
        /* select only GG0170 Admission Performance excluding Q, R and S */
        $('.persistable[id^=GG0170]:not([id*=Discharge_Performance]):not([id*=Discharge_Goal]):not([id*=GG0170Q]):not([id*=GG0170R]):not([id*=GG0170S])').each(function () {
            const $thisControl = $(this);
            const thisControlScore = commonUtility.getControlValue($thisControl);
            const thisControlID = $thisControl.prop('id');
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
        if (stage === 'Base') {
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
            const $thisControl = $(this);
            const thisControlScore = commonUtility.getControlValue($thisControl);
            const thisControlID = $thisControl.prop('id');
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
        const GG0170I = $('.persistable[id^=GG0170I_Discharge_Performance]');
        const GG0170I_Value = commonUtility.getControlValue(GG0170I);
        if (GG0170I_Value >= 7)
            multiplier = 2;
        if (GG0170I_Value <= 6)
            multiplier = 0;
        if (isNaN(GG0170I_Value))
            multiplier = 1; //when GG0170I is not answered score R and S as is
        const GG0170R = $('.persistable[id^=GG0170R_Discharge_Performance]');
        const GG0170R_Value = commonUtility.getControlValue(GG0170R);
        if (GG0170R_Value > 0) {
            updateScore(GG0170R, GG0170R_Value * multiplier);
            R_Performance += GG0170R_Value * multiplier;
        }
        else
            updateScore(GG0170R, 0);
        const GG0170S = $('.persistable[id^=GG0170S_Discharge_Performance]');
        const GG0170S_Value = commonUtility.getControlValue(GG0170S);
        if (GG0170S_Value > 0) {
            updateScore(GG0170S, GG0170S_Value * multiplier);
            S_Performance += GG0170S_Value * multiplier;
        }
        else
            updateScore(GG0170S, 0);
        return R_Performance + S_Performance;
    }
    /***************************************************************************
     * public functions exposing the private functions to outside of the closure
    ***************************************************************************/
    return {
        'scrollToAnchor': scrollToAnchor,
        'breakLongSentence': breakLongSentence,
        'submitTheForm': submitTheForm,
        'validate': validateForm,
        'selfCareScore': selfCareScore,
        'mobilityScore': mobilityScore,
        'updateScore': updateScore,
        'grandTotal': grandTotal
    };
})();
/******************************* end of closure ****************************/
$(function () {
    //const dialogOptions =commonUtility.dialogOptions();
    const dialogOptions = {
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
    $('input[type=date]').each(function () {
        const thisDate = $(this);
        thisDate.on('change', function () {
            if (thisDate.val() !== '') {
                const thisDateReset = $('button.calendarReset[data-target=' + thisDate.prop('id') + ']');
                if (thisDateReset.length !== 0) {
                    thisDateReset.prop('disabled', false);
                }
            }
        });
    });
    /* each reset calendar click reset the date of the target sibling */
    $('.calendarReset').on('click', function (e) {
        const thisResetButton = $(this);
        const thisTargetDate = $('#' + thisResetButton.data('target'));
        console.log('reset ' + thisTargetDate.prop('id'), thisTargetDate);
        const isTargetOnQ12B = thisTargetDate.prop('id').indexOf('Q12B') !== -1;
        if (thisTargetDate.length > 0) {
            thisTargetDate.val('');
            //after clear the date, disable the reset button since there is no date to clear
            thisResetButton.prop('disabled', true);
            if (isTargetOnQ12B) {
                console.log('raise change() to let branching.Q12B_blank_then_Lock_Discharge() handle the change() event');
                thisTargetDate.change(); //else use the commonUtility to reset the value
            }
            else {
                console.log('reset ' + thisTargetDate.prop('type') + ' ' + thisTargetDate.prop('id'));
                commonUtility.resetControlValue(thisTargetDate); //raise change() commonUtility handle the event accordingly
            }
        }
        e.preventDefault();
    });
    $('select').each(function () {
        const thisDropdown = $(this);
        thisDropdown.on('change', function () {
            //beak long option text
            //commonUtility.resetControlValue(thisDropdown);
            if (thisDropdown.val() !== -1) {
                formController.breakLongSentence(thisDropdown);
            }
        });
    });
    /* section nav */
    $('#questionTab').hover(function () {
        $('#questionTab').css({ 'left': '0px', 'transition-duration': '1s' });
    }, function () {
        $('#questionTab').css({ 'left': '-245px', 'transition-duration': '1s' });
    });
    /* aggregate scores container */
    $('#rotateSlidingAggregatorHandle').on('click', function () {
        var _a;
        const stage = (_a = $('#stage').val()) === null || _a === void 0 ? void 0 : _a.toString();
        console.log(stage + ' slidingAggregator should slide into view');
        const slidingAggregator = $("#slidingAggregator");
        switch (stage) {
            case "Episode Of Care":
            case "New":
                slidingAggregator.css("width", "13.5em");
                break;
            case 'Interim':
            case 'Follow Up':
                slidingAggregator.css({ "width": "7.2em" });
                break;
        }
        slidingAggregator.css("right", "0em");
    });
    $('#closeSlidingAggregator').on('click', function () {
        const stage = $('#stage').val().toString();
        const slidingAggregator = $("#slidingAggregator");
        console.log(stage + ' slidingAggregator should slide out of view');
        switch (stage) {
            case "Episode Of Care":
            case "New":
                slidingAggregator.css("right", "-13.5em");
                break;
            case 'Interim':
            case 'Follow Up':
                slidingAggregator.css({ "right": "-7.2em" });
                break;
        }
    });
    /* jump to section anchor */
    $('.gotoSection').each(function () {
        const $this = $(this);
        $this.click(function () {
            const anchorID = $this.data("anchorid");
            if (anchorID != '') {
                formController.scrollToAnchor(anchorID);
            }
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
    $('.persistable').on('change', function (e) {
        const Q12 = $('.persistable[id^=Q12_]');
        const Q23 = $('.persistable[id^=Q23_]');
        const Q12_or_Q23_is_empty = commonUtility.isEmpty(Q12) || commonUtility.isEmpty(Q23);
        if (!Q12_or_Q23_is_empty)
            $('#ajaxPost').removeAttr('disabled');
    });
    /* ajax post form */
    $('#ajaxPost').click(function () {
        if (formController.validate) {
            formController.submitTheForm($(this), dialogOptions);
        }
    });
    /* self care score on load */
    formController.selfCareScore();
    /* self care scorce on change */
    $('.persistable[id^=GG0130]:not([id*=Discharge_Goal])').each(function () {
        const $this = $(this);
        $this.on('change', function () {
            formController.selfCareScore();
            formController.grandTotal();
        });
    });
    /* mobility score on load */
    formController.mobilityScore();
    /* mobility score on change */
    $('.persistable[id^=GG0170]:not([id*=Discharge_Goal])').each(function () {
        $(this).on('change', function () {
            formController.mobilityScore();
            formController.grandTotal();
        });
    });
    /* grand total on load */
    formController.grandTotal();
});
//# sourceMappingURL=form.js.map