import { Utility } from "./commonImport.js";
/* jquery plugin dependsOn*/
/* https://dstreet.github.io/dependsOn */
/* http://emranahmed.github.io/Form-Field-Dependency */
var EnumChangeEventArg;
(function (EnumChangeEventArg) {
    EnumChangeEventArg["Load"] = "Load";
    EnumChangeEventArg["Change"] = "Change";
    EnumChangeEventArg["NoScroll"] = "NoScroll";
})(EnumChangeEventArg || (EnumChangeEventArg = {}));
$(function () {
    const commonUtility = new Utility();
    const defaultDialogOptions = commonUtility.dialogOptions();
    const formScope = $("form#userAnswerForm");
    const Q12 = $('.persistable[id=Q12]', formScope);
    const Q23 = $('.persistable[id=Q23]', formScope);
    function BranchingRuleDialog(thisRule) {
        let formDialog = $("#dialog")
            .html(thisRule)
            .dialog(defaultDialogOptions, {
            //custom options
            title: "Rules",
            closeText: "Close",
            buttons: [{
                    text: "ok",
                    icon: "ui-icon-close",
                    click: function () {
                        let thisButton = $(this);
                        thisButton.dialog("close");
                    }
                }]
        });
        //add options to formDialog default options
        //formDialog.dialog("option", "title", "Rules");
        //formDialog.dialog("open");
    }
    //self executing arrow function test
    //(() => {
    //    console.log('------ self executing arrow function test ------');
    //    $('.questionRow').on('change', '.persistable', function (e) {
    //        if (e.target !== undefined) {
    //            //console.log('e.target = ', e.target);
    //            //const trigger: string = e.target.prop('id');
    //            //switch (true) {
    //            //  case (trigger.indexOf('Q12') !== -1):
    //            //    console.log('event triggered ', trigger);
    //            //    break;
    //            //  case (trigger.indexOf('Q23') !== -1):
    //            //    console.log('event triggered ', trigger);
    //            //    break;
    //            //  case (trigger.indexOf('Q12B') !== -1):
    //            //    console.log('event triggered ', trigger);
    //            //    break;
    //            //}
    //            console.log('------ done test self executing ------');
    //        }
    //    });
    //})();
    /* event handler */
    function Q23_blank_then_Lock_All(eventType, byRef) {
        console.log('Q23 is fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        const admitDate = new Date(Q12.val());
        const onsetDate = new Date(Q23.val());
        const minDate = new Date('2020-01-01 00:00:00');
        const Q12_or_Q23_is_empty = commonUtility.isEmpty(Q12) || commonUtility.isEmpty(Q23);
        const onset_is_later_than_admit = onsetDate < minDate || admitDate < minDate || admitDate < onsetDate;
        const otherPersistables = $('.persistable', formScope).not(Q12).not(Q23);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        if (Q12_or_Q23_is_empty || (!Q12_or_Q23_is_empty && onset_is_later_than_admit)) {
            console.log('Q12 or Q23 is empty, or onset day is later than admit date, lock all other questions.');
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                //with warning dialog
                console.log('with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                defaultDialogOptions.title = 'Date';
                const theseDialogButtons = {
                    "Ok": function () {
                        console.log('lock all other persistables');
                        otherPersistables.each(function () {
                            let thisPersistable = $(this);
                            thisPersistable.prop("disabled", true);
                        });
                        $(this).dialog("close");
                        setSeenTheDialog(true); //callback function
                        Q23.trigger('focus');
                    },
                    "Cancel": function () {
                        $(this).dialog("close");
                        setSeenTheDialog(true); //callback function
                        Q23.trigger('focus');
                    }
                };
                let dialogText;
                if (Q12_or_Q23_is_empty)
                    dialogText = 'The Onset Date and the Admission Date are episode keys, when either is blank, all fields with current values will be locked.';
                if (onset_is_later_than_admit)
                    dialogText = 'The Onset Date and the Admission Date must be later than 01/01/2021, and the Admission Date must be on Onset Date or later.';
                $("#dialog").html(dialogText)
                    .dialog(defaultDialogOptions, {
                    title: 'Warning Q12 Q23',
                    buttons: theseDialogButtons
                });
            }
            else { //without warning dialog
                console.log('lock all other persistables', otherPersistables);
                otherPersistables.each(function () {
                    const thisPersistable = $(this);
                    thisPersistable.prop('disabled', true);
                });
            }
        }
        else {
            //see branchingTree.txt
            const triggers = $('.persistable[id ^= Q12B], .persistable[id ^= Q14A], .persistable[id ^= Q16A], .persistable[id ^= Q42], .persistable[id ^= Q44C]'
                + ', .persistable[id ^= Q44D], .persistable[id ^= GG0170I], .persistable[id ^= GG0170M], .persistable[id ^= GG0170Q], .persistable[id ^= J0510], .persistable[id ^= J0520]', formScope);
            const controledByTriggers = $('.persistable[id ^= Q14B], .persistable[id ^= Q16B], .persistable[id ^= Q17], .persistable[id ^= Q21B], .persistable[id ^= Q41]'
                + ', .persistable[id ^= Q43], .persistable[id ^= Q44C]:not([id *= FollowUp]), .persistable[id ^= Q44D], .persistable[id ^= Q45], .persistable[id ^= Q46]:not([id *= FollowUp])'
                + ', .persistable[id ^= GG0170I]:not([id *= Discharge_Goal]), .persistable[id ^= GG0170J]:not([id *= Discharge_Goal])'
                + ', .persistable[id ^= GG0170K]:not([id *= Discharge_Goal]), .persistable[id ^= GG0170L]:not([id *= Discharge_Goal])'
                + ', .persistable[id ^= GG0170N]:not([id *= Discharge_Goal]), .persistable[id ^= GG0170O]:not([id *= Discharge_Goal])'
                + ', .persistable[id ^= GG0170R]:not([id *= Discharge_Goal]), .persistable[id ^= GG0170S]:not([id *= Discharge_Goal])'
                + ', [id ^= Complete]', formScope);
            const notControlledByTriggers = $('.persistable, .calendarReset', formScope).not(controledByTriggers).not(Q12).not('.summary header');
            //enable handlerless controls but don't raise change event to prevent infinite loop
            console.log('enable ' + notControlledByTriggers.length + ' controls on load.', notControlledByTriggers);
            notControlledByTriggers.each(function () {
                const thisControl = $(this);
                thisControl.prop('disabled', false);
            });
            //not('[id^=Q12]', formScope) so it doesn't raise change to cause infinite loop
            //only raise change for the following to unlock or remain lock of the respective fields
            triggers.each(function () {
                const thisTrigger = $(this);
                console.log(thisTrigger.prop('id') + ' change triggered by key question valid change.');
                thisTrigger.trigger('change');
            });
        }
        console.log('------ done handling Q12 Q23 ' + eventType + '------');
        return byRef.seenTheDialog;
    }
    /* self executing event listener, add this last to raise change chain */
    (function Q23_addListener() {
        console.log('adding Q23_addListener()');
        /* add onchange event listner */
        let seenTheDialog = false;
        Q23.on('change', { x: EnumChangeEventArg.Change }, function (e) {
            Q23.prop('disabled', false);
            console.log('onchange calling Q23_blank_then_Lock_All(), seenTheDialog = ', seenTheDialog);
            //checkQ12_Q23(e.data.x);
            seenTheDialog = Q23_blank_then_Lock_All(EnumChangeEventArg.Change, { seenTheDialog: seenTheDialog });
        });
    })();
    (function Q12_Q23Rules() {
        const Q12_Q23RuleTriggers = $('.branchingRule[data-target=Q12], .branchingRule[data-target=Q23]', formScope);
        const Q12_Q23rule = 'If Q12 or Q23 is blank or Q12 date is later than Q23 date, lock all inputs and the save button.';
        Q12_Q23RuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', Q12_Q23rule).show();
            thisTrigger.on('click', function () {
                BranchingRuleDialog(Q12_Q23rule);
            });
        });
        console.log('Q12_Q23 listener added');
    })();
    /* event handler */
    function Q12B_blank_then_Lock_Discharge(eventType, byRef) {
        //event hooked during checkAllRules()
        const admitDate = new Date(Q12.val());
        const minDate = new Date('2020-01-01 00:00:00');
        const Q12B = $('.persistable[id=Q12B]', formScope);
        console.log(Q12B.prop('id') + ' fired by ' + eventType + " with seenTheDalog = " + byRef.seenTheDialog);
        const dischargeDate = new Date(Q12B.val());
        const isDischarged = dischargeDate >= admitDate;
        function actQ12B(thisDisabled) {
            const dischargeRelatedDropdown = $('select.persistable[id^=Q16B],select.persistable[id^=Q17B], select.persistable[id^=Q21B]', formScope);
            const dischargeRelatedCheckboxes = $('.persistable[id^=Q41]:not([id*=FollowUp]),.persistable[id^=Q44C]:not([id*=FollowUp])', formScope);
            //enable or disable related .persistable elements
            dischargeRelatedDropdown.each(function () {
                const thisDropdown = $(this);
                thisDropdown.prop('disabled', thisDisabled);
                if (thisDisabled)
                    thisDropdown.val(-1).siblings('.longTextOption').text('');
            });
            dischargeRelatedCheckboxes.each(function () {
                const thisCheckbox = $(this);
                thisCheckbox.prop('disabled', thisDisabled);
                if (thisDisabled)
                    thisCheckbox.prop('checked', false);
            });
        }
        let dialogText;
        if (isDischarged) {
            //without warning dialog
            actQ12B(false);
        }
        else {
            //lock all discharege related fields with
            dialogText = 'When Q12B is clearred, is an invalid date, or is earlier than the admit date, rseset and locks related discharge fields:  Q16B, Q17B, Q21B, Q41, and Q44C.';
            Q12B.val('');
            console.log(dialogText);
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                function setSeenTheDialog(value) {
                    //callback after async dialog is done and return the seenTheDialog to the caller
                    byRef.seenTheDialog = value;
                }
                function DischargeRelatedButtonClosure() {
                    return function (dischargeState) {
                        return {
                            "Ok": function () {
                                actQ12B(true);
                                $(this).dialog("close");
                                setSeenTheDialog(true); //callback
                            },
                            "Cancel": function () {
                                $(this).dialog("close");
                                setSeenTheDialog(true); //callback
                            }
                        };
                    };
                }
                //with warning dialog
                const Q12Bbuttons = DischargeRelatedButtonClosure();
                $("#dialog").html(dialogText)
                    .dialog(defaultDialogOptions, {
                    title: 'Warning Q12B',
                    buttons: Q12Bbuttons(true)
                });
                console.log('Q12B handler return byRef.seenTheDialog = ', byRef.seenTheDialog);
            }
            else
                actQ12B(true);
        }
        console.log('byRef.seenTheDialog = ', byRef.seenTheDialog);
        console.log('------ done handling Q12B ' + eventType + '------');
        return byRef.seenTheDialog;
    }
    /* self executing event listener */
    (function Q12B_addListener() {
        const Q12B = $('.persistable[id^=Q12B]', formScope);
        console.log(Q12B.prop('id') + ' adding listener()');
        //no need to raise onload event, it is only triggered by Q12_Q23 change event chain
        let seenTheDialog = false;
        /* on change */
        Q12B.on('change', { x: EnumChangeEventArg.Change, y: Q12B }, function (e) {
            Q12B.prop('disabled', false);
            console.log('before calling Q12B_blank_then_Lock_Discharge(), seenTheDialog = ', seenTheDialog);
            //JavaScript, and TypeScript can pass an object by reference, but not by value.
            //Therefore box values into an object  { seenTheDialog: seenTheDialog }
            Q12B_blank_then_Lock_Discharge(e.data.x, { seenTheDialog: seenTheDialog });
            console.log('Q12B listener added');
        });
    })();
    (function Q12BRules() {
        const Q12BRuleTriggers = $('.branchingRule[data-target^=Q12B],[data-target^=Q16B],[data-target^=Q17B],[data-target^=Q21B],[data-target^=Q41],[data-target^=Q44C]');
        const ruleText = 'If Q12B is not a date, reset and lock related discharge fields: Q16B, Q17B, Q21B, Q41, Q44C.';
        Q12BRuleTriggers.each(function () {
            const thisTrigger = $(this);
            thisTrigger.prop('title', ruleText).show();
            thisTrigger.on('click', function () {
                BranchingRuleDialog(ruleText);
            });
        });
    })();
    /* event handler */
    function Q14B_enabled_if_Q14A_is_Yes(eventType, byRef) {
        console.log('Q14A handler fired by ' + eventType + " with seenTheDalog = " + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function actQ14A(isDisable14B) {
            const Q14Bs = $('.persistable[id^=Q14B]', formScope);
            Q14Bs.each(function () {
                const thisQ14B = $(this);
                thisQ14B.prop('disabled', isDisable14B).trigger('change'); //uncheck Q14Bs. Trigger('change') becuase programmatically set val(new value) doesn't raise change event
                if (isDisable14B) {
                    thisQ14B.prop('checked', !isDisable14B);
                }
            });
        }
        const Q14AYes = $('.persistable[id^=Q14A][data-codesetdescription*=Yes]:checked', formScope).length === 1;
        if (Q14AYes) {
            const isDisable14B = false;
            /* without warning dialog */
            console.log('Q14A is Yes, uncheck and unlock all Q14B, no dialog.');
            actQ14A(isDisable14B);
        }
        else {
            let isDisable14B = true;
            const dialogText = 'Q14A is unknown or is No, uncheck and lock all Q14B.';
            console.log(dialogText);
            //with warning dialog
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                console.log('Q14A with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                const theseDialogButtons = {
                    "Ok": function () {
                        actQ14A(isDisable14B);
                        setSeenTheDialog(true);
                        $(this).dialog("close");
                    },
                    "Cancel": function () {
                        setSeenTheDialog(true);
                    }
                };
                $("#dialog").html(dialogText)
                    .dialog(defaultDialogOptions, {
                    title: 'Warning 14A', buttons: theseDialogButtons
                });
            }
            else {
                console.log('Q14A without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                actQ14A(isDisable14B);
            }
        }
        console.log('------ done handling Q14A ' + eventType + '------');
        return byRef.seenTheDialog;
    }
    /* self executing event listener */
    (function Q14A_addListener() {
        const Q14As = $('.persistable[id^=Q14A]', formScope);
        Q14As.each(function () {
            const thisQ14A = $(this);
            console.log('adding ' + thisQ14A.prop('id') + ' listener');
            let seenTheDialog = true;
            /* on change. No need to raise onload event, it is only triggered by Q12_Q23 change event chain */
            thisQ14A.on('change', { x: EnumChangeEventArg.Change, y: thisQ14A }, function (e) {
                thisQ14A.prop('disabled', false);
                console.log('before calling Q14B_enabled_if_Q14A_is_Yes(), seenTheDialog = ', seenTheDialog);
                seenTheDialog = Q14B_enabled_if_Q14A_is_Yes(e.data.x, { seenTheDialog: seenTheDialog });
            });
        });
        console.log('Q14A listener added');
    })();
    (function Q14ARules() {
        /* add rule help */
        const Q14ARuleTriggers = $('.branchingRule[data-target^=Q14A], .branchingRule[data-target^=Q14B]');
        const Q14ARuleText = 'If Q14A is unknown or is No, uncheck and lock all Q14B.';
        Q14ARuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', Q14ARuleText).show()
                .on("click", function () {
                BranchingRuleDialog(Q14ARuleText);
            });
        });
    })();
    /* event handler */
    function Q16A_is_Home_then_Q17(eventType, byRef, Q16A) {
        //event hooked during checkAllRules()
        console.log(Q16A.prop('id') + ", fired by " + eventType + " with seenTheDalog = " + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function actQ16A(isDisableQ17) {
            const Q17 = $(".persistable[id^=Q17]:not([id^=Q17B])", formScope);
            if (Q17.length > 0) {
                Q17.prop('disabled', isDisableQ17);
                console.log(Q17.prop('id') + 'disabled ' + Q17.prop('disabled'));
                if (isDisableQ17) {
                    //commonUtility.resetControlValue(Q17);
                    Q17.val(-1).siblings('.longTextOption').text('');
                }
                else {
                    console.log('focus on Q17', Q17);
                    //    Q17.trigger('focus');
                }
            }
        }
        const Q16AisHome = $("option:selected", Q16A).text().indexOf('1. Home') !== -1;
        if (Q16AisHome) {
            const dialogText = 'Q16A is home, unlock Q17';
            console.log('Q16A without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
            const isDisableQ17 = false;
            actQ16A(isDisableQ17);
        }
        else {
            const isDisableQ17 = true;
            const dialogText = 'Q16A is not home, clear and lock Q17';
            console.log(dialogText);
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                console.log('Q16A with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                const theseDialogButtons = {
                    "Ok": function () {
                        actQ16A(isDisableQ17);
                        setSeenTheDialog(true);
                        $(this).dialog("close");
                    },
                    "Cancel": function () {
                        setSeenTheDialog(true);
                        $(this).dialog("close");
                    }
                };
                $("#dialog").html(dialogText)
                    .dialog(defaultDialogOptions, {
                    title: 'Warning Q16A, Q17',
                    buttons: theseDialogButtons
                });
            }
            else {
                console.log('Q16A without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                actQ16A(isDisableQ17);
            }
        }
        console.log('------ done handling Q16A ' + eventType + '------');
        return byRef.seenTheDialog;
    }
    /* self executing event listener */
    (function Q16A_addListener() {
        const Q16A = $('.persistable[id^=Q16A]', formScope);
        console.log('adding ' + Q16A.prop('id') + ' listener()');
        //no need to raise onload event, it is only trigger by Q12_Q23 change event chain
        /* on change */
        let seenTheDialog = true;
        Q16A.on('change', { x: EnumChangeEventArg.Change }, function (e) {
            Q16A.prop("disabled", false);
            console.log('before calling Q16A_is_Home_then_Q17(), seenTheDialog = ', seenTheDialog);
            seenTheDialog = Q16A_is_Home_then_Q17(e.data.x, { seenTheDialog: seenTheDialog }, $(this));
        });
        console.log('Q16 listener added');
    })();
    (function Q16ARules() {
        /* add rule help */
        const Q16ARuleTriggers = $('.branchingRule[data-target^=Q16A], .branchingRule[data-target^=Q17]').not('[data-target^=Q17B]');
        const Q16ARuleText = 'If Q16A is home, unlock Q17';
        Q16ARuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', Q16ARuleText).show()
                .on("click", function () {
                BranchingRuleDialog(Q16ARuleText);
            });
        });
    })();
    /* event handler, not used per stakeholder request */
    function Q22_Q24_is_Arthritis_then_Q24A(eventType, byRef) {
        //event hooked during checkAllRules()
        console.log('inside of Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        /* nested event handler */
        function checkArthritis(e) {
            var _a;
            console.log('inside of checkArthritis(e) $(this)', $(this));
            const thisICD = $(this);
            //any arthritis ICD can check Q24, but all are not arthritis ICD to uncheck Q24
            if (!commonUtility.isEmpty(thisICD) && thisICD.val() === '123.45') { //ICD for diabetes
                $('.persistable[id^=Q24A]', formScope).prop('checked', true);
            }
            else {
                //check all other ICD fields to ensure none has arthritis then uncheck Q24A
                arthritis.each(function () {
                    const thisOtherICD = $(this);
                    if (thisOtherICD.prop('id') !== thisICD && thisOtherICD.val() === '123.45') {
                        return false; //shortcut the loop since at least one other ICD is arthritis
                    }
                    //since loop completed and none other ICD has arthritis then it's safe to uncheck Q24A
                    $('.persistable[id^=Q24A]', formScope).prop('checked', false);
                });
                if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    const theseDialogButtons = {
                        "Ok": function () {
                            $('.persistable[id^=Q24A]', formScope).prop('checked', false);
                            setSeenTheDialog(true);
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            setSeenTheDialog(true);
                            $(this).dialog("close");
                        }
                    };
                    $("#dialog").html('Not an arthritis ICD, lock Q24A')
                        .dialog(defaultDialogOptions, {
                        title: 'Warning Q21A, Q21B, Q22, Q24', buttons: theseDialogButtons
                    });
                }
                else {
                    $('.persistable[id^=Q24A]', formScope).prop('checked', false);
                }
            }
        }
        /* on change */
        console.log('check Q24A if Q22, or Q24 is diabetes');
        const arthritis = $('.persistable[id^= Q22], .persistable[id^= Q24]', formScope);
        arthritis.each(function () {
            $(this).on('change', { x: EnumChangeEventArg.Change }, checkArthritis);
        });
        console.log('------ done handling Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A ' + eventType + '------');
    }
    /* event handler */
    function Q42_Interrupted_then_Q43(eventType, byRef) {
        //event hooked during checkAllRules()
        console.log('inside of Q42_Interrupted_then_Q43, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function actQ42(isDisableQ43) {
            const Q43s = $('.persistable[id^=Q43]', formScope);
            Q43s.each(function () {
                const thisQ43 = $(this);
                thisQ43.prop('disabled', isDisableQ43);
                if (isDisableQ43)
                    thisQ43.val('');
            });
        }
        let Q42Yes = $('.persistable[id^=Q42][data-codesetdescription*=Yes]:checked').length === 1;
        if (Q42Yes) {
            const isDisableQ43 = false;
            actQ42(isDisableQ43);
        }
        else {
            const isDisableQ43 = true;
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                //with warning dialog
                console.log('Q42 with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                const theseDialogButtons = {
                    "Ok": function () {
                        actQ42(isDisableQ43);
                        setSeenTheDialog(true); //callback
                        $(this).dialog("close");
                    },
                    "Cancel": function () {
                        setSeenTheDialog(true); //callback
                        $(this).dialog("close");
                    }
                };
                $("#dialog").html('Q42 is a No, reset and lock all Q43.')
                    .dialog(defaultDialogOptions, {
                    title: 'Warning Q42, Q43',
                    buttons: theseDialogButtons
                });
            }
            else {
                //warning dialog
                console.log('Q42 without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                actQ42(isDisableQ43);
            }
        }
        console.log('------ done handling Q42 ' + eventType + '------');
        return byRef.seenTheDialog;
    }
    /* self executing event listener */
    (function Q42_addListener() {
        console.log('adding Q42_addListener()');
        //no need to raise onload event, it is only trigger by Q12_Q23 change event chain
        /* on change */
        let seenTheDialog = true;
        const Q42s = $('.persistable[id^=Q42]', formScope);
        Q42s.each(function () {
            const thisQ42 = $(this);
            thisQ42.on('change', { x: EnumChangeEventArg.Change, y: thisQ42 }, function (e) {
                thisQ42.prop('disabled', false);
                console.log('before calling Q42_Interrupted_then_Q43(), seenTheDialog = ', seenTheDialog);
                seenTheDialog = Q42_Interrupted_then_Q43(e.data.x, { seenTheDialog: seenTheDialog });
            });
        });
        console.log('Q42 listener added');
    })();
    (function Q42Rules() {
        const Q42RuleTriggers = $('.branchingRule[data-target^=Q42], .branchingRule[data-target^=Q43]');
        const Q42RuleText = 'If Q42 is a No, reset and lock all Q43.';
        Q42RuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', Q42RuleText).show()
                .on("click", function () {
                BranchingRuleDialog(Q42RuleText);
            });
        });
    })();
    /* event handler */
    function Q43_Intterrupt_Return_Dates_Coordination(eventType, byRef, thisQ43) {
        console.log('inside of Q43_Rules(), fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog, thisQ43);
        /*
            1. find interrupt and returning date pair
            2. when interrupt date is null then reset pairing return date
            2. ensure paired return date is later then interrupt date
            3. only enable paried return date
        */
        const thisQ43Id = thisQ43.prop('id');
        const isThisIntrruptDate = thisQ43Id.indexOf('Interrupt') >= 0;
        let thisInterruptDateControl;
        let thisReturnDateControl;
        let numberPattern = /(\d+)(?!.*\d)/;
        let controlCounter = thisQ43Id.match(numberPattern)[1];
        if (isThisIntrruptDate) {
            thisInterruptDateControl = thisQ43;
            thisReturnDateControl = $('.persistable[id^=Q43][id*=Return_Date][id$=' + controlCounter + ']');
            if (thisInterruptDateControl.val() === '')
                thisReturnDateControl.val('');
        }
        else {
            thisInterruptDateControl = $('.persistable[id^=Q43][id*=Interrupt_Date][id*=' + controlCounter + ']');
            thisReturnDateControl = thisQ43;
        }
        if (thisInterruptDateControl.val() !== '' && thisReturnDateControl.val() !== '') {
            const thisInterruptDate = new Date(thisReturnDateControl.val());
            const thisReturnDate = new Date(thisReturnDateControl.val());
            if (thisReturnDate < thisInterruptDate)
                $('#dialog')
                    .text('The return date cannot be earlier than the interrupt date.')
                    .dialog(defaultDialogOptions);
            const admitDate = new Date(Q12.val());
            const onsetDate = new Date(Q23.val());
            if (thisReturnDate < admitDate || thisReturnDate < onsetDate || thisInterruptDate < admitDate || thisInterruptDate < onsetDate)
                $('#dialog')
                    .text('The interrupt date and the return date cannot be earlier than the admission or the onset date.')
                    .dialog(defaultDialogOptions);
        }
        console.log('------ done handling Q43 ' + eventType + '------');
    }
    /* self executing event listener */
    (function Q43_addListener() {
        console.log('adding Q43_addListener()');
        //no need to raise onload event, it is only triggered by Q12_Q23 change event chain
        /* on change. Q43 only triggered by change event manually here or programmmatically in Q42*/
        let seenTheDialog = true;
        const Q43s = $('.persistable[id^=Q43]', formScope);
        Q43s.each(function () {
            const thisQ43 = $(this);
            /* Q43 enabled only when Q42 Yes is checked and set in Q42_Interrupted_then_Q43() */
            thisQ43.on('change', { x: EnumChangeEventArg.Change, y: thisQ43 }, function (e) {
                Q43_Intterrupt_Return_Dates_Coordination(e.data.x, { seenTheDialog: seenTheDialog }, thisQ43);
            });
        });
        /* no need to add rule help which is added with Q42 */
        console.log('Q43 listener added');
    })();
    /* event handler */
    function Q44C_Affect_Q44D_Q45_Q46(eventType, byRef) {
        //event hooked during checkAllRules()
        console.log('inside of Q44C_Affect_Q44D_Q46, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        const Q44C_is_Yes = $('.persistable[id^=Q44C][data-codesetdescription*=Yes]:checked, .persistable[id^=Q44C-FollowUp][id*=Yes]:checked', formScope).length === 1;
        const Q44D = $('.persistable[id^=Q44D]', formScope);
        const Q45 = $('.persistable[id^=Q45]', formScope);
        const Q46 = $('.persistable[id^=Q46]', formScope);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function act44C_is_no() {
            console.log('disable Q44D');
            Q44D.val(-1).prop('disabled', true);
            console.log('disable Q45');
            Q45.val(-1).prop('disabled', true);
            console.log('enable and focus on Q46');
            Q46.val(-1).prop('disabled', false);
        }
        /* nested event handler */
        if (Q44C_is_Yes) {
            console.log('Q44C is yes, unlock Q44D, Q45 and focus on Q44D.');
            if (Q45.length > 0) {
                Q45.prop('disabled', false);
            }
            if (Q44D.length > 0) {
                Q44D.prop('disabled', false);
            }
        }
        else {
            if (Q44D.length > 0 && Q45.length > 0 && Q46.length > 0) {
                let dialogText = 'Q44C is no, lock Q44D, Q45 and focus on Q46.';
                console.log(dialogText);
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    console.log('Q44C with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                    const theseDialogButtons = {
                        "Ok": function () {
                            act44C_is_no();
                            setSeenTheDialog(true); //callback
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            setSeenTheDialog(true); //callback
                            $(this).dialog("close");
                        }
                    };
                    $("#dialog").html(dialogText)
                        .dialog(defaultDialogOptions, {
                        title: 'Warning Q44C, Q44D, Q45, Q46', buttons: theseDialogButtons
                    });
                }
                else {
                    //without warning dialog
                    console.log('Q44C without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                    act44C_is_no();
                }
            }
        }
        console.log('------ done handling Q44C ' + eventType + '------');
        return byRef.seenTheDialog;
    }
    /* self executing event listener */
    (function Q44C_addListener() {
        console.log('adding Q44C_addListener()');
        //no need to raise onload event, it is only triggered by Q12_Q23 change event chain
        /* on change */
        let seenTheDialog = true;
        const Q44C = $('.persistable[id^=Q44C]', formScope);
        Q44C.each(function (i, el) {
            const thisQ44C = $(el); //don't use $(this) because in the arrow function it will be undefined
            thisQ44C.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                console.log('before calling Q44C_Affect_Q44D_Q45_Q46(), seenTheDialog = ', seenTheDialog);
                seenTheDialog = Q44C_Affect_Q44D_Q45_Q46(e.data.x, { seenTheDialog: seenTheDialog });
            });
        });
        console.log('Q44C listener added');
    })();
    (function Q44CRules() {
        /* add rule help */
        const Q44CRuleTriggers = $('.branchingRule[data-target^=Q44C], .branchingRule[data-target^=Q44D], .branchingRule[data-target^=Q45], .branchingRule[data-target^=Q46]', formScope);
        Q44CRuleTriggers.each(function () {
            let thisTrigger = $(this);
            let thisDescription = thisTrigger.prop('title');
            thisTrigger.prop('title', thisDescription).show()
                .on("click", function () {
                BranchingRuleDialog(thisDescription);
            });
        });
        const Q44CFollowuUpRuleTriggers = $('.branchingRule[data-target^=Q44C-FollowUp],[data-target^=Q44C-Follow_Up],[data-target^=Q44C_FollowUp],[data-target^=Q44C_Follow_Up]', formScope);
        const Q44CFollowuUpRuleText = 'If Q44C is yes, unlock Q44D, Q45 and focus on Q44D. If Q44C is no, lock Q44D, Q45 and focus on Q46';
        Q44CFollowuUpRuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', Q44CFollowuUpRuleText).show()
                .on("click", function () {
                BranchingRuleDialog(Q44CFollowuUpRuleText);
            });
        });
    })();
    /* event handler */
    function Q44D_Affect_Q45(eventType, byRef, thisQ44D) {
        console.log('inside of Q44D_Affect_Q45, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        const Q45 = $('.persistable[id^=Q45]', formScope);
        if (thisQ44D.val() === -1)
            Q45.prop('disabled', true);
        console.log('------ done handling Q44D ' + eventType + '------');
        //no diaglog is used so just return the original byRef.seenTheDialog value
        return byRef.seenTheDialog;
    }
    /* self executing event listener */
    (function Q44D_addListener() {
        console.log('adding Q44D_addListener()');
        /* on change */
        let seenTheDialog = true;
        const Q44D = $('.persistable[id^=Q44D]', formScope);
        Q44D.on('change', { x: EnumChangeEventArg.Change }, function (e) {
            console.log('before calling Q44C_Affect_Q44D_Q45_Q46(), seenTheDialog = ', seenTheDialog);
            seenTheDialog = Q44D_Affect_Q45(e.data.x, { seenTheDialog: seenTheDialog }, Q44D);
        });
        /* no need to add rule help, it is added in Q44C */
        console.log('Q44D listener added');
    })();
    /* event handler */
    function GG0170JKLMN_depends_on_GG0170I(eventType, byRef, thisI, measure) {
        console.log('inside of GG0170JKLMN_depends_on_GG0170I with measure = ' + measure + ', fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        //-----------------sample code-------------------
        //var extra_data = { one: "One", two: "Two" };
        //var make_handler = function (extra_data) {
        //  return function (event) {
        //    // event and extra_data will be available here
        //  };
        //};
        //element.addEventListener("click", make_handler(extra_data));
        //-----------------sample code-------------------
        //$(document).on({
        //  "touchstart": function () {
        //    // DO THIS SINCE EVENT WAS TOUCHSTART
        //  },
        //  "click": function () {
        //    // DO THIS SINCE EVENT WAS CLICK
        //  }
        //}, "#element");
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        //act on J, K, L
        function actGG0170I(isDisabled, eventArg) {
            const GG0170JKL = $('select.persistable[id^=GG0170J][id*=' + measure + '], select.persistable[id^=GG0170K][id*=' + measure + '], select.persistable[id^=GG0170L][id*=' + measure + ']');
            GG0170JKL.each(function () {
                const thisDropdown = $(this);
                thisDropdown.prop('disabled', isDisabled);
                if (isDisabled) {
                    commonUtility.resetControlValue(thisDropdown);
                    thisDropdown.siblings('.longTextOption, .score').text('');
                    thisDropdown.removeClass(['changedFlag', 'Create', 'Update', 'Delete']);
                }
            });
        }
        const GG0170J = $('select.persistable[id^=GG0170J][id*=' + measure + ']');
        const intGG0170I = +commonUtility.getControlScore(thisI); //+($('option:selected', thisI).data('score'));
        switch (true) {
            case (intGG0170I > 0 && intGG0170I < 7): {
                /* unlock and clear J K L, skip to J */
                //let consoleLog: string = 'GG0170I ' + measure + ' is between 1 and 6, unlock GG0170J ' + measure + ', GG0170K ' + measure + ', GG0170L ' + measure + ', and advance to GG0170J ' + measure + '. Other measures are kept intact.';
                //without warning dialog
                console.log('GG0170I without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                const focusJ = GG0170J;
                const isDisableJKL = false;
                actGG0170I(isDisableJKL, eventType);
                break;
            }
            default: {
                /* GG0170I is not selected, clear and lock J K L then advance to M */
                const dialogText = 'GG0170I ' + measure + ' is unknown or is not between 1 and 6, clear and lock GG0170J ' + measure + ', GG0170K ' + measure + 'and GG0170L ' + measure + '.';
                console.log('eventData ' + dialogText);
                const isDisableJKL = true;
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    console.log('GG0170I with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                    const theseDialogButtons = {
                        "Ok": function () {
                            actGG0170I(isDisableJKL, eventType);
                            setSeenTheDialog(true); //callback
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            setSeenTheDialog(true); //callback
                            $(this).dialog("close");
                        }
                    };
                    $("#dialog").html(dialogText)
                        .dialog(defaultDialogOptions, {
                        title: 'Warning GG0170JKLM', buttons: theseDialogButtons
                    });
                }
                else {
                    //without warning dialog
                    console.log('GG0170I without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                    actGG0170I(isDisableJKL, eventType);
                }
                break;
            }
        }
        console.log('------ done handling GG0170I ' + eventType + '------');
        return byRef.seenTheDialog;
    }
    /* self executing event listener */
    (function GG0170I_addListener() {
        console.log('adding GG0170I_addListener()');
        //no need to raise onload event, it is only trigger by Q12_Q23 change event chain
        /* on change */
        let seenTheDialog = true;
        const GG0170Is = $('select.persistable[id ^= GG0170I]:not([id *= Discharge_Goal])', formScope);
        GG0170Is.each(function () {
            const thisI = $(this);
            thisI.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                thisI.prop('disabled', false);
                const theID = thisI.prop('id');
                let measure = thisI.data('measuredescription');
                measure = measure.replace(/ /g, "_");
                seenTheDialog = GG0170JKLMN_depends_on_GG0170I(e.data.x, { seenTheDialog: seenTheDialog }, thisI, measure);
            });
        });
        console.log('GG0170I listener added');
    })();
    (function GG0170IRules() {
        const GG0170IruleTriggers = $('.branchingRule[data-target=GG0170I]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170J]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170K]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170L]:not([id*=Discharge_Goal])', formScope);
        const GG0170IruleText = 'When GG0170I Admission Performance or Discharge Performance is between 1 and 6, unlock only the corresponding measure in J, K, and L, other measures remain unchanged. When the measure is 7 or greater, then reset and lock J, K, and L.';
        GG0170IruleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', GG0170IruleText).show()
                .on("click", function () {
                BranchingRuleDialog(GG0170IruleText);
            });
        });
    })();
    /* event handler */
    function GG0170O_GG0170P_depends_on_GG0170M_and_GG0170N(eventType, byRef, thisMorN, measure) {
        //event hooked during checkAllRules()
        console.log('inside of GG0170P_depends_on_GG0170M_and_GG0170N, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        let factor, M, N, O, P, dialogText;
        if (thisMorN.prop('id').indexOf('GG0170M') !== -1) {
            //only when thisMorN is GG0170M will M be assigned
            M = $('select.persistable[id^=GG0170M][id*=' + measure + ']', formScope);
            N = $('select.persistable[id^=GG0170N][id*=' + measure + ']', formScope);
            factor = +commonUtility.getControlScore(M); //+($('option:selected', M).data('score'));
            dialogText = 'M is unkown or M is greater than 7, reset and lock N and O.';
        }
        else {
            N = $('select.persistable[id^=GG0170N][id*=' + measure + ']', formScope);
            factor = +commonUtility.getControlScore(N); //+($('option:selected', N).data('score'));
            dialogText = 'N is unknow or N is greater than 7, reset and lock O.';
        }
        O = $('select.persistable[id^=GG0170O][id*=' + measure + ']', formScope);
        P = $('select.persistable[id^=GG0170P][id*=' + measure + ']', formScope);
        function act(controlScore) {
            return function () {
                if (M !== undefined && M !== null) {
                    M.prop('disabled', false);
                    const M_score = controlScore;
                    switch (true) {
                        case (M_score >= 7):
                            /* M >= 7, reset and lock both N and O */
                            if (N.length > 0) {
                                //commonUtility.resetControlValue(GG0170N);
                                N.val(-1).prop('disabled', true);
                                N.siblings('.longTextOption ,.score').text('');
                            }
                            if (O.length > 0) {
                                //commonUtility.resetControlValue(GG0170O);
                                O.val(-1).prop('disabled', true);
                                O.siblings('.longTextOption ,.score').text('');
                            }
                            if (P.length > 0) {
                                P.prop('disabled', false);
                                P.siblings('.longTextOption ,.score').text('');
                                //commonUtility.scrollTo(P.prop('id'));
                            }
                            break;
                        case (M_score > 0 && M_score < 7):
                            /* M between 0 and 6, disable O, and enable N */
                            if (O.length > 0) {
                                O.prop('disabled', true);
                                O.siblings('.longTextOption ,.score').text('');
                            }
                            if (N.length > 0) {
                                N.prop('disabled', false);
                                N.siblings('.longTextOption ,.score').text('');
                                //commonUtility.scrollTo(N.prop('id'));
                            }
                            break;
                        default:
                            /* M is unknown, lock N and O */
                            N.val(-1).prop('disabled', true).siblings('.longTextOption ,.score').text('');
                            ;
                            O.val(-1).prop('disabled', true).siblings('.longTextOption ,.score').text('');
                            ;
                            break;
                    }
                }
                else /* GG0170N */ {
                    const N_score = controlScore;
                    N.prop('disabled', false);
                    switch (true) {
                        case (N_score >= 7):
                            /* N >= 7, reset and lock both N and O */
                            if (O.length > 0) {
                                //commonUtility.resetControlValue(GG0170O);
                                O.val(-1).prop('disabled', true).siblings('.longTextOption, .score').text('');
                            }
                            if (P.length > 0) {
                                P.prop('disabled', false).siblings('.longTextOption, .score').text('');
                                //commonUtility.scrollTo(P.prop('id'));
                            }
                            break;
                        case (N_score > 0 && N_score < 7):
                            /* N between 0 and 6, unlock O */
                            if (O.length > 0) {
                                O.prop('disabled', false).siblings('.longTextOption, .score').text('');
                                ;
                                //commonUtility.scrollTo(O.prop('id'));
                            }
                            break;
                        default:
                            /* N is unknown, lock O */
                            if (O.length > 0) {
                                O.val(-1).prop('disabled', true).siblings('.longTextOption, .score').text('');
                            }
                            break;
                    }
                }
            };
        }
        if (eventType == EnumChangeEventArg.Change && !byRef.seenTheDialog) {
            /* with warning dialog */
            console.log('GG0170MN with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
            const theseDialogButtons = {
                "Ok": function () {
                    (act(factor))();
                    setSeenTheDialog(true); //callback
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                    setSeenTheDialog(true); //callback
                }
            };
            $("#dialog").html(dialogText)
                .dialog(defaultDialogOptions, {
                title: 'Warning GG0170M, N, O, and P', buttons: theseDialogButtons
            });
        }
        else {
            (act(factor))(); //self execute because actGG0170I() return a function to be executed.
        }
        console.log('------ done handling GG0170MN ' + eventType + '------');
        return byRef.seenTheDialog;
    }
    /* self executing event listener */
    (function GG0170M_N_addListener() {
        console.log('adding GG0170M_N_addListener()');
        //no need to raise onload event, it is only triggered by Q12_Q23 change event chain
        /* on change of M and N */
        const GG0170M_and_N = $('select.persistable[id^=GG0170M]:not([id*=Discharge_Goal]), select.persistable[id^=GG0170N]:not([id*=Discharge_Goal])', formScope);
        GG0170M_and_N.each(function () {
            const thisMorN = $(this);
            thisMorN.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                let seenTheDialog = true;
                let measure = thisMorN.data('measuredescription');
                measure = measure.replace(/ /g, "_");
                console.log('before calling GG0170P_depends_on_GG0170M_and_GG0170N() seenTheDialog = ', seenTheDialog);
                seenTheDialog = GG0170O_GG0170P_depends_on_GG0170M_and_GG0170N(e.data.x, { seenTheDialog: seenTheDialog }, thisMorN, measure);
            });
        });
        console.log('GG0170M_N listener added');
    })();
    (function GG0170MRules() {
        /* add rule help */
        const GG0170M_and_NruleTriggers = $('.branchingRule[data-target=GG0170M]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170N]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170O]:not([id*=Discharge_Goal])', formScope);
        const GG0170M_and_NruleText = 'When GG0170M Admission Performance or Discharge Performance is between 1 and 6, unlock only the matching N and O.  If M is unknown or 7 or greater, reset and lock matching N and O.';
        GG0170M_and_NruleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', GG0170M_and_NruleText).show()
                .on("click", function () {
                BranchingRuleDialog(GG0170M_and_NruleText);
            });
        });
    })();
    /* self executing change event listener and handler */
    (function GG0170Q_addListner() {
        console.log('adding GG0170Q_addListner()');
        //no need to raise onload event, it is only triggered by Q12_Q23 change event chain
        console.log('process GG0170Rs');
        const GG0170Qs = $('.persistable[id^=GG0170Q]', formScope);
        GG0170Qs.each(function () {
            const thisGG0170Q = $(this);
            thisGG0170Q.on('change', function () {
                const theseNoQs = $('.persistable[id^=GG0170Q][data-codesetdescription*=No]:checked', formScope);
                if (theseNoQs.length !== 0) {
                    //if Q is no, lock pairing gg0170R, RR, S, SS
                    theseNoQs.each(function () {
                        const thisNo = $(this);
                        const thisNoMeasure = thisNo.data('measuredescription').replace(" ", "_");
                        const pairingGG0170No = $('.persistable[id ^= GG0170R][id *= ' + thisNoMeasure + '], .persistable[id ^= GG0170S][id *= ' + thisNoMeasure + ']', formScope);
                        console.log('lock corresponding ' + thisNoMeasure + ' GG0170R, GG0170RR, GG0170S, and GG0170SS');
                        pairingGG0170No.prop('disabled', true).val(-1).siblings('.longTextOption ,.score').text('');
                        const completes = $('.persistable[id^= Assessment]', formScope);
                        completes.each(function () {
                            const thisComplete = $(this);
                            thisComplete.prop('disabled', false);
                        });
                    });
                }
                const theseYesQs = $('.persistable[id^=GG0170Q][data-codesetdescription*=Yes]:checked', formScope);
                if (theseYesQs.length !== 0) {
                    theseYesQs.each(function () {
                        const thisYes = $(this);
                        //if Q is yes, unlock pairing GG0710R, RR, S, SS
                        const thisYesMeasure = thisYes.data('measuredescription').replace(" ", "_");
                        const paringGG0170Yes = $('.persistable[id ^= GG0170R][id *= ' + thisYesMeasure + '], .persistable[id ^= GG0170S][id *= ' + thisYesMeasure + ']', formScope);
                        console.log('enable corresponding GG0170R_' + thisYesMeasure);
                        paringGG0170Yes.prop('disabled', false).siblings('.longTextOption ,.score').text('');
                    });
                }
            });
        });
        console.log('GG0170Q listner added');
    })();
    (function GG0170QRules() {
        /* add rule help */
        const GG0170QRuleTriggers = $('.branchingRule[data-target^=GG0170Q], .branchingRule[data-target^=AssessmentCompleted]');
        const GG0170QRuleText = 'If any GG0170Q is NO, unlock the Complete Assessment.';
        GG0170QRuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', GG0170QRuleText).show()
                .on("click", function () {
                BranchingRuleDialog(GG0170QRuleText);
            });
        });
    })();
    /* event handler */
    function J1750_depends_on_J0510_or_J0520(eventType, byRef) {
        console.log('inside of J1750_depends_on_J0510_or_J0520, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        const J1750_yes = $('.persistable[id^=J1750][data-codesetdescription*=Yes]', formScope);
        J1750_yes.each(function () {
            const thisJ1750_yes = $(this);
            thisJ1750_yes.prop('disabled', false);
        });
        if (eventType === EnumChangeEventArg.Change) {
            console.log('scroll to ' + J1750_yes.prop('id'));
            //commonUtility.scrollTo(J1750_yes.prop('id'));
            //J1750_yes.parent('focus');
        }
        console.log('------ done handling J0510 ' + eventType + '------');
        //no dialog is used so just return the original value of byRef.seenTheDialog
        return byRef.seenTheDialog;
    }
    /* self executing event listener */
    (function J0510_J0520_addListener() {
        console.log('adding J0510_J0520_addListener()');
        let seenTheDialog = true;
        //const this_has_Not_Apply: boolean = $(".persistable[id=" + thisJ0510.prop('id') + "] option:selected").text().indexOf('0. Does not apply') !== -1;
        //const Does_not_apply: any = thisJ0510.find('option[text="0. Does not apply"]');
        //let J0510firstDoesNotApply: any;
        const J0510_J0520 = $('.persistable[id^=J0510], .persistable[id^=J0520]');
        J0510_J0520.each(function () {
            const thisJ = $(this);
            thisJ.on('change', function () {
                const selectedOptions = $(':selected', thisJ);
                selectedOptions === null || selectedOptions === void 0 ? void 0 : selectedOptions.each(function () {
                    const thisSelectedOption = $(this).text();
                    const rgxPattern = /0. /;
                    if (rgxPattern.test(thisSelectedOption)) {
                        console.log('found "Does not apply in ' + thisJ.prop('id') + ', advance to J1750 Yes');
                        seenTheDialog = J1750_depends_on_J0510_or_J0520(EnumChangeEventArg.Change, { seenTheDialog: seenTheDialog });
                        return false; //break out selectedOptions.each()
                    }
                    //no locking per 07/27/2022 email
                    //else {
                    //    $('[id^=J1750]', thisJ).prop('disabled', true);
                    //}
                });
            });
        });
        console.log('J05X0 listener added');
    })();
    (function J05X0Rules() {
        /* add rule help diaglog */
        const J05X0RuleTriggers = $('.branchingRule[data-target^=J0510], .branchingRule[data-target^=J0520],.branchingRule[data-target^=J1750]');
        const J05X0RuleText = 'When J0510 or J0520 is "0. Does not apply", set focus on J1750 Yes.';
        J05X0RuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', J05X0RuleText).show()
                .on("click", function () {
                BranchingRuleDialog(J05X0RuleText);
            });
        });
    })();
    (function triggerChangeChain() {
        console.log('trigger Q23 change');
        /* event handler, add this last to raise change chain
           set focus on Q23 because triggers in the change chain might set focus on their respective target */
        $('.persistable[id^=Q23]', formScope).trigger("change").trigger("focus");
    })();
});
//# sourceMappingURL=branching.js.map