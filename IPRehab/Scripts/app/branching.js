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
                text: "Close",
                //icon: "ui-icon-close",
                click: function () {
                    $(this).dialog("close");
                }
            }]
    };
    function scrollTo(thisElement) {
        let scrollAmount = thisElement.prop('offsetTop') + 15;
        if (thisElement.prop('id').indexOf('Q12') !== -1)
            scrollAmount = 0; //scroll up further by 15
        console.log('scroll to ' + thisElement.prop('id') + ', amount ' + scrollAmount, thisElement);
        $('html,body').animate({ scrollTop: scrollAmount }, 'fast');
        thisElement.focus();
    }
    /* private function */
    function Q12_Q23_blank_then_Lock_All() {
        console.log('branching:::', 'Inside of Q12_Q23_blank_then_Lock_All()');
        const primaryKeys = $('.persistable[id^=Q12_], .persistable[id^=Q23]');
        /* nested function */
        function checkQ12_Q23(e) {
            var _a, _b, _c;
            console.log('e.data.x in check!12_23() = ', (_a = e.data) === null || _a === void 0 ? void 0 : _a.x);
            const Q12 = $('.persistable[id^=Q12_]');
            const Q23 = $('.persistable[id^=Q23]');
            const otherPersistables = $('.persistable:not([id^=Q12_]):not([id^=Q23])');
            const myButtons = {
                "Ok": function () {
                    otherPersistables.each(function () {
                        $(this).prop("disabled", true);
                    });
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            };
            if (commonUtility.isEmpty(Q12) || commonUtility.isEmpty(Q23)) {
                console.log('Q12 or Q23 is empty or both, lock all other questions');
                if (((_b = e.data) === null || _b === void 0 ? void 0 : _b.x) === EnumChangeEventArg.Change) {
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
                    otherPersistables.each(function () {
                        $(this).prop("disabled", true);
                    });
                }
            }
            else {
                const minDate = new Date('2020-01-01 00:00:00');
                const onsetDate = new Date(Q23.val());
                const admitDate = new Date(Q12.val());
                if (onsetDate < minDate || admitDate < minDate || admitDate < onsetDate) {
                    console.log('Onset Date and Admit Date must be later than 01/01/2021, and Admit Date must be on Onset Date or later');
                    if (((_c = e.data) === null || _c === void 0 ? void 0 : _c.x) === EnumChangeEventArg.Change) {
                        //with warning dialog
                        dialogOptions.title = 'Date';
                        $('#dialog')
                            .text('Onset Date and Admit Date must be later than 01/01/2021, and Admit Date must be on Onset Date or later')
                            .dialog(dialogOptions, {
                            title: 'Warning Q12, Q23', buttons: myButtons
                        });
                    }
                    else {
                        otherPersistables.each(function () {
                            $(this).prop("disabled", true);
                        });
                    }
                }
                else {
                    console.log('Onset Date and Admit Date are not empty, apply rules of other ' + otherPersistables.length + ' perPersistables');
                    otherPersistables.each(function () {
                        //some persistable doesn't track change(), so enable them first then trigger chage() of those that do.
                        $(this).prop('disabled', false);
                        $(this).change();
                    });
                }
            }
        }
        /* on check */
        primaryKeys.each(function () {
            const thisPrimaryKey = $(this);
            thisPrimaryKey.on('change', { x: EnumChangeEventArg.Change }, checkQ12_Q23);
        });
        /* on load */
        checkQ12_Q23(EnumChangeEventArg.Load);
    }
    /* private function */
    function Q12B_blank_then_Lock_Discharge() {
        //event hooked during checkAllRules()
        console.log("branching::: inside of Q12B_blank_then_Lock_Discharge");
        let Q12B = $('.persistable[id^=Q12B_]');
        /* nested event handler */
        function checkQ12B(e) {
            var _a, _b, _c, _d, _e;
            //lock all field with pertaining discharge Q15B,Q16B, Q17B, Q21B, Q41, Q44C
            if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change)
                Q12B = (_b = e.data) === null || _b === void 0 ? void 0 : _b.y;
            console.log('inside of checkQ12B(e) Q12B.val()', Q12B.val());
            const isDischarged = ((_c = Q12B.val()) === null || _c === void 0 ? void 0 : _c.toString()) !== '';
            const Q15B = $('.persistable[id^=Q15B_]'); /* dropdown */
            const Q16B = $('.persistable[id^=Q16B_]'); /* dropdown */
            const Q17B = $('.persistable[id^=Q17B_]'); /* dropdown */
            const Q21B = $('.persistable[id^=Q21B_]'); /* dropdown */
            const Q41 = $('.persistable[id^=Q41_]'); /* checkbox */
            const Q44C = $('.persistable[id^=Q44C_'); /* checkbox */
            //const DischargeRelated: any = $('.persistable[id ^= Q15B],.persistable[id ^= Q16B],.persistable[id ^= Q17B],.persistable[id ^= Q21B],.persistable[id ^= Q41],.persistable[id ^= Q44C]');
            const seenTheDialog = false;
            const myButtons = {
                "Ok": function () {
                    if (isDischarged) {
                        //DischargeRelated.each(function () {
                        //  const thisRelatedQ = $(this);
                        //  thisRelatedQ.prop('disabled', false).change();
                        //});
                        console.log('reset and enable Q15B');
                        Q15B.val(-1).prop('disabled', false).change();
                        console.log('reset and enable Q16B');
                        Q16B.val(-1).prop('disabled', false).change();
                        console.log('reset and enable Q17B');
                        Q17B.val(-1).prop('disabled', false).change();
                        console.log('reset and enable Q21B');
                        Q21B.val(-1).prop('disabled', false).change();
                        Q41.each(function () {
                            const thisQ41 = $(this);
                            console.log('reset and enable ' + thisQ41.prop('id'));
                            thisQ41.prop('checked', false).prop('disabled', false).change();
                        });
                        Q44C.each((i, el) => {
                            const thisQ44C = $(el);
                            console.log('reset and enable ' + thisQ44C.prop('id'));
                            thisQ44C.prop('checked', false).prop('disabled', false).change();
                        });
                    }
                    else {
                        //DischargeRelated.each(function () {
                        //  const thisRelatedQ = $(this);
                        //  commonUtility.resetControlValue(thisRelatedQ); //change() event would be fired in the commonUtility
                        //  thisRelatedQ.val('').prop('disabled', true);
                        //});
                        console.log('reset and disable Q15B');
                        Q15B.val(-1).prop('disabled', true).change();
                        console.log('reset and disable Q16B');
                        Q16B.val(-1).prop('disabled', true).change();
                        console.log('reset and disable Q17B');
                        Q17B.val(-1).prop('disabled', true).change();
                        console.log('reset and disable Q21');
                        Q21B.val(-1).prop('disabled', true).change();
                        Q41.each(function () {
                            const thisQ41 = $(this);
                            console.log('uncheck and disable ' + thisQ41.prop('id'));
                            thisQ41.prop('checked', false).prop('disabled', true).change();
                        });
                        Q44C.each((i, el) => {
                            const thisQ44C = $(el);
                            console.log('uncheck and disable ' + thisQ44C.prop('id'));
                            thisQ44C.prop('checked', false).prop('disabled', true).change();
                        });
                    }
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            };
            if (isDischarged) {
                const dialogText = 'Q12B is a discharge date, unlock all related discharge fields:  Q15B,Q16B, Q17B, Q21B, Q41, Q44C';
                console.log(dialogText);
                if (((_d = e.data) === null || _d === void 0 ? void 0 : _d.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                    //with warning dialog
                    dialogOptions.title = 'Date';
                    $('#dialog')
                        .text(dialogText)
                        .dialog(dialogOptions, {
                        title: 'Warning Q12B', buttons: myButtons
                    });
                }
                else {
                    //without warning dialog
                    //DischargeRelated.each(function () {
                    //  const thisRelatedQ = $(this);
                    //  thisRelatedQ.prop('disabled', false).change();
                    //});
                    console.log('reset and enable Q15');
                    Q15B.val(-1).prop('disabled', false).change();
                    console.log('reset and enable Q16B');
                    Q16B.val(-1).prop('disabled', false).change();
                    console.log('reset and enable Q17B');
                    Q17B.val(-1).prop('disabled', false).change();
                    console.log('reset and enable Q21B');
                    Q21B.val(-1).prop('disabled', false).change();
                    Q41.each(function () {
                        const thisQ41 = $(this);
                        console.log('uncheck and enable ' + thisQ41.prop('id'));
                        thisQ41.prop('checked', false).prop('disabled', false).change();
                    });
                    Q44C.each((i, el) => {
                        const thisQ44C = $(el);
                        console.log('uncheck and enable ' + thisQ44C.prop('id'));
                        thisQ44C.prop('checked', false).prop('disabled', false).change();
                    });
                }
            }
            else {
                const dialogText = 'Q12B is not a discharge date. Also reset and lock related discharge fields:  Q15B,Q16B, Q17B, Q21B, Q41, Q44C';
                console.log('checkQ12B() ' + dialogText);
                if (((_e = e.data) === null || _e === void 0 ? void 0 : _e.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                    //with warning dialog
                    dialogOptions.title = 'Date';
                    $('#dialog')
                        .text(dialogText)
                        .dialog(dialogOptions, {
                        title: 'Warning 12B', buttons: myButtons
                    });
                }
                else {
                    //without warning dialog
                    //DischargeRelated.each(function () {
                    //  const thisRelatedQ = $(this);
                    //  commonUtility.resetControlValue(thisRelatedQ); //change() event would be fired in the commonUtility
                    //  thisRelatedQ.prop('disabled', true).change();
                    //});
                    console.log('checkQ12B() reset and disable Q15B');
                    Q15B.val(-1).prop('disabled', true).change();
                    console.log('checkQ12B() reset and disable Q16B');
                    Q16B.val(-1).prop('disabled', true).change();
                    console.log('checkQ12B() reset and disable Q17B');
                    Q17B.val(-1).prop('disabled', true).change();
                    console.log('checkQ12B() reset and disable Q21B');
                    Q21B.val(-1).prop('disabled', true).change();
                    Q41.each(function () {
                        const thisQ41 = $(this);
                        console.log('checkQ12B() uncheck and disable Q21B');
                        thisQ41.prop('checked', false).prop('disabled', true).change();
                    });
                    Q44C.each((i, el) => {
                        const thisQ44C = $(el);
                        console.log('checkQ12B() uncheck and disable Q21B');
                        thisQ44C.prop('checked', false).prop('disabled', true).change();
                    });
                }
            }
        }
        /* on change */
        Q12B.on('change', { x: EnumChangeEventArg.Change, y: Q12B }, checkQ12B);
        /* on load */
        checkQ12B(EnumChangeEventArg.Load);
    }
    /* private function */
    function Q14B_enabled_if_Q14A_is_Yes() {
        console.log("branching::: inside of Q14B_enabled_if_Q14A_is_Yes");
        /* nested event handler */
        function checkQ14A(e) {
            var _a, _b, _c;
            console.log('inside of checkQ14A(e) e.data.y', (_a = e.data) === null || _a === void 0 ? void 0 : _a.y);
            const Q14AYes = $('.persistable[id^=Q14A_][id*=Yes]:checked').length === 1;
            const Q14Bs = $('.persistable[id^=Q14B_]');
            let seenTheDialog = false;
            if (Q14AYes) {
                const dialogText = 'Q14A is Yes, unlock all Q14B and focus on first Q14B';
                console.log(dialogText);
                if (Q14Bs.length > 0) {
                    if (((_b = e.data) === null || _b === void 0 ? void 0 : _b.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                        const myButtons = {
                            "Ok": function () {
                                Q14Bs.each(function () {
                                    $(this).prop('disabled', false);
                                });
                                seenTheDialog = true;
                                $(this).dialog("close");
                            },
                            "Cancel": function () {
                                $(this).dialog("close");
                                seenTheDialog = true;
                            }
                        };
                        dialogOptions.title = 'Date';
                        $('#dialog')
                            .text(dialogText)
                            .dialog(dialogOptions, {
                            title: 'Warning Q14B', buttons: myButtons
                        });
                    }
                    else {
                        /* without warning dialog */
                        Q14Bs.each(function () {
                            $(this).prop('disabled', false);
                        });
                    }
                    Q14Bs.first().focus();
                }
            }
            else {
                const dialogText = 'Q14A is not answered or is No, uncheck and lock all Q14B';
                console.log(dialogText);
                if (((_c = e.data) === null || _c === void 0 ? void 0 : _c.x) === EnumChangeEventArg.Change) {
                    //with warning dialog
                    const myButtons = {
                        "Ok": function () {
                            Q14Bs.each(function () {
                                $(this).prop('checked', false).prop('disabled', true);
                            });
                            seenTheDialog = true;
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            seenTheDialog = true;
                            $(this).dialog("close");
                        }
                    };
                    dialogOptions.title = 'Date';
                    $('#dialog')
                        .text(dialogText)
                        .dialog(dialogOptions, {
                        title: 'Warning 14A', buttons: myButtons
                    });
                }
                else {
                    Q14Bs.each(function () {
                        $(this).prop('checked', false).prop('disabled', true);
                    });
                }
            }
        }
        /* on change */
        const Q14A = $('.persistable[id^=Q14A_]');
        Q14A.on('change', { x: EnumChangeEventArg.Change, y: $(this) }, checkQ14A);
        /* on load */
        checkQ14A(EnumChangeEventArg.Load);
    }
    /* private function */
    function Q16A_is_Home_then_Q17() {
        //event hooked during checkAllRules()
        console.log('branching::: inside of Q16A_is_Home_then_Q17');
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
        /* nested event handler */
        function checkQ16A(e) {
            var _a;
            let seenTheDialog = false;
            const Q16AisHome = $(".persistable[id^=Q16A_] option:selected").text().indexOf('1. Home') !== -1;
            const myButtons = {
                "Ok": function () {
                    if (Q16AisHome)
                        actQ16A(false);
                    else
                        actQ16A(true);
                    $(this).dialog("close");
                    seenTheDialog = true;
                },
                "Cancel": function () {
                    $(this).dialog("close");
                    seenTheDialog = true;
                }
            };
            if (Q16AisHome) {
                const dialogText = 'Q16A is home, unlock Q17';
                console.log(dialogText);
                if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                    $('#dialog')
                        .text(dialogText)
                        .dialog(dialogOptions, {
                        title: 'Warning 16A, Q17', buttons: myButtons
                    });
                }
                else {
                    actQ16A(false);
                }
            }
            else {
                const dialogText = 'Q16A is not home, clear and lock Q17';
                console.log(dialogText);
                if (e === EnumChangeEventArg.Change && !seenTheDialog) {
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
        }
        /* on change */
        const Q16A = $('.persistable[id^=Q16A_]');
        Q16A.on('change', { x: EnumChangeEventArg.Change }, checkQ16A);
        /* on load */
        checkQ16A(EnumChangeEventArg.Load);
    }
    /* private function, not used per stakeholder request */
    function Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A() {
        //event hooked during checkAllRules()
        console.log('branching::: inside of Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A');
        let seenTheDialog = false;
        /* nested event handler */
        function checkArthritis(e) {
            var _a;
            console.log('inside of checkArthritis(e) $(this)', $(this));
            const thisICD = $(this);
            const myButtons = {
                "Ok": function () {
                    $('.persistable[id^=Q24A_]').prop('checked', false);
                    $(this).dialog("close");
                    seenTheDialog = true;
                },
                "Cancel": function () {
                    $(this).dialog("close");
                    seenTheDialog = true;
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
                if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                    if (!seenTheDialog) {
                        $('#dialog')
                            .text('Not an arthritis ICD, lock Q24A')
                            .dialog(dialogOptions, {
                            title: 'Warning Q21A, Q21B, Q22, Q24', buttons: myButtons
                        });
                    }
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
    /* private function */
    function Q42_Interrupted_then_Q43() {
        //event hooked during checkAllRules()
        console.log('branching::: inside of Q42_Interrupted_then_Q43');
        let Q42Yes = $('.persistable[id^=Q42][id*=Yes]:checked').length === 1;
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
        /* nested event handler */
        function checkQ42(e) {
            var _a, _b, _c;
            console.log('checkQ42() e.data', e.data);
            let dialogText;
            let Q42;
            if ((_a = e.data) === null || _a === void 0 ? void 0 : _a.y) {
                Q42 = (_b = e.data) === null || _b === void 0 ? void 0 : _b.y;
                Q42Yes = Q42.prop('id').indexOf('Yes') !== -1;
                if (Q42Yes)
                    dialogText = 'Q42 is a Yes, unlock all Q43 till the first blank';
                else
                    dialogText = 'Q42 is a No, reset and lock all Q43';
            }
            console.log(dialogText);
            let seenTheDialog = false;
            if (((_c = e.data) === null || _c === void 0 ? void 0 : _c.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                //with warning dialog
                const myButtons = {
                    "Ok": function () {
                        if (Q42Yes)
                            actQ42(false);
                        else
                            actQ42(true);
                        $(this).dialog("close");
                        seenTheDialog = true;
                    },
                    "Cancel": function () {
                        $(this).dialog("close");
                        seenTheDialog = true;
                    }
                };
                $('#dialog')
                    .text(dialogText)
                    .dialog(dialogOptions, {
                    title: 'Warning Q42, Q43', buttons: myButtons
                });
            }
            else {
                //without warning dialog
                if (Q42Yes)
                    actQ42(false);
                else
                    actQ42(true);
            }
        }
        /* on change */
        $('.persistable[id^=Q42_]').each(function () {
            let thisQ42 = $(this);
            thisQ42.on('change', { x: EnumChangeEventArg.Change, y: thisQ42 }, checkQ42);
        });
        /* on load */
        /* do not use each() because checkQ42 will only find once of the Yes if it is clicked, otherwise, Q43s disabled status will be always determined by the last iteration of each() */
        if (Q42Yes) {
            console.log('Q42_Interrupted_then_Q43() onload Q42Yes = ', Q42Yes);
            checkQ42(EnumChangeEventArg.Load);
        }
        else {
            //call actQ42 directly since it will iterate Q43s and disable each
            actQ42(true);
        }
    }
    /* private function */
    function Q43_Rules() {
        console.log('branching::: inside of Q43_Rules()');
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
                    thisQ43.prop('disabled', true);
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
                    thisQ43.prop('disabled', false);
                    thisQ43.next('.bi-calendar-x').prop('diabled', false);
                    return foundBlank;
                }
            }
        }
        let foundBlank;
        /* nested event handler */
        function checkQ43(e) {
            var _a, _b, _c, _d;
            console.log('e.data.y in Q43_Rules().checkQ43() =', (_a = e.data) === null || _a === void 0 ? void 0 : _a.y);
            const Q42Yes = $('.persistable[id^=Q42][id*=Yes]:checked').length === 1;
            const thisQ43 = (_b = e.data) === null || _b === void 0 ? void 0 : _b.y;
            const thisQ43CRUD = commonUtility.getCRUD(thisQ43, thisQ43.data('oldvalue'), thisQ43.val());
            let seenTheDialog = false;
            function myButtonsClosure(Q42Yes) {
                return function (thisQ43, thisQ43CRUD) {
                    seenTheDialog = true;
                    return {
                        "Ok": function () {
                            if (Q42Yes) {
                                switch (thisQ43CRUD) {
                                    case "D":
                                    case "D1":
                                    case "D2":
                                        foundBlank = actQ43s('D', thisQ43);
                                    default:
                                        foundBlank = actQ43s('CU', thisQ43);
                                }
                            }
                            else {
                                thisQ43.val('').prop('disabled', true);
                            }
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            $(this).dialog("close");
                        }
                    };
                };
            }
            if (Q42Yes) {
                console.log("thisQ43CRUD ? " + thisQ43CRUD === undefined ? "unchanged" : thisQ43CRUD);
                if (((_c = e.data) === null || _c === void 0 ? void 0 : _c.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                    //with warning dialog
                    const buttonQ42Yes = myButtonsClosure(Q42Yes);
                    $('#dialog')
                        .text('Find next blank Q43 for editing')
                        .dialog(dialogOptions, {
                        title: 'Warning Q43', buttons: buttonQ42Yes(thisQ43, thisQ43CRUD)
                    });
                }
                else {
                    //without warning dialog
                    switch (thisQ43CRUD) {
                        case "D1":
                        case "D1": {
                            foundBlank = actQ43s('D', thisQ43);
                            break;
                        }
                        default: {
                            foundBlank = actQ43s('CU', thisQ43);
                            break;
                        }
                    }
                }
            }
            else {
                if (((_d = e.data) === null || _d === void 0 ? void 0 : _d.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                    //with warning dialog
                    const buttonQ42No = myButtonsClosure(Q42Yes);
                    $('#dialog')
                        .text('Find next blank Q43 for editing')
                        .dialog(dialogOptions, {
                        title: 'Warning Q43', buttons: buttonQ42No(thisQ43, thisQ43CRUD)
                    });
                }
                else {
                    //without warning dialog
                    //commonUtility.resetControlValue(thisQ43)
                    thisQ43.val('');
                    thisQ43.prop('disabled', true);
                }
            }
        }
        /* no load.  It is handled by Q42_Interrupted_then_Q43()*/
        /* on change. Q43 only raised by change() event manually here or programmmatically in Q42*/
        const Q43s = $('.persistable[id^=Q43_]');
        Q43s.each(function () {
            if (!foundBlank) {
                const thisQ43 = $(this);
                thisQ43.on('change', { x: EnumChangeEventArg.Change, y: thisQ43 }, checkQ43);
            }
        });
    }
    function Q44C_Affect_Q44D_Q46() {
        //event hooked during checkAllRules()
        console.log('branching::: inside of Q44C_Affect_Q44D_Q46');
        const Q44C = $('.persistable[id^=Q44C_]');
        /* nested event handler */
        function checkQ44C(e) {
            var _a, _b, _c;
            const Q44D = $('.persistable[id^=Q44D_]');
            const Q45 = $('.persistable[id^=Q45_]');
            const Q46 = $('.persistable[id^=Q46_]');
            const myButtons = {
                "Ok": function () {
                    if (Q44D.length > 0) {
                        //commonUtility.resetControlValue(Q44D);
                        Q44D.val(-1);
                        Q44D.prop('disabled', true);
                    }
                    if (Q45.length > 0) {
                        Q45.prop('disabled', true);
                    }
                    if (Q46.length > 0) {
                        Q46.prop('disabled', false).focus();
                    }
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            };
            const Q44C_is_Yes = $('.persistable[id^=Q44C_][id*=Yes]:checked').length === 1;
            if (Q44C_is_Yes) {
                console.log(((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) + ' Q44C is yes, unlock Q44D, Q45 and focus on Q44D');
                if (Q44D.length > 0) {
                    Q44D.prop('disabled', false).focus();
                }
                if (Q45.length > 0) {
                    Q45.prop('disabled', false);
                }
            }
            else {
                console.log(((_b = e.data) === null || _b === void 0 ? void 0 : _b.x) + 'Q44C is no, lock Q44D, Q45 and focus on Q46');
                if (((_c = e.data) === null || _c === void 0 ? void 0 : _c.x) === EnumChangeEventArg.Change) {
                    //with warning dialog
                    $('#dialog')
                        .text('Q44C is no, lock Q44D, Q45 and focus on Q46')
                        .dialog(dialogOptions, {
                        title: 'Warning Q44C, Q44D, Q45, Q46', buttons: myButtons
                    });
                }
                else {
                    //without warning dialog
                    if (Q44D.length > 0) {
                        //commonUtility.resetControlValue(Q44D);
                        Q44D.val(-1).change();
                        Q44D.prop('disabled', true);
                    }
                    if (Q45.length > 0) {
                        Q45.prop('disabled', true);
                    }
                    if (Q46.length > 0) {
                        Q46.prop('disabled', false).focus();
                    }
                }
            }
        }
        /* on change */
        Q44C.each(function (i, el) {
            const thisQ44C = $(el); //don't use $(this) because in the arrow function it will be undefined
            thisQ44C.on('change', { x: EnumChangeEventArg.Change }, checkQ44C);
        });
        /* on load */
        checkQ44C(EnumChangeEventArg.Load);
    }
    /* private function */
    function GG0170JKLMN_depends_on_GG0170I() {
        //sample code
        //var extra_data = { one: "One", two: "Two" };
        //var make_handler = function (extra_data) {
        //  return function (event) {
        //    // event and extra_data will be available here
        //  };
        //};
        //element.addEventListener("click", make_handler(extra_data));
        //event hooked during checkAllRules()
        console.log('branching::: inside of GG0170JKLMN_depends_on_GG0170I');
        const GG0170I = $('.persistable[id^=GG0170I]:not([id*=Discharge_Goal])');
        let thisI;
        function actGG0170I(GG0170JKL, focusThis, isDisabled, eventArg) {
            console.log('actGG0170I() focusThis', focusThis);
            GG0170JKL.each(function () {
                commonUtility.resetControlValue($(this));
                $(this).prop('disabled', isDisabled);
            });
            if (eventArg === EnumChangeEventArg.Change)
                scrollTo(focusThis);
            else
                focusThis.focus();
        }
        /* nested event handler */
        function checkGG0170I(e) {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j;
            console.log('checkGG0170I() eventData ' + ((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) + ' checkGG0170I() e.data.x = ' + ((_b = e.data) === null || _b === void 0 ? void 0 : _b.x) + ' e.data.y = ' + ((_c = e.data) === null || _c === void 0 ? void 0 : _c.y));
            if ((_d = e.data) === null || _d === void 0 ? void 0 : _d.y)
                thisI = (_e = e.data) === null || _e === void 0 ? void 0 : _e.y;
            console.log('checkGG0170I() thisI =', thisI);
            const intGG0170I = commonUtility.getControlValue(thisI);
            console.log('checkGG0170I() eventData ' + ((_f = e.data) === null || _f === void 0 ? void 0 : _f.x) + ' commonUtility.getControlValue(' + thisI.prop('id') + ') = ', intGG0170I);
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
            GG0170JKL = $('.persistable[id^=GG0170J_' + measure + '], .persistable[id^=GG0170K_' + measure + '], .persistable[id^=GG0170L_' + measure + ']');
            GG0170M = $('.persistable[id^=GG0170M_' + measure + ']');
            GG0170J = $('.persistable[id^=GG0170J_' + measure + ']');
            console.log('checkGG0170I() GG017M', GG0170M);
            console.log('checkGG0170I() GG017J', GG0170J);
            console.log('checkGG0170I() GG017JKL', GG0170JKL);
            const myButtons = {
                "Ok": function () {
                    switch (true) {
                        case (intGG0170I > 0 && intGG0170I < 7):
                            const focusJ = GG0170J;
                            const enableJKL = false;
                            actGG0170I(GG0170JKL, focusJ, enableJKL, EnumChangeEventArg.Change);
                            break;
                        default:
                            const focusM = GG0170M;
                            const disableJKL = true;
                            actGG0170I(GG0170JKL, focusM, disableJKL, EnumChangeEventArg.Change);
                            break;
                    }
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            };
            switch (true) {
                case (intGG0170I > 0 && intGG0170I < 7):
                    {
                        let dialogText = 'GG0170I ' + measure + ' is between 1 and 6, unlock GG0170J ' + measure + ', GG0170K ' + measure + ', GG0170L ' + measure + ', and advance to GG0170J ' + measure + '. Other measures are kept intact.';
                        console.log('checkGG0170I() eventData ' + ((_g = e.data) === null || _g === void 0 ? void 0 : _g.x) + dialogText);
                        if (((_h = e.data) === null || _h === void 0 ? void 0 : _h.x) === EnumChangeEventArg.Change) {
                            //with warning dialog
                            $('#dialog')
                                .text(dialogText)
                                .dialog(dialogOptions, {
                                title: 'Warning GG0170IJKLM', buttons: myButtons
                            });
                        }
                        else {
                            //without warning dialog
                            /* unlock and clear J K L, skip to J */
                            const focusJ = GG0170J;
                            const DisaenableJKL = false;
                            actGG0170I(GG0170JKL, focusJ, DisaenableJKL, EnumChangeEventArg.Load);
                        }
                    }
                    break;
                default: {
                    /* GG0170I is not selected, clear and lock J K L then advance to M */
                    const dialogText = 'GG0170I ' + measure + ' is unknown or is not between 1 and 6, clear and lock GG0170J ' + measure + ', GG0170K ' + measure + 'and GG0170L ' + measure + ', then advance to GG0170M ' + measure;
                    console.log('checkGG0170I() eventData ' + dialogText);
                    if (((_j = e.data) === null || _j === void 0 ? void 0 : _j.x) === EnumChangeEventArg.Change) {
                        //with warning dialog
                        $('#dialog')
                            .text(dialogText)
                            .dialog(dialogOptions, {
                            title: 'Warning GG0170JKLM', buttons: myButtons
                        });
                    }
                    else {
                        //without warning dialog
                        const focusM = GG0170M;
                        const disableJKL = true;
                        actGG0170I(GG0170JKL, focusM, disableJKL, EnumChangeEventArg.Load);
                    }
                }
            }
        }
        /* on change */
        GG0170I.each(function () {
            $(this).on('change', { x: EnumChangeEventArg.Change, y: $(this) }, checkGG0170I);
        });
        /* on load */
        GG0170I.each(function () {
            thisI = $(this);
            if (commonUtility.getControlValue(thisI) > 0)
                checkGG0170I(EnumChangeEventArg.Load);
        });
    }
    /* private function */
    function GG0170P_depends_on_GG0170M_and_GG0170N() {
        //event hooked during checkAllRules()
        console.log('branching::: inside of GG0170P_depends_on_GG0170M_and_GG0170N');
        let thisMorN;
        const GG0170M_and_N = $('.persistable[id^=GG0170M]:not([id*=Discharge_Goal]), .persistable[id^=GG0170N]:not([id*=Discharge_Goal])');
        /* nested event handler */
        function checkGG0170MN(e) {
            var _a, _b, _c, _d, _e;
            console.log('checkGG0170MN() e.data?.x = ' + ((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) + 'e.data?.y = ' + ((_b = e.data) === null || _b === void 0 ? void 0 : _b.y));
            if ((_c = e.data) === null || _c === void 0 ? void 0 : _c.y)
                thisMorN = (_d = e.data) === null || _d === void 0 ? void 0 : _d.y;
            console.log('checkGG0170MN() thisMorN in e.data.y = ', thisMorN);
            let GG0170M, GG0170N, GG0170O, GG0170P;
            /* thisMorN could be either M or N */
            if (thisMorN.prop('id').indexOf('GG0170M') !== -1)
                GG0170M = thisMorN;
            if (thisMorN.prop('id').indexOf('GG0170N') !== -1)
                GG0170N = thisMorN;
            let measure;
            if (GG0170M != null) {
                console.log("checkGG0170MN() it's M", GG0170M);
                /* determ if this M is an admission_performance */
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
            else {
                console.log("checkGG0170MN() it's N", GG0170N);
                /* goes to O or P */
                let measure;
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
            const intGG0170 = commonUtility.getControlValue(thisMorN);
            ;
            const myButtons = {
                "Ok": function () {
                    if (GG0170M) {
                        switch (true) {
                            case (intGG0170 >= 7 && GG0170P.length > 0):
                                /* M >= 7, reset and lock both N and O then advance to P */
                                if (GG0170N.length > 0) {
                                    //commonUtility.resetControlValue(GG0170N);
                                    GG0170N.val(-1).change();
                                    GG0170N.prop('disabled', true);
                                }
                                if (GG0170O.length > 0) {
                                    //commonUtility.resetControlValue(GG0170O);
                                    GG0170O.val(-1).change();
                                    GG0170O.prop('disabled', true);
                                }
                                scrollTo(GG0170P);
                                break;
                            case ((intGG0170 > 0 && intGG0170 < 7) && GG0170N.length > 0):
                                /* M between 0 and 6, scroll to N */
                                if (GG0170O.length > 0) {
                                    GG0170O.prop('disabled', false);
                                    scrollTo(GG0170N);
                                }
                                break;
                            default:
                                /* M is unknown, reset and lock N and O */
                                //commonUtility.resetControlValue(GG0170N);
                                GG0170N.val(-1);
                                GG0170N.prop('disabled', true);
                                //commonUtility.resetControlValue(GG0170O);
                                GG0170O.val(-1);
                                GG0170O.prop('disabled', true);
                                break;
                        }
                    }
                    if (GG0170N) {
                        switch (true) {
                            case (intGG0170 >= 7 && GG0170P.length > 0):
                                /* N >= 7, reset and lock both N and O then advance to P */
                                if (GG0170O.length > 0) {
                                    //commonUtility.resetControlValue(GG0170O
                                    GG0170O.val(-1);
                                    GG0170O.prop('disabled', true);
                                }
                                scrollTo(GG0170P);
                                break;
                            case ((intGG0170 > 0 && intGG0170 < 7) && GG0170N.length > 0):
                                /* N between 0 and 6, unlock and scroll to N */
                                if (GG0170O.length > 0) {
                                    //commonUtility.resetControlValue(GG0170O);
                                    GG0170O.val(-1);
                                    GG0170O.prop('disabled', false);
                                    scrollTo(GG0170O);
                                }
                                break;
                            default:
                                /* N is unknown, reset and lock O */
                                //commonUtility.resetControlValue(GG0170O);
                                GG0170O.val(-1);
                                GG0170O.prop('disabled', true);
                        }
                    }
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            };
            switch ((_e = e.data) === null || _e === void 0 ? void 0 : _e.x) {
                case EnumChangeEventArg.Change: {
                    /* with warning dialog */
                    let dialogText;
                    if (GG0170M) {
                        switch (true) {
                            case (intGG0170 >= 7 && GG0170P.length > 0):
                                dialogText = 'M >= 7, reset and lock both N and O then advance to P';
                                break;
                            case ((intGG0170 > 0 && intGG0170 < 7) && GG0170N.length > 0):
                                dialogText = 'M between 0 and 6, scroll to N';
                                break;
                            default:
                                dialogText = 'M is unknown, reset and lock N and O';
                                break;
                        }
                        $('#dialog')
                            .text(dialogText)
                            .dialog(dialogOptions, {
                            title: 'Warning GG0170M', buttons: myButtons
                        });
                    }
                    if (GG0170N) {
                        switch (true) {
                            case (intGG0170 >= 7 && GG0170P.length > 0):
                                dialogText = 'N >= 7, reset and lock both N and O then advance to P';
                                break;
                            case ((intGG0170 > 0 && intGG0170 < 7) && GG0170N.length > 0):
                                dialogText = 'N between 0 and 6, unlock and scroll to N';
                                break;
                            case (intGG0170 <= 0):
                                dialogText = 'N is unknown, reset and lock O';
                                break;
                        }
                        $('#dialog')
                            .text(dialogText)
                            .dialog(dialogOptions, {
                            title: 'Warning GG0170N', buttons: myButtons
                        });
                    }
                    break;
                }
                default: {
                    /* without warning dialog */
                    if (GG0170M) {
                        switch (true) {
                            case (intGG0170 >= 7 && GG0170P.length > 0):
                                /* M >= 7, reset and lock both N and O then advance to P */
                                if (GG0170N.length > 0) {
                                    //commonUtility.resetControlValue(GG0170N);
                                    GG0170N.val(-1);
                                    GG0170N.prop('disabled', true);
                                }
                                if (GG0170O.length > 0) {
                                    //commonUtility.resetControlValue(GG0170O);
                                    GG0170O.val(-1);
                                    GG0170O.prop('disabled', true);
                                }
                                scrollTo(GG0170P);
                                break;
                            case ((intGG0170 > 0 && intGG0170 < 7) && GG0170N.length > 0):
                                /* M between 0 and 6, scroll to N */
                                if (GG0170O.length > 0) {
                                    GG0170O.prop('disabled', false);
                                    scrollTo(GG0170N);
                                }
                                break;
                            default:
                                /* M is unknown, reset and lock N and O */
                                //commonUtility.resetControlValue(GG0170N);
                                GG0170N.val(-1);
                                GG0170N.prop('disabled', true);
                                //commonUtility.resetControlValue(GG0170O);
                                GG0170O.val(-1);
                                GG0170O.prop('disabled', true);
                                break;
                        }
                    }
                    if (GG0170N) {
                        switch (true) {
                            case (intGG0170 >= 7 && GG0170P.length > 0):
                                /* N >= 7, reset and lock both N and O then advance to P */
                                if (GG0170O.length > 0) {
                                    //commonUtility.resetControlValue(GG0170O);
                                    GG0170O.val(-1);
                                    GG0170O.prop('disabled', true);
                                }
                                scrollTo(GG0170P);
                                break;
                            case ((intGG0170 > 0 && intGG0170 < 7) && GG0170N.length > 0):
                                /* N between 0 and 6, unlock and scroll to N */
                                if (GG0170O.length > 0) {
                                    //commonUtility.resetControlValue(GG0170O);
                                    GG0170O.val(-1);
                                    GG0170O.prop('disabled', false);
                                    scrollTo(GG0170O);
                                }
                                break;
                            default:
                                /* N is unknown, reset and lock O */
                                //commonUtility.resetControlValue(GG0170O);
                                GG0170O.val(-1);
                                GG0170O.prop('disabled', true);
                        }
                    }
                    break;
                }
            }
        }
        /* on change */
        GG0170M_and_N.each(function () {
            $(this).on('change', { x: EnumChangeEventArg.Change, y: $(this) }, checkGG0170MN);
        });
        /* on load */
        GG0170M_and_N.each(function () {
            thisMorN = $(this);
            if (commonUtility.getControlValue(thisMorN) > 0)
                checkGG0170MN(EnumChangeEventArg.Load);
        });
    }
    /* private function */
    function GG0170Q_is_No_skip_to_Complete() {
        console.log('branching::: inside of GG0170Q_is_No_skip_to_Complete');
        const GG0170Q = $('.persistable[id^=GG0170Q_]:not([id*=Discharge_Goal])');
        const completed = $('.persistable[id ^= Assessment][id*=Yes]');
        let thisQ;
        function checkGG0170Q(e) {
            var _a;
            let GG0170Rs = null, checkNext = true;
            const thisQ_is_No = thisQ.prop('id').indexOf('No') !== -1;
            const myButtons = {
                "Ok": function () {
                    scrollTo(completed);
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            };
            if (thisQ_is_No) {
                const dialogText = 'At least one of the GG0170Qs is a no, lock all GG0170Rs, advance to Assesment Complete';
                console.log('checkGG0170Q() ' + dialogText);
                if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change) {
                    //with warning dialog
                    $('#dialog')
                        .text(dialogText)
                        .dialog(dialogOptions, {
                        title: 'Warning GG0170Q', buttons: myButtons
                    });
                }
                else {
                    completed.focus();
                }
            }
            else {
                if (checkNext) {
                    const GG0170Q_Admission_Performance_Yes = $('.persistable[id^=GG0170Q_Admission_Performance][id*=Yes]:checked').length === 1;
                    if (GG0170Q_Admission_Performance_Yes) {
                        console.log('checkGG0170Q() GG0170Q Admission Performance is yes, unlock and focus on GG0170R Admission Performance');
                        GG0170Rs = $('.persistable[id^=GG0170R_Admission]');
                        console.log('checkGG0170Q() GG0170Rs', GG0170Rs);
                        GG0170Rs.each(function () {
                            $(this).prop('disabled', false);
                        });
                        if (e === EnumChangeEventArg.Change) {
                            scrollTo(GG0170Rs.first());
                        }
                        GG0170Rs.first().focus();
                        checkNext = false;
                    }
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
                        if (e === EnumChangeEventArg.Change) {
                            scrollTo(GG0170Rs.first());
                        }
                        GG0170Rs.first().focus();
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
                        if (e === EnumChangeEventArg.Change) {
                            scrollTo(GG0170Rs.first());
                        }
                        GG0170Rs.first().focus();
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
                        if (e === EnumChangeEventArg.Change) {
                            scrollTo(GG0170Rs.first());
                        }
                        GG0170Rs.first().focus();
                        checkNext = false;
                    }
                }
            }
        }
        /* on change */
        GG0170Q.each(function () {
            thisQ = $(this);
            thisQ.on('change', { x: EnumChangeEventArg.Change }, checkGG0170Q);
        });
        /* on load. do not use because we don't want to jump to completion at load */
        //GG0170Q.each(function () {
        //  thisQ = $(this);
        //  checkGG0170Q(EnumChangeEventArg.Load);
        //});
    }
    /* private function */
    function J1750_depends_on_J0510() {
        console.log('branching::: inside of J1750_depends_on_J0510');
        let thisJ0510;
        const J1750s = $('.persistable[id^=J1750]');
        const J1750_yes = $('.persistable[id^=J1750][id*=Yes]');
        function actJ0510(isDisabled, eventArg) {
            J1750s.each(function () {
                $(this).prop('disabled', isDisabled);
            });
            if (!isDisabled && eventArg === EnumChangeEventArg.Change) {
                console.log('scroll to ' + J1750_yes.prop('id'));
                scrollTo(J1750_yes);
            }
        }
        function checkJ0510(e) {
            var _a, _b, _c;
            const this_has_Not_Apply = $(".persistable[id=" + thisJ0510.prop('id') + "] option:selected").text().indexOf('0.') !== -1;
            if (this_has_Not_Apply) {
                console.log('This J0510 dropdown has "0. Does not apply", unlock all J1750s and focus on J1750 Yes option');
                actJ0510(false, (_a = e.data) === null || _a === void 0 ? void 0 : _a.x);
            }
            else {
                //check other J0510 if they don't have 0 selected
                const other_has_Not_Apply = $(".persistable[id^=J0510]:not([id=" + thisJ0510.prop('id') + "]) option:selected").text().indexOf('0.') !== -1;
                if (other_has_Not_Apply)
                    actJ0510(false, (_b = e.data) === null || _b === void 0 ? void 0 : _b.x);
                else
                    actJ0510(true, (_c = e.data) === null || _c === void 0 ? void 0 : _c.x);
            }
        }
        /* target */
        const J0510 = $('.persistable[id^=J0510]');
        /* on change */
        J0510.each(function () {
            thisJ0510 = $(this);
            thisJ0510.on('change', { x: EnumChangeEventArg.Change }, checkJ0510);
        });
        /* on load. if found does not apply then breakout the each() , otherwise following iteration will reset the condition */
        let this_is_Not_Apply = false;
        J0510.each(function () {
            thisJ0510 = $(this);
            this_is_Not_Apply = $(".persistable[id=" + thisJ0510.prop('id') + "] option:selected").text().indexOf('0.') !== -1;
            if (this_is_Not_Apply)
                return false; //break out J0510.each
        });
        if (this_is_Not_Apply) {
            actJ0510(false, EnumChangeEventArg.Load);
        }
        else {
            //reset all J1750s
            actJ0510(true, EnumChangeEventArg.Load);
        }
    }
    /*private function*/
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
    /* private function */
    function LoadAllRules() {
        console.log('branching:::', 'Inside of LoadAllRules() loadEventData');
        Q12_Q23_blank_then_Lock_All();
        Q12B_blank_then_Lock_Discharge();
        Q14B_enabled_if_Q14A_is_Yes();
        Q16A_is_Home_then_Q17();
        Q42_Interrupted_then_Q43();
        Q43_Rules();
        Q44C_Affect_Q44D_Q46();
        GG0170JKLMN_depends_on_GG0170I();
        GG0170P_depends_on_GG0170M_and_GG0170N();
        GG0170Q_is_No_skip_to_Complete();
        J1750_depends_on_J0510();
        const Q12 = $(".persistable[id^=Q12_]");
        //set focus on Q12 after all the above that might have scrolled the screen during load
        scrollTo(Q12);
    }
    /***************************************************************************
     * public functions exposing the private functions to outside of the closure
    ***************************************************************************/
    return {
        'LoadAllRules': LoadAllRules
    };
})();
/******************************* end of closure ****************************/
$(function () {
    let sysTitle = $('.pageTitle').data('systitle');
    /* on ready */
    if (sysTitle === 'Full') {
        $('.persistable').each(function () { $(this).prop("disabled", true); });
    }
    else {
        console.log('branching::: LoadAllRules');
        branchingController.LoadAllRules();
    }
});
//# sourceMappingURL=branching.js.map