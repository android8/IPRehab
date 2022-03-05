/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

import { Utility } from "./utility.js";

/* jquery plugin dependsOn*/
/* https://dstreet.github.io/dependsOn */
/* http://emranahmed.github.io/Form-Field-Dependency */

$(function () {
  const commonUtility: Utility = new Utility();
  const stage: string = $('.pageTitle').data('stagecode').replace(/\s/g, '_');

  /* on ready */
  if (stage == 'Full') {
    $('.persistable').prop("disabled", true);
  }
  else {
    branchingController.CommonUnlock();
  }

  /* on click */
  $('button[id^=btnMore]').each(
    function () {
      $(this).click(
        function () {
          let questionKey: string = $(this).data('questionkey');
          branchingController.AddMore(stage, questionKey);
        }
      );
    }
  );

  /* on change */
  $('.persistable[id^=Q12], .persistable[id^=Q23]').each(
    function () {
      let $this = $(this);
      $this.change(
        function () {
          branchingController.CommonUnlock();
        }
      );
    }
  );

  /* on change */
  $('.persistable[id^=Q14A]').each(
    function () {
      $(this).change(
        function () {
          branchingController.Q14B_enabled_if_Q14A_is_Yes();
        }
      );
    }
  );

  /* on change of dropdown Q16A*/
  $('.persistable[id^=Q16A]').each(
    function () {
      $(this).change(
        function () {
          branchingController.Q16A_is_Home_then_Q17();
        }
      );
    }
  );

  /* on change */
  $('.persistable[id^=Q42]').each(
    function () {
      $(this).change(
        function () {
          branchingController.Q42_Interrupted_then_Q43();
        }
      );
    });

  $('.persistable[id^=Q44C]').each(
    function () {
      $(this).change(
        function () {
          branchingController.Q44C_Affect_Q44D_Q46($(this));
        }
      );
    })

  /* on change */
  $('.persistable[id^=GG0170I]').each(
    function () {
      $(this).change(
        function () {
          branchingController.GG0170JKLMN_depends_on_GG0170I($(this));
        }
      );
    });

  /* on change */
  $('.persistable[id^=GG0170M], .persistable[id^=GG0170N]').each(
    function () {
      $(this).change(
        function () {
          branchingController.GG0170P_depends_on_GG0170M_and_GG0170N($(this));
        }
      );
    });

  /* on change */
  $('.persistable[id^=GG0170Q]').each(
    function () {
      $(this).change(
        function () {
          branchingController.H0350_depends_on_GG0170Q($(this));
        }
      );
    });

  /* on change */
  $('.persistable[id^=J0510]').each(
    function () {
      $(this).change(
        function () {
          branchingController.J1750_depends_on_J0510($(this));
        }
      );
    });
});

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

let branchingController = (function () {
  const commonUtility: Utility = new Utility();

  /* private function */
  function CommonUnlock(): void {
    Q12_Q23_blank_then_Lock_All();
    Q14B_enabled_if_Q14A_is_Yes();
    Q16A_is_Home_then_Q17();

    $('.persistable[id^=Q42]').each(
      function () {
        Q42_Interrupted_then_Q43();
      }
    );

    $('.persistable[id^=Q44C]').each(
      function () {
        Q44C_Affect_Q44D_Q46($(this));
      }
    );

    $('.persistable[id^=GG0170I]').each(
      function () {
        GG0170JKLMN_depends_on_GG0170I($(this));
      }
    );

    $('.persistable[id^=GG0170M]').each(
      function () {
        GG0170P_depends_on_GG0170M_and_GG0170N($(this));
      }
    );

    H0350_depends_on_GG0170Q($('.persistable[id^=GG0170Q]:not([id*=Discharge_Goal])'));
    J1750_depends_on_J0510($('.persistable[id^=J0510]:not("Discharge_Goal")'));
    $(".persistable[id^=Q12]")[0].focus();
  }

  /* private function */
  function Q12_Q23_blank_then_Lock_All(): void {
    let Q12: any = $('.persistable[id^=Q12]');
    let Q23: any = $('.persistable[id^=Q23]');
    let otherPersistables: any = $('.persistable:not("[id^=Q12]"):not([id^=Q23])');
    if (commonUtility.isEmpty(Q12) || commonUtility.isEmpty(Q23)) {
      otherPersistables.each(
        function () {
          $(this).prop("disabled", true);
        }
      );
    }
    else {
      $('.persistable').each(
        function () {
          $(this).prop("disabled", false);
        }
      );
    }
  }

  /* private function */
  function Q14B_enabled_if_Q14A_is_Yes(): void {
    let Q14AYes: boolean = false, Q14ANo: boolean = false;
    let Q14AYesChecked: boolean = false, Q14ANoChecked: boolean = false;
    $('.persistable[id^=Q14A]').each(
      function () {
        if ($(this).prop('checked')) {
          Q14AYes = $(this).prop('id').indexOf('Yes') != -1;
          if (Q14AYes) Q14AYesChecked = true;
          Q14ANo = $(this).prop('id').indexOf('No') != -1;
          if (Q14ANo) Q14ANoChecked = true;
        }
      }
    );

    let Q14Bs: any = $('.persistable[id^=Q14B]');
    if (!Q14AYesChecked && !Q14ANoChecked && Q14Bs.length > 0) {
      Q14Bs.each(
        function () {
          $(this).prop('checked', false).prop('disabled', true);
        }
      );
    }
    else {
      if (Q14AYes && Q14Bs.length > 0) {
        Q14Bs.removeAttr('disabled');
        Q14Bs[0].focus();
      }

      if (Q14ANo && Q14Bs.length > 0) {
        Q14Bs.each(
          function () {
            $(this).prop("checked", false).prop('disabled', true);
          }
        );
      }
    }
  }

  /* private function */
  function Q16A_is_Home_then_Q17(): void {
    const Q16A: any = $(".persistable[id^=Q16A]");
    const Q17: any = $(".persistable[id^=Q17]");

    if (Q16A.length > 0 && !commonUtility.isEmpty(Q16A) && commonUtility.getControlValue(Q16A) == 1 /* 1. Home */) {
      if (Q17.length > 0) {
        Q17.val(-1).prop('disabled', false).focus();
      }
    }
    else {
      if (Q17.length > 0) {
        Q17.val(-1).prop('disabled', true);
      }
    }
  }

  /* private function, not used per stakeholder request */
  function Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A(): void {
    let Q24A_is_set: boolean = false;
    $('.persistable[id ^= Q21A_], .persistable[id ^= Q21B_], .persistable[id ^= Q22_], .persistable[id ^= Q24_]').each(
      function () {
        if (!commonUtility.isEmpty($(this)) && $(this).val() == '123.45') { //ICD for diabetes
          $('.persistable[id^=Q44C][id*=86]').prop('checked', true);
        }
      }
    );
  }

  /* private function */
  function Q42_Interrupted_then_Q43(): void {
    const Q42Yes: boolean = $('.persistable[id^=Q42][id*=Yes]').prop('checked') == true;
    const Q42No: boolean = $('.persistable[id^=Q42][id*=No]').prop('checked') == true;
    const Q43: any = $('.persistable[id^=Q43]');

    switch (true) {
      case Q42No: {
        /* no interruption so clear all Q43 dates */
        Q43.val('');
        Q43.prop('disabled', true);
        break;
      }
      case Q42Yes: {
        if (Q43.length > 0)
          Q43.prop('disabled', false);
          Q43.first().focus();
        break;
      }
      default:
        Q43.prop('disabled', true);
        break;
    }
  }

  function Q44C_Affect_Q44D_Q46(Q44C: any): void {
    /*codeset ID 86(Y) 87(N)*/

    const Q44D: any = $('.persistable[id^=Q44D]');
    const Q45: any = $('.persistable[id^=Q45]');
    const Q46: any = $('.persistable[id^=Q46]');

    let Q44C_is_Yes: boolean = Q44C.prop('checked') == true && Q44C.prop('id').indexOf('Yes') != -1;
    let Q44C_is_No: boolean = Q44C.prop('checked') == true && Q44C.prop('id').indexOf('No') != -1;

    if (Q44C_is_Yes) {
      Q44D.prop('disabled', false).focus();
      Q45.prop('disabled', false);
    }

    if (Q44C_is_No) {
      Q44D.val(-1);
      Q44D.prop('disabled', true);
      Q46.prop('disabled', false).focus();
    }
  }

  /* private function */
  function GG0170JKLMN_depends_on_GG0170I(GG0170I: any): void {
    let GG0170IInt: number, GG0170J: any, GG0170JKL: any, GG0170M: any;

    GG0170IInt = commonUtility.getControlValue(GG0170I);

    if (GG0170I.prop('id').indexOf('Admission_Performance') != -1) {
      GG0170JKL = $('.persistable[id^=GG0170J_Admission_Performance], .persistable[id^=GG0170K_Admission_Performance], .persistable[id^=GG0170L_Admission_Performance]');
      GG0170M = $('.persistable[id^=GG0170M_Admission_Performance]');
    }
    else {
      GG0170JKL = $('.persistable[id^=GG0170J_Discharge_Performance], .persistable[id^=GG0170K_Discharge_Performance], .persistable[id^=GG0170L_Discharge_Performance]');
      GG0170M = $('.persistable[id^=GG0170M_Discharge_Performance]');
    }

    switch (true) {
      case (GG0170IInt >= 7):
        {
          /* lock and clear J K L, advance to M */
          if (GG0170JKL.length > 0) {
            GG0170JKL.each(
              function () {
                $(this).prop('disabled', true).val(-1).change(); //need .change() to automatically calculate the score
              }
            );
          }
          if (GG0170M.length > 0) {
            GG0170M[0].focus();
          }
        }
        break;
      case (GG0170IInt > 0 && GG0170IInt <= 6):
        {
          /* unlock and clear J K L, skip to J */
          if (GG0170JKL.length > 0) {
            GG0170JKL.each(
              function () {
                $(this).prop('disabled', false).val(-1).change(); //need .change() to automatically calculate the score
              }
            );
            GG0170J[0].focus();
          }
        }
        break;
      default: {
        /* GG0170I is not selected, clear and lock J K L */
        GG0170JKL.each(
          function () {
            $(this).prop('disabled', true).val(-1).change(); //need .change() to automatically calculate the score
          }
        );
      }
    }
  }

  /* private function */
  function GG0170P_depends_on_GG0170M_and_GG0170N(GG0170M: any): void {
    let GG0170MInt: number, GG0170N: any, GG0170NInt: number, GG0170O: any, GG0170P: any;

    GG0170MInt = commonUtility.getControlValue(GG0170M);

    if (GG0170M.prop('id').indexOf('Admission_Performance') != -1) {
      GG0170N = $('.persistable[id^=GG0170N_Admission_Performance]');
      GG0170NInt = commonUtility.getControlValue(GG0170N);
      GG0170O = $('.persistable[id^=GG0170O_Admission_Performance]');
      GG0170P = $('.persistable[id^=GG0170P_Admission_Performance]');
    }
    else {
      GG0170N = $('.persistable[id^=GG0170N_Discharge_Performance]');
      GG0170NInt = commonUtility.getControlValue(GG0170N);
      GG0170O = $('.persistable[id^=GG0170O_Discharge_Performance]');
      GG0170P = $('.persistable[id^=GG0170P_Discharge_Performance]');
    }

    switch (true) {
      case (GG0170MInt >= 7):
        /* skp to P */
        if (GG0170P.length > 0) {
          GG0170P[0].focus();
        }
        break;
      case (GG0170MInt > 0 && GG0170MInt <= 6):
        /* skip to N */
        if (GG0170N.length > 0)
          GG0170N[0].focus();
        break;
    }

    switch (true) {
      case (GG0170NInt >= 7):
        /* skp to P */
        if (GG0170P.length > 0) {
          GG0170P[0].focus();
        }
        break;
      case (GG0170NInt > 0 && GG0170NInt <= 6):
        /* skip to O */
        if (GG0170O.length > 0)
          GG0170O[0].focus();
        break;
    }
  }

  /* private function */
  function H0350_depends_on_GG0170Q(GG0170Q: any): void {
    /* 314 = yes, 315 = no */
    let H0350s: any, GG0170Rs: any;
    const GG0170Q_Admission_Performance_Yes: boolean = GG0170Q.prop('id').indexOf('Admission_Performance') != -1 && GG0170Q.prop('id').indexOf('314') != -1 && GG0170Q.prop('checked');
    const GG0170Q_Admission_Performance_No: boolean = GG0170Q.prop('id').indexOf('Admission_Performance') != -1 && GG0170Q.prop('id').indexOf('315') != -1 && GG0170Q.prop('checked');
    const GG0170Q_Discharge_Performance_Yes: boolean = GG0170Q.prop('id').indexOf('Discharge_Performance') != -1 && GG0170Q.prop('id').indexOf('314') != -1 && GG0170Q.prop('checked');
    const GG0170Q_Discharge_Performance_No: boolean = GG0170Q.prop('id').indexOf('Discharge_Performance') != -1 && GG0170Q.prop('id').indexOf('315') != -1 && GG0170Q.prop('checked');

    if (GG0170Q_Admission_Performance_Yes) {
      GG0170Rs = $('.persistable[id^=GG0170R][id*=Admission]');
      GG0170Rs.each(
        function () {
          $(this).prop('disabled', false)
        }
      );
      GG0170Rs[0].focus();
    }

    if (GG0170Q_Discharge_Performance_Yes) {
      GG0170Rs = $('.persistable[id^=GG0170R][id*=Discharge]');
      GG0170Rs.each(
        function () {
          $(this).prop('disabled', false)
        }
      );
      GG0170Rs[0].focus();
    }

    if (GG0170Q_Admission_Performance_No || GG0170Q_Discharge_Performance_No) {
      H0350s = $('.persistable[id^=H0350]');
      H0350s.each(
        function () {
          $(this).prop('disabled', false);
        }
      );
      H0350s[0].focus();
    }
  }

  /* private function */
  function J1750_depends_on_J0510(J0510: any): void {
    let J1750s: any;
    if (J0510.prop('id').indexOf('Admission_Performance') != -1)
      J1750s = $('.persistable[id^=J1750][id*=Admission_Performance]');
    else
      J1750s = $('.persistable[id^=J1750][id*=Discharge_Performance]');

    if (commonUtility.getControlValue(J0510) == 0) { /* 0. Does not apply */
      J1750s.each(
        function () {
          $(this).prop('disabled', false);
        }
      );
      J1750s[0].focus();
    }
    else {
      J1750s.each(
        function () {
          $(this).prop('checked', false);
        }
      );
    }
  }

  /*private function*/
  function AddMore(stage: string, questionKey: string) {
    let lastInputIdx: number = $('.persistable[id^=' + questionKey + '_' + stage + ']').length;
    let lastInputDate: any = $('.persistable[id^=' + questionKey + '_' + stage + '_' + lastInputIdx + ']');
    let dateClone: any = lastInputDate.clone();
    dateClone.val('').focus();
    lastInputDate.append(dateClone);
  }

  /***************************************************************************
   * public functions exposing the private functions to outside of the closure
  ***************************************************************************/
  return {
    'CommonUnlock': CommonUnlock,
    'Q12_Q23_blank_then_Lock_All': Q12_Q23_blank_then_Lock_All,
    'Q14B_enabled_if_Q14A_is_Yes': Q14B_enabled_if_Q14A_is_Yes,
    'Q16A_is_Home_then_Q17': Q16A_is_Home_then_Q17,
    'Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A': Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A,
    'Q42_Interrupted_then_Q43': Q42_Interrupted_then_Q43,
    'Q44C_Affect_Q44D_Q46': Q44C_Affect_Q44D_Q46,
    'GG0170JKLMN_depends_on_GG0170I': GG0170JKLMN_depends_on_GG0170I,
    'GG0170P_depends_on_GG0170M_and_GG0170N': GG0170P_depends_on_GG0170M_and_GG0170N,
    'H0350_depends_on_GG0170Q': H0350_depends_on_GG0170Q,
    'J1750_depends_on_J0510': J1750_depends_on_J0510,
    'AddMore': AddMore
  }
})();