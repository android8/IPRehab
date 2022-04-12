/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

import { Utility, EnumChangeEventArg } from "./commonImport.js";

/* jquery plugin dependsOn*/
/* https://dstreet.github.io/dependsOn */
/* http://emranahmed.github.io/Form-Field-Dependency */

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

const branchingController = (function () {
  const commonUtility: Utility = new Utility();
  const dialogOptions = commonUtility.dialogOptions();
  /* private function */
  function Q12_Q23_blank_then_Lock_All(thisEventData: EnumChangeEventArg): void {

    console.log('branching:::', 'Inside of Q12_Q23_blank_then_Lock_All()', thisEventData.toString());

    const Q12: any = $('.persistable[id^=Q12_]');
    const Q23: any = $('.persistable[id^=Q23]');

    const otherPersistables: any = $('.persistable:not([id^=Q12_]):not([id^=Q23])');

    console.log('Q12 or Q23 is empty, lock all other questions', $(this));
    if (commonUtility.isEmpty(Q12) || commonUtility.isEmpty(Q23)) {
      otherPersistables.each(
        function () {
          $(this).prop("disabled", true);
        }
      );
    }
    else {
      const minDate = new Date('2020-01-01 00:00:00');
      const onsetDate = new Date(Q23.val());
      const admitDate = new Date(Q12.val());
      console.log('Oneset Date and/or Admit Date violates this rule', $(this));
      if (onsetDate < minDate || admitDate < minDate || admitDate < onsetDate) {
        dialogOptions.title = 'Date';
        $('#dialog')
          .text('Onset Date and Admit Date must be later than 01/01/2021, and Admit Date must be on Onset Date or later')
          .dialog(dialogOptions, {
            title: 'Warning'
          });
        otherPersistables.each(
          function () {
            $(this).prop("disabled", true);
          }
        );
      }
      else {
        //check e event data to determine if the event is fired by onready or by onload, if onload keep ajaxPost button disabled
        if (thisEventData === EnumChangeEventArg.Change)
          $('#ajaxPost').removeAttr('disabled');

        console.log('Onset Date and Admit date satisfy this rule, enable other ' + otherPersistables.length + 'perPersistables');
        otherPersistables.each(
          function () {
            $(this).prop("disabled", false);
          }
        );
      }
    }
  }

  /* private function */
  function Q12B_blank_then_Lock_Discharge(): void {
    console.log("branching::: inside of Q12B_blank_then_Lock_Discharge");

    //lock all field with pertaining discharge Q15B,Q16B, Q17B, Q21B, Q41, Q44C
    const Q12B = $('.persistable[id^=Q12B_]');
    const isDischarged: boolean = Q12B.val().toString() !== '';
    const DischargeRelated = $('.persistable[id ^= Q15B],.persistable[id ^= Q16B],.persistable[id ^= Q17B],.persistable[id ^= Q21B],.persistable[id ^= Q41],.persistable[id ^= Q44C]');

    if (isDischarged) {
      console.log('Q12B is a discharge date, unlock all discharged Qs in the base series only, not other questions in other series');
      DischargeRelated.each(function () {
        const thisRelatedQ = $(this);
        thisRelatedQ.prop('disabled', false);
      });
    }
    else {
      console.log('Q12B is not a discharge date, lock all discharged Qs in the base series only, not other questions in other series', DischargeRelated );
      DischargeRelated.each(function () {
        const thisRelatedQ = $(this);
        commonUtility.resetControlValue(thisRelatedQ);
        thisRelatedQ.prop('disabled', true);
      });
    }
    Q12B.focus();
  }

  /* private function */
  function Q14B_enabled_if_Q14A_is_Yes(): void {
    console.log("branching::: inside of Q14B_enabled_if_Q14A_is_Yes");

    const Q14Bs: any = $('.persistable[id^=Q14B]');
    let Q14AYes = $('.persistable[id^=Q14A][id*=Yes]:checked').length === 1;
    if (Q14AYes) {
      console.log('Q14A is Yes, unlock all Q14B and focus on first Q14B');
      if (Q14Bs.length > 0) {
        Q14Bs.each(
          function () {
            $(this).prop('disabled', false);
          }
        );
        Q14Bs.first().focus();
      }
    }
    else {
      console.log('Q14A is not answered or is No, uncheck and lock all Q14B');
      Q14Bs.each(
        function () {
          $(this).prop('checked', false).prop('disabled', true);
        }
      );
    }
  }

  /* private function */
  function Q16A_is_Home_then_Q17(): void {
    console.log('branching::: insite of Q14B_enabled_if_Q14A_is_Yes');

    const Q16A: any = $(".persistable[id^=Q16A]");
    const Q17: any = $(".persistable[id^=Q17]:not([id^=Q17B])");

    console.log('Q16A is home, unlock and focus on Q17');
    if (Q16A.length > 0 && !commonUtility.isEmpty(Q16A) && commonUtility.getControlValue(Q16A) === 1 /* 1. Home */) {
      if (Q17.length > 0) {
        Q17.prop('disabled', false).focus();
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
    console.log('branching::: insite of Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A');

    //const Q24A_is_set: boolean = false;
    console.log('check 44C if Q21A, Q21B, Q22, or Q24 is diabetes');
    $('.persistable[id ^= Q21A_], .persistable[id ^= Q21B_], .persistable[id ^= Q22_], .persistable[id ^= Q24_]').each(
      function () {
        if (!commonUtility.isEmpty($(this)) && $(this).val() === '123.45') { //ICD for diabetes
          $('.persistable[id^=Q44C][id*=86]').prop('checked', true);
        }
      }
    );
  }

  /* private function */
  function Q42_Interrupted_then_Q43(): void {
    console.log('branching::: insite of Q42_Interrupted_then_Q43');

    const Q42Yes: boolean = $('.persistable[id^=Q42][id*=Yes]:checked').length === 1;
    const Q43: any = $('.persistable[id^=Q43]');


    if (Q42Yes) {
      console.log('Q42 is yes, unlock and focus on Q43 next blank interrupt date');

      Q43.each(function () {
        const thisDate = $(this);
        thisDate.prop('disabled', false); /* unlock all non-blank dates */
        thisDate.next().prop('disabled', false);  /* unlock next reset button */
        if (thisDate.val() === '') {
          thisDate.focus();
          return false; /* break out each */
        }
      });
    }
    else {
      /* no interruption so clear all Q43 dates */
      console.log('Q42 is no or is not answered, reset and lock all Q43');
      Q43.each(function () {
        const thisDate = $(this);
        thisDate.val('').prop('disabled', true);
        thisDate.next().prop('disabled', true);
      });
    }
  }

  function Q44C_Affect_Q44D_Q46(Q44C: any): void {
    console.log('branching::: insite of Q44C_Affect_Q44D_Q46', Q44C);

    const Q44D: any = $('.persistable[id^=Q44D]');
    const Q45: any = $('.persistable[id^=Q45]');
    const Q46: any = $('.persistable[id^=Q46]');

    const Q44C_is_Yes: boolean = Q44C.prop('checked') === true && Q44C.prop('id').indexOf('Yes') !== -1;

    if (Q44C_is_Yes) {
      console.log('Q44C is yes, unlock Q44D, Q45 and focus on Q44D');
      if (Q44D.length > 0 && Q45.length > 0) {
        Q44D.prop('disabled', false).focus();
        Q45.prop('disabled', false);
      }
    }
    else {
      console.log('Q44C is no, lock Q44D, Q45 and focus on Q46');
      if (Q44D.length > 0 && Q45.length > 0 && Q46.length > 0) {
        Q44D.val(-1);
        Q44D.prop('disabled', true);
        Q45.prop('disabled', true);
        Q46.prop('disabled', false).focus();
      }
    }
  }

  /* private function */
  function GG0170JKLMN_depends_on_GG0170I(GG0170I: any, thisEventData: EnumChangeEventArg): void {
    console.log('branching::: insite of GG0170JKLMN_depends_on_GG0170I', GG0170I);

    const GG0170IInt: number = commonUtility.getControlValue(GG0170I);
    let GG0170J: any, GG0170JKL: any, GG0170M: any;

    if (GG0170I.prop('id').indexOf('Admission_Performance') !== -1) {
      GG0170JKL = $('.persistable[id^=GG0170J_Admission_Performance], .persistable[id^=GG0170K_Admission_Performance], .persistable[id^=GG0170L_Admission_Performance]');
      GG0170M = $('.persistable[id^=GG0170M_Admission_Performance]');
      GG0170J = $('.persistable[id^=GG0170J_Admission_Performance]');
    }
    else {
      GG0170JKL = $('.persistable[id^=GG0170J_Discharge_Performance], .persistable[id^=GG0170K_Discharge_Performance], .persistable[id^=GG0170L_Discharge_Performance]');
      GG0170M = $('.persistable[id^=GG0170M_Discharge_Performance]');
      GG0170J = $('.persistable[id^=GG0170J_Discharge_Performance]');
    }

    switch (true) {
      case (GG0170IInt >= 7):
        {
          /* lock and clear J K L, advance to M */
          console.log('I >= 7, lock J, K, L, and focus on M');
          if (GG0170JKL.length > 0) {
            GG0170JKL.each(
              function () {
                commonUtility.resetControlValue($(this));
                //$(this).prop('disabled', true).val(-1).change(); //need .change() to automatically calculate the score
              }
            );
          }
          if (GG0170M.length > 0) {
            if (thisEventData === EnumChangeEventArg.Change) {
              commonUtility.scrollTo(GG0170M.first());
            }
            GG0170M.first().focus();
          }
        }
        break;
      case (GG0170IInt > 0 && GG0170IInt <= 6):
        {
          /* unlock and clear J K L, skip to J */
          console.log('0 < I <= 6, unlock J, K, L, and focus on J');
          if (GG0170JKL.length > 0) {
            GG0170JKL.each(
              function () {
                commonUtility.resetControlValue($(this));
                //$(this).prop('disabled', false).val(-1).change(); //need .change() to automatically calculate the score
              }
            );
            if (thisEventData === EnumChangeEventArg.Change) {
              commonUtility.scrollTo(GG0170J.first());
            }
            GG0170J.first().focus();
          }
        }
        break;
      default: {
        /* GG0170I is not selected, clear and lock J K L */
        console.log('I is not answered, lock J, K, L and focus on M');

        GG0170JKL.each(
          function () {
            commonUtility.resetControlValue($(this));
            //$(this).prop('disabled', true).val(-1).change(); //need .change() to automatically calculate the score
          }
        );
        if (GG0170M.length > 0) {
          if (thisEventData === EnumChangeEventArg.Change) {
            commonUtility.scrollTo(GG0170M.first());
          }
          GG0170M.first().focus();
        }
      }
    }
  }

  /* private function */
  function GG0170P_depends_on_GG0170M_and_GG0170N(thisGG0170: any, thisEventData): void {
    console.log('branching::: insite of GG0170P_depends_on_GG0170M_and_GG0170N', thisGG0170, thisEventData);

    /* thisGG0170 could be M or N so inspect .prop('id') to determine which */
    const GG0170Int: number = commonUtility.getControlValue(thisGG0170);;
    let GG0170M: any = null, GG0170N: any = null, GG0170O: any, GG0170P: any;

    console.log('determine the trigger is GG0170 M or N', thisGG0170);

    if (thisGG0170.prop('id').indexOf('GG0170M') !== -1)
      GG0170M = thisGG0170;
    else
      GG0170N = thisGG0170;

    if (GG0170M != null) {
      /* goes to N or P */
      console.log("it's M", GG0170M);

      if (GG0170M.prop('id').indexOf('Admission_Performance') !== -1) {
        GG0170N = $('.persistable[id^=GG0170N_Admission_Performance]');
        GG0170O = $('.persistable[id^=GG0170O_Admission_Performance]');
        GG0170P = $('.persistable[id^=GG0170P_Admission_Performance]');
      }
      else {
        GG0170N = $('.persistable[id^=GG0170N_Discharge_Performance]');
        GG0170O = $('.persistable[id^=GG0170O_Discharge_Performance]');
        GG0170P = $('.persistable[id^=GG0170P_Discharge_Performance]');
      }
    }
    else {
      console.log("it's N", GG0170N);

      /* goes to O or P */
      if (GG0170N.prop('id').indexOf('Admission_Performance') !== -1) {
        GG0170N = $('.persistable[id^=GG0170N_Admission_Performance]'); /* required in case GG0170N is answered first */
        GG0170O = $('.persistable[id^=GG0170O_Admission_Performance]');
        GG0170P = $('.persistable[id^=GG0170P_Admission_Performance]');
      }
      else {
        GG0170N = $('.persistable[id^=GG0170N_Discharge_Performance]'); /* required in case GG0170N is answered first */
        GG0170O = $('.persistable[id^=GG0170O_Discharge_Performance]');
        GG0170P = $('.persistable[id^=GG0170P_Discharge_Performance]');
      }
    }

    switch (true) {
      case (GG0170Int >= 7):
        console.log('I > 7 go to P');
        if (GG0170P.length > 0) {
          if (thisEventData === EnumChangeEventArg.Change) {
            commonUtility.scrollTo(GG0170P.first());
          }
          GG0170P.first().focus(); /* go to P */
        }
        break;
      case (GG0170Int > 0 && GG0170Int <= 6):
        console.log('0 < I <= 6 go to N');
        if (GG0170N.length > 0) {
          if (thisEventData === EnumChangeEventArg.Change) {
            commonUtility.scrollTo(GG0170N.first());
          }
          GG0170N.first().focus(); /* go to N */
        }
        break;
      default:
        console.log('I is blank, disable GG0170N', GG0170N);
        GG0170N.prop('disabled', true);

        console.log('I is blank, disable GG0170O', GG0170O);
        GG0170O.prop('disabled', true);
        break;
    }
  }

  /* private function */
  function GG0170Q_is_No_skip_to_Complete(GG0170Q: any, thisEventData): void {
    console.log('branching::: insite of GG0170Q_is_No_skip_to_Complete', GG0170Q);

    /* 314 = yes, 315 = no */
    let completed: any, GG0170Rs: any;
    const GG0170Q_Admission_Performance_Yes: boolean = GG0170Q.prop('id').indexOf('Admission_Performance') !== -1 && GG0170Q.prop('id').indexOf('314') !== -1 && GG0170Q.prop('checked');
    const GG0170Q_Admission_Performance_No: boolean = GG0170Q.prop('id').indexOf('Admission_Performance') !== -1 && GG0170Q.prop('id').indexOf('315') !== -1 && GG0170Q.prop('checked');
    const GG0170Q_Discharge_Performance_Yes: boolean = GG0170Q.prop('id').indexOf('Discharge_Performance') !== -1 && GG0170Q.prop('id').indexOf('314') !== -1 && GG0170Q.prop('checked');
    const GG0170Q_Discharge_Performance_No: boolean = GG0170Q.prop('id').indexOf('Discharge_Performance') !== -1 && GG0170Q.prop('id').indexOf('315') !== -1 && GG0170Q.prop('checked');

    console.log('Q Admission Performance is yes, unlock and focus on R Admission Performance');
    if (GG0170Q_Admission_Performance_Yes) {
      GG0170Rs = $('.persistable[id^=GG0170R_Admission]');
      GG0170Rs.each(
        function () {
          $(this).prop('disabled', false)
        }
      );
      if (thisEventData === EnumChangeEventArg.Change) {
        commonUtility.scrollTo(GG0170Rs.first());
      }
      GG0170Rs.first().focus();
    }

    console.log('Q Discharge Performance is yes, unlock and focus on R Discharge');
    if (GG0170Q_Discharge_Performance_Yes) {
      GG0170Rs = $('.persistable[id^=GG0170R_Discharge_Performance]');
      GG0170Rs.each(
        function () {
          $(this).prop('disabled', false)
        }
      );
      if (thisEventData === EnumChangeEventArg.Change) {
        commonUtility.scrollTo(GG0170Rs.first());
      }
      GG0170Rs.first().focus();
    }

    console.log('Q Admission Performance is no, unlock and focus on Assesment Complete');
    if (GG0170Q_Admission_Performance_No || GG0170Q_Discharge_Performance_No) {
      completed = $('.persistable[id ^= Assessment]:not([id*=No]');
      const assessments = $('.persistable[id^=Assessment]');

      assessments.each(
        function () {
          $(this).prop('disabled', false);
        }
      );
      if (thisEventData === EnumChangeEventArg.Change) {
        commonUtility.scrollTo(completed);
      }
      completed.focus();
    }
  }

  /* private function */
  function J1750_depends_on_J0510(J0510: any, thisEventData: EnumChangeEventArg): void {
    console.log('branching::: insite of J1750_depends_on_J0510', J0510);

    const J1750s = $('.persistable[id^=J1750]');
    const J1750_yes = $('.persistable[id^=J1750][id*=Yes]');

    if (commonUtility.getControlValue(J0510) === 0) { /* 0. Does not apply */
      console.log('J0510 is 0, unlock J1750s and focus on J1750 Yes option');

      J1750s.each(
        function () {
          $(this).prop('disabled', false);
        }
      );
      if (thisEventData === EnumChangeEventArg.Change) {
        commonUtility.scrollTo(J1750_yes);
      }
      J1750_yes.focus();
    }
    else {
      console.log('J0510 is not 0, uncheck all J1750');

      J1750s.each(
        function () {
          $(this).prop('checked', false);
        }
      );
    }
  }

  /*private function*/
  function AddMore(stage: string, questionKey: string) {
    console.log('branching::: insite of AddMore');

    const lastInputIdx: number = $('.persistable[id^=' + questionKey + '_' + stage + ']').length;
    const lastInputDate: any = $('.persistable[id^=' + questionKey + '_' + stage + '_' + lastInputIdx + ']');
    const dateClone: any = lastInputDate.clone();
    dateClone.val('').focus();
    lastInputDate.append(dateClone);
  }

  /* private function */
  function CheckAllRules(thisEventData:EnumChangeEventArg): void {
    console.log('branching:::', 'Inside of CheckAllRules()', thisEventData);

    const Q12 = $(".persistable[id^=Q12_]");
    Q12_Q23_blank_then_Lock_All(thisEventData);
    Q12B_blank_then_Lock_Discharge();
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

    $('.persistable[id^=GG0170I]:not([id*=Discharge_Goal])').each(
      function () {
        GG0170JKLMN_depends_on_GG0170I($(this), thisEventData);
      }
    );

    $('.persistable[id^=GG0170M]:not([id*=Discharge_Goal]),.persistable[id^=GG0170N]:not([id*=Discharge_Goal])').each(
      function () {
        GG0170P_depends_on_GG0170M_and_GG0170N($(this), thisEventData);
      }
    );

    GG0170Q_is_No_skip_to_Complete($('.persistable[id^=GG0170Q]:not([id*=Discharge_Goal])'), thisEventData);
    J1750_depends_on_J0510($('.persistable[id^=J0510]:not([id*=Discharge_Goal])'), thisEventData);

    console.log('focus on Q12 at the end of CheckAllRules()');
    if (thisEventData != EnumChangeEventArg.Change) {
      commonUtility.scrollTo(Q12);
    }
    Q12.focus();
  }

  /***************************************************************************
   * public functions exposing the private functions to outside of the closure
  ***************************************************************************/
  return {
    'CheckAllRules': CheckAllRules,
    'Q12_Q23_blank_then_Lock_All': Q12_Q23_blank_then_Lock_All,
    'Q12B_blank_then_Lock_Discharge': Q12B_blank_then_Lock_Discharge,
    'Q14B_enabled_if_Q14A_is_Yes': Q14B_enabled_if_Q14A_is_Yes,
    'Q16A_is_Home_then_Q17': Q16A_is_Home_then_Q17,
    'Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A': Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A,
    'Q42_Interrupted_then_Q43': Q42_Interrupted_then_Q43,
    'Q44C_Affect_Q44D_Q46': Q44C_Affect_Q44D_Q46,
    'GG0170JKLMN_depends_on_GG0170I': GG0170JKLMN_depends_on_GG0170I,
    'GG0170P_depends_on_GG0170M_and_GG0170N': GG0170P_depends_on_GG0170M_and_GG0170N,
    'GG0170Q_is_No_skip_to_Complete': GG0170Q_is_No_skip_to_Complete,
    'J1750_depends_on_J0510': J1750_depends_on_J0510,
    'AddMore': AddMore
  }
})();
/******************************* end of closure ****************************/

$(function () {
  const stage: string = $('.pageTitle').data('systitle').replace(/\s/g, '_');

  /* on ready */
  if (stage === 'Full') {
    $('.persistable').prop("disabled", true);
  }
  else {
    console.log('branching::: CheckAllRules is called by on ready');
    branchingController.CheckAllRules(EnumChangeEventArg.Ready);
  }

  /* on click */
  $('button[id^=btnMore]').each(
    function () {
      $(this).click(
        function () {
          const questionKey: string = $(this).data('questionkey');
          branchingController.AddMore(stage, questionKey);
        }
      );
    }
  );

  /* on change */
  $('.persistable[id^=Q12_], .persistable[id^=Q23]').each(
    function () {
      const $this = $(this);
      $this.on('change', { eventTriger: EnumChangeEventArg.NoScroll },
        function (e) {
          console.log('branching::: ' + $this.prop('id') + ' is called by change()', e.data.eventTriger);

          branchingController.CheckAllRules(e.data.eventTriger);
        }
      );
    }
  );

  /* on change */
  $('.persistable[id^=Q12B_]').each(function () {
    $(this).on('change', function () {
      branchingController.Q12B_blank_then_Lock_Discharge();
    });
  })

  /* on change */
  $('.persistable[id^=Q14A]').each(
    function () {
      $(this).on('change',
        function () {
          branchingController.Q14B_enabled_if_Q14A_is_Yes();
        }
      );
    }
  );

  /* on change of dropdown Q16A*/
  $('.persistable[id^=Q16A]').each(
    function () {
      $(this).on('change',
        function () {
          branchingController.Q16A_is_Home_then_Q17();
        }
      );
    }
  );

  /* on change */
  $('.persistable[id^=Q42]').each(
    function () {
      $(this).on('change',
        function () {
          branchingController.Q42_Interrupted_then_Q43();
        }
      );
    });

  /* on change */
  $('.persistable[id^=Q44C]').each(
    function () {
      $(this).on('change',
        function () {
          branchingController.Q44C_Affect_Q44D_Q46($(this));
        }
      );
    })

  /* on change */
  $('.persistable[id^=GG0170I]:not([id*=Discharge_Goal])').each(
    function () {
      $(this).on('change', { eventTriger: EnumChangeEventArg.Change },
        function (e) {
          branchingController.GG0170JKLMN_depends_on_GG0170I($(this), e.data.eventTriger);
        }
      );
    });

  /* on change */
  $('.persistable[id^=GG0170M]:not([id*=Discharge_Goal]), .persistable[id^=GG0170N]:not([id*=Discharge_Goal])').each(
    function () {
      $(this).on('change', { eventTriger: EnumChangeEventArg.Change },
        function (e) {
          branchingController.GG0170P_depends_on_GG0170M_and_GG0170N($(this), e.data.eventTriger);
        }
      );
    });

  /* on change */
  $('.persistable[id^=GG0170Q]:not([id*=Discharge_Goal])').each(
    function () {
      $(this).on('change', { eventTriger: EnumChangeEventArg.Change },
        function (e) {
          branchingController.GG0170Q_is_No_skip_to_Complete($(this), e.data.eventTriger);
        }
      );
    });

  /* on change */
  $('.persistable[id^=J0510]:not([id*=Discharge_Goal])').each(
    function () {
      $(this).on('change', { eventTriger: EnumChangeEventArg.Change },
        function (e) {
          branchingController.J1750_depends_on_J0510($(this), e.data.eventTriger);
        }
      );
    });
});
