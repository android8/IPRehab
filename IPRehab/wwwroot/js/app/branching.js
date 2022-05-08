/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
import { Utility /*, EnumChangeEventArg*/ } from "./commonImport.js";
/* jquery plugin dependsOn*/
/* https://dstreet.github.io/dependsOn */
/* http://emranahmed.github.io/Form-Field-Dependency */
var EnumChangeEventArg;
(function (EnumChangeEventArg) {
    EnumChangeEventArg["Load"] = "Load";
    EnumChangeEventArg["Change"] = "Change";
    EnumChangeEventArg["NoScroll"] = "NoScroll";
})(EnumChangeEventArg || (EnumChangeEventArg = {}));
/****************************************************************************
 * javaScript closure
 ***************************************************************************/
const branchingController = (function () {
    const commonUtility = new Utility();
    //const dialogOptions = commonUtility.dialogOptions();
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
                //text: "Close",
                //icon: "ui-icon-close",
                click: function () {
                    $(this).dialog("close");
                }
            }]
    };
    function scrollTo1(elementId) {
        console.log('branching::: scrollTo() elementId = ', elementId);
        const thisElement = $('#' + elementId);
        let scrollAmount = 0;
        if (thisElement.length > 0) {
            //scrollAmount = thisElement.prop('offsetTop');
            scrollAmount = (thisElement.offset() || { "top": NaN }).top;
            console.log('scroll to ' + elementId + ', amount ' + scrollAmount, thisElement);
            $('html,body').stop().animate({ scrollTop: scrollAmount }, 'fast');
            thisElement.focus();
        }
        else {
            alert(elementId + " doesn't exist in the current context, can not scroll to that element");
        }
    }
    function scrollTo(elementId) {
        const element = document.querySelector('#' + elementId);
        // Get the size and position of our element in the viewport
        const rect = element.getBoundingClientRect();
        // The top offset of our element is the top position of the element in the viewport plus the amount the body is scrolled
        let offsetTop = rect.top + document.body.scrollTop;
        console.log('scroll to ' + elementId + ' with amount ' + offsetTop);
        // Now we can scroll the window to this position
        window.scrollTo(0, offsetTop);
        document.getElementById(elementId).focus();
    }
    /* private function */
    function Q12_Q23_blank_then_Lock_All(eventType, byRef) {
        console.log('inside of Q12_Q23_blank_then_Lock_All(), fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        const Q12 = $('.persistable[id^=Q12_]');
        const Q23 = $('.persistable[id^=Q23]');
        const otherPersistables = $('.persistable:not([id^=Q12_]):not([id^=Q23])');
        const minDate = new Date('2020-01-01 00:00:00');
        const onsetDate = new Date(Q23.val());
        const admitDate = new Date(Q12.val());
        const Q12_or_Q23_is_empty = commonUtility.isEmpty(Q12) || commonUtility.isEmpty(Q23);
        const onset_is_later_than_admit = onsetDate < minDate || admitDate < minDate || admitDate < onsetDate;
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        const myButtons = {
            "Ok": function () {
                console.log('lock all other persistables');
                otherPersistables.each(function () {
                    $(this).prop("disabled", true);
                });
                $(this).dialog("close");
                setSeenTheDialog(true); //callback function
            },
            "Cancel": function () {
                $(this).dialog("close");
                setSeenTheDialog(true); //callback function
            }
        };
        switch (true) {
            case (Q12_or_Q23_is_empty): {
                console.log('Q12 or Q23 is empty or both, lock all other questions');
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    dialogOptions.title = 'Date';
                    $('#dialog')
                        .text('Onset Date and Admit Date are record keys, when either is blank, all fields with current values will be locked')
                        .dialog(dialogOptions, {
                        title: 'Warning Q12 Q23', buttons: myButtons
                    });
                }
                else {
                    //without warning dialog
                    console.log('lock all other persistables');
                    otherPersistables.each(function () {
                        $(this).prop("disabled", true);
                    });
                }
                break;
            }
            case (onset_is_later_than_admit): {
                console.log('Onset Date and Admit Date must be later than 01/01/2021, and Admit Date must be on Onset Date or later');
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    dialogOptions.title = 'Date';
                    $('#dialog')
                        .text('Onset Date and Admit Date must be later than 01/01/2021, and Admit Date must be on Onset Date or later')
                        .dialog(dialogOptions, {
                        title: 'Warning Q12, Q23', buttons: myButtons
                    });
                }
                else {
                    console.log('lock all other persistables');
                    otherPersistables.each(function () {
                        $(this).prop("disabled", true);
                    });
                }
                break;
            }
            default: {
                console.log('Onset Date and Admit Date are not empty, apply all rules of other ' + otherPersistables.length + ' fields');
                otherPersistables.each(function () {
                    const thisOtherPersistable = $(this);
                    //unlock then raise its change() event handler to set the element state
                    thisOtherPersistable.prop('disabled', false).change();
                });
                break;
            }
        }
        return byRef.seenTheDialog;
    }
    /* event handler */
    function Q12B_blank_then_Lock_Discharge(eventType, byRef) {
        //event hooked during checkAllRules()
        console.log("inside of Q12B_blank_then_Lock_Discharge, fired by " + eventType + " with seenTheDalog = " + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function actQ12B(dischargeState) {
            const dischargeRelatedDropdown = $('.persistable[id^=Q15B_], .persistable[id^=Q16B_],.persistable[id^=Q17B_], .persistable[id^=Q21B_]');
            const dischargeRelatedCheckboxes = $('.persistable[id^=Q41_],.persistable[id^=Q44C_]');
            dischargeRelatedDropdown.each(function () {
                const thisDropdown = $(this);
                console.log('reset ' + thisDropdown.prop('id'));
                thisDropdown.val(-1);
                console.log(dischargeState ? 'enable ' : 'disable ' + thisDropdown.prop('id'));
                if (dischargeState)
                    thisDropdown.prop('disabled', false).removeAttr('disabled');
                else
                    thisDropdown.prop('disabled', true);
                console.log('raise chage() on ' + thisDropdown.prop('id'));
                thisDropdown.change();
            });
            dischargeRelatedCheckboxes.each(function () {
                const thisCheckbox = $(this);
                console.log('uncheck ' + thisCheckbox.prop('id'));
                thisCheckbox.prop('checked', false);
                console.log(dischargeState ? 'enable ' : 'disable ' + thisCheckbox.prop('id'));
                if (dischargeState)
                    thisCheckbox.prop('disabled', false).removeAttr('disabled');
                else
                    thisCheckbox.prop('disabled', true);
                console.log('raise change() ' + thisCheckbox.prop('id'));
                thisCheckbox.change();
            });
        }
        //lock all field with pertaining discharge Q15B,Q16B, Q17B, Q21B, Q41, Q44C
        function DischargeRelatedButtonClosure() {
            return function (dischargeState) {
                return {
                    "Ok": function () {
                        actQ12B(dischargeState);
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
        const Q12B = $('.persistable[id^=Q12B_]');
        const isDischarged = Q12B.val() !== '';
        let dialogText;
        Q12B.prop('disabled', false);
        if (isDischarged) {
            dialogText = 'Q12B is a discharge date, unlock all related discharge fields: Q15B, Q16B, Q17B, Q21B, Q41, Q44C';
        }
        else
            dialogText = 'Q12B is not a discharge date. Reset and lock related discharge fields:  Q15B,Q16B, Q17B, Q21B, Q41, Q44C';
        console.log(dialogText);
        if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
            //with warning dialog
            const Q12Bbuttons = DischargeRelatedButtonClosure();
            $('#dialog')
                .text(dialogText)
                .dialog(dialogOptions, {
                title: 'Warning Q12B', buttons: Q12Bbuttons(isDischarged)
            });
            console.log('Q12B handler return byRef.seenTheDialog = ', byRef.seenTheDialog);
        }
        else {
            //without warning dialog
            actQ12B(isDischarged);
        }
        console.log('byRef.seenTheDialog = ', byRef.seenTheDialog);
        return byRef.seenTheDialog;
    }
    /* event handler */
    function Q14B_enabled_if_Q14A_is_Yes(eventType, byRef) {
        console.log("branching::: inside of Q14B_enabled_if_Q14A_is_Yes, fired by " + eventType + " with seenTheDalog = " + byRef.seenTheDialog);
        const Q14Bs = $('.persistable[id^=Q14B_]');
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function actQ14A(disableState) {
            Q14Bs.each(function () {
                const thisQ14B = $(this);
                console.log('uncheck ' + thisQ14B.prop('id'));
                thisQ14B.prop('checked', false);
                console.log(disableState ? 'disable ' : 'enable ' + thisQ14B.prop('id'));
                thisQ14B.prop('disabled', disableState);
            });
        }
        const Q14AYes = $('.persistable[id^=Q14A_][id*=Yes]:checked').length === 1;
        if (Q14AYes) {
            console.log('Q14A is Yes, uncheck and unlock all Q14B, no dialog');
            if (Q14Bs.length > 0) {
                /* without warning dialog */
                actQ14A(false);
                Q14Bs.first().focus();
            }
        }
        else {
            const dialogText = 'Q14A is unknown or is No, uncheck and lock all Q14B';
            console.log(dialogText);
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                //with warning dialog
                const myButtons = {
                    "Ok": function () {
                        actQ14A(true);
                        setSeenTheDialog(true);
                        $(this).dialog("close");
                    },
                    "Cancel": function () {
                        setSeenTheDialog(true);
                    }
                };
                $('#dialog')
                    .text(dialogText)
                    .dialog(dialogOptions, {
                    title: 'Warning 14A', buttons: myButtons
                });
            }
            else {
                actQ14A(true);
            }
        }
        return byRef.seenTheDialog;
    }
    /* event handler */
    function Q16A_is_Home_then_Q17(eventType, byRef) {
        //event hooked during checkAllRules()
        console.log("inside of Q16A_is_Home_then_Q17, fired by " + eventType + " with seenTheDalog = " + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function actQ16A(isDisabled) {
            const Q17 = $(".persistable[id^=Q17_]");
            if (Q17.length > 0) {
                console.log('thisQ17', Q17);
                Q17.prop('disabled', isDisabled);
                if (isDisabled)
                    //commonUtility.resetControlValue(Q17);
                    Q17.val(-1);
                else {
                    console.log('focus on Q17', Q17);
                    Q17.focus();
                }
            }
        }
        const Q16AisHome = $(".persistable[id^=Q16A_] option:selected").text().indexOf('1. Home') !== -1;
        if (Q16AisHome) {
            const dialogText = 'Q16A is home, unlock Q17';
            actQ16A(false);
        }
        else {
            const dialogText = 'Q16A is not home, clear and lock Q17';
            console.log(dialogText);
            const myButtons = {
                "Ok": function () {
                    actQ16A(true);
                    setSeenTheDialog(true);
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    setSeenTheDialog(true);
                    $(this).dialog("close");
                }
            };
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                $('#dialog')
                    .text(dialogText)
                    .dialog(dialogOptions, {
                    title: 'Warning Q16A, Q17', buttons: myButtons
                });
            }
            else {
                actQ16A(true);
            }
        }
        return byRef.seenTheDialog;
    }
    /* event handler, not used per stakeholder request */
    function Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A(eventType, byRef) {
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
            const myButtons = {
                "Ok": function () {
                    $('.persistable[id^=Q24A_]').prop('checked', false);
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
                $('.persistable[id^=Q24A_]').prop('checked', true);
            }
            else {
                //check all other ICD fields to ensure none has arthritis then uncheck Q24A
                arthritis.each(function () {
                    const thisOtherICD = $(this);
                    if (thisOtherICD.prop('id') !== thisICD && thisOtherICD.val() === '123.45') {
                        return false; //shortcut the loop since at least one other ICD is arthritis
                    }
                    //since loop completed and none other ICD has arthritis then it's safe to uncheck Q24A
                    $('.persistable[id^=Q24A_]').prop('checked', false);
                });
                if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    $('#dialog')
                        .text('Not an arthritis ICD, lock Q24A')
                        .dialog(dialogOptions, {
                        title: 'Warning Q21A, Q21B, Q22, Q24', buttons: myButtons
                    });
                }
                else {
                    $('.persistable[id^=Q24A_]').prop('checked', false);
                }
            }
        }
        /* on change */
        console.log('check Q24A if Q21A, Q21B, Q22, or Q24 is diabetes');
        const arthritis = $('.persistable[id ^= Q21A_], .persistable[id ^= Q21B_], .persistable[id ^= Q22_], .persistable[id ^= Q24_]');
        arthritis.each(function () {
            $(this).on('change', { x: EnumChangeEventArg.Change }, checkArthritis);
        });
        /* on load */
        checkArthritis(EnumChangeEventArg.Load);
    }
    /* event handler */
    function Q42_Interrupted_then_Q43(eventType, byRef) {
        //event hooked during checkAllRules()
        console.log('branching::: inside of Q42_Interrupted_then_Q43, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function actQ42(lockQ43) {
            const Q43s = $('.persistable[id^=Q43_]');
            Q43s.each(function () {
                const thisQ43 = $(this);
                if (lockQ43) {
                    console.log('lock Q43');
                    thisQ43.val('').prop('disabled', true);
                }
                else {
                    console.log('unlock Q43 then raise Q43.change()');
                    thisQ43.prop('disabled', false).change();
                }
            });
        }
        let Q42Yes = $('.persistable[id^=Q42][id*=Yes]:checked').length === 1;
        if (Q42Yes)
            actQ42(false);
        else {
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                //with warning dialog
                const myButtons = {
                    "Ok": function () {
                        actQ42(true);
                        setSeenTheDialog(true); //callback
                        $(this).dialog("close");
                    },
                    "Cancel": function () {
                        setSeenTheDialog(true); //callback
                        $(this).dialog("close");
                    }
                };
                $('#dialog')
                    .text('Q42 is a No, reset and lock all Q43')
                    .dialog(dialogOptions, {
                    title: 'Warning Q42, Q43', buttons: myButtons
                });
            }
            else {
                //warning dialog
                actQ42(true);
            }
        }
        return byRef.seenTheDialog;
    }
    /* event handler */
    function Q43_Rules(eventType, byRef, thisQ43) {
        console.log('branching::: inside of Q43_Rules(), fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog, thisQ43);
        let foundBlank = false;
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function actQ43s(CRUD, thisQ43) {
            switch (CRUD) {
                case 'D': {
                    console.log('D', thisQ43);
                    const OtherQ43s = $('.persistable[id^=Q43][id!=' + thisQ43.prop('id') + ']');
                    OtherQ43s.each(function () {
                        const thisOtherQ43 = $(this);
                        if (thisOtherQ43.val() === '')
                            foundBlank = true;
                    });
                    thisQ43.prop('disabled', true); //don't call change(), otherwise it will cause infinite Q43_Rules() loop
                    thisQ43.next('.bi-calendar-x').prop('diabled', true);
                    break;
                }
                case "CU": {
                    console.log('CU', thisQ43);
                    const blankQ43s = $('.persistable[id^=Q43][value=""]');
                    if (blankQ43s.length > 0) {
                        const firstBlank = blankQ43s.first();
                        firstBlank.prop('disabled', false).focus(); /* unlock this non-blank dates */
                        firstBlank.next('.bi-calendar-x').prop('disabled', false); /* unlock next reset button */
                        foundBlank = true;
                    }
                    thisQ43.prop('disabled', false); //don't call change(), otherwise it will cause infinite Q43_Rules() loop
                    thisQ43.next('.bi-calendar-x').prop('diabled', false);
                    break;
                }
            }
            return foundBlank;
        }
        const Q42Yes = $('.persistable[id^=Q42][id*=Yes]:checked').length === 1;
        const Q42No = $('.persistable[id^=Q42][id*=No]:checked').length === 1;
        const thisQ43CRUD = commonUtility.getCRUD(thisQ43, thisQ43.data('oldvalue'), thisQ43.val());
        function Q42ButtonsClosure(Q42Yes) {
            return function (thisQ43, thisQ43CRUD) {
                return {
                    "Ok": function () {
                        if (Q42Yes) {
                            switch (thisQ43CRUD) {
                                case "D":
                                case "D1":
                                case "D2":
                                    actQ43s('D', thisQ43);
                                default:
                                    actQ43s('CU', thisQ43);
                            }
                        }
                        else {
                            thisQ43.val('').prop('disabled', true); //don't call change(), otherwise it will cause infinite Q43_Rules() loop
                        }
                        setSeenTheDialog(true); //callback
                        $(this).dialog("close");
                    },
                    "Cancel": function () {
                        setSeenTheDialog(true); //callback
                        $(this).dialog("close");
                    }
                };
            };
        }
        switch (true) {
            case Q42Yes: {
                console.log("thisQ43CRUD ? " + thisQ43CRUD === undefined ? "unchanged" : thisQ43CRUD);
                //without warning dialog
                switch (thisQ43CRUD) {
                    case "D":
                    case "D1":
                    case "D2":
                        foundBlank = actQ43s('D', thisQ43);
                        break;
                    default:
                        foundBlank = actQ43s('CU', thisQ43);
                        break;
                }
                break;
            }
            case Q42No: {
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    const buttonQ42No = Q42ButtonsClosure(Q42No);
                    $('#dialog')
                        .text('Find next blank Q43 for editing')
                        .dialog(dialogOptions, {
                        title: 'Warning Q43', buttons: buttonQ42No(thisQ43, thisQ43CRUD)
                    });
                }
                else {
                    //without warning dialog
                    thisQ43.val('');
                    thisQ43.prop('disabled', true); //don't call change(), otherwise it will cause infinite Q43_Rules() loop
                }
                break;
            }
        }
        //return boexed values 
        return { foundBlank: foundBlank, seenTheDialog: byRef.seenTheDialog };
    }
    /* event handler */
    function Q44C_Affect_Q44D_Q45_Q46(eventType, byRef) {
        //event hooked during checkAllRules()
        console.log('inside of Q44C_Affect_Q44D_Q46, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        const Q44C_is_Yes = $('.persistable[id^=Q44C_][id*=Yes]:checked').length === 1;
        const Q44D = $('.persistable[id^=Q44D_]');
        const Q45 = $('.persistable[id^=Q45_]');
        const Q46 = $('.persistable[id^=Q46_]');
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        function act44C_is_no() {
            if (Q44D.length > 0) {
                console.log('disable Q44D');
                Q44D.val(-1).prop('disabled', true).change();
            }
            if (Q45.length > 0) {
                console.log('disable Q45');
                Q45.val(-1).prop('disabled', true).change();
            }
            if (Q46.length > 0) {
                console.log('enable and focus on Q46');
                Q46.val(-1).prop('disabled', false).chagne().focus();
            }
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
        };
        if (Q44C_is_Yes) {
            console.log('Q44C is yes, unlock Q44D, Q45 and focus on Q44D');
            if (Q45.length > 0) {
                Q45.prop('disabled', false).change();
            }
            if (Q44D.length > 0) {
                Q44D.prop('disabled', false).change().focus();
            }
        }
        else {
            if (Q44D.length > 0 && Q45.length > 0 && Q46.length > 0) {
                let dialogText = 'Q44C is no, lock Q44D, Q45 and focus on Q46';
                console.log(dialogText);
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    $('#dialog')
                        .text(dialogText)
                        .dialog(dialogOptions, {
                        title: 'Warning Q44C, Q44D, Q45, Q46', buttons: myButtons
                    });
                }
                else {
                    //without warning dialog
                    act44C_is_no();
                }
            }
        }
        return byRef.seenTheDialog;
    }
    /* event handler */
    function Q44D_Affect_Q45(eventType, byRef, thisQ44D) {
        console.log('inside of Q44D_Affect_Q45, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        const Q45 = $('.persistable[id^=Q45_]');
        if (thisQ44D.val() === -1)
            Q45.prop('disabled', true).change();
        //no diaglog is used so just return the original byRef.seenTheDialog value
        return byRef.seenTheDialog;
    }
    /* event handler */
    function GG0170JKLMN_depends_on_GG0170I(eventType, byRef, thisI) {
        console.log('inside of GG0170JKLMN_depends_on_GG0170I, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
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
        function actGG0170I(GG0170JKL, focusThis, isDisabled, eventArg) {
            GG0170JKL.each(function () {
                commonUtility.resetControlValue($(this));
                $(this).prop('disabled', isDisabled);
            });
            if (eventArg === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                console.log('actGG0170I() scroll to ', focusThis.prop('id'));
                focusThis.prop('disabled', false); //.change();
                scrollTo(focusThis.prop('id'));
            }
            else {
                console.log('actGG0170I() no scroll but focus ', focusThis.prop('id'));
                const focusedElement = $('#' + focusThis.prop('id'));
                if (focusedElement.length > 0) {
                    focusedElement.prop('disabled', false); //.change();
                    focusedElement.focus();
                }
                else
                    alert(focusThis.prop('id') + "dosen't exist in this context, can not set focus on that element");
            }
        }
        let GG0170J, GG0170JKL, GG0170M;
        let measure;
        if (thisI.prop('id').indexOf('Admission_Performance') !== -1) {
            measure = 'Admission_Performance';
        }
        else if (thisI.prop('id').indexOf('Discharge_Performance') !== -1) {
            measure = 'Discharge_Performance';
        }
        else if (thisI.prop('id').indexOf('Interim_Performance') !== -1) {
            measure = 'Interim_Performance';
        }
        else if (thisI.prop('id').indexOf('Followup_Performance') !== -1) {
            measure = 'Followup_Performance';
        }
        GG0170J = $('.persistable[id^=GG0170J_' + measure + ']');
        console.log('checkGG0170I()::: GG017J = ', GG0170J);
        GG0170JKL = $('.persistable[id^=GG0170J_' + measure + '], .persistable[id^=GG0170K_' + measure + '], .persistable[id^=GG0170L_' + measure + ']');
        console.log('checkGG0170I()::: GG017JKL = ', GG0170JKL);
        GG0170M = $('.persistable[id^=GG0170M_' + measure + ']');
        console.log('checkGG0170I()::: GG017M = ', GG0170M);
        const intGG0170I = commonUtility.getControlValue(thisI);
        switch (true) {
            case (intGG0170I > 0 && intGG0170I < 7):
                {
                    let consoleLog = 'GG0170I ' + measure + ' is between 1 and 6, unlock GG0170J ' + measure + ', GG0170K ' + measure + ', GG0170L ' + measure + ', and advance to GG0170J ' + measure + '. Other measures are kept intact.';
                    //without warning dialog
                    /* unlock and clear J K L, skip to J */
                    const focusJ = GG0170J;
                    const DisaenableJKL = false;
                    actGG0170I(GG0170JKL, focusJ, DisaenableJKL, eventType);
                }
                break;
            default: {
                /* GG0170I is not selected, clear and lock J K L then advance to M */
                const dialogText = 'GG0170I ' + measure + ' is unknown or is not between 1 and 6, clear and lock GG0170J ' + measure + ', GG0170K ' + measure + 'and GG0170L ' + measure + ', then advance to GG0170M ' + measure;
                console.log('checkGG0170I() eventData ' + dialogText);
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    //with warning dialog
                    const myButtons = {
                        "Ok": function () {
                            let focusM = GG0170M;
                            const disableJKL = true;
                            console.log('focusM = ', focusM);
                            actGG0170I(GG0170JKL, focusM, disableJKL, eventType);
                            setSeenTheDialog(true); //callback
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            setSeenTheDialog(true); //callback
                            $(this).dialog("close");
                        }
                    };
                    $('#dialog')
                        .text(dialogText)
                        .dialog(dialogOptions, {
                        title: 'Warning GG0170JKLM', buttons: myButtons
                    });
                }
                else {
                    //without warning dialog
                    let focusM = GG0170M;
                    const disableJKL = true;
                    console.log('focusM = ', focusM);
                    actGG0170I(GG0170JKL, focusM, disableJKL, eventType);
                }
            }
        }
        return byRef.seenTheDialog;
    }
    /* event handler */
    function GG0170P_depends_on_GG0170M_and_GG0170N(eventType, byRef, thisMorN) {
        //event hooked during checkAllRules()
        console.log('inside of GG0170P_depends_on_GG0170M_and_GG0170N, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        console.log('checkGG0170MN() thisMorN = ', thisMorN);
        let GG0170M, GG0170N, GG0170O, GG0170P;
        let measure;
        if (thisMorN.prop('id').indexOf('GG0170M') !== -1) {
            console.log("it's M", thisMorN);
            GG0170M = thisMorN;
            /* check measure */
            if (GG0170M.prop('id').indexOf('Admission_Performance') !== -1) {
                measure = "Admission_Performance";
            }
            else if (GG0170M.prop('id').indexOf('Discharge_Performance') !== -1) {
                measure = 'Discharge_Performance';
            }
            else if (GG0170M.prop('id').indexOf('Interim_Performance') !== -1) {
                measure = 'Interim_Performance';
            }
            else if (GG0170M.prop('id').indexOf('Followup_Performance') !== -1) {
                measure = 'Followup_Performance';
            }
            GG0170N = $('.persistable[id^=GG0170N_' + measure + ']');
            GG0170O = $('.persistable[id^=GG0170O_' + measure + ']');
            GG0170P = $('.persistable[id^=GG0170P_' + measure + ']');
        }
        else /* (thisMorN.prop('id').indexOf('GG0170N') !== -1) */ {
            console.log("it's N", thisMorN);
            GG0170N = thisMorN;
            if (GG0170N.prop('id').indexOf('Admission_Performance') !== -1) {
                measure = 'Admission_Performance';
            }
            else if (GG0170N.prop('id').indexOf('Discharge_Performance') !== -1) {
                measure = 'Discharge_Performance';
            }
            else if (GG0170N.prop('id').indexOf('Interim_Performance') !== -1) {
                measure = 'Interim_Performance';
            }
            else if (GG0170N.prop('id').indexOf('Followup_Performance') !== -1) {
                measure = 'Followup_Performance';
            }
            GG0170O = $('.persistable[id^=GG0170O_' + measure + ']');
            GG0170P = $('.persistable[id^=GG0170P_' + measure + ']');
        }
        let intGG0170 = commonUtility.getControlValue(thisMorN);
        const myButtons = {
            "Ok": function () {
                if (GG0170M) {
                    switch (true) {
                        case (intGG0170 >= 7 && GG0170P.length > 0):
                            /* M >= 7, reset and lock both N and O then advance to P */
                            if (GG0170N.length > 0) {
                                //commonUtility.resetControlValue(GG0170N);
                                GG0170N.val(-1).prop('disabled', true).change();
                            }
                            if (GG0170O.length > 0) {
                                //commonUtility.resetControlValue(GG0170O);
                                GG0170O.val(-1).prop('disabled', true).change();
                            }
                            GG0170P.prop('disabled', false);
                            scrollTo(GG0170P.prop('id'));
                            break;
                        default:
                            /* M is unknown, reset and lock N and O */
                            //commonUtility.resetControlValue(GG0170N);
                            GG0170N.val(-1).prop('disabled', true).change();
                            //commonUtility.resetControlValue(GG0170O);
                            GG0170O.val(-1).prop('disabled', true).change();
                            break;
                    }
                }
                else /*GG0170N*/ {
                    switch (true) {
                        case (intGG0170 >= 7 && GG0170P.length > 0):
                            /* N >= 7, reset and lock O then advance to P */
                            if (GG0170O.length > 0) {
                                //commonUtility.resetControlValue(GG0170O
                                GG0170O.val(-1).prop('disabled', true).change();
                            }
                            GG0170P.prop('disabled', false);
                            scrollTo(GG0170P.prop('id'));
                            break;
                        default:
                            /* N is unknown, reset and lock O */
                            //commonUtility.resetControlValue(GG0170O);
                            GG0170O.val(-1).prop('disabled', true).change();
                    }
                }
                setSeenTheDialog(true); //callback
                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
                setSeenTheDialog(true); //callback
            }
        };
        function noDialog() {
            return function () {
                console.log('noDialog() is fired');
                if (GG0170M) {
                    switch (true) {
                        case (intGG0170 >= 7 && GG0170P.length > 0):
                            /* M >= 7, reset and lock both N and O then advance to P */
                            if (GG0170N.length > 0) {
                                //commonUtility.resetControlValue(GG0170N);
                                GG0170N.val(-1).prop('disabled', true).change();
                            }
                            if (GG0170O.length > 0) {
                                //commonUtility.resetControlValue(GG0170O);
                                GG0170O.val(-1).prop('disabled', true).change();
                            }
                            GG0170P.prop('disabled', false).change();
                            scrollTo(GG0170P.prop('id'));
                            break;
                        case ((intGG0170 > 0 && intGG0170 < 7) && GG0170N.length > 0):
                            /* M between 0 and 6, reset and disable O then scroll to N */
                            if (GG0170O.length > 0) {
                                GG0170O.val(-1).prop('disabled', true).change();
                                GG0170N.prop('disabled', false).change();
                                scrollTo(GG0170N.prop('id'));
                            }
                            break;
                        default:
                            /* M is unknown, reset and lock N and O */
                            //commonUtility.resetControlValue(GG0170N);
                            GG0170N.val(-1).prop('disabled', true).change();
                            //commonUtility.resetControlValue(GG0170O);
                            GG0170O.val(-1).prop('disabled', true).change();
                            break;
                    }
                }
                else /* GG0170N */ {
                    switch (true) {
                        case (intGG0170 >= 7 && GG0170P.length > 0):
                            /* N >= 7, reset and lock both N and O then advance to P */
                            if (GG0170O.length > 0) {
                                //commonUtility.resetControlValue(GG0170O);
                                GG0170O.val(-1).prop('disabled', true).change();
                            }
                            GG0170P.prop('disabled', false).change();
                            scrollTo(GG0170P.prop('id'));
                            break;
                        case ((intGG0170 > 0 && intGG0170 < 7) && GG0170N.length > 0):
                            /* N between 0 and 6, unlock and scroll to N */
                            if (GG0170O.length > 0) {
                                //commonUtility.resetControlValue(GG0170O);
                                GG0170O.val(-1).prop('disabled', false).change();
                                scrollTo(GG0170O.prop('id'));
                            }
                            break;
                        default:
                            /* N is unknown, reset and lock O */
                            //commonUtility.resetControlValue(GG0170O);
                            GG0170O.val(-1).prop('disabled', true).change();
                    }
                }
            };
        }
        if (eventType == EnumChangeEventArg.Change && !byRef.seenTheDialog) {
            /* with warning dialog */
            let dialogText;
            /* do not show dialogue if 0 < M < 7 */
            if ((intGG0170 <= 0 || intGG0170 >= 7) && GG0170P.length > 0) {
                switch (true) {
                    case (GG0170M):
                        dialogText = 'M is unkown or M is greater than 7, reset and lock N and O then advance to P';
                        break;
                    case (GG0170N):
                        dialogText = 'N is unknow or N is greater than 7, reset and lock O then advance to P';
                        break;
                }
                $('#dialog')
                    .text(dialogText)
                    .dialog(dialogOptions, {
                    title: 'Warning GG0170M, N, O, and P', buttons: myButtons
                });
            }
        }
        else {
            noDialog();
        }
        return byRef.seenTheDialog;
    }
    /* event handler */
    function GG0170Q_is_No_skip_to_Complete(eventType, byRef, thisQ) {
        console.log('inside of GG0170Q_is_No_skip_to_Complete, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        function setSeenTheDialog(value) {
            //callback after async dialog is done and return the seenTheDialog to the caller
            byRef.seenTheDialog = value;
        }
        const completed = $('.persistable[id ^= Assessment][id*=Yes]');
        let GG0170Rs = null, checkNext = true;
        const thisQ_is_No = $('.persistable[id^=GG0170Q_][id*=No]:not([id*=Discharge_Goal]):checked').length > 0;
        const myButtons = {
            "Ok": function () {
                GG0170Rs.each(function () {
                    $(this).prop('disabled', false);
                });
                completed.prop('disabled', false);
                scrollTo(completed.prop('id'));
                setSeenTheDialog(true); //callback
                $(this).dialog("close");
            },
            "Cancel": function () {
                setSeenTheDialog(true); //callback
                $(this).dialog("close");
            }
        };
        if (thisQ_is_No) {
            const dialogText = 'At least one of the GG0170Qs is a no, lock all GG0170Rs, advance to Assesment Complete';
            console.log('checkGG0170Q() ' + dialogText);
            if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                //with warning dialog
                $('#dialog')
                    .text(dialogText)
                    .dialog(dialogOptions, {
                    title: 'Warning GG0170Q', buttons: myButtons
                });
            }
            //onload don't scroll to complete 
        }
        else {
            const GG0170Q_Admission_Performance_Yes = $('.persistable[id^=GG0170Q_Admission_Performance][id*=Yes]:checked').length === 1;
            if (GG0170Q_Admission_Performance_Yes) {
                console.log('checkGG0170Q() GG0170Q Admission Performance is yes, unlock and focus on GG0170R Admission Performance');
                GG0170Rs = $('.persistable[id^=GG0170R_Admission]');
                console.log('checkGG0170Q() GG0170Rs', GG0170Rs);
                GG0170Rs.each(function () {
                    $(this).prop('disabled', false);
                });
                if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                    scrollTo(GG0170Rs.first().prop('id'));
                }
                else {
                    GG0170Rs.first().focus();
                }
                checkNext = false;
            }
            if (checkNext) {
                const GG0170Q_Discharge_Performance_Yes = $('.persistable[id^=GG0170Q_Discharge_Performance][id*=Yes]:checked').length === 1;
                if (GG0170Q_Discharge_Performance_Yes) {
                    console.log('GG0170Q Discharge Performance is yes, unlock and focus on GG0170R Discharge Performance');
                    GG0170Rs = $('.persistable[id^=GG0170R_Discharge_Performance]');
                    console.log('GG0170Rs', GG0170Rs);
                    GG0170Rs.each(function () {
                        $(this).prop('disabled', false);
                    });
                    if (eventType === EnumChangeEventArg.Change && !byRef.seenTheDialog) {
                        scrollTo(GG0170Rs.first().prop('id'));
                    }
                    else {
                        GG0170Rs.first().focus();
                    }
                    checkNext = false;
                }
            }
            if (checkNext) {
                const GG0170Q_Interim_Performance_Yes = $('.persistable[id^=GG0170Q_Interim_Performance][id*=Yes]:checked').length === 1;
                if (GG0170Q_Interim_Performance_Yes) {
                    console.log('GG0170Q Interim Performance is yes, unlock and focus on GG0170R Interim Performance');
                    GG0170Rs = $('.persistable[id^=GG0170R_Interim_Performance]');
                    console.log('GG0170Rs', GG0170Rs);
                    GG0170Rs.each(function () {
                        $(this).prop('disabled', false);
                    });
                    if (eventType === EnumChangeEventArg.Change) {
                        scrollTo(GG0170Rs.first().prop('id'));
                    }
                    else {
                        GG0170Rs.first().focus();
                    }
                    checkNext = false;
                }
            }
            if (checkNext) {
                const GG0170Q_Followup_Performance_Yes = $('.persistable[id^=GG0170Q_Followup_Performance][id*=Yes]:checked').length === 1;
                if (GG0170Q_Followup_Performance_Yes) {
                    console.log('GG0170Q Follow Up Performance is yes, unlock and focus on GG0170R Follow Up Performance');
                    GG0170Rs = $('.persistable[id^=GG0170R_Followup_Performance]');
                    console.log('GG0170Rs', GG0170Rs);
                    GG0170Rs.each(function () {
                        $(this).prop('disabled', false);
                    });
                    if (eventType === EnumChangeEventArg.Change) {
                        scrollTo(GG0170Rs.first().prop('id'));
                    }
                    else {
                        GG0170Rs.first().focus();
                    }
                    checkNext = false;
                }
            }
        }
        return byRef.seenTheDialog;
    }
    /* event handler */
    function J1750_depends_on_J0510(eventType, byRef, thisJ0510) {
        console.log('inside of J1750_depends_on_J0510, fired by ' + eventType + ' with seenTheDalog = ' + byRef.seenTheDialog);
        function actJ0510() {
            const J1750s = $('.persistable[id^=J1750]');
            const J1750_yes = $('.persistable[id^=J1750][id*=Yes]');
            J1750s.each(function () {
                $(this).prop('disabled', false);
            });
            if (eventType === EnumChangeEventArg.Change) {
                console.log('scroll to ' + J1750_yes.prop('id'));
                scrollTo(J1750_yes.prop('id'));
            }
        }
        //const this_has_Not_Apply: boolean = $(".persistable[id=" + thisJ0510.prop('id') + "] option:selected").text().indexOf('0. Does not apply') !== -1;
        const Does_not_apply = thisJ0510.find('option[text="0. Does not apply"]');
        if (Does_not_apply.length > 0) {
            //found any 'Does not apply' should scroll to J1750 Yes
            console.log('"0. Does not apply" is found in ' + thisJ0510.prop('id') + ', unlock all J1750s and focus on J1750 Yes option');
            actJ0510();
        }
        //no dialog is used so just return the original value of byRef.seenTheDialog
        return byRef.seenTheDialog;
    }
    /* event handler */
    function AddMore(stage) {
        console.log('branching::: inside of AddMore');
        const addMoreBtns = $('button[id^=btnMore]');
        addMoreBtns.click(function () {
            const questionKey = $(this).data('questionkey');
            const lastInputIdx = $('.persistable[id^=' + questionKey + '_' + stage + ']').length;
            const lastInputDate = $('.persistable[id^=' + questionKey + '_' + stage + '_' + lastInputIdx + ']');
            const dateClone = lastInputDate.clone();
            //commonUtility.resetControlValue(dateClone);
            dateClone.val('');
            dateClone.focus();
            lastInputDate.append(dateClone);
        });
    }
    function Q12_Q23_addListener() {
        console.log('adding Q12_Q23_addListener()');
        /* add onchange event listner */
        let seenTheDialog = false;
        const primaryKeys = $('.persistable[id^=Q12_], .persistable[id^=Q23_]');
        primaryKeys.each(function () {
            const thisPrimaryKey = $(this);
            $(this).prop('disabled', false);
            thisPrimaryKey.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                console.log('onchange calling Q12_Q23_blank_then_Lock_All(), seenTheDialog = ', seenTheDialog);
                //checkQ12_Q23(e.data.x);
                seenTheDialog = Q12_Q23_blank_then_Lock_All(EnumChangeEventArg.Change, { seenTheDialog: seenTheDialog });
            });
        });
        console.log('added Q12_Q23 listener');
    }
    function Q12B_addListener() {
        console.log('adding Q12B_addListener()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change */
        let seenTheDialog = false;
        const Q12B = $('.persistable[id^=Q12B_]');
        Q12B.on('change', { x: EnumChangeEventArg.Change, y: Q12B }, function (e) {
            console.log('before calling Q12B_blank_then_Lock_Discharge(), seenTheDialog = ', seenTheDialog);
            //JavaScript, and TypeScript can pass an object by reference, but not by value.
            //Therefore box values into an object  { seenTheDialog: seenTheDialog }
            seenTheDialog = Q12B_blank_then_Lock_Discharge(e.data.x, { seenTheDialog: seenTheDialog });
        });
        console.log('added Q12B listener');
    }
    function Q14A_addListener() {
        console.log('adding Q14A_addListener()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change */
        let seenTheDialog = false;
        const Q14A = $('.persistable[id^=Q14A_]');
        Q14A.on('change', { x: EnumChangeEventArg.Change, y: $(this) }, function (e) {
            console.log('before calling Q14B_enabled_if_Q14A_is_Yes(), seenTheDialog = ', seenTheDialog);
            seenTheDialog = Q14B_enabled_if_Q14A_is_Yes(e.data.x, { seenTheDialog: seenTheDialog });
        });
        console.log('added Q14A listener');
    }
    function Q16_addListener() {
        console.log('adding Q16_addListener()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change */
        let seenTheDialog = false;
        const Q16A = $('.persistable[id^=Q16A_]');
        Q16A.on('change', { x: EnumChangeEventArg.Change }, function (e) {
            console.log('before calling Q16A_is_Home_then_Q17(), seenTheDialog = ', seenTheDialog);
            seenTheDialog = Q16A_is_Home_then_Q17(e.data.x, { seenTheDialog: seenTheDialog });
        });
        console.log('added Q16 listener');
    }
    function Q42_addListener() {
        console.log('adding Q42_addListener()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change */
        let seenTheDialog = false;
        const Q42s = $('.persistable[id^=Q42_]');
        Q42s.each(function () {
            let thisQ42 = $(this);
            thisQ42.on('change', { x: EnumChangeEventArg.Change, y: thisQ42 }, function (e) {
                console.log('before calling Q42_Interrupted_then_Q43(), seenTheDialog = ', seenTheDialog);
                seenTheDialog = Q42_Interrupted_then_Q43(e.data.x, { seenTheDialog: seenTheDialog });
            });
        });
        console.log('added Q42 listener');
    }
    function Q43_addListener() {
        console.log('adding Q43_addListener()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change. Q43 only raised by change() event manually here or programmmatically in Q42*/
        let seenTheDialog = false;
        let foundBlank;
        const Q43s = $('.persistable[id^=Q43_]');
        Q43s.each(function () {
            if (!foundBlank) {
                const thisQ43 = $(this);
                thisQ43.on('change', { x: EnumChangeEventArg.Change, y: thisQ43 }, function (e) {
                    console.log('before calling Q43_Rules(), foundBlank = ', foundBlank, ' seeTheDialog = ', seenTheDialog);
                    const returnedObject = Q43_Rules(e.data.x, { seenTheDialog: seenTheDialog }, thisQ43);
                    foundBlank = returnedObject.foundBlank;
                    seenTheDialog = returnedObject.seenTheDialog;
                });
            }
        });
        console.log('added Q43 listener');
    }
    function Q44C_addListener() {
        console.log('adding Q44C_addListener()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change */
        let seenTheDialog = false;
        const Q44C = $('.persistable[id^=Q44C_]');
        Q44C.each(function (i, el) {
            const thisQ44C = $(el); //don't use $(this) because in the arrow function it will be undefined
            thisQ44C.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                console.log('before calling Q44C_Affect_Q44D_Q45_Q46(), seenTheDialog = ', seenTheDialog);
                seenTheDialog = Q44C_Affect_Q44D_Q45_Q46(e.data.x, { seenTheDialog: seenTheDialog });
            });
        });
        console.log('added Q44 listener');
    }
    function Q44D_addListener() {
        /* on change */
        let seenTheDialog = false;
        const Q44D = $('.persistable[id^=Q44D_]');
        Q44D.on('change', { x: EnumChangeEventArg.Change }, function (e) {
            console.log('before calling Q44C_Affect_Q44D_Q45_Q46(), seenTheDialog = ', seenTheDialog);
            seenTheDialog = Q44D_Affect_Q45(e.data.x, { seenTheDialog: seenTheDialog }, Q44D);
        });
    }
    function GG0170I_addListener() {
        console.log('adding GG0170I_addListener()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change */
        let seenTheDialog = false;
        const GG0170I = $('.persistable[id^=GG0170I]:not([id*=Discharge_Goal])');
        GG0170I.each(function () {
            const thisI = $(this);
            thisI.on('change', { x: EnumChangeEventArg.Change, y: thisI }, function (e) {
                console.log('before calling GG0170JKLMN_depends_on_GG0170I() seenTheDialog = ', seenTheDialog);
                seenTheDialog = GG0170JKLMN_depends_on_GG0170I(e.data.x, { seenTheDialog: seenTheDialog }, thisI);
            });
        });
        console.log('added GG0170M_N listener');
        //  const GG0170I_Admission: any = $('.persistable[id^=GG0170I_Admission]');
        //  const GG0170I_Admission_Value: number = commonUtility.getControlValue(GG0170I_Admission);
        //  const GG0170JKL_Admission: any = $('.persistable[id^=GG0170J_Admission], .persistable[id ^= GG0170K_Admission], .persistable[id ^= GG0170L_Admission]');
        //  const GG0170M_Admission: any = $('.persistable[id ^= GG0170M_Admission]');
        //  if (GG0170I_Admission_Value > 0 && GG0170I_Admission_Value <= 6) {
        //    /* do not scroll or set focus */
        //    GG0170JKL_Admission.each(function () {
        //      const thisGG0170 = $(this);
        //      thisGG0170.val(-1).prop('disabled', false);
        //    })
        //  }
        //  else {
        //    /* do not scroll or set focus */
        //    GG0170JKL_Admission.each(function () {
        //      $(this).val(-1).prop('disabled', true);
        //    });
        //  }
        //  const GG0170I_Discharge: any = $('.persistable[id^=GG0170I_Discharge_Performance]');
        //  const GG0170I_Discharge_Value: number = commonUtility.getControlValue(GG0170I_Discharge);
        //  const GG0170JKL_Discharge: any = $('.persistable[id ^= GG0170J_Discharge_Performance], .persistable[id ^= GG0170K_Discharge_Performance], .persistable[id ^= GG0170L_Discharge_Performance]');
        //  const GG0170M_Discharge: any = $('.persistable[id ^= GG0170M_Discharge_Performance]');
        //  if (GG0170I_Discharge_Value > 0 && GG0170I_Discharge_Value <= 6) {
        //    /* do not scroll or set focus */
        //    GG0170JKL_Discharge.each(function () {
        //      const thisGG0170 = $(this);
        //      thisGG0170.val(-1).prop('disabled', false);
        //    })
        //  }
        //  else {
        //    /* do not scroll or set focus */
        //    GG0170JKL_Discharge.each(function () {
        //      $(this).val(-1).prop('disabled', true);
        //    });
        //  }
    }
    function GG0170M_N_addListener() {
        console.log('adding GG0170M_N_addListener()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change */
        let seenTheDialog = false;
        const GG0170M_and_N = $('.persistable[id^=GG0170M]:not([id*=Discharge_Goal]), .persistable[id^=GG0170N]:not([id*=Discharge_Goal])');
        GG0170M_and_N.each(function () {
            const thisMorN = $(this);
            thisMorN.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                console.log('before calling GG0170P_depends_on_GG0170M_and_GG0170N() seenTheDialog = ', seenTheDialog);
                seenTheDialog = GG0170P_depends_on_GG0170M_and_GG0170N(e.data.x, { seenTheDialog: seenTheDialog }, thisMorN);
            });
        });
        console.log('added GG0170M_N listener');
        //const GG0170M_Admission = $('.persistable[id^=GG0170M_Admission_]');
        //const GG0170N_Admission = $('.persistable[id^=GG0170N_Admission_]');
        //const GG0170O_Admission = $('.persistable[id^=GG0170O_Admission_]');
        //const GG0170P_Admission = $('.persistable[id^=GG0170P_Admission_]');
        //const GG0170M_Admission_Value: number = commonUtility.getControlValue(GG0170M_Admission);
        //switch (true) {
        //  case (GG0170M_Admission_Value >= 7):
        //    /* M >= 7, reset and lock both N and O, do not scroll or set focus */
        //    GG0170N_Admission.val(-1).prop('disabled', true);
        //    GG0170O_Admission.val(-1).prop('disabled', true);
        //    break;
        //  case (GG0170M_Admission_Value > 0 && GG0170M_Admission_Value < 7):
        //    /* M between 0 and 6, reset and disable O, do not scroll or set focus */
        //    GG0170O_Admission.val(-1).prop('disabled', true).change();
        //    break;
        //  default:
        //    /* M is unknown, reset and lock N and O */
        //    //commonUtility.resetControlValue(GG0170N);
        //    GG0170N_Admission.val(-1).prop('disabled', true).change();
        //    //commonUtility.resetControlValue(GG0170O);
        //    GG0170O_Admission.val(-1).prop('disabled', true).change();
        //    break;
        //}
        //const GG0170M_Discharge = $('.persistable[id^=GG0170M_Discharge_Performance]');
        //const GG0170N_Discharge = $('.persistable[id^=GG0170N_Discharge_Performance]');
        //const GG0170O_Discharge = $('.persistable[id^=GG0170O_Discharge_Performance]');
        //const GG0170P_Discharge = $('.persistable[id^=GG0170P_Discharge_Performance]');
        //const GG0170M_Discharge_Value: number = commonUtility.getControlValue(GG0170M_Discharge);
        //switch (true) {
        //  case (GG0170M_Discharge_Value >= 7):
        //    /* M >= 7, reset and lock both N and O, do not scroll or set focus */
        //    GG0170N_Discharge.val(-1).prop('disabled', true);
        //    GG0170O_Discharge.val(-1).prop('disabled', true);
        //    break;
        //  case (GG0170M_Discharge_Value > 0 && GG0170M_Discharge_Value < 7):
        //    /* M between 0 and 6, reset and disable O, do not scroll or set focus */
        //    GG0170O_Discharge.val(-1).prop('disabled', true).change();
        //    break;
        //  default:
        //    /* M is unknown, reset and lock N and O */
        //    GG0170N_Discharge.val(-1).prop('disabled', true);
        //    GG0170O_Discharge.val(-1).prop('disabled', true);
        //    break;
        //}
    }
    //function GG0170N_onload() {
    //  //console.log('inside GG0170N_onload()');
    //  //const GG0170N_Admission = $('.persistable[id^=GG0170N_Admission_]');
    //  //const GG0170O_Admission = $('.persistable[id^=GG0170O_Admission_]');
    //  //const GG0170P_Admission = $('.persistable[id^=GG0170P_Admission_]');
    //  //const GG0170N_Admission_Value: number = commonUtility.getControlValue(GG0170N_Admission);
    //  //switch (true) {
    //  //  case (GG0170N_Admission_Value >= 7):
    //  //    /* N >= 7, reset and lock O, do not scroll or set focus */
    //  //    GG0170O_Admission.val(-1).prop('disabled', true);
    //  //    break;
    //  //  case (GG0170N_Admission_Value > 0 && GG0170N_Admission_Value < 7):
    //  //    /* N between 0 and 6, reset and disable O, do not scroll or set focus */
    //  //    GG0170O_Admission.val(-1).prop('disabled', true);
    //  //    break;
    //  //  default:
    //  //    /* N is unknown, reset O */
    //  //    GG0170O_Admission.val(-1).prop('disabled', true);
    //  //    break;
    //  //}
    //  //const GG0170N_Discharge = $('.persistable[id^=GG0170N_Discharge_Performance]');
    //  //const GG0170O_Discharge = $('.persistable[id^=GG0170O_Discharge_Performance]');
    //  //const GG0170P_Discharge = $('.persistable[id^=GG0170P_Discharge_Performance]');
    //  //const GG0170N_Discharge_Value: number = commonUtility.getControlValue(GG0170N_Discharge);
    //  //switch (true) {
    //  //  case (GG0170N_Discharge_Value >= 7):
    //  //    /* N >= 7, reset and lock O, do not scroll or set focus */
    //  //    GG0170O_Discharge.val(-1).prop('disabled', true);
    //  //    break;
    //  //  case (GG0170N_Discharge_Value > 0 && GG0170N_Discharge_Value < 7):
    //  //    /* N between 0 and 6, reset and disable O, do not scroll or set focus */
    //  //    GG0170O_Discharge.val(-1).prop('disabled', true).change();
    //  //    break;
    //  //  default:
    //  //    /* N is unknown, reset O */
    //  //    GG0170O_Discharge.val(-1).prop('disabled', true);
    //  //    break;
    //  //}
    //}
    function GG0170Q_addListner() {
        console.log('adding GG0170Q_addListner()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change */
        let seenTheDialog = false;
        const GG0170Qs = $('.persistable[id^=GG0170Q_]:not([id*=Discharge_Goal])');
        GG0170Qs.each(function () {
            let thisQ = $(this);
            thisQ.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                seenTheDialog = GG0170Q_is_No_skip_to_Complete(e.data.x, { seenTheDialog: seenTheDialog }, thisQ);
            });
        });
    }
    function J0510_addListener() {
        console.log('adding J0510_addListener()');
        //no need to raise onload event, it is only raised by Q12_Q23 change() event chain
        /* on change */
        let seenTheDialog = false;
        const J0510s = $('.persistable[id^=J0510]:not([id*=Discharge_Goal]');
        J0510s.each(function () {
            const thisJ0510 = $(this);
            thisJ0510.on('change', { x: EnumChangeEventArg.Change }, function (e) {
                console.log('before calling J1750_depends_on_J0510() seenTheDialog = ', seenTheDialog);
                seenTheDialog = J1750_depends_on_J0510(e.data.x, { seenTheDialog: seenTheDialog }, thisJ0510);
            });
        });
        console.log('added J0510 listener');
    }
    function raise_Onload_Event_Once_Without_Dialog() {
        console.log('raise_Onload_Event_Once_Without_Dialog()');
        //JavaScript, and TypeScript can pass an object by reference, but not by value.
        //Therefore box values into an object like { seenTheDialog: true }
        const suppressDialog = true;
        Q12_Q23_blank_then_Lock_All(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog });
        Q12B_blank_then_Lock_Discharge(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog });
        Q14B_enabled_if_Q14A_is_Yes(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog });
        Q16A_is_Home_then_Q17(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog });
        Q42_Interrupted_then_Q43(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog });
        const Q43s = $('.persistable[id^=Q43_]');
        Q43s.each(function () {
            Q43_Rules(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog }, $(this));
        });
        const Q44C = $('.persistable[id^=Q44C_]');
        Q44C.each(function (i, el) {
            Q44C_Affect_Q44D_Q45_Q46(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog });
        });
        const Q44D = $('.persistable[id^=Q44D_]');
        Q44D_Affect_Q45(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog }, Q44D);
        const GG0170I = $('.persistable[id^=GG0170I]:not([id*=Discharge_Goal])');
        GG0170I.each(function () {
            const thisI = $(this);
            GG0170JKLMN_depends_on_GG0170I(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog }, thisI);
        });
        const GG0170M_and_N = $('.persistable[id^=GG0170M]:not([id*=Discharge_Goal]), .persistable[id^=GG0170N]:not([id*=Discharge_Goal])');
        GG0170M_and_N.each(function () {
            const thisMorN = $(this);
            GG0170P_depends_on_GG0170M_and_GG0170N(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog }, thisMorN);
        });
        const GG0170Qs = $('.persistable[id^=GG0170Q_]:not([id*=Discharge_Goal])');
        GG0170Qs.each(function () {
            let thisQ = $(this);
            GG0170Q_is_No_skip_to_Complete(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog }, thisQ);
        });
        const J0510s = $('.persistable[id^=J0510]:not([id*=Discharge_Goal]');
        J0510s.each(function () {
            const thisJ0510 = $(this);
            J1750_depends_on_J0510(EnumChangeEventArg.Load, { seenTheDialog: suppressDialog }, thisJ0510);
        });
        //set focus on Q12 after all the above that might have scrolled the screen during load
        const Q12 = $('.persistable[id^=Q12_]');
        scrollTo(Q12.prop('id'));
    }
    /* private function */
    function LoadAllRules() {
        console.log('inside of LoadAllRules()');
        /* add event listener */
        console.log('add Q12B Listener');
        Q12B_addListener();
        console.log('add Q14A Listener');
        Q14A_addListener();
        console.log('add Q16 Listener');
        Q16_addListener();
        console.log('add Q42 Listener');
        Q42_addListener();
        console.log('add Q43 Listener');
        Q43_addListener();
        console.log('add Q44 Listener');
        Q44C_addListener();
        console.log('add Q44D Listener');
        Q44D_addListener();
        console.log('add GG0170I Listener');
        GG0170I_addListener();
        console.log('add GG0170M_N Listener');
        GG0170M_N_addListener();
        console.log('add GG0170Q_addListner');
        GG0170Q_addListner();
        console.log('add J0510 Listener');
        J0510_addListener();
        //add this listen last due to change() is a async fashion
        console.log('add Q12_Q23 Listener');
        Q12_Q23_addListener();
        raise_Onload_Event_Once_Without_Dialog();
    }
    /***************************************************************************
     * public functions exposing the private functions to outside of the closure
    ***************************************************************************/
    return {
        'LoadAllRules': LoadAllRules
    };
})();
/******************************* end of branchingController closure ****************************/
$(function () {
    let sysTitle = $('.pageTitle').data('systitle');
    /* on ready */
    if (sysTitle === 'Full') {
        $('.persistable').each(function () { $(this).prop("disabled", true); });
    }
    else {
        console.log('LoadAllRules');
        branchingController.LoadAllRules();
    }
});
//# sourceMappingURL=branching.js.map