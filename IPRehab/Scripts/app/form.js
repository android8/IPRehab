/// <reference path="./../../node_modules/@types/jqueryui/index.d.ts" />
import { Utility, UserAnswer, AjaxPostbackModel, EnumDbCommandType } from "./commonImport.js";
import { EpisodeScore } from "./episodeScore.js";
//https://www.typescriptlang.org/docs/handbook/asp-net-core.html
const commonUtility = new Utility();
/****************************************************************************
 * javaScript closure
 ***************************************************************************/
const formController = (function () {
    const formScope = $("form#userAnswerForm");
    const Q12 = $('.persistable[id=Q12]', formScope);
    const Q23 = $('.persistable[id=Q23]', formScope);
    /* initialize controller variables */
    function scrollTo(thisElement) {
        let scrollAmount = thisElement.prop('offsetTop') + 15; //scroll up further by 15px
        if (thisElement.prop('id').indexOf('Q12') !== -1)
            scrollAmount = 0;
        console.log('scroll to ' + thisElement.prop('id') + ', amount ' + scrollAmount, thisElement);
        $('html,body').animate({ scrollTop: scrollAmount }, 'fast');
        thisElement.trigger('focus');
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
        //console.log('breakLongSentence() thisSelectElement', thisSelectElement);
        const maxLength = 50;
        const longTextOptionDIV = thisSelectElement.next('div.longTextOption');
        //console.log('longTextOptionDIV', longTextOptionDIV);
        const thisSelectWidth = thisSelectElement[0].clientWidth;
        const selectedValue = parseInt(thisSelectElement.prop('value'));
        if (selectedValue <= 0) {
            longTextOptionDIV.text('');
        }
        else {
            $.each($('option:selected', thisSelectElement), function () {
                const $thisOption = $(this);
                const oldText = $thisOption.text();
                const font = $thisOption.css('font');
                //console.log('option font', font);
                const oldTextInPixel = commonUtility.getTextPixels(oldText, font);
                if (oldTextInPixel > thisSelectWidth) { //the 50 characters is longer then the option box width
                    //console.log('oldTextInPixel', oldTextInPixel);
                    //console.log('thisSelectWidth', thisSelectWidth);
                    const regX = new RegExp("([\\w\\s]{" + (maxLength - 2) + ",}?\\w)\\s?\\b", "g"); //get first 50 characters
                    let newStr = oldText.replace(regX, "$1\n"); //replace the last space with carriage return
                    //console.log('newStr', newStr);
                    let firstCharacter = newStr.substring(0, 1);
                    const startWithNumber = typeof +firstCharacter == 'number' && !Number.isNaN(+firstCharacter); //ensure the 1 character is numeric
                    if (startWithNumber) {
                        newStr = newStr.trim().substring(3); //if the 1st character is a number, then take the remaining string starting the 1st non-space character
                        //console.log('newStr after 1st numeric character = ', newStr);
                    }
                    //console.log('old ->', oldText);
                    //console.log('new ->', newStr);
                    longTextOptionDIV.text(newStr).removeClass(["invisible"]);
                }
                else {
                    //console.log('reset ' + thisSelectElement.prop("id") + ' sibling EL longTextOptionDIV for short text.');
                    longTextOptionDIV.text('');
                }
            });
        }
    }
    /* private function */
    function submitTheForm(saveBtn, dialogOptions) {
        var _a, _b, _c, _d;
        //use jquery ajax
        function jQueryAjax(thisUrl, postBackModel, episodeID) {
            /* refresh screen */
            function reloadPageAfterPost() {
                let currentUrl = window.location.href;
                //let currentUrl: string = window.location.href;
                //let newUrl = currentUrl.replace("stage=New", "stage=Base");
                /* update url. changing window.location will cause navigate to the new url */
                //let currentUrl: string = window.location.href;
                //let newUrl = currentUrl.replace("stage=New", "stage=Base");
                //window.history.replaceState({}, "", newUrl);
                const host = window.location.host;
                const pathName = window.location.pathname;
                const newEpisodeId = $('#episodeID').val();
                const queryParms = window.location.search.split('&');
                let admitDate;
                queryParms.forEach(function (keyValuePair) {
                    const thisPair = keyValuePair.split('=');
                    const thisKey = thisPair[0];
                    let thisValue;
                    if (thisKey.toLowerCase() === 'admitdate') {
                        thisValue = thisPair[1];
                        admitDate = thisValue;
                        return false;
                    }
                });
                $('.spinnerContainer').show();
                window.location.href = pathName + '?stage=Base&episodeid=' + newEpisodeId + '&pageNumber=0&admitDate=' + admitDate;
            }
            $.ajax({
                type: "POST",
                url: thisUrl,
                data: JSON.stringify(postBackModel),
                headers: {
                    //when post to MVC (not WebAPI) controller, the antiforerytoken must be named 'RequestVerificationToken' in the header
                    'RequestVerificationToken': $('input[name=X-CSRF-TOKEN-IPREHAB]').val().toString(),
                    'Accept': 'application/json',
                    'Access-Control-Allow-Origin': '*'
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
                const jsonResult = JSON.parse(result);
                console.log('jsonResult after ajax().done', jsonResult);
                console.log('without iterating changed elements, remove changed styles');
                $('.changedFlag, .Create, .Update, .Delete').removeClass(['changedFlag', 'Create', 'Update', 'Delete']);
                if ($('.Create,.Update,.Delete,.changedFlag').length === 0) {
                    console.log('disable the SAVE buton');
                    saveBtn.prop('disabled', true);
                }
                let dialogText = 'Data is saved (Done).';
                dialogText += ' The screen will fresh automatically.';
                //ToDo: update the hidden fields in the form, without refreshing the screen and repost it will create duplicate record.
                $('#episodeID').val(jsonResult);
                $('#stage').val('Base');
                /* update on screen episode_legend and pageTitle */
                $('#pageTitle').text('Episode of Care');
                $('#episodeID_legend').text(jsonResult);
                dialogOptions.title = 'Success';
                $('#dialog').text(dialogText).dialog(dialogOptions);
                reloadPageAfterPost();
            })
                .fail(function (error) {
                $('.spinnerContainer').hide();
                console.log('postback error', error);
                dialogOptions.title = error.statusText;
                //false failure
                if (error.statusText.toUpperCase() === "OK") {
                    $('.changedFlag, .Create, .Update, .Delete').removeClass(['changedFlag', 'Create', 'Update', 'Delete']);
                    $('#dialog')
                        .text('Data is saved (' + error.statusText.toUpperCase() + ').')
                        .dialog(dialogOptions);
                    saveBtn.prop('disabled', true);
                    reloadPageAfterPost();
                }
                else {
                    //true failure
                    $('#dialog')
                        .text('Data is not saved. ' + error.responseText)
                        .dialog(dialogOptions);
                    saveBtn.prop('disabled', false);
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
                saveBtn.attr('disabled', 'false');
                console.log('postback error', error);
                $('.spinnerContainer').hide();
                dialogOptions.title = error.statusText;
                $('#dialog')
                    .text('Data is not saved. ' + error)
                    .dialog(dialogOptions);
            });
        }
        $('.spinnerContainer').show();
        const deleteAnswers = new Array();
        const insertAnswers = new Array();
        const updateAnswers = new Array();
        const episodeScores = new Array();
        const theScope = $('#userAnswerForm');
        //from hidden form variables
        const stage = (_a = $('#stage', theScope).val()) === null || _a === void 0 ? void 0 : _a.toString();
        const patientID = (_b = $('#patientID', theScope).val()) === null || _b === void 0 ? void 0 : _b.toString();
        const patientName = (_c = $('#patientName', theScope).val()) === null || _c === void 0 ? void 0 : _c.toString();
        let facilityID = (_d = $('#facilityID', theScope).val()) === null || _d === void 0 ? void 0 : _d.toString();
        let episodeID = +($('#episodeID', theScope).val());
        //get the key answers. these must be done outside of the .map()
        //because each answer in .map() will use the same episode onset date and admission date
        console.log('facilityID', facilityID);
        const facilityIdPattern = /\d+\w+/g;
        facilityID = facilityID.match(facilityIdPattern).join('');
        console.log("facility ID after regex match=", facilityID);
        //if (facilityID && facilityID.indexOf('(') !== -1) {
        //    const tmp = facilityID.split('(')[1];
        //    const tmp2pos = tmp.indexOf(')');
        //    facilityID = tmp.substring(0, tmp2pos);
        //}
        //ToDo: make this closure available to other modules to avoid code duplication in commandBtns.ts
        const persistables = $('.persistable', theScope);
        let answerCounter = 0;
        persistables.each(function () {
            answerCounter++;
            const thisPersistable = $(this);
            const thisPersistableID = thisPersistable.prop('id');
            if (thisPersistableID == null || thisPersistableID == undefined || thisPersistableID == '') {
                console.log('element has no id', thisPersistable);
                return;
            }
            const thisAnswer = new UserAnswer();
            const questionKey = thisPersistable.data('questionkey'); //get value from data- attribute
            const oldAnswer = thisPersistable.data('oldvalue');
            const currentAnswer = commonUtility.getControlCurrentValue(thisPersistable);
            const CRUD = commonUtility.getCRUD(thisPersistable, oldAnswer, currentAnswer);
            if (CRUD === EnumDbCommandType.Unchanged) {
                return;
            }
            console.log('--------- old and new values not equal ---------');
            const controlType = thisPersistable.prop('type');
            const questionId = +thisPersistable.data('questionid');
            const answerId = thisPersistable.data('answerid');
            const measureId = +thisPersistable.data('measureid');
            const measureDescription = thisPersistable.data('measuredescription');
            const codesetDescription = thisPersistable.data('codesetdescription');
            const answerSequence = +thisPersistable.data('answersequence');
            const AnswerByUserID = thisPersistable.data('userid');
            thisAnswer.PatientName = patientName;
            thisAnswer.PatientID = patientID;
            thisAnswer.EpisodeID = episodeID;
            thisAnswer.OnsetDate = new Date(Q23.val());
            ;
            thisAnswer.AdmissionDate = new Date(Q12.val());
            thisAnswer.QuestionID = questionId;
            thisAnswer.QuestionKey = questionKey;
            thisAnswer.MeasureID = measureId;
            thisAnswer.MeasureName = measureDescription;
            thisAnswer.AnswerCodeSetDescription = codesetDescription;
            if (answerSequence)
                thisAnswer.AnswerSequenceNumber = answerSequence;
            thisAnswer.AnswerByUserID = AnswerByUserID;
            thisAnswer.LastUpdate = new Date();
            switch (controlType) {
                case 'text':
                case 'date':
                case 'textarea':
                case 'number':
                    /* these input types have the codeset id is always 92. */
                    thisAnswer.AnswerCodeSetID = +thisPersistable.data('codesetid');
                    thisAnswer.OldAnswerCodeSetID = +thisPersistable.data('codesetid');
                    ;
                    thisAnswer.Description = currentAnswer; /* The current answer value is from answer desription column pre-populated in the WebAPI */
                    break;
                default: /* dropdown, radio, checkbox current value is the the code set ID*/
                    thisAnswer.AnswerCodeSetID = +currentAnswer;
                    thisAnswer.OldAnswerCodeSetID = +oldAnswer;
                    break;
            }
            /* question score */
            if (/GG0130/i.test(thisPersistableID) || /GG0170/i.test(thisPersistableID)) //regex
             {
                const thisAnswerScoreElement = $("i[id*=" + thisPersistableID + "]");
                if (thisAnswerScoreElement.length > 0) {
                    let thisAnswerScore = +commonUtility.getControlScore(thisAnswerScoreElement); //parseInt(thisAnswerScoreElement.data('score'));
                    thisAnswer.Score = thisAnswerScore;
                }
            }
            switch (CRUD) {
                case EnumDbCommandType.Create:
                    //console.log('Insert (' + thisPersistableID + ')', thisAnswer);
                    insertAnswers.push(thisAnswer);
                    break;
                case EnumDbCommandType.Delete:
                    //console.log('Delete (' + thisPersistableID + ')', thisAnswer);
                    thisAnswer.AnswerID = +answerId;
                    deleteAnswers.push(thisAnswer);
                    break;
                case EnumDbCommandType.Update:
                    //console.log('Update (' + thisPersistableID + ')', thisAnswer);
                    thisAnswer.AnswerID = +answerId;
                    updateAnswers.push(thisAnswer);
                    break;
            }
            /* aggregate score */
            let self_care_admission_score, self_care_discharge_score, mobility_admission_score, mobility_discharge_score;
            self_care_admission_score = new EpisodeScore();
            self_care_admission_score.Measure = "self_care_admission_score";
            self_care_admission_score.Description = "self_care_admission_score";
            self_care_admission_score.Score = parseInt($('#self_care_admission_score').text());
            self_care_discharge_score = new EpisodeScore();
            self_care_discharge_score.Measure = "self_care_discharge_score";
            self_care_discharge_score.Description = "self_care_discharge_score";
            self_care_discharge_score.Score = parseInt($('#self_care_discharge_score').text());
            mobility_admission_score = new EpisodeScore();
            mobility_admission_score.Measure = "mobility_admission_score";
            mobility_admission_score.Description = "mobility_admission_score";
            mobility_admission_score.Score = parseInt($('#mobility_admission_score').text());
            mobility_discharge_score = new EpisodeScore();
            mobility_discharge_score.Measure = "mobility_discharge_score";
            mobility_discharge_score.Description = "mobility_discharge_score";
            mobility_discharge_score.Score = parseInt($('#mobility_discharge_score').text());
            episodeScores.push(self_care_admission_score);
            episodeScores.push(self_care_discharge_score);
            episodeScores.push(mobility_admission_score);
            episodeScores.push(mobility_discharge_score);
        });
        if (insertAnswers.length === 0 && deleteAnswers.length === 0 && updateAnswers.length === 0) {
            $('#dialog')
                .text('Nothing to save.  All fields seem be unchanged')
                .dialog(dialogOptions);
        }
        $('.spinnerContainer').hide();
        const postBackModel = new AjaxPostbackModel();
        postBackModel.EpisodeID = episodeID;
        postBackModel.FacilityID = facilityID;
        postBackModel.InsertAnswers = insertAnswers;
        postBackModel.DeleteAnswers = deleteAnswers;
        postBackModel.UpdateAnswers = updateAnswers;
        postBackModel.EpisodeScores = episodeScores;
        console.log('postBackModel', postBackModel);
        const apiBaseUrl = saveBtn.data('apibaseurl');
        const apiController = saveBtn.data('controller');
        let thisUrl;
        if (episodeID <= 0) {
            //post to different api when episode === -1
            thisUrl = apiBaseUrl + '/api/' + apiController + '/PostNewEpisode';
        }
        else {
            thisUrl = apiBaseUrl + '/api/' + apiController;
        }
        //thisUrl = $('form').prop('action');
        jQueryAjax(thisUrl, postBackModel, episodeID);
        //onPost(thisUrl);
    }
    /* private function */
    function validateForm(theForm) {
        theForm.validate();
    }
    /* private function */
    /* update full text display below the selected option */
    function updateScore(thisControl, newScore, scoreMsg = '') {
        console.log('thisControl (' + thisControl.prop('id') + ') = ' + newScore);
        const i_score_element = thisControl.siblings('i.score');
        switch (true) {
            case ((newScore <= 0 || isNaN(newScore)) && i_score_element.length > 0): {
                console.log('path1: remove the score');
                i_score_element.remove();
                break;
            }
            case (newScore > 0 && i_score_element.length === 0): {
                console.log('path2: append the score');
                thisControl.parent().closest('div').append("<i id='" + thisControl.prop('id') + "_score' class='persistable score' data-score='" + newScore + "'>score: " + newScore + " " + scoreMsg + "</i>");
                break;
            }
            case (newScore > 0 && i_score_element.length > 0): {
                console.log('path3: update existing score');
                i_score_element.text('score: ' + newScore + " " + scoreMsg);
                break;
            }
        }
        //if (newScore <= 0 || isNaN(newScore)) {
        //    if (theScoreEl.length > 0) {
        //        console.log('path1')
        //        theScoreEl.remove();
        //    }
        //}
        //else {
        //    if (theScoreEl.length === 0) {
        //        {
        //            console.log('path2');
        //            thisControl.parent().closest('div').append("<i class='score'>score: " + newScore + "<i>");
        //        }
        //    }
        //    else {
        //        console.log('path3');
        //        theScoreEl.text('score: ' + newScore);
        //    }
        //}
    }
    /* private function */
    /* calculate individual score of each Self Care question and Score Card summary */
    function selfCareScore() {
        let selfCareAdmissionPerformance = 0, selfCareDischargePerformance = 0, selfCarePerformance = 0 /* Interim Performance or Discharge Performance */;
        console.log('----- update self-care scores -----');
        $('.persistable[id^=GG0130]:not([id*=Discharge_Goal])').each(function () {
            const thisControl = $(this);
            const thisID = $(this).prop('id');
            const dataAttributeScore = +commonUtility.getControlScore(thisControl); //$('option:selected', thisControl).data('score')
            switch (true) {
                case (dataAttributeScore >= 7): // greater than 7,9,10,88
                    {
                        const adjustedScore = 1;
                        updateScore(thisControl, adjustedScore);
                        if (thisID.indexOf('Admission') >= 0)
                            selfCareAdmissionPerformance += adjustedScore;
                        else if (thisID.indexOf('Discharge') >= 0)
                            selfCareDischargePerformance += adjustedScore;
                        else
                            selfCarePerformance += adjustedScore;
                        break;
                    }
                case (dataAttributeScore > 0 && dataAttributeScore <= 6): // between 1 and 6
                    {
                        updateScore(thisControl, dataAttributeScore);
                        if (thisID.indexOf('Admission') >= 0)
                            selfCareAdmissionPerformance += dataAttributeScore;
                        else if (thisID.indexOf('Discharge') >= 0)
                            selfCareDischargePerformance += dataAttributeScore;
                        else
                            selfCarePerformance += dataAttributeScore;
                        break;
                    }
                default:
                    {
                        const scoreZero = 0;
                        updateScore(thisControl, scoreZero);
                        break;
                    }
            }
        });
        if ($('.scoreSection #self_care_aggregate_score_admission_performance').length > 0 && $('.scoreSection #self_care_aggregate_score_discharge_performance').length > 0) {
            /* update self care section agggregate score */
            $('.scoreSection #self_care_aggregate_score_admission_performance').text(selfCareAdmissionPerformance);
            $('.scoreSection #self_care_aggregate_score_discharge_performance').text(selfCareDischargePerformance);
            $('.scoreSection #self_care_aggregate_score_total_change').text(selfCareDischargePerformance - selfCareAdmissionPerformance);
            /* update sliding score care agggregate score */
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
    /* calculate individual score of each Mobility question and Score Card summary */
    function mobilityScore() {
        switch (true) {
            case $('.scoreSection #mobility_aggregate_score_admission_performance').length > 0:
            case $('.scoreSection #mobility_aggregate_score_discharge_performance').length > 0:
                {
                    let mobilityAdmissionPerformance = 0, mobilityDischargePerformance = 0;
                    mobilityAdmissionPerformance += Score_GG0170AtoP_Performance('Admission_Performance');
                    mobilityAdmissionPerformance += Score_GG0170RandS_Performance('Admission_Performance');
                    console.log('aggregate Mobility Admission Performance score = ' + mobilityAdmissionPerformance);
                    mobilityDischargePerformance += Score_GG0170AtoP_Performance('Discharge_Performance');
                    mobilityDischargePerformance += Score_GG0170RandS_Performance('Discharge_Performance');
                    console.log('aggregate Mobility Discharge Performance score = ' + mobilityDischargePerformance);
                    /* update scores in the question section */
                    $('.scoreSection #mobility_aggregate_score_admission_performance').text(mobilityAdmissionPerformance);
                    $('.scoreSection #mobility_aggregate_score_discharge_performance').text(mobilityDischargePerformance);
                    $('.scoreSection #mobility_aggregate_score_total_change').text(mobilityDischargePerformance - mobilityAdmissionPerformance);
                    /* update scores in the sliding scorecard */
                    $('#slidingAggregator #mobility_admission_score').text(mobilityAdmissionPerformance);
                    $('#slidingAggregator #mobility_discharge_score').text(mobilityDischargePerformance);
                    $('#slidingAggregator #mobility_total').text(mobilityDischargePerformance - mobilityAdmissionPerformance);
                }
                break;
            case $('.persistable[id*=Interim]').length > 0: {
                let mobilityInterminPerformance = 0;
                /* Interim Performance or Follow Up Performance reuse Score_GG0170x_Performance() since the element selector will pickup only GG0170x */
                mobilityInterminPerformance += Score_GG0170AtoP_Performance('Interim_Performance');
                mobilityInterminPerformance += Score_GG0170RandS_Performance('Interim_Performance');
                console.log('aggregate Mobility Interim Performance score = ' + mobilityInterminPerformance);
                /* update Interim or Follow Up performance scores in the question section and the slideing scorecard */
                $('.scoreSection #mobility_aggregate_score').text(mobilityInterminPerformance);
                $('#slidingAggregator #mobility_aggregate_score').text(mobilityInterminPerformance);
                break;
            }
            case $('.persistable[id*=Follow_Up]').length > 0: {
                let mobilityFollowUpPerformance = 0;
                mobilityFollowUpPerformance += Score_GG0170AtoP_Performance('Follow_Up_Performance');
                mobilityFollowUpPerformance += Score_GG0170RandS_Performance('Follow_Up_Performance');
                console.log('aggregate Mobility Follow Up Performance score = ' + mobilityFollowUpPerformance);
                /* update Follow Up performance scores in the question section and the slideing scorecard */
                $('.scoreSection #mobility_aggregate_score').text(mobilityFollowUpPerformance);
                $('#slidingAggregator #mobility_aggregate_score').text(mobilityFollowUpPerformance);
                break;
            }
        }
        //    if ($('.scoreSection #mobility_aggregate_score_admission_performance').length > 0 && $('.scoreSection #mobility_aggregate_score_discharge_performance').length > 0) {
        //        let mobilityAdmissionPerformance = 0, mobilityDischargePerformance = 0;
        //        mobilityAdmissionPerformance += Score_GG0170AtoP_Performance('Admission_Performance');
        //        mobilityAdmissionPerformance += Score_GG0170RandS_Performance('Admission_Performance');
        //        console.log('mobilityPerformance (Admission) = ' + mobilityAdmissionPerformance);
        //        mobilityDischargePerformance += Score_GG0170AtoP_Performance('Discharge_Performance');
        //        mobilityDischargePerformance += Score_GG0170RandS_Performance('Discharge_Performance');
        //        console.log('mobilityPerformance (Discharge) = ' + mobilityDischargePerformance);
        //        /* update Admission and Discharge performance scores in the question section */
        //        $('.scoreSection #mobility_aggregate_score_admission_performance').text(mobilityAdmissionPerformance);
        //        $('.scoreSection #mobility_aggregate_score_discharge_performance').text(mobilityDischargePerformance);
        //        $('.scoreSection #mobility_aggregate_score_total_change').text(mobilityDischargePerformance - mobilityAdmissionPerformance);
        //        /* update Admission and Discharge performance scores in the sliding scorecard */
        //        $('#slidingAggregator #mobility_admission_score').text(mobilityAdmissionPerformance);
        //        $('#slidingAggregator #mobility_discharge_score').text(mobilityDischargePerformance);
        //        $('#slidingAggregator #mobility_total').text(mobilityDischargePerformance - mobilityAdmissionPerformance);
        //    }
        //    else {
        //        let mobilityPerformance = 0;
        //        const isInterim: boolean = $('.persistable[id*=Interim]').length > 0;
        //        const isFollowup: boolean = $('.persistable[id*=Followup]').length > 0;
        //        if (isInterim) {
        //            /* Interim Performance or Follow Up Performance reuse Score_GG0170x_Performance() since the element selector will pickup only GG0170x */
        //            mobilityPerformance += Score_GG0170AtoP_Performance('Interim_Performance');
        //            mobilityPerformance += Score_GG0170RandS_Performance('Interim_Performance');
        //            console.log('mobilityPerformance (Interim) = ' + mobilityPerformance);
        //        }
        //        if (isFollowup) {
        //            mobilityPerformance += Score_GG0170AtoP_Performance('Followup_Performance');
        //            mobilityPerformance += Score_GG0170RandS_Performance('Followup_Performance');
        //            console.log('mobilityPerformance (Follow Up) = ' + mobilityPerformance);
        //        }
        //        /* update Interim or Follow Up performance scores in the question section and the slideing scorecard */
        //        $('.scoreSection #mobility_aggregate_score').text(mobilityPerformance);
        //        $('#slidingAggregator #mobility_aggregate_score').text(mobilityPerformance);
        //    }
    }
    /* private function */
    /* calculate the aggregate totals of Mobility and Self Care questions and score card */
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
    /* private function */
    function addMoreDate(stage) {
        console.log('branching::: inside of AddMore');
        const addMoreBtns = $('button[id^=btnMore]');
        addMoreBtns.on('click', function () {
            const thisMoreBtn = $(this);
            const questionKey = thisMoreBtn.data('questionkey');
            const lastInputIdx = $('.persistable[id^=' + questionKey + '][id*=' + stage + ']', formScope).length - 1;
            const lastInputDate = $('.persistable[id^=' + questionKey + '][id*=' + stage + '_' + lastInputIdx + ']', formScope);
            const dateClone = lastInputDate.clone();
            //commonUtility.resetControlValue(dateClone);
            dateClone.val('').trigger('focus');
            lastInputDate.append(dateClone);
        });
    }
    /* private function */
    function clear_long_text_and_score() {
        const longTextOptions = $('.longTextOption');
        longTextOptions.each(function () {
            const thisLongText = $(this);
            const siblingDropdown = thisLongText.prev('select');
            const siblingScore = thisLongText.next('i.score');
            if (+siblingDropdown.val() === -1) {
                thisLongText.text('');
                siblingScore.text('');
            }
        });
    }
    function changeCssClassIfOldAndNewValuesAreDifferent() {
        $('input.persistable, select.persistable').not(Q12).each(function () {
            const thisPersistable = $(this);
            //don't use on(change) to add the class because it is triggered by Q23 change chain on load 
            thisPersistable.on('change', function () {
                const controlType = thisPersistable.prop('type');
                const controlId = thisPersistable.prop('id');
                const thisControlLabel = $('#' + controlId + '_label');
                const Q12_or_Q23_is_empty = commonUtility.isEmpty(Q12) || commonUtility.isEmpty(Q23);
                const currentAnswer = commonUtility.getControlCurrentValue(thisPersistable);
                const oldAnswer = thisPersistable.data('oldvalue');
                const changeType = commonUtility.getCRUD(thisPersistable, oldAnswer, currentAnswer);
                const admitDate = new Date(commonUtility.getControlCurrentValue(Q12));
                const onsetDate = new Date(commonUtility.getControlCurrentValue(Q23));
                const minDate = new Date('2020-01-01 00:00:00');
                const is_onset_on_or_later_than_admit_and_valid = onsetDate > minDate && admitDate > minDate && admitDate >= onsetDate;
                const noMoreChangeCssClass = $('.persistable.changedFlag, .persistable.Create, .persistable.Update, .persistable.Delete').length === 0;
                const saveButton = $('#saveButton');
                console.log('Q12_or_Q23_is_empty = ' + Q12_or_Q23_is_empty);
                console.log('is_onset_on_or_later_than_admit_and_valid = ' + is_onset_on_or_later_than_admit_and_valid);
                console.log(thisPersistable.prop('id') + ' changeType = ' + EnumDbCommandType[changeType]);
                //if (Q12_and_Q23_is_not_empty && is_onset_on_or_later_than_admit && (EnumDbCommandType[changeType] !== EnumDbCommandType[EnumDbCommandType.Unchanged])) {
                if (changeType === EnumDbCommandType.Unchanged) {
                    console.log('remove radio ' + controlId + ' change css style');
                    thisPersistable.removeClass(['changedFlag', 'Create', 'Update', 'Delete']);
                    switch (controlType) {
                        case 'radio':
                        case 'checkbox':
                            console.log('remove ' + controlId + ' label ' + thisControlLabel.prop('id') + ' change css style');
                            thisControlLabel.removeClass(['changedFlag', 'Create', 'Update', 'Delete']);
                            break;
                    }
                }
                else {
                    //const deltaSVG = '<span class="changedFlag">' +
                    //    '<svg xmlns="http://www.w3.org/2000/svg" viewBox = "0 0 512 512">' +
                    //    '<!--!Font Awesome Free 6.6.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.-->' +
                    //    '<path d="M256 32c14.2 0 27.3 7.5 34.5 19.8l216 368c7.3 12.4 7.3 27.7 .2 40.1S486.3 480 472 480L40 480c-14.3 0-27.6-7.7-34.7-20.1s-7-27.8 .2-40.1l216-368C228.7 39.5 241.8 32 256 32zm0 128c-13.3 0-24 10.7-24 24l0 112c0 13.3 10.7 24 24 24s24-10.7 24-24l0-112c0-13.3-10.7-24-24-24zm32 224a32 32 0 1 0 -64 0 32 32 0 1 0 64 0z" /> </svg>' +
                    //    '</span>'
                    //thisPersistable.parent().prepend(deltaSVG);
                    console.log('add radio ' + controlId + ' change css style');
                    thisPersistable.removeClass(['changedFlag', 'Create', 'Update', 'Delete']);
                    thisPersistable.addClass(['changedFlag', EnumDbCommandType[changeType]]);
                    switch (controlType) {
                        case 'radio':
                        case 'checkbox':
                            console.log('add ' + controlId + ' label ' + thisControlLabel.prop('id') + ' change css style');
                            thisControlLabel.addClass(['changedFlag']);
                            break;
                    }
                }
                //remove mutually exclusive radios change css style
                if (controlType === 'radio') {
                    const thisRadioContainer = thisPersistable.closest('div.radioContainer');
                    const mutuallyExclusiveRadios = $('[data-questionkey=' + thisPersistable.attr('data-questionkey') + ']', thisRadioContainer).not(thisPersistable);
                    mutuallyExclusiveRadios.each(function () {
                        const thisMutuallyExclusiveRadio = $(this);
                        const thisMutuallyExclusiveRadioLabel = $('#' + thisMutuallyExclusiveRadio.prop('id') + '_label');
                        console.log('remove mutually exclusive radio ' + thisMutuallyExclusiveRadioLabel.prop('id') + ' change css style');
                        thisMutuallyExclusiveRadio.removeClass(['changedFlag', 'Create', 'Update', 'Delete']);
                        console.log('remove mutually exclusive radio label ' + thisMutuallyExclusiveRadioLabel.prop('id') + ' change css style');
                        thisMutuallyExclusiveRadioLabel.removeClass(['changedFlag', 'Create', 'Update', 'Delete']);
                    });
                }
                //when no controls with CRUD classes, disable the SAVE button
                if (noMoreChangeCssClass || Q12_or_Q23_is_empty || !is_onset_on_or_later_than_admit_and_valid) {
                    //console.log('no more change, disable SAVE button');
                    saveButton.prop('disabled', true);
                }
                else
                    //console.log('enable the SAVE button by ' + thisPersistable.prop('id') + ' change');
                    saveButton.prop('disabled', false);
            });
        });
    }
    /* internal function */
    //function Score_GG0170AtoP_Performance_old(): number {
    //  let GG0170_AtoP_Performance = 0;
    //  /* select only GG0170 Admission Performance excluding Q, R and S */
    //  $('.persistable[id^=GG0170]:not([id*=Discharge_Performance]):not([id*=Discharge_Goal]):not([id*=GG0170Q]):not([id*=GG0170R]):not([id*=GG0170S])').each(function () {
    //    const $thisControl = $(this);
    //    const thisControlScore: number = +commonUtility.getControlScore($thisControl);     //$('option:selected', $thisControl).data('score');
    //    const thisControlID: string = $thisControl.prop('id');
    //    switch (true) {
    //      case (thisControlScore >= 7): {
    //        if (thisControlID.indexOf('GG0170I') >= 0) {
    //          //7,9,10, or 88 don't score per customer 12/8/2021
    //          updateScore($thisControl, 0);
    //        }
    //        else {
    //          updateScore($thisControl, 1);
    //          GG0170_AtoP_Performance += 1;
    //        }
    //        break;
    //      }
    //      case (thisControlScore > 0 && thisControlScore <= 6): {
    //        //btw 1 and 6 add value point
    //        updateScore($thisControl, thisControlScore);
    //        GG0170_AtoP_Performance += thisControlScore;
    //        break;
    //      }
    //      default: {
    //        updateScore($thisControl, 0);
    //        break;
    //      }
    //    }
    //  });
    //  return GG0170_AtoP_Performance;
    //}
    /* internal function */
    /* calculate only matching GG0170 performance type excluding Q, R and S */
    function Score_GG0170AtoP_Performance(performanceType) {
        /* select only GG0170 matching the performance type parameter excluding Q, R and S */
        const targetELs = $('.persistable[id^=GG0170][id*=' + performanceType + ']:not([id*=GG0170Q]):not([id*=GG0170R]):not([id*=GG0170S])');
        console.log('targetELs', targetELs);
        let PerformanceScore = 0;
        console.log('----- update Mobility score -----');
        targetELs.each(function () {
            const thisEL = $(this);
            const thisControl_id = thisEL.prop('id');
            const thisControl_Score = +commonUtility.getControlScore(thisEL); //$('option:selected', thisEL).data('score'); 
            const isThisGG0170I = thisControl_id.indexOf('GG0170I_' + performanceType) >= 0;
            const isThisGG0170M = thisControl_id.indexOf('GG0170M_' + performanceType) >= 0;
            const isThisGG0170N = thisControl_id.indexOf('GG0170N_' + performanceType) >= 0;
            console.log(thisControl_id + ' Mobility score = ', thisControl_Score);
            switch (true) {
                case (+thisControl_Score >= 7):
                    {
                        if (isThisGG0170I) {
                            console.log('\t Score_GG0170AtoP_Performance(): ' + thisControl_id + ' value >= 7 path 1');
                            updateScore(thisEL, 0); //don't count GG0170I when I >= 7 per customer 12/8/2021
                        }
                        else if (isThisGG0170M) {
                            console.log('\t Score_GG0170AtoP_Performance(): ' + thisControl_id + ' value >= 7 path 2');
                            updateScore(thisEL, 3, ' (1 each for M, N and O)');
                            /* when M >= 7 add 1 point for M, N, and O each*/
                            PerformanceScore += 3;
                            const thisGG0170N = $('#GG0170N' + performanceType);
                            updateScore(thisGG0170N, 0);
                            const thisGG0170O = $('#GG0170O' + performanceType);
                            updateScore(thisGG0170O, 0);
                        }
                        else if (isThisGG0170N) {
                            console.log('\t Score_GG0170AtoP_Performance::: ' + thisControl_id + ' value >= 7 path 3');
                            updateScore(thisEL, 2, ' (1 each for N and O)');
                            /* when N >= 7 add 1 point for N and O each*/
                            PerformanceScore += 2;
                            const thisGG0170O = $('#GG0170O' + performanceType);
                            updateScore(thisGG0170O, 0);
                        }
                        else {
                            console.log('\t Score_GG0170AtoP_Performance::: ' + thisControl_id + ' value >= 7 path 4');
                            updateScore(thisEL, 1);
                            PerformanceScore += 1;
                        }
                    }
                    break;
                case +thisControl_Score > 0 && +thisControl_Score < 7:
                    console.log('\t Score_GG0170AtoP_Performance::: ' + thisControl_id + ' value < 7 ');
                    PerformanceScore += thisControl_Score;
                    updateScore(thisEL, thisControl_Score);
                    if (isThisGG0170I) {
                        //set R and S value to 0 when 0 < I < 7
                        const GG0170R = $('.persistable[id^=GG0170R_' + performanceType + ']');
                        updateScore(GG0170R, 0);
                        const GG0170S = $('.persistable[id^=GG0170S_' + performanceType + ']');
                        updateScore(GG0170S, 0);
                    }
                    break;
                default:
                    console.log('\t Score_GG0170AtoP_Performance::: control value has no value');
                    updateScore(thisEL, 0);
                    break;
            }
        });
        return PerformanceScore;
    }
    /* internal function */
    function Score_GG0170RandS_Performance(performanceType = '') {
        let multiplier = 1, R_PerformanceScore = 0, S_PerformanceScore = 0;
        let GG0170I, GG0170R, GG0170S;
        let GG0170I_Score = 0, GG0170R_Score = 0, GG0170S_Score = 0;
        GG0170I = $('.persistable[id^=GG0170I_' + performanceType + ']');
        GG0170R = $('.persistable[id^=GG0170R_' + performanceType + ']');
        GG0170S = $('.persistable[id^=GG0170S_' + performanceType + ']');
        GG0170I_Score = +commonUtility.getControlScore(GG0170I); //$('option:selected', GG0170I).data('score');    
        GG0170R_Score = +commonUtility.getControlScore(GG0170R); //$('option:selected', GG0170R).data('score');    
        GG0170S_Score = +commonUtility.getControlScore(GG0170S); //$('option:selected', GG0170S).data('score');    
        if (GG0170I_Score >= 7)
            multiplier = 2;
        else if (GG0170I_Score > 0 && GG0170I_Score < 7) {
            multiplier = 0; /* take I value which has been counted so don't count it again */
        }
        if (GG0170R_Score >= 7) {
            GG0170R_Score = 1;
        }
        updateScore(GG0170R, GG0170R_Score * multiplier);
        R_PerformanceScore += GG0170R_Score * multiplier;
        console.log('GG0170R_Score_' + performanceType + ' * ' + multiplier + ' = ' + (GG0170R_Score * multiplier));
        if (GG0170S_Score >= 7) {
            GG0170S_Score = 1;
        }
        updateScore(GG0170S, GG0170S_Score * multiplier);
        S_PerformanceScore += GG0170S_Score * multiplier;
        console.log('GG0170S_Value_' + performanceType + ' * ' + multiplier + ' = ' + (GG0170S_Score * multiplier));
        return R_PerformanceScore + S_PerformanceScore;
    }
    /* internal function */
    //function Score_GG0170RandS_Discharge_Performance(): number {
    //  let multiplier = 1, R_Performance = 0, S_Performance = 0;
    //  /* use GG0170I to determine the multipliers for GG0170R and GG0170S */
    //  const GG0170I: any = $('.persistable[id^=GG0170I_Discharge_Performance]');
    //  const GG0170I_Score: number = +commonUtility.getControlScore(GG0170I);    //+($('option:selected', GG0170I).data('score'));
    //  if (GG0170I_Score >= 7)
    //    multiplier = 2;
    //  if (GG0170I_Score <= 6)
    //    multiplier = 0;
    //  if (isNaN(GG0170I_Score))
    //    multiplier = 1; //when GG0170I is not answered score R and S as is
    //  const GG0170R: any = $('.persistable[id^=GG0170R_Discharge_Performance]');
    //  const GG0170R_Score: number = +commonUtility.getControlScore(GG0170R);  //+($('option:selected', GG0170R).data('score'));
    //  if (GG0170R_Score > 0) {
    //    updateScore(GG0170R, GG0170R_Score * multiplier);
    //    R_Performance += GG0170R_Score * multiplier;
    //  }
    //  else
    //    updateScore(GG0170R, 0);
    //  const GG0170S: any = $('.persistable[id^=GG0170S_Discharge_Performance]');
    //  const GG0170S_Score: number = +commonUtility.getControlScore(GG0170S);  //+($('option:selected', GG0170S).data('score'));
    //  if (GG0170S_Score > 0) {
    //    updateScore(GG0170S, GG0170S_Score * multiplier);
    //    S_Performance += GG0170S_Score * multiplier;
    //  }
    //  else
    //    updateScore(GG0170S, 0);
    //  return R_Performance + S_Performance;
    //}
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
        'grandTotal': grandTotal,
        'addMoreDate': addMoreDate,
        'clear_long_text_and_score': clear_long_text_and_score,
        'changeCssClassIfOldAndNewValuesAreDifferent': changeCssClassIfOldAndNewValuesAreDifferent
    };
})( /* optional parameters go here */);
/******************************* end of closure ****************************/
//self execution function
(function () {
    $('input[type=date]').each(function () {
        const thisDate = $(this);
        thisDate.on('change', function () {
            if (thisDate.val() !== '') {
                const thisDateReset = $('button.calendarReset[data-target=' + thisDate.prop('id') + ']');
                if (thisDateReset.length !== 0) {
                    thisDateReset.prop('disabled', false);
                    thisDateReset.removeClass(['changedFlag', 'Create', 'Update', 'Delete']); //remove both class
                }
            }
        });
    });
    /* each reset calendar click reset the date of the target element */
    $('.calendarReset').on('click', function (e) {
        const thisResetButton = $(this);
        const thisResetDateTarget = $('#' + thisResetButton.data('target'));
        if (thisResetDateTarget.length > 0) {
            console.log(thisResetButton.prop('id') + ' resets ' + thisResetDateTarget.prop('id') + ' of type ' + thisResetDateTarget.prop('type'), thisResetDateTarget);
            commonUtility.resetControlValue(thisResetDateTarget);
            thisResetDateTarget.trigger('change'); //programmatically trigger('change') becuase set val(new value) doesn't raise change event
        }
        e.preventDefault(); //this is must to prevent submit the form and leave the page
    });
    $('select').each(function () {
        const thisDropdown = $(this);
        thisDropdown.on('change', function () {
            //beak long option text
            //commonUtility.resetControlValue(thisDropdown);
            if (thisDropdown.val() !== -1)
                formController.breakLongSentence(thisDropdown);
            else {
                const siblinglongTextOptions = thisDropdown.next('.longTextOption');
                const siblingScore = thisDropdown.next('i.score');
                siblinglongTextOptions.text('');
                siblingScore.text('');
            }
        });
    });
    /* section nav */
    $('#questionTab')
        .on('mouseenter', function () {
        $('#questionTab').css({ 'left': '0px', 'transition-duration': '1s' });
    })
        .on('mouseleave', function () {
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
                slidingAggregator.css({ "width": "13.5em" });
                break;
            case 'Interim':
            case 'Follow Up':
                slidingAggregator.css({ "width": "7.2em" });
                break;
        }
        slidingAggregator.css({ "right": "0em" });
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
        $this.on('click', function () {
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
    /* ajax post form */
    $('#saveButton').on('click', function () {
        if (formController.validate) {
            const saveButton = $(this);
            const defaultDialogOptions = commonUtility.dialogOptions();
            formController.submitTheForm(saveButton, defaultDialogOptions);
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
    formController.clear_long_text_and_score();
    formController.changeCssClassIfOldAndNewValuesAreDifferent();
})( /* optional parameters go here */);
//# sourceMappingURL=form.js.map