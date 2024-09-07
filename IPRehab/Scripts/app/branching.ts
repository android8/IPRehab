﻿import { event } from "jquery";
import { setObserversEnabled } from "./../../node_modules/@material/base/observer.js";
import { isScrollAtBottom } from "./../../node_modules/@material/dialog/util.js";
import { Utility, EnumDbCommandType } from "./commonImport.js";

/* jquery plugin dependsOn*/
/* https://dstreet.github.io/dependsOn */
/* http://emranahmed.github.io/Form-Field-Dependency */

enum EnumChangeEventArg {
    Load = 'Load',
    Change = 'Change',
    NoScroll = 'NoScroll'
}

//const branchingController = 
$(function () {
    const commonUtility: Utility = new Utility();
    let defaultDialogOptions = commonUtility.dialogOptions();
    //let defaultDialogOptions: any = {
    //    resizable: true,
    //    //height: ($(window).height() - 200),
    //    //width: '90%',
    //    classes: { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' },
    //    modal: true,
    //    stack: true,
    //    sticky: true,
    //    position: { my: 'center', at: 'center', of: window },
    //    buttons: [
    //        //{
    //        //  text: "save",
    //        //  icon: "ui-icon-disk",
    //        //  click:  function () {
    //        //      do something here
    //        //      let thisUrl: string = $('form').prop('action');
    //        //      let postBackModel: AjaxPostbackModel = new AjaxPostbackModel();
    //        //      postBackModel.NewAnswers = newAnswers;
    //        //      postBackModel.OldAnswers = oldAnswers;
    //        //      postBackModel.UpdatedAnswers = updatedAnswers;
    //        //      alert('ToDo: sending ajax postBackModel to ' + thisUrl);
    //        //  }
    //        //},
    //        //{
    //        //  text: "Ok",
    //        //  icon: "ui-icon-check",
    //        //  click: function () {
    //        //    let $this = $(this);
    //        //    $this.dialog("close");
    //        //  }
    //        //},
    //        {
    //            text: "Close",
    //            icon: "ui-icon-close",
    //            click: function () {
    //                $(this).dialog("close");
    //            },
    //            open: function (event, ui) {
    //                $('.ui-widget-overlay').css({
    //                    'opacity': '0.5',
    //                    'filter': 'Alpha(Opacity = 50)',
    //                    'background-color': 'black'
    //                });
    //            },
    //        }]
    //};
    const formScope: any = $("form#userAnswerForm");
    const Q12: any = $('.persistable[id^=Q12]', formScope);
    const Q23: any = $('.persistable[id^=Q23]', formScope);

    $('.persistable').on('change', function (e) {
        const Q12_or_Q23_is_empty: boolean = commonUtility.isEmpty(Q12) || commonUtility.isEmpty(Q23);
        if (!Q12_or_Q23_is_empty)
            $('#ajaxPost').removeAttr('disabled');
    });

    function BranchingRuleDialog(thisRule: string) {
        let formDialog: any = $("#dialog")
            .html(thisRule)
            .dialog(defaultDialogOptions,
                {
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

    function scrollTo1(elementId: string) {
        console.log('branching::: scrollTo() elementId = ', elementId);

        const thisElement: any = $('#' + elementId, formScope);

        let scrollAmount: number = 0;

        if (thisElement.length > 0) {
            //scrollAmount = thisElement.prop('offsetTop');
            scrollAmount = (thisElement.offset() || { "top": NaN }).top;

            console.log('scroll to ' + elementId + ', amount ' + scrollAmount, thisElement);

            $('html,body').stop().animate({ scrollTop: scrollAmount }, 'fast');
            thisElement.trigger('focus');
        }
        else {
            console.log(elementId + " doesn't exist in the current context, can not scroll to that element");
        }
    }

    function scrollTo(elementId: string) {
        const element = document.querySelector('#' + elementId);

        // Get the size and position of our element in the viewport
        const rect = element.getBoundingClientRect();
        // The top offset of our element is the top position of the element in the viewport plus the amount the body is scrolled
        let offsetTop = rect.top + document.body.scrollTop;
        console.log('scroll to ' + elementId + ' with amount ' + offsetTop);

        // Now we can scroll the window to this position
        window.scrollTo(0, offsetTop - 15);
        document.getElementById(elementId).focus(); /* use focus() because this is not a jquery element */
    }

    /* event handler */
    function Q23_blank_then_Lock_All(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }): boolean {
        console.log('Q23 is fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        const otherPersistables: any = $('.persistable', formScope).not(Q12).not(Q23);
        const minDate = new Date('2020-01-01 00:00:00');
        const onsetDate = new Date(Q23.val());
        const admitDate = new Date(Q12.val());
        const Q12_or_Q23_is_empty: boolean = commonUtility.isEmpty(Q12) || commonUtility.isEmpty(Q23);
        const onset_is_later_than_admit: boolean = onsetDate < minDate || admitDate < minDate || admitDate < onsetDate;

        Q23.prop('disabled', false);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }

        switch (true) {
            case (Q12_or_Q23_is_empty):
            case (onset_is_later_than_admit): {
                console.log('Q12 or Q23 is empty, or onset day is later than admit date, lock all other questions');

                $('#ajaxPost').prop('disabled', true);

                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    console.log('with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);

                    let dialogText: string;

                    if (Q12_or_Q23_is_empty)
                        dialogText = 'Onset Date and Admit Date are record keys, when either is blank, all fields with current values will be locked';
                    if (onset_is_later_than_admit)
                        dialogText = 'Onset Date and Admit Date must be later than 01/01/2021, and Admit Date must be on Onset Date or later';

                    defaultDialogOptions.title = 'Date';
                    const myButtons = {
                        "Ok": function () {
                            console.log('lock all other persistables');
                            otherPersistables.each(function () {
                                let thisPersistable = $(this);
                                thisPersistable.prop("disabled", true);
                            }
                            );
                            $(this).dialog("close");
                            setSeenTheDialog(true); //callback function
                        },
                        "Cancel": function () {
                            $(this).dialog("close");
                            setSeenTheDialog(true); //callback function
                        }
                    };
                    $("#dialog").html(dialogText)
                        .dialog(defaultDialogOptions, {
                            title: 'Warning Q12 Q23',
                            buttons: myButtons
                        });
                }
                else {//without warning dialog
                    console.log('Q12 or Q23 without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);

                    console.log('lock all other persistables');
                    otherPersistables.each(function () {
                        const thisPersistable = $(this);
                        thisPersistable.prop('disabled', true);
                    });
                }
                break;
            }
            default: {
                //see branchingTree.txt

                const triggers = $('.persistable[id ^= Q12B], .persistable[id ^= Q14A], .persistable[id ^= Q16A], .persistable[id ^= Q42], .persistable[id ^= Q44C]'
                    + ', .persistable[id ^= Q44D], .persistable[id ^= GG0170I], .persistable[id ^= GG0170M], .persistable[id ^= GG0170Q], .persistable[id ^= J0510], .persistable[id ^= J0520]', formScope);

                const triggerTargets =
                    $('.persistable[id ^= Q14B], .persistable[id ^= Q16B], .persistable[id ^= Q17], .persistable[id ^= Q21B], .persistable[id ^= Q41]'
                        + ', .persistable[id ^= Q43], .persistable[id ^= Q44C], .persistable[id ^= Q44D], .persistable[id ^= Q45], .persistable[id ^= Q46]'
                        + ', .persistable[id ^= GG0170I]:not([id *= Discharge_Goal]), .persistable[id ^= GG0170J]:not([id *= Discharge_Goal])'
                        + ', .persistable[id ^= GG0170K]:not([id *= Discharge_Goal]), .persistable[id ^= GG0170L]:not([id *= Discharge_Goal])'
                        + ', .persistable[id ^= GG0170N]:not([id *= Discharge_Goal]), .persistable[id ^= GG0170O]:not([id *= Discharge_Goal])'
                        + ', .persistable[id ^= GG0170R]:not([id *= Discharge_Goal])'
                        + ', [id ^= Complete]'
                        , formScope);

                const unlockOnload = $('.persistable', formScope).not(triggerTargets).not(Q12).not(Q23).not('.summary header');
                //enable handlerless controls but don't raise change event to prevent infinite loop
                console.log(unlockOnload.length + ' fields to be unlocked (disabled = false)');
                unlockOnload.each(function () {
                    const scopedControl = $(this);
                    scopedControl.prop('disabled', false);
                    console.log(scopedControl.prop('id') + ' disabled ' + scopedControl.prop('disabled'));
                });

                //not('[id^=Q12]', formScope) so it doesn't raise change to cause infinite loop
                //only raise change for the following to unlock or remain lock of the respective fields
                triggers.each(function () {
                    const thisTrigger = $(this);
                    thisTrigger.prop('disabled', false);//.trigger('change');
                    console.log(thisTrigger.prop('id') + ' disabled');
                });

                $('#ajaxPost').prop('disabled', false);

                break;
            }
        }

        console.log('------ done handling Q12 Q23 ' + eventType + '------');
        return byRef.seenTheDialog;
    }

    /* self executing event listener, add this last to raise change chain */
    (function Q23_addListener() {
        console.log('adding Q23_addListener()');

        /* add onchange event listner */
        let seenTheDialog: boolean = false;

        Q23.on('change', { x: EnumChangeEventArg.Change }, function (e) {
            console.log('onchange calling Q23_blank_then_Lock_All(), seenTheDialog = ', seenTheDialog);
            //checkQ12_Q23(e.data.x);
            seenTheDialog = Q23_blank_then_Lock_All(EnumChangeEventArg.Change, { seenTheDialog: seenTheDialog });
        });

        /* add rule help */
        const Q12_Q23ruleTriggers = $('.branchingRule[data-target=Q12], .branchingRule[data-target=Q23]', formScope);
        const Q12_Q23rule = 'If Q12 or Q23 is blank or Q12 date is later than Q23 date, lock all inputs and the save button.'
        Q12_Q23ruleTriggers.each(function () {

            let thisTrigger = $(this);
            thisTrigger.prop('title', Q12_Q23rule).show();
            thisTrigger.on('click', function () {
                BranchingRuleDialog(Q12_Q23rule);
            });
        });
        console.log('Q12_Q23 listener added');
    })();

    /* event handler */
    function Q12B_blank_then_Lock_Discharge(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }): boolean {
        //event hooked during checkAllRules()
        const Q12B: any = $('.persistable[id^=Q12B]', formScope);
        console.log(Q12B.prop('id') + ' fired by ' + eventType + " with seenTheDalog = " + byRef.seenTheDialog);

        const Q12: any = $('.persistable[id^=Q12]', formScope);
        const admitDate: Date = new Date(Q12.val());
        const dischargeDate: Date = new Date(Q12B.val());
        const isDischarged: boolean = dischargeDate >= admitDate;

        function actQ12B(thisDisabled: boolean) {

            const dischargeRelatedDropdown: any = $('.persistable[id^=Q16B],.persistable[id^=Q17B], .persistable[id^=Q21B]', formScope);
            const dischargeRelatedCheckboxes: any = $('.persistable[id^=Q41],.persistable[id^=Q44C]', formScope);

            //enable or disable related .persistable elements
            dischargeRelatedDropdown.each(function () {
                const thisDropdown = $(this);
                if (thisDisabled) {
                    thisDropdown.val(-1);
                    thisDropdown.siblings('.longTextOption').text('');
                }
                thisDropdown.prop('disabled', thisDisabled);
                console.log(thisDropdown.prop('id') + ' disabled = ' + thisDisabled, thisDropdown);
            });

            dischargeRelatedCheckboxes.each(function () {
                const thisCheckbox: any = $(this);
                thisCheckbox.prop('disabled', thisDisabled);
                console.log(thisCheckbox.prop('id') + thisCheckbox.prop('disabled').length, thisCheckbox);
                if (thisDisabled)
                    thisCheckbox.prop('checked', false);
            });
        }

        let dialogText: string;

        if (isDischarged) {
            //without warning dialog
            actQ12B(false);
        } else {
            if (Q12B.val() === null || Q12B.val() === '') {
                //lock all discharge related fields without dialog
                actQ12B(true);
            }
            else {
                //lock all discharege related fields with
                dialogText = 'Q12B is clearred, is an invalid date, or is earlier than the admit date, rseset and locks related discharge fields:  Q16B, Q17B, Q21B, Q41, and Q44C';

                console.log(dialogText);

                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    function setSeenTheDialog(value) {
                        //callback after async dialog is done and return the seenTheDialog to the caller
                        byRef.seenTheDialog = value;
                    }
                    function DischargeRelatedButtonClosure() {
                        return function (dischargeState: boolean) {
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
                            }
                        }
                    }

                    //with warning dialog
                    const Q12Bbuttons = DischargeRelatedButtonClosure();

                    $("#dialog").html(dialogText)
                        .dialog(defaultDialogOptions, {
                            title: 'Warning Q12B', buttons: Q12Bbuttons(true)
                        });
                    console.log('Q12B handler return byRef.seenTheDialog = ', byRef.seenTheDialog);
                }
            }
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

        let seenTheDialog: boolean = false;

        /* on change */
        Q12B.on('change', { x: EnumChangeEventArg.Change, y: Q12B }, function (e) {
            console.log('before calling Q12B_blank_then_Lock_Discharge(), seenTheDialog = ', seenTheDialog);
            //JavaScript, and TypeScript can pass an object by reference, but not by value.
            //Therefore box values into an object  { seenTheDialog: seenTheDialog }
            seenTheDialog = Q12B_blank_then_Lock_Discharge(e.data.x, { seenTheDialog: seenTheDialog });
        });

        /* add rule help */
        const Q12BRuleTargets = $('.branchingRule[data-target^=Q12B],[data-target^=Q16B],[data-target^=Q17B],[data-target^=Q21B],[data-target^=Q41],[data-target^=Q44C]');
        const ruleText = 'If Q12B is not a date, reset and lock related discharge fields: Q16B, Q17B, Q21B, Q41, Q44C.';

        Q12BRuleTargets.each(function () {
            $(this).prop('title', ruleText);
            $(this).on('click', function () {
                $("#dialog").html(ruleText)
                    .dialog(defaultDialogOptions, {
                        title: 'Rules', buttons: {
                            'Ok': function () {
                                $(this).dialog("close");
                            }
                        }
                    });
            })
        })
        console.log('Q12B listener added');
    })();

    /* event handler */
    function Q14B_enabled_if_Q14A_is_Yes(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }): boolean {
        console.log('Q14A handler fired by ' + eventType + " with seenTheDalog = " + byRef.seenTheDialog);

        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }

        function actQ14A(isDisable14B: boolean) {
            const Q14Bs: any = $('.persistable[id^=Q14B]', formScope);
            Q14Bs.each(function () {
                const thisQ14B = $(this);
                thisQ14B.prop('disabled', isDisable14B);
                console.log(thisQ14B.prop('id') + ' disabled = ' + isDisable14B);
                if (isDisable14B) {
                    thisQ14B.prop('checked', false);   //uncheck Q14Bs
                }
            });
        }

        const Q14AYes: boolean = $('.persistable[id^=Q14A][id*=Yes]:checked', formScope).length === 1;

        if (Q14AYes) {
            const isDisable14B: boolean = false;
            /* without warning dialog */
            console.log('Q14A is Yes, uncheck and unlock all Q14B, no dialog');
            actQ14A(isDisable14B);
        }
        else {
            let isDisable14B: boolean = true;
            const dialogText = 'Q14A is unknown or is No, uncheck and lock all Q14B';
            console.log(dialogText);

            //with warning dialog
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                console.log('Q14A with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);

                const myButtons = {
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
                        title: 'Warning 14A', buttons: myButtons
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
            let seenTheDialog: boolean = true;

            /* on change. No need to raise onload event, it is only triggered by Q12_Q23 change event chain */
            thisQ14A.on('change', { x: EnumChangeEventArg.Change, y: thisQ14A }, function (e) {
                console.log('before calling Q14B_enabled_if_Q14A_is_Yes(), seenTheDialog = ', seenTheDialog);
                seenTheDialog = Q14B_enabled_if_Q14A_is_Yes(e.data.x, { seenTheDialog: seenTheDialog });
            });
        });

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
        console.log('Q14A listener added');
    })();

    /* event handler */
    function Q16A_is_Home_then_Q17(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }, Q16A: any): boolean {
        //event hooked during checkAllRules()
        console.log(Q16A.prop('id') + ", fired by " + eventType + " with seenTheDalog = " + byRef.seenTheDialog);

        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }

        function actQ16A(isDisableQ17: boolean) {
            const Q17: any = $(".persistable[id^=Q17]", formScope);
            if (Q17.length > 0) {
                Q17.prop('disabled', isDisableQ17);
                console.log(Q17.prop('id') + 'disabled ' + Q17.prop('disabled'));
                if (isDisableQ17) {
                    //commonUtility.resetControlValue(Q17);
                    Q17.val(-1).siblings('.longTextOption').text('');
                }
                else {
                    console.log('focus on Q17', Q17);
                    Q17.trigger('focus');
                }
            }
        }

        const Q16AisHome: boolean = $(".persistable[id^=Q16A] option:selected", formScope).text().indexOf('1. Home') !== -1;

        if (Q16AisHome) {
            const dialogText = 'Q16A is home, unlock Q17';
            console.log('Q16A without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
            const isDisableQ17: boolean = false;
            actQ16A(isDisableQ17);
        }
        else {
            const isDisableQ17: boolean = true;
            const dialogText = 'Q16A is not home, clear and lock Q17';
            console.log(dialogText);
            const myButtons = {
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
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                console.log('Q16A with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                $("#dialog").html(dialogText)
                    .dialog(defaultDialogOptions, {
                        title: 'Warning Q16A, Q17', buttons: myButtons
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
    (function Q16_addListener() {
        const Q16A = $('.persistable[id^=Q16A]', formScope);
        console.log('adding ' + Q16A.prop('id') + ' listener()');

        //no need to raise onload event, it is only trigger by Q12_Q23 change event chain

        /* on change */
        let seenTheDialog: boolean = true;
        Q16A.on('change', { x: EnumChangeEventArg.Change }, function (e) {
            console.log('before calling Q16A_is_Home_then_Q17(), seenTheDialog = ', seenTheDialog);
            seenTheDialog = Q16A_is_Home_then_Q17(e.data.x, { seenTheDialog: seenTheDialog }, $(this));
        });

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
        console.log('Q16 listener added');
    })();

    /* event handler, not used per stakeholder request */
    function Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }) {
        //event hooked during checkAllRules()
        console.log('inside of Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);

        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }

        /* nested event handler */
        function checkArthritis(e) {
            console.log('inside of checkArthritis(e) $(this)', $(this));
            const thisICD = $(this);
            const myButtons = {
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
                if (e.data?.x === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    $("#dialog").html('Not an arthritis ICD, lock Q24A')
                        .dialog(defaultDialogOptions, {
                            title: 'Warning Q21A, Q21B, Q22, Q24', buttons: myButtons
                        });
                }
                else {
                    $('.persistable[id^=Q24A]', formScope).prop('checked', false);
                }
            }
        }

        /* on change */
        console.log('check Q24A if Q21A, Q21B, Q22, or Q24 is diabetes');
        const arthritis = $('.persistable[id^= Q21A], .persistable[id^= Q21B], .persistable[id^= Q22], .persistable[id^= Q24]', formScope);
        arthritis.each(function () {
            $(this).on('change', { x: EnumChangeEventArg.Change }, checkArthritis);
        });

        console.log('------ done handling Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A ' + eventType + '------');
    }

    /* event handler */
    function Q42_Interrupted_then_Q43(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }): boolean {
        //event hooked during checkAllRules()
        console.log('inside of Q42_Interrupted_then_Q43, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);

        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }

        function actQ42(isDisabledQ43: boolean) {
            const Q43s: any = $('.persistable[id^=Q43]', formScope);
            Q43s.each(function () {
                const thisQ43 = $(this)
                thisQ43.prop('disabled', isDisabledQ43);
                thisQ43.prop('checked', !isDisabledQ43);

                const thisDateResetButton = $('button.calendarReset[data-target=' + thisQ43.prop('id') + ']');
                if (thisDateResetButton.length !== 0) {
                    thisDateResetButton.prop('disabled', !isDisabledQ43);
                }
                console.log(thisQ43.prop('id') + ' disabled = ' + thisQ43.prop('disabled'));
            });
        }

        let Q42Yes: boolean = $('.persistable[id^=Q42][id*=Yes]:checked').length === 1;

        if (Q42Yes) {
            const isDisableQ43: boolean = false;
            actQ42(isDisableQ43);
        }
        else {
            const isDisableQ43: boolean = true;
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                //with warning dialog
                console.log('Q42 with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                const myButtons = {
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

                $("#dialog").html('Q42 is a No, reset and lock all Q43')
                    .dialog(defaultDialogOptions, {
                        title: 'Warning Q42, Q43', buttons: myButtons
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
        const Q42s: any = $('.persistable[id^=Q42]', formScope);
        Q42s.each(function () {
            let thisQ42: any = $(this);
            thisQ42.on('change', { x: EnumChangeEventArg.Change, y: thisQ42 }, function (e) {
                console.log('before calling Q42_Interrupted_then_Q43(), seenTheDialog = ', seenTheDialog);
                seenTheDialog = Q42_Interrupted_then_Q43(e.data.x, { seenTheDialog: seenTheDialog });
            })
        });

        /* add rule help */
        const Q42RuleTriggers = $('.branchingRule[data-target^=Q42], .branchingRule[data-target^=Q43]');
        const Q42RuleText = 'If Q42 is a No, reset and lock all Q43.';
        Q42RuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', Q42RuleText).show()
                .on("click", function () {
                    BranchingRuleDialog(Q42RuleText);
                });
        });

        console.log('Q42 listener added');
    })();

    /* event handler */
    function Q43_Rules(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }, thisQ43: any): object {
        console.log('inside of Q43_Rules(), fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog, thisQ43);
        let foundBlank = false;

        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }

        function actQ43s(CRUD: EnumDbCommandType, thisQ43: any): boolean {
            switch (CRUD) {
                case EnumDbCommandType.Delete: {
                    console.log('D', thisQ43);
                    const OtherQ43s: any = $('.persistable[id^=Q43][id!=' + thisQ43.prop('id') + ']')
                    OtherQ43s.each(function () {
                        const thisOtherQ43 = $(this);
                        if (thisOtherQ43.val() === '')
                            foundBlank = true;
                    });
                    thisQ43.prop('disabled', true); //don't trigger change, otherwise it will cause infinite Q43_Rules() loop
                    thisQ43.next('.bi-calendar-x').prop('diabled', true);
                    break;
                }
                case EnumDbCommandType.Create:
                case EnumDbCommandType.Update: {
                    console.log('CU', thisQ43);
                    const blankQ43s = $('.persistable[id^=Q43][value=""]');
                    if (blankQ43s.length > 0) {
                        const firstBlank = blankQ43s.first();
                        firstBlank.prop('disabled', false).trigger('focus'); /* unlock this non-blank dates */
                        firstBlank.next('.bi-calendar-x').prop('disabled', false);  /* unlock next reset button */
                        foundBlank = true;
                    }
                    thisQ43.prop('disabled', false);  //don't trigger change, otherwise it will cause infinite Q43_Rules() loop
                    thisQ43.next('.bi-calendar-x').prop('diabled', false);
                    break;
                }
            }
            return foundBlank;
        }

        const Q42Yes: boolean = $('.persistable[id^=Q42][id*=Yes]:checked', formScope).length === 1;
        const Q42No: boolean = $('.persistable[id^=Q42][id*=No]:checked', formScope).length === 1;
        const thisQ43CRUD = commonUtility.getCRUD(thisQ43, thisQ43.data('oldvalue'), thisQ43.val());

        function Q42ButtonsClosure(Q42Yes: boolean) {
            return function (thisQ43: any, thisQ43CRUD: EnumDbCommandType) {
                return {
                    "Ok": function () {
                        if (Q42Yes) {
                            actQ43s(thisQ43CRUD, thisQ43);
                        }
                        else {
                            thisQ43.val('').prop('disabled', true); //don't trigger change, otherwise it will cause infinite Q43_Rules() loop
                        }
                        setSeenTheDialog(true); //callback
                        $(this).dialog("close");
                    },
                    "Cancel": function () {
                        setSeenTheDialog(true);  //callback
                        $(this).dialog("close");
                    }
                };
            }
        }

        switch (true) {
            case Q42Yes: {
                console.log("thisQ43CRUD ? " + thisQ43CRUD === undefined ? "unchanged" : thisQ43CRUD);
                //without warning dialog
                console.log('Q42Yes without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);

                foundBlank = actQ43s(thisQ43CRUD, thisQ43);

                break;
            }
            case Q42No: {
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning 
                    console.log('Q42No with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                    const buttonQ42No = Q42ButtonsClosure(Q42No);

                    $("#dialog").html('Find next blank Q43 for editing')
                        .dialog(defaultDialogOptions, {
                            title: 'Warning Q43', buttons: buttonQ42No(thisQ43, thisQ43CRUD)
                        });
                }
                else {
                    //without warning dialog
                    console.log('Q42No without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                    thisQ43.val('');
                    thisQ43.prop('disabled', true); //don't triggere change, otherwise it will cause infinite Q43_Rules() loop
                }
                break;
            }
        }

        console.log('------ done handling Q43 ' + eventType + '------');

        //return boexed values 
        return { foundBlank: foundBlank, seenTheDialog: byRef.seenTheDialog };
    }

    /* self executing event listener */
    (function Q43_addListener() {
        console.log('adding Q43_addListener()');

        //no need to raise onload event, it is only triggered by Q12_Q23 change event chain

        /* on change. Q43 only triggered by change event manually here or programmmatically in Q42*/

        let seenTheDialog = true;
        let foundBlank: boolean;

        const Q43s: any = $('.persistable[id^=Q43]', formScope);
        Q43s.each(function () {
            if (!foundBlank) {
                const thisQ43 = $(this);
                thisQ43.on('change', { x: EnumChangeEventArg.Change, y: thisQ43 },
                    function (e) {
                        console.log('before calling Q43_Rules(), foundBlank = ', foundBlank, ' seeTheDialog = ', seenTheDialog);
                        const returnedObject: any = Q43_Rules(e.data.x, { seenTheDialog: seenTheDialog }, thisQ43);
                        foundBlank = returnedObject.foundBlank;
                        seenTheDialog = returnedObject.seenTheDialog;
                    }
                );
            }
        });

        /* no need to add rule help which is added with Q42 */

        console.log('Q43 listener added');
    })();

    /* event handler */
    function Q44C_Affect_Q44D_Q45_Q46(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }): boolean {
        //event hooked during checkAllRules()
        console.log('inside of Q44C_Affect_Q44D_Q46, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);

        const Q44C_is_Yes: boolean = $('.persistable[id^=Q44C][id*=Yes]:checked, .persistable[id^=Q44C-FollowUp][id*=Yes]:checked', formScope).length === 1;
        const Q44D: any = $('.persistable[id^=Q44D]', formScope);
        const Q45: any = $('.persistable[id^=Q45]', formScope);
        const Q46: any = $('.persistable[id^=Q46]', formScope);

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
            Q46.val(-1).prop('disabled', false).trigger('focus');
        }

        /* nested event handler */
        const myButtons = {
            "Ok": function () {
                act44C_is_no();
                setSeenTheDialog(true); //callback
                $(this).dialog("close");
            },
            "Cancel": function () {
                setSeenTheDialog(true); //callback
                $(this).dialog("close");
            }
        }

        if (Q44C_is_Yes) {
            console.log('Q44C is yes, unlock Q44D, Q45 and focus on Q44D');
            if (Q45.length > 0) {
                Q45.prop('disabled', false)
            }
            if (Q44D.length > 0) {
                Q44D.prop('disabled', false);
            }
        }
        else {
            if (Q44D.length > 0 && Q45.length > 0 && Q46.length > 0) {
                let dialogText: string = 'Q44C is no, lock Q44D, Q45 and focus on Q46';
                console.log(dialogText);
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    console.log('Q44C with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);

                    $("#dialog").html(dialogText)
                        .dialog(defaultDialogOptions, {
                            title: 'Warning Q44C, Q44D, Q45, Q46', buttons: myButtons
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
        let seenTheDialog: boolean = true;
        const Q44C = $('.persistable[id^=Q44C]', formScope);
        Q44C.each(function (i, el) {
            const thisQ44C = $(el); //don't use $(this) because in the arrow function it will be undefined
            thisQ44C.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                console.log('before calling Q44C_Affect_Q44D_Q45_Q46(), seenTheDialog = ', seenTheDialog);
                seenTheDialog = Q44C_Affect_Q44D_Q45_Q46(e.data.x, { seenTheDialog: seenTheDialog });
            });
        })

        /* add rule help */
        const Q44CRuleTriggers = $('.branchingRule[data-target^=Q44C], .branchingRule[data-target^=Q44D], .branchingRule[data-target^=Q45], .branchingRule[data-target^=Q46]', formScope);
        const Q44CFollowuUpRuleTriggers = $('.branchingRule[data-target^=Q44C-FollowUp],[data-target^=Q44C-Follow_Up],[data-target^=Q44C_FollowUp],[data-target^=Q44C_Follow_Up]', formScope);
        const Q44CFollowuUpRuleText = 'If Q44C is yes, unlock Q44D, Q45 and focus on Q44D. If Q44C is no, lock Q44D, Q45 and focus on Q46';

        Q44CRuleTriggers.each(function () {
            let thisTrigger = $(this);
            let thisDescription: string = thisTrigger.prop('title');
            thisTrigger.prop('title', thisDescription).show()
                .on("click", function () {
                    BranchingRuleDialog(thisDescription);
                });
        });

        Q44CFollowuUpRuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', Q44CFollowuUpRuleText).show()
                .on("click", function () {
                    BranchingRuleDialog(Q44CFollowuUpRuleText);
                });
        });

        console.log('Q44 listener added');
    })();

    /* event handler */
    function Q44D_Affect_Q45(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }, thisQ44D): boolean {
        console.log('inside of Q44D_Affect_Q45, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);

        const Q45: any = $('.persistable[id^=Q45]', formScope);
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
        const Q44D: any = $('.persistable[id^=Q44D]', formScope);
        Q44D.on('change', { x: EnumChangeEventArg.Change }, function (e) {
            console.log('before calling Q44C_Affect_Q44D_Q45_Q46(), seenTheDialog = ', seenTheDialog);
            seenTheDialog = Q44D_Affect_Q45(e.data.x, { seenTheDialog: seenTheDialog }, Q44D);
        });

        /* no need to add rule help, it is added in Q44C */
    })();

    /* event handler */
    function GG0170JKLMN_depends_on_GG0170I(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }, thisI: any, measure: string): boolean {
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

        function actGG0170I(focusThis: any, isDisabled: boolean, eventArg: EnumChangeEventArg) {
            const GG0170JKL: any = $('select.persistable[id^=GG0170J][id*=' + measure + '], select.persistable[id^=GG0170K][id*=' + measure + '], select.persistable[id^=GG0170L][id*=' + measure + ']');
            GG0170JKL.each(function () {
                if (isDisabled) {
                    commonUtility.resetControlValue($(this));
                }
                $(this).prop('disabled', isDisabled);
            });

            if (eventArg === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                if (focusThis?.length > 0) {
                    console.log('actGG0170I() scroll to ', focusThis?.prop('id'));
                    focusThis.prop('disabled', false);
                    //scrollTo(focusThis.prop('id'));
                }
            }
            else {
                if (focusThis?.length > 0) {
                    console.log('actGG0170I() no scroll but focus ', focusThis.prop('id'));
                    focusThis.prop('disabled', false);
                    //scrollTo(focusThis.prop('id'));
                    //focusThis.trigger('focus');
                }
                else
                    console.log(focusThis.prop('id') + " dosen't exist in this context, can not set focus on that element");
            }
        }

        let GG0170J: any, GG0170M: any;

        GG0170J = $('select.persistable[id^=GG0170J][id*=' + measure + ']');
        GG0170M = $('select.persistable[id^=GG0170M][id*=' + measure + ']');

        const intGG0170I: number = +commonUtility.getControlScore(thisI);
        switch (true) {
            case (intGG0170I > 0 && intGG0170I < 7): {
                /* unlock and clear J K L, skip to J */
                //let consoleLog: string = 'GG0170I ' + measure + ' is between 1 and 6, unlock GG0170J ' + measure + ', GG0170K ' + measure + ', GG0170L ' + measure + ', and advance to GG0170J ' + measure + '. Other measures are kept intact.';

                //without warning dialog
                console.log('GG0170I without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);

                const focusJ: any = GG0170J;
                const isDisableJKL = false;
                actGG0170I(focusJ, isDisableJKL, eventType);

                break;
            }
            default: {
                /* GG0170I is not selected, clear and lock J K L then advance to M */
                const dialogText = 'GG0170I ' + measure + ' is unknown or is not between 1 and 6, clear and lock GG0170J ' + measure + ', GG0170K ' + measure + 'and GG0170L ' + measure + ', then advance to GG0170M ' + measure;
                console.log('eventData ' + dialogText);

                const isDisableJKL = true;
                const focusM: any = GG0170M;
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    console.log('GG0170I with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);

                    const myButtons = {
                        "Ok": function () {
                            console.log('focusM = ', focusM);
                            actGG0170I(focusM, isDisableJKL, eventType);
                            setSeenTheDialog(true); //callback
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            setSeenTheDialog(true); //callback
                            $(this).dialog("close");
                        }
                    }

                    $("#dialog").html(dialogText)
                        .dialog(defaultDialogOptions, {
                            title: 'Warning GG0170JKLM', buttons: myButtons
                        });
                }
                else {
                    //without warning dialog
                    console.log('GG0170I without warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);
                    actGG0170I(focusM, isDisableJKL, eventType);
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
        let seenTheDialog: boolean = true;
        const GG0170I: any = $('select.persistable[id^=GG0170I]:not([id*=Discharge_Goal])');
        GG0170I.each(function () {
            const thisI = $(this);
            thisI.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                const theID = thisI.prop('id');
                let measure: string;
                switch (true) {
                    case (theID.indexOf('Admission') !== -1):
                        measure = 'Admission_Performance';
                        break;
                    case (theID.indexOf('Discharge') !== -1):
                        measure = 'Discharge_Performance';
                        break;
                    case (theID.indexOf('Interim') !== -1):
                        measure = 'Interim';
                        break;
                    case (theID.indexOf('Follow_Up') !== -1):
                        measure = 'Follow_Up';
                        break;
                }
                seenTheDialog = GG0170JKLMN_depends_on_GG0170I(e.data.x, { seenTheDialog: seenTheDialog }, thisI, measure);
            })
        });

        /* add rule help */
        const GG0170IruleTriggers = $('.branchingRule[data-target=GG0170I]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170J]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170K]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170L]:not([id*=Discharge_Goal])', formScope);
        const GG0170IruleText = 'If GG0170I Admission Performance or Discharge Performance is between 1 and 6, unlock the corresponding measure in J, K, L, and set focus on the corresponding measure in GG0170J. Other measures are unchanged. If the measure is 7 or greater, then reset and lock J, K, L, and set focus on the corresponding measure in GG0170M.';

        GG0170IruleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', GG0170IruleText).show()
                .on("click", function () {
                    BranchingRuleDialog(GG0170IruleText);
                });
        });

        console.log('GG0170I listener added');
    })();

    /* event handler */
    function GG0170P_depends_on_GG0170M_and_GG0170N(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }, thisMorN: any, measure: string): boolean {
        //event hooked during checkAllRules()
        console.log('inside of GG0170P_depends_on_GG0170M_and_GG0170N, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);

        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }

        let factor: number, M: any, N: any, O: any, P: any, dialogText: string;
        if (thisMorN.prop('id').indexOf('GG0170M') !== -1) {
            //only when thisMorN is GG0170M will M be assigned
            M = $('select.persistable[id^=GG0170M][id*=' + measure + ']', formScope);
            N = $('select.persistable[id^=GG0170N][id*=' + measure + ']', formScope);
            factor = +commonUtility.getControlValue(M);
            dialogText = 'M is unkown or M is greater than 7, reset and lock N and O then advance to P';
        }
        else {
            N = $('select.persistable[id^=GG0170N][id*=' + measure + ']', formScope);
            factor = +commonUtility.getControlValue(N);
            dialogText = 'N is unknow or N is greater than 7, reset and lock O then advance to P';
        }
        O = $('select.persistable[id^=GG0170O][id*=' + measure + ']', formScope);
        P = $('select.persistable[id^=GG0170P][id*=' + measure + ']', formScope);

        function act(controlValue: number) {
            return function () {
                if (M !== undefined && M !== null) {
                    const M_value: number = controlValue;
                    switch (true) {
                        case (M_value >= 7):
                            /* M >= 7, reset and lock both N and O then advance to P */
                            if (N.length > 0) {
                                //commonUtility.resetControlValue(GG0170N);
                                N.val(-1).prop('disabled', true);
                            }
                            if (O.length > 0) {
                                //commonUtility.resetControlValue(GG0170O);
                                O.val(-1).prop('disabled', true);
                            }
                            if (P.length > 0) {
                                P.prop('disabled', false);
                                //scrollTo(P.prop('id'));
                            }
                            break;
                        case (M_value > 0 && M_value < 7):
                            /* M between 0 and 6, reset and disable O then scroll to N */
                            if (O.length > 0) {
                                O.prop('disabled', true);
                            }
                            if (N.length > 0) {
                                N.prop('disabled', false);
                                //scrollTo(N.prop('id'));
                            }
                            break;
                        default:
                            /* M is unknown, lock N and O */
                            N.val(-1).prop('disabled', true);
                            O.val(-1).prop('disabled', true);
                            break;
                    }
                }
                else /* GG0170N */ {
                    const N_value: number = controlValue;
                    switch (true) {
                        case (N_value >= 7):
                            /* N >= 7, reset and lock both N and O then advance to P */
                            if (O.length > 0) {
                                //commonUtility.resetControlValue(GG0170O);
                                O.val(-1).prop('disabled', true);
                            }
                            if (P.length > 0) {
                                P.prop('disabled', false);
                                //scrollTo(P.prop('id'));
                            }
                            break;
                        case (N_value > 0 && N_value < 7):
                            /* N between 0 and 6, unlock and scroll to O */
                            if (O.length > 0) {
                                O.prop('disabled', false);
                                //scrollTo(O.prop('id'));
                            }
                            break;
                        default:
                            /* N is unknown, lock O */
                            if (O.length > 0) {
                                O.val(-1).prop('disabled', true);
                            }
                            break;
                    }
                }
            }
        }

        const myButtons = {
            "Ok": function () {
                (act(factor))();
                setSeenTheDialog(true); //callback
                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
                setSeenTheDialog(true); //callback
            }
        }

        if (eventType == EnumChangeEventArg.Change && !byRef.seenTheDialog) {
            /* with warning dialog */
            console.log('GG0170MN with warning dialog eventType = ' + eventType + 'seenTheDialog = ' + byRef.seenTheDialog);

            $("#dialog").html(dialogText)
                .dialog(defaultDialogOptions, {
                    title: 'Warning GG0170M, N, O, and P', buttons: myButtons
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

        /* on change */
        const GG0170M_and_N = $('select.persistable[id^=GG0170M]:not([id*=Discharge_Goal]), select.persistable[id^=GG0170N]:not([id*=Discharge_Goal])', formScope);
        GG0170M_and_N.each(function () {
            const thisMorN: any = $(this);
            thisMorN.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                let seenTheDialog: boolean = true;
                let measure: string = thisMorN.data('measuredescription');
                measure = measure.replace(/ /g, "_");

                console.log('before calling GG0170P_depends_on_GG0170M_and_GG0170N() seenTheDialog = ', seenTheDialog);
                seenTheDialog = GG0170P_depends_on_GG0170M_and_GG0170N(e.data.x, { seenTheDialog: seenTheDialog }, thisMorN, measure);
            });
        });

        /* add rule help */
        const GG0170M_and_NruleTriggers = $('.branchingRule[data-target=GG0170M]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170N]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170O]:not([id*=Discharge_Goal]), .branchingRule[data-target=GG0170P]:not([id*=Discharge_Goal])', formScope);
        const GG0170M_and_NruleText = 'If GG0170M Admission Performance or Discharge Performance is between 1 and 6, unlock the corresponding measure of the next question in sequence.  That is M opens N, and N opens O respectively.  If value is 7 or greater, N and O will be reset and locked, then cursor advances to P.';

        GG0170M_and_NruleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', GG0170M_and_NruleText).show()
                .on("click", function () {
                    BranchingRuleDialog(GG0170M_and_NruleText);
                });
        });

        console.log('GG0170M_N listener added');
    })();

    /* event handler for any GG0170Q no */
    function GG0170Q_is_No_skip_to_Complete(thisR: any, eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }): boolean {
        console.log('inside of GG0170Q_is_No_skip_to_Complete, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);

        const completed = $('.persistable[id^= Assessment]', formScope);
        const completed_Yes = $('.persistable[id^= Assessment][id*=Yes]', formScope);
        const myButtons = {
            "Ok": function () {
                thisR.prop('disabled', true);
                completed.each(function () {
                    const thisCompleted: any = $(this);
                    thisCompleted.prop('disabled', false);
                });
                //scrollTo(completed_Yes.prop('id'));
                byRef.seenTheDialog = true; //callback
                $(this).dialog("close");
            },
            "Cancel": function () {
                byRef.seenTheDialog = true; //callback
                $(this).dialog("close");
            }
        }

        const dialogText = 'The GG0170Q is a no, lock coresponding GG0170R, advance to Assesment Complete';
        console.log(dialogText);
        if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
            //with warning dialog
            $("#dialog").html(dialogText)
                .dialog(defaultDialogOptions, {
                    title: 'Warning GG0170Q', buttons: myButtons
                });
        }
        else {
            thisR.prop('disabled', true);
            completed.each(function () {
                const thisCompleted: any = $(this);
                thisCompleted.prop('disabled', false);
            });
            //scrollTo(completed_Yes.prop('id'));
            //completed_Yes.trigger('focus');
        }
        //onload don't scroll to complete 

        console.log('------ done handling GG0170Q ' + eventType + '------');

        return byRef.seenTheDialog;
    }

    /* event handler for any GG0170Q yes */
    function GG0170Q_is_Yes_skip_to_GG0170R(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }, thisGG0170R: any): boolean {
        console.log('inside of GG0170Q_is_Yes_skip_to_GG0170R, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);

        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }

        const myButtons = {
            "Ok": function () {
                thisGG0170R.prop('disabled', false);
                //scrollTo(thisGG0170R.prop('id'));
                setSeenTheDialog(true); //callback
                $(this).dialog("close");
            },
            "Cancel": function () {
                setSeenTheDialog(true); //callback
                $(this).dialog("close");
            }
        }

        if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
            //with warning dialog
            const dialogText = 'GG0170Q is yes, unlock and advance to corespondent GG0170Rs';
            console.log(dialogText);
            $("#dialog").html(dialogText)
                .dialog(defaultDialogOptions, {
                    title: 'Warning GG0170Q', buttons: myButtons
                });
        }
        else {
            //without warning dialog
            //thisGG0170R.prop('disabled', false).trigger('focus');
            thisGG0170R.prop('disabled', false);
            //scrollTo(thisGG0170R.prop('id'));
        }

        console.log('------ done handling GG0170Q ' + eventType + '------');

        return byRef.seenTheDialog;
    }

    /* self executing event listener */
    (function GG0170Q_addListner() {
        console.log('adding GG0170Q_addListner()');

        //no need to raise onload event, it is only triggered by Q12_Q23 change event chain

        /* on change */
        let seenTheDialog: boolean = true;
        const GG0170Qs: any = $('.persistable[id^=GG0170Q]:not([id*=label])', formScope);
        GG0170Qs.each(function () {
            let thisQ = $(this);
            thisQ.on('change', { x: EnumChangeEventArg.Change },
                function (e) {
                    const Q_admission_is_No: any = $('[id^=Admission][id*=No]:checked', thisQ).length > 0;
                    const Q_admission_is_Yes: any = $('[id^=Admission][id*=Yes]:checked', thisQ).length > 0;

                    const Q_discharge_is_No: any = $('[id^=Discharge][id*=No]:checked', thisQ).length > 0;
                    const Q_discharge_is_Yes: any = $('[id^=Discharge][id*=Yes]:checked', thisQ).length > 0;

                    const Q_interim_is_No: any = $('[id^=Interim][id*=No]:checked', thisQ).length > 0;
                    const Q_interim_is_Yes: any = $('[id^=Interim][id*=Yes]:checked', thisQ).length > 0;

                    const Q_followUp_is_No: any = $('[id^=FollowUp][id*=No]:checked, [id^=Follow_Up][id*=No]:checked', thisQ).length > 0;
                    const Q_followUp_is_Yes: any = $('[id^=FollowUp][id*=Yes]:checked, [id^=Follow_Up][id*=Yes]:checked', thisQ).length > 0;

                    let GG0170Rs: any = $('.persistable[id^=GG0170R]');
                    let correspondingR: any;
                    switch (true) {
                        case (Q_admission_is_No):
                            console.log('Q_admission_is_No');
                            correspondingR = $('[id*=Admission_Performance]', GG0170Rs);
                            correspondingR.prop('disabled', true).val(-1).siblings('div.longTextOption, i.score').remove();
                            seenTheDialog = GG0170Q_is_No_skip_to_Complete(correspondingR, e.data.x, { seenTheDialog: seenTheDialog });
                            break;
                        case (Q_discharge_is_No):
                            console.log('Q_discharge_is_No');
                            correspondingR = $('[id*=Discharge_Performance]', GG0170Rs);
                            correspondingR.prop('disabled', true).val(-1).siblings('div.longTextOption, i.score').remove();
                            seenTheDialog = GG0170Q_is_No_skip_to_Complete(correspondingR, e.data.x, { seenTheDialog: seenTheDialog });
                            break;
                        case (Q_interim_is_No):
                            console.log("Q_interim_is_No");
                            correspondingR = $('[id*=Interim_Performance]', GG0170Rs);
                            correspondingR.prop('disabled', true).val(-1).siblings('idiv.longTextOption, i.score').remove();
                            seenTheDialog = GG0170Q_is_No_skip_to_Complete(correspondingR, e.data.x, { seenTheDialog: seenTheDialog });
                            break;
                        case (Q_followUp_is_No):
                            console.log("Q_followUp_is_No");
                            correspondingR = $('[id*=Follow_Up_Performance], [id*=FollowUp_Performance]', GG0170Rs);
                            correspondingR.prop('disabled', true).val(-1).siblings('div.longTextOption, i.score').remove();
                            seenTheDialog = GG0170Q_is_No_skip_to_Complete(correspondingR, e.data.x, { seenTheDialog: seenTheDialog });
                            break;
                        case Q_admission_is_Yes:
                            console.log("Q_admission_is_Yes");
                            correspondingR = $('[id*=Admission_Performance]', GG0170Rs);
                            seenTheDialog = GG0170Q_is_Yes_skip_to_GG0170R(e.data.x, { seenTheDialog: seenTheDialog }, correspondingR);
                            break;
                        case Q_discharge_is_Yes:
                            console.log("Q_discharge_is_Yes");
                            correspondingR = $('[id*=Discharge_Performance]', GG0170Rs);
                            seenTheDialog = GG0170Q_is_Yes_skip_to_GG0170R(e.data.x, { seenTheDialog: seenTheDialog }, correspondingR);
                            break;
                        case Q_interim_is_Yes:
                            console.log("Q_interim_is_Yes");
                            correspondingR = $('[id*=Interim_Performance]', GG0170Rs);
                            seenTheDialog = GG0170Q_is_Yes_skip_to_GG0170R(e.data.x, { seenTheDialog: seenTheDialog }, correspondingR);
                            break;
                        case Q_followUp_is_Yes:
                            console.log("Q_followUp_is_Yes");
                            correspondingR = $('[id*=Follow_Up_Performance],[id*=FollowUp_Performance]', GG0170Rs);
                            seenTheDialog = GG0170Q_is_Yes_skip_to_GG0170R(e.data.x, { seenTheDialog: seenTheDialog }, correspondingR);
                            break;
                        default:
                            console.log("GG0170Qs disabled");
                            thisQ.prop('disabled', false);
                            break;
                    }
                }
            );
        });

        /* add rule help */
        const GG0170QRuleTriggers = $('.branchingRule[data-target^=GG0170Q], .branchingRule[data-target^=AssessmentCompleted]');
        const GG0170QRuleText = 'If any GG0170Q is NO, unlock and set focus on YES of the Complete Assessment.';

        GG0170QRuleTriggers.each(function () {
            let thisTrigger = $(this);
            thisTrigger.prop('title', GG0170QRuleText).show()
                .on("click", function () {
                    BranchingRuleDialog(GG0170QRuleText);
                });
        });

        console.log('GG0170Q listner added');
    })();

    /* event handler */
    function J1750_depends_on_J0510_or_J0520(eventType: EnumChangeEventArg, byRef: { seenTheDialog: boolean }): boolean {
        console.log('inside of J1750_depends_on_J0510_or_J0520, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);

        const J1750s = $('.persistable[id^=J1750]', formScope);
        const J1750_yes = $('[id*=Yes]', J1750s);
        J1750s.each(function () {
            $(this).prop('disabled', false);
        });
        if (eventType === EnumChangeEventArg.Change) {
            console.log('scroll to ' + J1750_yes.prop('id'));
            //scrollTo(J1750_yes.prop('id'));
            //J1750_yes.parent('focus');
        }

        console.log('------ done handling J0510 ' + eventType + '------');

        //no dialog is used so just return the original value of byRef.seenTheDialog
        return byRef.seenTheDialog;
    }

    /* self executing event listener */
    (function J0510_J0520_addListener() {
        console.log('adding J0510_J0520_addListener()');

        let seenTheDialog: boolean = true;

        //const this_has_Not_Apply: boolean = $(".persistable[id=" + thisJ0510.prop('id') + "] option:selected").text().indexOf('0. Does not apply') !== -1;
        //const Does_not_apply: any = thisJ0510.find('option[text="0. Does not apply"]');
        let J0510firstDoesNotApply: any;
        const J0510_J0520 = $('.persistable[id^=J0510], .persistable[id^=J0520]');
        J0510_J0520.each(function () {
            const thisJ: any = $(this);
            thisJ.on('change', function () {
                const selectedOptions: any = $(':selected', thisJ);
                selectedOptions?.each(function () {
                    const thisSelectedOption: any = $(this).text();
                    const rgxPattern: any = /0. /;
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

        console.log('J05X0 listener added');
    })();

    /* event handler */
    function AddMore(stage: string) {
        console.log('branching::: inside of AddMore');
        const addMoreBtns = $('button[id^=btnMore]');
        addMoreBtns.on('click', function () {
            const questionKey: string = $(this).data('questionkey');
            const lastInputIdx: number = $('.persistable[id^=' + questionKey + '][id*=' + stage + ']', formScope).length - 1;
            const lastInputDate: any = $('.persistable[id^=' + questionKey + '][id*=' + stage + '_' + lastInputIdx + ']', formScope);
            const dateClone: any = lastInputDate.clone();
            //commonUtility.resetControlValue(dateClone);
            dateClone.val('');
            dateClone.trigger('focus');
            lastInputDate.append(dateClone);
        });
    }

    (function triggerChangeChain() {
        console.log('trigger Q23 change');

        /* event handler, add this last to raise change chain
           set focus on Q23 because triggers in the change chain might set focus on their respective target */
        $('.persistable[id^=Q23]', formScope).trigger("change").trigger("focus");
    })();
})
