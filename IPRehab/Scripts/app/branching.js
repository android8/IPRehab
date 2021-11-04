/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/* jquery plugin dependsOn*/
/* https://dstreet.github.io/dependsOn */
/* http://emranahmed.github.io/Form-Field-Dependency */
$(function () {
    const stage = $('.pageTitle').text().replace(' ', '_');
    formController.Q12_Q23_blank_then_Lock_All(stage);
    $("input[id^=Q12_" + stage + ", input[id ^= Q23_" + stage + "]").change(function () {
        let Q12 = $("input[id^=Q12_" + stage + "]");
        let Q23 = $("input[id^=Q23_" + stage + "]");
        if (Q12.val() == null || Q23.val() == null) {
            alert('Q12 and Q23 are key questions can not be blank');
            formController.Q12_Q23_blank_then_Lock_All(stage);
        }
        else {
            formController.Q14B_enabled_if_Q14A_is_86(stage);
            formController.Q16A_is_Home_then_Q17(stage);
            formController.Q42_Interrupted_then_Q43(stage, $(this));
        }
    });
    formController.Q14B_enabled_if_Q14A_is_86(stage);
    $('input[id^=Q14A_' + stage + ']').change(function () {
        formController.Q14B_enabled_if_Q14A_is_86(stage);
    });
    formController.Q16A_is_Home_then_Q17(stage);
    $('input[id^=Q16A_' + stage + ']').change(function () {
        formController.Q16A_is_Home_then_Q17(stage);
    });
    $('input[id^="Q42_' + stage + '"]').each(function () {
        $(this).change(function () {
            formController.Q42_Interrupted_then_Q43(stage, $(this));
        });
    });
    $('#btnMoreQ42').each(function () {
        $(this).click(function () {
            formController.AddMoreQ42Q43(stage, 'Q42');
        });
    });
    $('#btnMoreQ43').each(function () {
        $(this).click(function () {
            formController.AddMoreQ42Q43(stage, 'Q43');
        });
    });
});
let formController = (function () {
    /* private function */
    function Q12_Q23_blank_then_Lock_All(stage) {
        let Q12 = $("input[id^=Q12_" + stage + "]");
        let Q23 = $("input[id^=Q23_" + stage + "]");
        if (Q12.val() == null || Q23.val() == null) {
            $('.persistable').each(function () {
                let $this = $(this);
                if ($this.prop("id").indexOf('Q12') < 0 && $this.prop("id").indexOf('Q23') < 0) {
                    $this.prop("disabled", true);
                }
            });
        }
    }
    /* private function */
    function Q14B_enabled_if_Q14A_is_86(stage) {
        let Q14A = $('input[id^=Q14A_' + stage + ']');
        let Q14B = $('input[id^=Q14B_' + stage + ']');
        if (Q14A.val() == '86') //codeset id 86 == yes
            Q14B.removeAttr('disabled').focus();
        else
            Q14B.prop('disabled', true);
    }
    /* private function */
    function Q16A_is_Home_then_Q17(stage) {
        let Q16A_is_Home = $("select[id^=Q16_" + stage + "]").val() == 94 /*1. Home */;
        if (Q16A_is_Home) {
            $("input[id^=Q17_" + stage + "]").removeAttr('disabled').focus();
        }
        else {
            $("input[id^=Q17_" + stage + "]").prop('disabled', true);
        }
    }
    /* private function */
    function Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A(stage) {
        let Q24A_is_set = false;
        $('[id ^= Q21A]').each(function () {
            if ($(this).val() == '123.45') {
                $('input[id^=Q24A_' + stage + '_86]').prop('checked', true);
                Q24A_is_set = true;
                return; //break out each()
            }
        });
        if (!Q24A_is_set) {
            $('input[id^=Q22]').each(function () {
                if ($(this).val() == '123.45') {
                    $('input[id^=Q24A_' + stage + '_86]').prop('checked', true);
                    Q24A_is_set = true;
                    return; //break out each()
                }
            });
        }
        if (!Q24A_is_set) {
            $('input[id^=Q24]').each(function () {
                if ($(this).val() == '123.45') {
                    $('input[id^=Q24A_' + stage + '_86]').prop('checked', true);
                    Q24A_is_set = true;
                    return; //break out each()
                }
            });
        }
        if (Q24A_is_set)
            return false;
        else
            return true;
    }
    /* private function */
    function Q42_Interrupted_then_Q43(stage, $this) {
        let Q42_Interrupted = $this.val() != null;
        let thisQ43ID = $this.prop('id').replace('42', '43');
        if (Q42_Interrupted) {
            if ($('#' + thisQ43ID).length == 0) {
                let dateClone = $this;
                dateClone.prop('id', thisQ43ID);
                dateClone.val(null);
                $this.append(dateClone);
                dateClone.focus();
            }
            else {
                $('#' + thisQ43ID).prop('disabled', true).focus();
            }
        }
        else {
            if ($('#' + thisQ43ID).length != 0) {
                $('#' + thisQ43ID).remove();
            }
        }
        return false;
    }
    /*private function*/
    function AddMoreQ42Q43(stage, questionKey) {
        /* add interrup date control */
        let lastInputIdx = $('input[id^=' + questionKey + '_' + stage + ']').length;
        let lastInputDate = $('#' + questionKey + '_' + stage + '_' + lastInputIdx);
        let dateClone = lastInputDate.clone();
        dateClone.val(null).focus();
    }
    /* private function */
    function Q44C_is_Y_then_Q44D(stage) {
        /*codeset ID 86(Y) 87(N)*/
        let Q44C_Y = $('input[id^=Q44C' + stage + '_86]');
        if (Q44C_Y.prop('checked')) {
            $('input[id^=Q44D]').prop('disabled', false).focus();
            $('input[id^=Q44E]').prop('disabled', false).focus();
            $('input[id^=Q45]').prop('disabled', false).focus();
            return false;
        }
        return true;
    }
    /* private function */
    function Q44C_is_N_then_Q46(stage) {
        /*codeset ID 87(N)*/
        let Q44C_N = $('input[id^=Q44C' + stage + '_87]');
        if (Q44C_N.prop('checked')) {
            if (confirm('Q44D and Q44E answers will be resetted')) {
                $('input[id^=Q44D]').val(-1);
                $('input[id^=Q44D]').prop('disabled', true).focus();
                $('input[id^=Q44E]').val(-1);
                $('input[id^=Q44E]').prop('disabled', true).focus();
                $('input[id^=Q46]').prop('disabled', false).focus();
            }
        }
        return true;
    }
    /* private function */
    function GG0170M_depends_on_GG0170I() {
        const GG0170IAdmPerformance = $('#GG0170I_Admission_Performance_1');
        const GG0170Ms = $('select[id^=GG0170M]');
        const factors = ['309', '310', '311', '312'];
        if (factors.indexOf(GG0170IAdmPerformance.val()) !== -1) {
            GG0170Ms.each(function () {
                $(this).prop('disabled', false).focus();
            });
        }
        else {
            GG0170Ms.each(function () {
                $(this).prop('disabled', false).val(-1);
            });
        }
    }
    /* private function */
    function GG0170P_depends_on_GG0170M() {
        let GG0170MAdmPerformance = $('#GG0170M_Admission_Performance_1');
        let GG0170Ps = $('select[id^=GG0170P]');
        let factors = ['309', '310', '311', '312'];
        if (factors.indexOf(GG0170MAdmPerformance.val()) !== -1) {
            GG0170Ps.each(function () {
                $(this).prop('disabled', false).focus();
            });
        }
        else {
            GG0170Ps.each(function () {
                $(this).prop('disabled', true).val(-1);
            });
        }
    }
    /* private function */
    function GG0170P_depends_on_GG0170N() {
        let GG0170NAdmPerformance = $('#GG0170N_Admission_Performance_1');
        let GG0170Ps = $('select[id^=GG0170P]');
        let factors = ['309', '310', '311', '312'];
        if (factors.indexOf(GG0170NAdmPerformance.val()) !== -1) {
            GG0170Ps.each(function () {
                $(this).prop('disabled', false).focus();
            });
        }
        else {
            GG0170Ps.each(function () {
                $(this).prop('disabled', true).val(-1);
            });
        }
    }
    /* private function */
    function H0350_depends_on_GG0170Q() {
        let GG0170QAdmPerformanceNo = $('#GG0170Q_Admission_Performance_315');
        let GG0170QAdmPerformanceYes = $('#GG0170Q_Admission_Performance_314');
        let GG0170Rs = $('#GG0170R_Admission_Performance_1');
        let H0350s = $('select[id^=H0350]');
        if (GG0170QAdmPerformanceNo.prop('checked')) {
            H0350s.each(function () {
                $(this).removeAttr('disabled').focus();
            });
        }
        else {
            H0350s.each(function () {
                $(this).prop('disabled', true).val(-1);
            });
        }
        if (GG0170QAdmPerformanceYes.prop('checked')) {
            GG0170Rs.each(function () {
                $(this).removeAttr('disabled').focus();
            });
        }
        else {
            GG0170Rs.each(function () {
                $(this).prop('disabled', true).val(-1);
            });
        }
    }
    /* private function */
    function J1750_depends_on_J0510(stage) {
        let J0510 = $('#J0510_' + stage + '_1');
        let J1750s = $('input[id^=J1750');
        if (J0510.val() == '345') { /*345 == 0. Does not apply */
            J1750s.each(function () {
                $(this).prop('disabled', false).focus();
            });
        }
        else {
            J1750s.each(function () {
                $(this).prop('checked', false);
            });
        }
    }
    /***************************************************************************
     * public functions exposing the private functions to outside of the closure
    ***************************************************************************/
    return {
        'Q12_Q23_blank_then_Lock_All': Q12_Q23_blank_then_Lock_All,
        'Q14B_enabled_if_Q14A_is_86': Q14B_enabled_if_Q14A_is_86,
        'Q16A_is_Home_then_Q17': Q16A_is_Home_then_Q17,
        'Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A': Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A,
        'Q42_Interrupted_then_Q43': Q42_Interrupted_then_Q43,
        'AddMoreQ42Q43': AddMoreQ42Q43,
        'Q44C_is_Y_then_Q44D': Q44C_is_Y_then_Q44D,
        'Q44C_is_N_then_Q46': Q44C_is_N_then_Q46,
        'GG0170M_depends_on_GG0170I': GG0170M_depends_on_GG0170I,
        'GG0170P_depends_on_GG0170M': GG0170P_depends_on_GG0170M,
        'GG0170P_depends_on_GG0170N': GG0170P_depends_on_GG0170N,
        'J1750_depends_on_J0510': J1750_depends_on_J0510,
        'H0350_depends_on_GG0170Q': H0350_depends_on_GG0170Q
    };
})();
//# sourceMappingURL=branching.js.map