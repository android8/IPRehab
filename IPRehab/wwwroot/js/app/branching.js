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
    const dialogOptions = commonUtility.dialogOptions();
    /* private function */
    function Q12_Q23_blank_then_Lock_All() {
        console.log('branching:::', 'Inside of Q12_Q23_blank_then_Lock_All()');
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
                        title: 'Warning', buttons: myButtons
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
                            title: 'Warning', buttons: myButtons
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
        const primaryKeys = $('.persistable[id^=Q12_], .persistable[id^=Q23]');
        /* on check */
        primaryKeys.each(function () {
            $(this).on('change', { x: EnumChangeEventArg.Change }, checkQ12_Q23);
        });
        /* on load */
        checkQ12_Q23(EnumChangeEventArg.Load);
    }
    /* private function */
    function Q12B_blank_then_Lock_Discharge() {
        //event hooked during checkAllRules()
        console.log("branching::: inside of Q12B_blank_then_Lock_Discharge");
        const Q12B = $('.persistable[id^=Q12B_]');
        /* nested event handler */
        function checkQ12B(e) {
            var _a, _b;
            //lock all field with pertaining discharge Q15B,Q16B, Q17B, Q21B, Q41, Q44C
            console.log('inside of checkQ12B(e) Q12B.val()', Q12B.val());
            const isDischarged = ((_a = Q12B.val()) === null || _a === void 0 ? void 0 : _a.toString()) !== '';
            const DischargeRelated = $('.persistable[id ^= Q15B],.persistable[id ^= Q16B],.persistable[id ^= Q17B],.persistable[id ^= Q21B],.persistable[id ^= Q41],.persistable[id ^= Q44C]');
            if (isDischarged) {
                console.log('Q12B is a discharge date, unlock all discharged Qs in the base series only, not other questions in other series');
                DischargeRelated.each(function () {
                    const thisRelatedQ = $(this);
                    thisRelatedQ.prop('disabled', false);
                });
            }
            else {
                console.log('Q12B is not a discharge date, lock all related discharged Q series', DischargeRelated);
                if (((_b = e.data) === null || _b === void 0 ? void 0 : _b.x) === EnumChangeEventArg.Change) {
                    //with warning dialog
                    const myButtons = {
                        "Ok": function () {
                            DischargeRelated.each(function () {
                                const thisRelatedQ = $(this);
                                commonUtility.resetControlValue(thisRelatedQ); //change() event would be fired in the commonUtility
                                thisRelatedQ.prop('disabled', true);
                            });
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            $(this).dialog("close");
                        }
                    };
                    dialogOptions.title = 'Date';
                    $('#dialog')
                        .text('Reseting discharge date also reset the following related discharge fields:  Q15B,Q16B, Q17B, Q21B, Q41, Q44C')
                        .dialog(dialogOptions, {
                        title: 'Warning', buttons: myButtons
                    });
                }
                else {
                    //without warning dialog
                    DischargeRelated.each(function () {
                        const thisRelatedQ = $(this);
                        commonUtility.resetControlValue(thisRelatedQ); //change() event would be fired in the commonUtility
                        thisRelatedQ.prop('disabled', true);
                    });
                }
            }
        }
        /* on change */
        Q12B.on('change', { x: EnumChangeEventArg.Change }, checkQ12B);
        /* on load */
        checkQ12B(EnumChangeEventArg.Load);
    }
    /* private function */
    function Q14B_enabled_if_Q14A_is_Yes() {
        console.log("branching::: inside of Q14B_enabled_if_Q14A_is_Yes");
        const Q14A = $('.persistable[id^=Q14A_]');
        /* nested event handler */
        function checkQ14A(e) {
            var _a, _b;
            console.log('inside of checkQ14A(e)', e);
            const Q14AYes = $('.persistable[id^=Q14A_][id*=Yes]:checked').length === 1;
            const Q14Bs = $('.persistable[id^=Q14B_]');
            if (Q14AYes) {
                console.log('Q14A is Yes, unlock all Q14B and focus on first Q14B');
                if (Q14Bs.length > 0) {
                    if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change) {
                        const myButtons = {
                            "Ok": function () {
                                Q14Bs.each(function () {
                                    $(this).prop('disabled', false);
                                });
                                $(this).dialog("close");
                            },
                            "Cancel": function () {
                                $(this).dialog("close");
                            }
                        };
                        dialogOptions.title = 'Date';
                        $('#dialog')
                            .text('14A is Yes, unlock all Q14B and focus on first Q14B')
                            .dialog(dialogOptions, {
                            title: 'Warning', buttons: myButtons
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
                console.log('Q14A is not answered or is No, uncheck and lock all Q14B');
                if (((_b = e.data) === null || _b === void 0 ? void 0 : _b.x) === EnumChangeEventArg.Change) {
                    //with warning dialog
                    const myButtons = {
                        "Ok": function () {
                            Q14Bs.each(function () {
                                $(this).prop('checked', false).prop('disabled', true);
                            });
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            $(this).dialog("close");
                        }
                    };
                    dialogOptions.title = 'Date';
                    $('#dialog')
                        .text('Q14A is not answered or is No, uncheck and lock all Q14B')
                        .dialog(dialogOptions, {
                        title: 'Warning', buttons: myButtons
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
        Q14A.on('change', { x: EnumChangeEventArg.Change, id: $(this).prop('id') }, checkQ14A);
        /* on load */
        checkQ14A(EnumChangeEventArg.Load);
    }
    /* private function */
    function Q16A_is_Home_then_Q17() {
        //event hooked during checkAllRules()
        console.log('branching::: inside of Q14B_enabled_if_Q14A_is_Yes');
        const Q16A = $('.persistable[id^=Q16A_]');
        const Q16AisHome = $(".persistable[id^=Q16A_] option:selected").text().indexOf('1. Home') === -1;
        const Q17 = $(".persistable[id^=Q17_]");
        function actQ16A(isDisabled) {
            if (isDisabled) {
                commonUtility.resetControlValue(Q17);
                Q17.prop('disabled', isDisabled);
                commonUtility.resetControlValue(Q17.next(), '');
            }
            else {
                Q17.prop('disabled', isDisabled).focus();
            }
        }
        /* nested event handler */
        function checkQ16A(e) {
            var _a;
            if (Q16AisHome) {
                const disableQ17 = true;
                if (Q17.length > 0) {
                    if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change) {
                        const myButtons = {
                            "Ok": function () {
                                actQ16A(disableQ17);
                                $(this).dialog("close");
                            },
                            "Cancel": function () {
                                $(this).dialog("close");
                            }
                        };
                        $('#dialog')
                            .text('Q16A is not home, clear and lock Q17')
                            .dialog(dialogOptions, {
                            title: 'Warning', buttons: myButtons
                        });
                    }
                    else {
                        actQ16A(disableQ17);
                    }
                }
            }
            else {
                console.log('Q16A is home, unlock and focus on Q17');
                const enableQ17 = false;
                if (Q17.length > 0) {
                    if (e === EnumChangeEventArg.Change) {
                        const myButtons = {
                            "Ok": function () {
                                actQ16A(enableQ17);
                                $(this).dialog("close");
                            },
                            "Cancel": function () {
                                $(this).dialog("close");
                            }
                        };
                        $('#dialog')
                            .text('Q16A is home, unlock and focus on Q17')
                            .dialog(dialogOptions, {
                            title: 'Warning', buttons: myButtons
                        });
                    }
                }
            }
        }
        /* on change */
        Q16A.on('change', { x: EnumChangeEventArg.Change }, checkQ16A);
        /* on load */
        checkQ16A(EnumChangeEventArg.Load);
    }
    /* private function, not used per stakeholder request */
    function Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A() {
        //event hooked during checkAllRules()
        console.log('branching::: inside of Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A');
        let seenDialog = false;
        /* nested event handler */
        function checkArthritis(e) {
            var _a;
            console.log('inside of checkArthritis(e) $(this)', $(this));
            const thisICD = $(this);
            const myButtons = {
                "Ok": function () {
                    $('.persistable[id^=Q24A_]').prop('checked', false);
                    $(this).dialog("close");
                    seenDialog = true;
                },
                "Cancel": function () {
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
                if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change) {
                    if (!seenDialog) {
                        $('#dialog')
                            .text('Not an arthritis ICD, lock Q24A')
                            .dialog(dialogOptions, {
                            title: 'Warning', buttons: myButtons
                        });
                    }
                    else {
                        $('.persistable[id^=Q24A_]').prop('checked', false);
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
        function actQ42(lockQ43) {
            const Q43s = $('.persistable[id^=Q43_]');
            Q43s.each(function () {
                const thisQ43 = $(this);
                if (lockQ43) {
                    thisQ43.prop('disabled', true);
                }
                else {
                    console.log('unlock Q43 then raise Q43.change()');
                    thisQ43.prop('disabled', false).change();
                }
            });
        }
        /* nested event handler */
        function checkQ42(e) {
            var _a;
            const Q42Yes = $('.persistable[id^=Q42][id*=Yes]:checked').length === 1;
            const dialogText = Q42Yes ? 'Q42 is a Yes, unlock all Q43 till the first blank' : 'Q42 is a No, reset and lock all Q43';
            console.log(Q42Yes ? 'Q42 is a Yes, unlock all Q43 till the first blank' : 'Q42 is a No, reset and lock all Q43');
            if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change) {
                //with warning dialog
                const myButtons = {
                    "Ok": function () {
                        if (Q42Yes)
                            actQ42(false);
                        else
                            actQ42(true);
                        $(this).dialog("close");
                    },
                    "Cancel": function () {
                        $(this).dialog("close");
                    }
                };
                $('#dialog')
                    .text(dialogText)
                    .dialog(dialogOptions, {
                    title: 'Warning', buttons: myButtons
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
            thisQ42.on('change', { x: EnumChangeEventArg.Change }, checkQ42);
        });
        /* on load */
        /* do not use each() because checkQ42 will only find once of the Yes if it is clicked, otherwise, Q43s disabled status will be always determined by the last iteration of each() */
        checkQ42(EnumChangeEventArg.Load);
    }
    /* private function */
    function Q43_Rules() {
        console.log('branching::: inside of Q43_Rules()');
        function actQ43s(CRUD, thisQ43) {
            console.log('thisQ43 in act43s()', thisQ43);
            switch (CRUD) {
                case 'D': {
                    thisQ43.prop('disabled', true);
                    thisQ43.nextAll().each(function () {
                        const thisNext = $(this);
                        thisNext.val('').prop('disabled', true);
                    });
                    const Q43NonBlanks = $('.persistable[id^=Q43][value!=""]');
                    if (Q43NonBlanks.length === 0) {
                        //set Q42 to No since all are blank
                        $('.persistable[id=^Q42][id*=No').prop('checked', true);
                    }
                    break;
                }
                case "CU": {
                    thisQ43.nextAll(".persistable").each(function () {
                        const thisNextQ43 = $(this);
                        if (thisNextQ43.val() === '') {
                            thisNextQ43.prop('disabled', false).focus(); /* unlock this non-blank dates */
                            thisNextQ43.next().prop('disabled', false); /* unlock next reset button */
                            return false; /* shortcut each() loop since first blank is found */
                        }
                    });
                }
            }
        }
        /* nested event handler */
        function checkQ43(e) {
            var _a, _b, _c, _d;
            const Q42Yes = $('.persistable[id^=Q42][id*=Yes]:checked').length === 1;
            console.log('e.data.y in Q43_Rules().checkQ43() =', (_a = e.data) === null || _a === void 0 ? void 0 : _a.y);
            let seenTheDialog = false;
            let thisQ43 = (_b = e.data) === null || _b === void 0 ? void 0 : _b.y;
            if (Q42Yes) {
                const thisQ43CRUD = commonUtility.getCRUD(thisQ43, thisQ43.val(), thisQ43.data('oldvalue'));
                console.log("thisQ43CRUD ? " + thisQ43CRUD == undefined ? "unchanged" : thisQ43CRUD);
                if (((_c = e.data) === null || _c === void 0 ? void 0 : _c.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                    //with warning dialog
                    switch (thisQ43CRUD) {
                        case 'D1':
                        case 'D2': {
                            console.log('D1 or D2');
                            const myButtons = {
                                "Ok": function () {
                                    actQ43s('D', thisQ43);
                                    $(this).dialog("close");
                                    seenTheDialog = true;
                                },
                                "Cancel": function () {
                                    $(this).dialog("close");
                                }
                            };
                            $('#dialog')
                                .text('Find next blank Q43 for editing')
                                .dialog(dialogOptions, {
                                title: 'Warning', buttons: myButtons
                            });
                            break;
                        }
                        default: //unchanged
                            {
                                console.log('C,U,and default');
                                const myButtons = {
                                    "Ok": function () {
                                        actQ43s('CU', thisQ43);
                                        $(this).dialog("close");
                                        seenTheDialog = true;
                                    },
                                    "Cancel": function () {
                                        $(this).dialog("close");
                                    }
                                };
                                $('#dialog')
                                    .text('Find next blank Q43 for editing')
                                    .dialog(dialogOptions, {
                                    title: 'Warning', buttons: myButtons
                                });
                                break;
                            }
                    }
                }
                else {
                    //without warning dialog
                    switch (thisQ43CRUD) {
                        case "D1":
                        case "D1": {
                            actQ43s('D', thisQ43);
                            break;
                        }
                        default: {
                            actQ43s('CU', thisQ43);
                            break;
                        }
                    }
                }
            }
            else {
                if (((_d = e.data) === null || _d === void 0 ? void 0 : _d.x) === EnumChangeEventArg.Change && !seenTheDialog) {
                    //with warning dialog
                    const myButtons = {
                        "Ok": function () {
                            commonUtility.resetControlValue(thisQ43);
                            thisQ43.prop('disabled', true);
                            $(this).dialog("close");
                        },
                        "Cancel": function () {
                            $(this).dialog("close");
                        }
                    };
                    $('#dialog')
                        .text('Find next blank Q43 for editing')
                        .dialog(dialogOptions, {
                        title: 'Warning', buttons: myButtons
                    });
                }
                else {
                    //without warning dialog
                    commonUtility.resetControlValue(thisQ43);
                    thisQ43.prop('disabled', true);
                }
            }
        }
        /* no load.  It is handled by Q42_Interrupted_then_Q43()*/
        /* on change. Q43 only raised by change() event manually here or programmmatically in Q42*/
        const Q43s = $('.persistable[id^=Q43_]');
        Q43s.each(function () {
            const thisQ43 = $(this);
            thisQ43.on('change', { x: EnumChangeEventArg.Change, y: thisQ43 }, checkQ43);
        });
    }
    function Q44C_Affect_Q44D_Q46() {
        //event hooked during checkAllRules()
        console.log('branching::: inside of Q44C_Affect_Q44D_Q46');
        const Q44C = $('.persistable[id^=Q44C_]');
        let thisQ44C;
        /* nested event handler */
        function checkQ44C(e) {
            var _a;
            const Q44D = $('.persistable[id^=Q44D_]');
            const Q45 = $('.persistable[id^=Q45_]');
            const Q46 = $('.persistable[id^=Q46_]');
            const Q44C_is_Yes = $('.persistable[id^=Q44C_][id*=Yes]:checked').length === 1;
            if (Q44C_is_Yes) {
                console.log('Q44C is yes, unlock Q44D, Q45 and focus on Q44D');
                if (Q44D.length > 0) {
                    Q44D.prop('disabled', false).focus();
                }
                if (Q45.length > 0) {
                    Q45.prop('disabled', false);
                }
            }
            else {
                console.log('Q44C is no, lock Q44D, Q45 and focus on Q46');
                if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change) {
                    //with warning dialog
                    const myButtons = {
                        "Ok": function () {
                            if (Q44D.length > 0) {
                                commonUtility.resetControlValue(Q44D);
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
                    $('#dialog')
                        .text('Q44C is no, lock Q44D, Q45 and focus on Q46')
                        .dialog(dialogOptions, {
                        title: 'Warning', buttons: myButtons
                    });
                }
                else {
                    //without warning dialog
                    if (Q44D.length > 0) {
                        commonUtility.resetControlValue(Q44D);
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
        Q44C.each(function () {
            thisQ44C = $(this);
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
        function actGG0170I(GG0170JKL, focusThis, isDisabled) {
            GG0170JKL.each(function () {
                commonUtility.resetControlValue($(this));
                $(this).prop('disabled', isDisabled);
            });
            commonUtility.scrollTo(focusThis.first());
            focusThis.first().focus();
        }
        /* nested event handler */
        function checkGG0170I(e) {
            var _a, _b;
            const intGG0170I = commonUtility.getControlValue(thisI);
            console.log('commonUtility.getControlValue(' + thisI.prop('id') + ') = ', intGG0170I);
            let GG0170J, GG0170JKL, GG0170M;
            if (thisI.prop('id').indexOf('Admission_Performance') !== -1) {
                GG0170JKL = $('.persistable[id^=GG0170J_Admission_Performance], .persistable[id^=GG0170K_Admission_Performance], .persistable[id^=GG0170L_Admission_Performance]');
                GG0170M = $('.persistable[id^=GG0170M_Admission_Performance]');
                GG0170J = $('.persistable[id^=GG0170J_Admission_Performance]');
            }
            else if (thisI.prop('id').indexOf('Discharge_Performance') !== -1) {
                GG0170JKL = $('.persistable[id^=GG0170J_Discharge_Performance], .persistable[id^=GG0170K_Discharge_Performance], .persistable[id^=GG0170L_Discharge_Performance]');
                GG0170M = $('.persistable[id^=GG0170M_Discharge_Performance]');
                GG0170J = $('.persistable[id^=GG0170J_Discharge_Performance]');
            }
            else if (thisI.prop('id').indexOf('Interim_Performance') !== -1) {
                GG0170JKL = $('.persistable[id^=GG0170J_Interim_Performance], .persistable[id^=GG0170K_Interim_Performance], .persistable[id^=GG0170L_Interim_Performance]');
                GG0170M = $('.persistable[id^=GG0170M_Interim_Performance]');
                GG0170J = $('.persistable[id^=GG0170J_Interim_Performance]');
            }
            else if (thisI.prop('id').indexOf('Followup_Performance') !== -1) {
                GG0170JKL = $('.persistable[id^=GG0170J_Followup_Performance], .persistable[id^=GG0170K_Followup_Performance], .persistable[id^=GG0170L_Followup_Performance]');
                GG0170M = $('.persistable[id^=GG0170M_Followup_Performance]');
                GG0170J = $('.persistable[id^=GG0170J_Followup_Performance]');
            }
            switch (true) {
                case (intGG0170I > 0 && intGG0170I <= 6):
                    {
                        console.log('I is between 1 and 6, unlock J, K, L, and advance to J');
                        if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change) {
                            //with warning dialog
                            const myButtons = {
                                "Ok": function () {
                                    const focusJ = GG0170J;
                                    const notDisaenableJKL = false;
                                    actGG0170I(GG0170JKL, focusJ, notDisaenableJKL);
                                    $(this).dialog("close");
                                },
                                "Cancel": function () {
                                    $(this).dialog("close");
                                }
                            };
                            $('#dialog')
                                .text('I is less than or equal to 6, unlock J, K, L, and advance t J')
                                .dialog(dialogOptions, {
                                title: 'Warning', buttons: myButtons
                            });
                        }
                        else {
                            //without warning dialog
                            /* unlock and clear J K L, skip to J */
                            const focusJ = GG0170J;
                            const notDisaenableJKL = false;
                            actGG0170I(GG0170JKL, focusJ, notDisaenableJKL);
                        }
                    }
                    break;
                default: {
                    /* GG0170I is not selected, clear and lock J K L */
                    console.log('GG0170I is not answered or not between 1 and 6, clear and lock J K L, then advance to M');
                    if (((_b = e.data) === null || _b === void 0 ? void 0 : _b.x) === EnumChangeEventArg.Change) {
                        //with warning dialog
                        const myButtons = {
                            "Ok": function () {
                                const focusM = GG0170M;
                                const disableJKL = true;
                                actGG0170I(GG0170JKL, focusM, disableJKL);
                                $(this).dialog("close");
                            },
                            "Cancel": function () {
                                $(this).dialog("close");
                            }
                        };
                        $('#dialog')
                            .text('GG0170I is not answered or not between 1 and 6, clear and lock J K L, then advance to M')
                            .dialog(dialogOptions, {
                            title: 'Warning', buttons: myButtons
                        });
                    }
                    else {
                        //without warning dialog
                        const focusM = GG0170M;
                        const disableJKL = true;
                        actGG0170I(GG0170JKL, focusM, disableJKL);
                    }
                }
            }
        }
        /* on change */
        GG0170I.each(function () {
            thisI = $(this);
            thisI.on('change', { x: EnumChangeEventArg.Change }, checkGG0170I);
        });
        /* on load */
        GG0170I.each(function () {
            thisI = $(this);
            checkGG0170I(EnumChangeEventArg.Load);
        });
    }
    /* private function */
    function GG0170P_depends_on_GG0170M_and_GG0170N() {
        //event hooked during checkAllRules()
        console.log('branching::: inside of GG0170P_depends_on_GG0170M_and_GG0170N');
        const GG0170M_and_N = $('.persistable[id^=GG0170M]:not([id*=Discharge_Goal]), .persistable[id^=GG0170N]:not([id*=Discharge_Goal])');
        let thisMorN;
        /* nested event handler */
        function checkGG0170MN(e) {
            var _a, _b, _c;
            console.log('thisMorN', thisMorN);
            /* thisGG0170 could be M or N so inspect .prop('id') to determine which */
            const intGG0170 = commonUtility.getControlValue(thisMorN);
            ;
            let GG0170M = null, GG0170N = null, GG0170O, GG0170P;
            console.log('determine the trigger is GG0170 M or N', thisMorN);
            if (thisMorN.prop('id').indexOf('GG0170M') !== -1)
                GG0170M = thisMorN;
            else
                GG0170N = thisMorN;
            if (GG0170M != null) {
                /* goes to N or P */
                console.log("it's M", GG0170M);
                if (GG0170M.prop('id').indexOf('Admission_Performance') !== -1) {
                    GG0170N = $('.persistable[id^=GG0170N_Admission_Performance]');
                    GG0170O = $('.persistable[id^=GG0170O_Admission_Performance]');
                    GG0170P = $('.persistable[id^=GG0170P_Admission_Performance]');
                }
                else if (GG0170M.prop('id').indexOf('Discharge_Performance') !== -1) {
                    GG0170N = $('.persistable[id^=GG0170N_Discharge_Performance]');
                    GG0170O = $('.persistable[id^=GG0170O_Discharge_Performance]');
                    GG0170P = $('.persistable[id^=GG0170P_Discharge_Performance]');
                }
                else if (GG0170M.prop('id').indexOf('Interim_Performance') !== -1) {
                    GG0170N = $('.persistable[id^=GG0170N_Interim_Performance]');
                    GG0170O = $('.persistable[id^=GG0170O_Interim_Performance]');
                    GG0170P = $('.persistable[id^=GG0170P_Interim_Performance]');
                }
                else if (GG0170M.prop('id').indexOf('Followup_Performance') !== -1) {
                    GG0170N = $('.persistable[id^=GG0170N_Followup_Performance]');
                    GG0170O = $('.persistable[id^=GG0170O_Followup_Performance]');
                    GG0170P = $('.persistable[id^=GG0170P_Followup_Performance]');
                }
            }
            else {
                console.log("it's N", GG0170N);
                /* goes to O or P */
                if (GG0170N.prop('id').indexOf('Admission_Performance') !== -1) {
                    GG0170O = $('.persistable[id^=GG0170O_Admission_Performance]');
                    GG0170P = $('.persistable[id^=GG0170P_Admission_Performance]');
                }
                else if (GG0170N.prop('id').indexOf('Discharge_Performance') !== -1) {
                    GG0170O = $('.persistable[id^=GG0170O_Discharge_Performance]');
                    GG0170P = $('.persistable[id^=GG0170P_Discharge_Performance]');
                }
                else if (GG0170N.prop('id').indexOf('Interim_Performance') !== -1) {
                    GG0170O = $('.persistable[id^=GG0170O_Interim_Performance]');
                    GG0170P = $('.persistable[id^=GG0170P_Interim_Performance]');
                }
                else if (GG0170N.prop('id').indexOf('Followup_Performance') !== -1) {
                    GG0170O = $('.persistable[id^=GG0170O_Followup_Performance]');
                    GG0170P = $('.persistable[id^=GG0170P_Followup_Performance]');
                }
            }
            switch (true) {
                case (intGG0170 >= 7):
                    console.log('I > 7 go to P');
                    if (GG0170P.length > 0) {
                        if (((_a = e.data) === null || _a === void 0 ? void 0 : _a.x) === EnumChangeEventArg.Change) {
                            //with warning dialog
                            const myButtons = {
                                "Ok": function () {
                                    if (GG0170P.length > 0) {
                                        commonUtility.scrollTo(GG0170P.first());
                                    }
                                    $(this).dialog("close");
                                },
                                "Cancel": function () {
                                    $(this).dialog("close");
                                }
                            };
                            $('#dialog')
                                .text('I > 7 go to P')
                                .dialog(dialogOptions, {
                                title: 'Warning', buttons: myButtons
                            });
                        }
                        else {
                            //without warning dialog
                            commonUtility.scrollTo(GG0170P.first());
                        }
                    }
                    break;
                case (intGG0170 > 0 && intGG0170 <= 6):
                    console.log('0 < I <= 6 go to N');
                    if (GG0170N.length > 0) {
                        if (((_b = e.data) === null || _b === void 0 ? void 0 : _b.x) === EnumChangeEventArg.Change) {
                            //with warning dialog
                            const myButtons = {
                                "Ok": function () {
                                    if (GG0170P.length > 0) {
                                        commonUtility.scrollTo(GG0170N.first());
                                    }
                                    $(this).dialog("close");
                                },
                                "Cancel": function () {
                                    $(this).dialog("close");
                                }
                            };
                            $('#dialog')
                                .text('0 < I <= 6 go to N')
                                .dialog(dialogOptions, {
                                title: 'Warning', buttons: myButtons
                            });
                        }
                        else {
                            //without warning dialog
                            commonUtility.scrollTo(GG0170N.first());
                        }
                    }
                    break;
                default:
                    console.log('I is blank, disable N and O');
                    if (((_c = e.data) === null || _c === void 0 ? void 0 : _c.x) === EnumChangeEventArg.Change) {
                        //with warning dialog
                        const myButtons = {
                            "Ok": function () {
                                if (GG0170N.length > 0) {
                                    GG0170N.prop('disabled', true);
                                }
                                if (GG0170O.length > 0) {
                                    GG0170O.prop('disabled', true);
                                }
                                $(this).dialog("close");
                            },
                            "Cancel": function () {
                                $(this).dialog("close");
                            }
                        };
                        $('#dialog')
                            .text('I is blank, disable GG0170N and GG0170O')
                            .dialog(dialogOptions, {
                            title: 'Warning', buttons: myButtons
                        });
                    }
                    else {
                        //without warning dialog
                        if (GG0170N.length > 0)
                            GG0170N.prop('disabled', true);
                        if (GG0170O.length > 0)
                            GG0170O.prop('disabled', true);
                    }
                    break;
            }
        }
        /* on change */
        GG0170M_and_N.each(function () {
            thisMorN = $(this);
            thisMorN.on('change', { x: EnumChangeEventArg.Change }, checkGG0170MN);
        });
        /* on load */
        GG0170M_and_N.each(function () {
            thisMorN = $(this);
            checkGG0170MN(EnumChangeEventArg.Load);
        });
    }
    /* private function */
    function GG0170Q_is_No_skip_to_Complete() {
        console.log('branching::: inside of GG0170Q_is_No_skip_to_Complete');
        const GG0170Q = $('.persistable[id^=GG0170Q]:not([id*=Discharge_Goal])');
        let thisQ;
        function checkGG0170Q(e) {
            let completed, GG0170Rs = null, checkNext = true;
            const GG0170Q_Admission_Performance_Yes = $('.persistable[id^=GG0170Q_Admission_Performance][id*=Yes]:checked').length === 1;
            if (checkNext) {
                if (GG0170Q_Admission_Performance_Yes) {
                    console.log('Q Admission Performance is yes, unlock and focus on R Admission Performance');
                    GG0170Rs = $('.persistable[id^=GG0170R_Admission]');
                    console.log('GG0170Rs', GG0170Rs);
                    GG0170Rs.each(function () {
                        $(this).prop('disabled', false);
                    });
                    if (e === EnumChangeEventArg.Change) {
                        commonUtility.scrollTo(GG0170Rs.first());
                    }
                    GG0170Rs.first().focus();
                    checkNext = false;
                }
            }
            if (checkNext) {
                const GG0170Q_Discharge_Performance_Yes = $('.persistable[id^=GG0170Q_Discharge_Performance][id*=Yes]:checked').length === 1;
                if (GG0170Q_Discharge_Performance_Yes) {
                    console.log('Q Discharge Performance is yes, unlock and focus on R Discharge Performance');
                    GG0170Rs = $('.persistable[id^=GG0170R_Discharge_Performance]');
                    console.log('GG0170Rs', GG0170Rs);
                    GG0170Rs.each(function () {
                        $(this).prop('disabled', false);
                    });
                    if (e === EnumChangeEventArg.Change) {
                        commonUtility.scrollTo(GG0170Rs.first());
                    }
                    GG0170Rs.first().focus();
                    checkNext = false;
                }
            }
            if (checkNext) {
                const GG0170Q_Interim_Performance_Yes = $('.persistable[id^=GG0170Q_Interim_Performance][id*=Yes]:checked').length === 1;
                if (GG0170Q_Interim_Performance_Yes) {
                    console.log('Q Interim Performance is yes, unlock and focus on R Interim Performance');
                    GG0170Rs = $('.persistable[id^=GG0170R_Interim_Performance]');
                    console.log('GG0170Rs', GG0170Rs);
                    GG0170Rs.each(function () {
                        $(this).prop('disabled', false);
                    });
                    if (e === EnumChangeEventArg.Change) {
                        commonUtility.scrollTo(GG0170Rs.first());
                    }
                    GG0170Rs.first().focus();
                    checkNext = false;
                }
            }
            if (checkNext) {
                const GG0170Q_Followup_Performance_Yes = $('.persistable[id^=GG0170Q_Followup_Performance][id*=Yes]:checked').length === 1;
                if (GG0170Q_Followup_Performance_Yes) {
                    console.log('Q Follow Up Performance is yes, unlock and focus on R Follow Up Performance');
                    GG0170Rs = $('.persistable[id^=GG0170R_Followup_Performance]');
                    console.log('GG0170Rs', GG0170Rs);
                    GG0170Rs.each(function () {
                        $(this).prop('disabled', false);
                    });
                    if (e === EnumChangeEventArg.Change) {
                        commonUtility.scrollTo(GG0170Rs.first());
                    }
                    GG0170Rs.first().focus();
                    checkNext = false;
                }
            }
            //none of the above so disable the GG0170R matching one of the 4 stages
            if (checkNext && (GG0170Rs && GG0170Rs.length > 0)) {
                console.log('Q Admission Performance is no, lock GG0170Rs, unlock and set focus on Assesment Complete');
                GG0170Rs.each(function () {
                    commonUtility.resetControlValue($(this));
                    $(this).prop('disabled', true);
                });
                completed = $('.persistable[id ^= Assessment][id*=Yes]');
                if (completed.length > 0) {
                    if (e === EnumChangeEventArg.Change) {
                        commonUtility.scrollTo(completed);
                    }
                    else {
                        completed.focus();
                    }
                }
            }
        }
        /* on change */
        GG0170Q.each(function () {
            thisQ = $(this);
            thisQ.on('change', { x: EnumChangeEventArg.Change }, checkGG0170Q);
        });
        /* on load */
        GG0170Q.each(function () {
            thisQ = $(this);
            checkGG0170Q(EnumChangeEventArg.Load);
        });
    }
    /* private function */
    function J1750_depends_on_J0510() {
        console.log('branching::: inside of J1750_depends_on_J0510');
        const J0510 = $('.persistable[id^=J0510]:not([id*=Discharge_Goal])');
        let thisJ0510;
        function checkJ0510(e) {
            const J1750s = $('.persistable[id^=J1750]');
            const J1750_yes = $('.persistable[id^=J1750][id*=Yes]');
            if (commonUtility.getControlValue(thisJ0510) === 0) { /* 0. Does not apply */
                console.log('J0510 is 0, unlock J1750s and focus on J1750 Yes option');
                J1750s.each(function () {
                    $(this).prop('disabled', false);
                });
                if (e === EnumChangeEventArg.Change) {
                    commonUtility.scrollTo(J1750_yes);
                }
                J1750_yes.focus();
            }
            else {
                console.log('J0510 is not 0, uncheck all J1750');
                J1750s.each(function () {
                    $(this).prop('checked', false);
                });
            }
        }
        /* on change */
        J0510.each(function () {
            thisJ0510 = $(this);
            thisJ0510.on('change', { x: EnumChangeEventArg.Change }, checkJ0510);
        });
        /* on load */
        checkJ0510(EnumChangeEventArg.Load);
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
            commonUtility.resetControlValue(dateClone);
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
        commonUtility.scrollTo(Q12);
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
    const stage = $('.pageTitle').data('systitle').replace(/\s/g, '_');
    /* on ready */
    if (stage === 'Full') {
        $('.persistable').each(function () { $(this).prop("disabled", true); });
    }
    else {
        console.log('branching::: LoadAllRules');
        branchingController.LoadAllRules();
    }
});
//# sourceMappingURL=branching.js.map