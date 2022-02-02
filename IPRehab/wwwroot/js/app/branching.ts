/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
import { formController } from '../app/form.js';

/* jquery plugin dependsOn*/
/* https://dstreet.github.io/dependsOn */
/* http://emranahmed.github.io/Form-Field-Dependency */

$(function () {
  const stage: string = $('.pageTitle').data('stagecode').replace(/\s/g, '_');

  /* on ready */
  if (stage == 'Full') {
    $('.persistable').prop("disabled", true);
  }
  else {
    branchingController.CommonUnlock(stage);
  }

  /* on click */
  $('button[id^=btnMore]').each(function () {
    $(this).click(function () {
      let questionKey: string = $(this).data('questionkey');
      branchingController.AddMore(stage, questionKey);
    });
  });

  /* on change */
  $("input[id^=Q12], input[id^=Q23]").each(
    function () {
      let $this = $(this);
      $this.change(function () {
        let Q12: any = $("input[id^=Q12]");
        let Q23: any = $("input[id^=Q23]");
        if (branchingController.isEmpty(Q12) && branchingController.isEmpty(Q23)) {
          alert('Q12 and Q23 can not be empty');
        }
        //branchingController.Q12_Q23_blank_then_Lock_All(stage);
        branchingController.CommonUnlock(stage);
      });
    }
  );

  /* on change */
  $('input[id^=Q14A]').each(function () {
    $(this).change(function () {
      branchingController.Q14B_enabled_if_Q14A_is_86(stage);
    });
  });

  /* on change of dropdown Q16A*/
  $("select[id^=Q16A]").each(function () {
    $(this).change(function () {
      branchingController.Q16A_is_Home_then_Q17(stage);
    });
  });

  /* on change */
  $('input[id^=Q42]').each(function () {
    $(this).change(function () {
      branchingController.Q42_Interrupted_then_Q43(stage);
    });
  });

  /* on change */
  $('input[id^=Q44C_' + stage + '_86]').each(function () {
    $(this).change(function () {
      branchingController.Q44C_is_Y_then_Q44D($(this));
    });
  });

  /* on change */
  $('input[id^=Q44C_' + stage + '_87]').each(function () {
    $(this).change(function () {
      branchingController.Q44C_is_N_then_Q46($(this));
    });
  });

  /* on change */
  $('select[id^=GG0170I_]').each(function () {
    $(this).change(function () {
      branchingController.GG0170JKLMN_depends_on_GG0170I($(this));
    });
  });

  /* on change */
  $('select[id^=GG0170M_]').each(function () {
    $(this).change(function () {
      branchingController.GG0170P_depends_on_GG0170M_and_GG0170N();
    });
  });

  /* on change */
  $('select[id^=GG0170N_]').each(function () {
    $(this).change(function () {
      branchingController.GG0170P_depends_on_GG0170M_and_GG0170N();
    });
  });

  /* on change */
  $('input[id^=GG0170Q_]').each(function () {
    $(this).change(function () {
      branchingController.H0350_depends_on_GG0170Q($(this));
    });
  });

  /* on change */
  $('select[id^=J0510_]').each(function () {
    $(this).change(function () {
      branchingController.J1750_depends_on_J0510($(this));
    });
  });
});

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

let branchingController = (function () {
  function isEmpty($this: any): boolean {
    if ((typeof $this.val() !== 'undefined') && $this.val())
      return false;
    else
      return true;
  }

  /* private function */
  function CommonUnlock(stage: string): void {
    Q12_Q23_blank_then_Lock_All(stage);
    Q14B_enabled_if_Q14A_is_86(stage);
    Q16A_is_Home_then_Q17(stage);

    $('input[id^=Q42_' + stage + ']').each(function () {
      Q42_Interrupted_then_Q43(stage);
    });

    $('input[id^=Q44C_' + stage + '_86]').each(function () {
      Q44C_is_Y_then_Q44D($(this));
    });

    $('input[id^=Q44C_' + stage + '_87]').each(function () {
      Q44C_is_N_then_Q46($(this));
    });

    GG0170JKLMN_depends_on_GG0170I($('#GG0170I_Admission_Performance_0'));
    GG0170P_depends_on_GG0170M_and_GG0170N();
    H0350_depends_on_GG0170Q($('#GG0170Q_Admission_Performance_315'));
    H0350_depends_on_GG0170Q($('#GG0170Q_Admission_Performance_314'));
    J1750_depends_on_J0510($('#J0510_' + stage + '_0'));
    $("input[id^=Q12]")[0].focus();
  }

  /* private function */
  function Q12_Q23_blank_then_Lock_All(stage: string): void {
    let Q12: any = $("input[id^=Q12]");
    let Q23: any = $("input[id^=Q23]");

    if (isEmpty(Q12) || isEmpty(Q23)) {
      $('.persistable').each(function () {
        let $this: any = $(this);
        if ($this.prop("id").indexOf('Q12') < 0 && $this.prop("id").indexOf('Q23') < 0) {
          $this.prop("disabled", true);
        }
      });
    }
    else {
      $('.persistable').each(function () {
        let $this: any = $(this);
        $this.prop("disabled", false);
      });
    }
  }

  /* private function */
  function Q14B_enabled_if_Q14A_is_86(stage: string): void {
    let Q14Achecked: boolean = false;
    let Q14Avalue: number;

    $('input[id^=Q14A]').each(function () {
     if ($(this).prop('checked')) {
       Q14Achecked = true;
       Q14Avalue = parseInt($(this).prop('value'));
      }
    });

    let Q14Bs: any = $('input[id^=Q14B]');
    if (!Q14Achecked && Q14Bs.length > 0) {
      Q14Bs.each(function () {
        $(this).prop('checked', false).prop('disabled', true);
      });
    }
    else {
      //codeset id 86 == yes
      //codeset id 87 = no
      if (Q14Avalue == 86) {
        Q14Bs.removeAttr('disabled');
        Q14Bs[0].focus();
      }
      else {
        Q14Bs.each(function () {
          $(this).prop("checked", false).prop('disabled', true);
        });
      }
    }
  }

  /* private function */
  function Q16A_is_Home_then_Q17(stage: string): void {
    const Q16A: any = $("select[id^=Q16A]");
    const Q17: any = $("select[id^=Q17]");
    // if (Q16A.length > 0 && Q16A.val() == '94') { /*1. Home */
    if (Q16A.length > 0 && !isEmpty(Q16A) && Q16A.val() == '94') {
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
  function Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A(stage: string): void {
    let Q24A_is_set: boolean = false;
    $('[id ^= Q21A_],[id ^= Q21B_],[id ^= Q22_],[id ^= Q24_]').each(function () {
      if (!isEmpty($(this)) && $(this).val() == '123.45') { //ICD for diabetes
        $('input[id^=Q24A_' + stage + '_86]').prop('checked', true);
      }
    });
  }

  /* private function */
  function Q42_Interrupted_then_Q43(stage: string): void {
    let Q42sInterrupts: any = $('input[id^=Q42_' + stage + ']');
    let Q43_Resumes: any = $('input[id^=Q43_' + stage + ']');
    let Q43sCount: number = Q43_Resumes.length;
    if (Q42sInterrupts.length > 0) {
      Q42sInterrupts.each(function () {
        const thisInterrupt: any = $(this);
        if (!isEmpty(thisInterrupt)) {
          if (Q43sCount > 0) {
            //clone last Q43
            let newResume: any = Q43_Resumes[Q43sCount - 1];
            newResume.prop('id', 'Q43_' + stage + '_' + Q43sCount);
            newResume.val(null);
            Q43_Resumes[Q43sCount - 1].append(newResume);
            newResume.focus();
          }
        }
      });
    }
    else {
      if (Q43sCount != 1) {
        Q43_Resumes.each(function () {
          let $this: any = $(this);
          if ($this.prop('id').indexOf('_0') >= 0) {
            $this.val(''); //reset date 
          } else {
            $this.remove(); //remove until _0 is found in the ID
          }
        });
      }
    }
  }

  /* private function */
  function Q44C_is_Y_then_Q44D(Q44C_is_Y: any): void {
    /*codeset ID 86(Y) 87(N)*/
    let Q44D: any = $('select[id^=Q44D]');
    let Q45: any = $('select[id^=Q45]')
    if (Q44C_is_Y.prop('checked')) {
      Q44D.prop('disabled', false).focus();
      Q45.prop('disabled', false);
    }
  }

  /* private function */
  function Q44C_is_N_then_Q46(Q44C_is_N: any): void {
    /*codeset ID 87(N)*/
    let Q44D: any = $('select[id^=Q44D]');
    let Q46: any = $('select[id^=Q46]')
    if (Q44C_is_N.prop('checked')) {
      if (confirm('Q44D will be resetted')) {
        Q44D.val(-1);
        Q44D.prop('disabled', true);
        Q46.prop('disabled', false).focus();
      }
    }
  }

  /* private function */
  function GG0170JKLMN_depends_on_GG0170I(GG0170I: any): void {
    const GG0170Js: any = $('select[id^=GG0170J]');
    const GG0170JKLs: any = $('select[id^=GG0170J], select[id^=GG0170K], select[id^=GG0170L]');
    const GG0170Ms: any = $('select[id^=GG0170M]');
    let GG0170ISelectedOption: any = $('#' + GG0170I.prop('id') + ' option:selected').text();
    let GG0170ISelectedOptionInt: number = parseInt(GG0170ISelectedOption);

    if (!isNaN(GG0170ISelectedOptionInt) && GG0170ISelectedOptionInt > 0) {
      switch (true) {
        case (GG0170ISelectedOptionInt >= 7):
          {
            /* lock and clear J K L, advance to M */
            if (GG0170JKLs.length > 0) {
              GG0170JKLs.each(function () {
                console.log('GG0170JKL changed because I >= 7 triggering breakLongSentence from branching.ts');
                $(this).prop('disabled', true).val(-1); //don't use .change() since we are in a change event will cause infinite loop
                formController.breakLongSentence($(this));
              });
            }
            if (GG0170Ms.length > 0) {
              GG0170Ms[0].focus();
            }
          }
          break;
        case (GG0170ISelectedOptionInt <= 6):
          {
            /* unlock and clear J K L, skip to J */
            if (GG0170JKLs.length > 0) {
              GG0170JKLs.each(function () {
                console.log('GG0170JKL changed because I <= 6 triggering breakLongSentence from branching.ts');
                $(this).prop('disabled', false).val(-1);  //don't use .change() since we are in a change event will cause infinite loop
                formController.breakLongSentence($(this));
              });
              GG0170Js[0].focus();
            }
          }
          break;
      }
    }
    else {
      /* lock and clear J K L M */
      if (GG0170JKLs.length > 0) {
        GG0170JKLs.each(function () {
          console.log('GG0170JKL are reset because I has no answer triggering breakLongSentence from branching.ts');
          $(this).prop('disabled', true).val(-1); //don't use .change() since we are in a change event will cause infinite loop
          formController.breakLongSentence($(this));
        });
      }
    }
  }

  /* private function */
  function GG0170P_depends_on_GG0170M_and_GG0170N(): void {
    let GG0170MSelectedOption: any = $('#GG0170M_Admission_Performance_0 option:selected').text();
    let GG0170MSelectedOptionInt: number = parseInt(GG0170MSelectedOption);
    let GG0170NSelectedOption: any = $('#GG0170N_Admission_Performance_0 option:selected').text();
    let GG0170NSelectedOptionInt: number = parseInt(GG0170NSelectedOption);
    let GG0170Ns: any = $('select[id^=GG0170N]');
    let GG0170Os: any = $('select[id^=GG0170O]');
    let GG0170Ps: any = $('select[id^=GG0170P]');

    if ((!isNaN(GG0170MSelectedOptionInt) && GG0170MSelectedOptionInt > 0)) {
      switch (true) {
        case (GG0170MSelectedOptionInt <= 6):
          /* skip to N */
          if (GG0170Ns.length > 0)
            GG0170Ns[0].focus();
          break;
        case (GG0170MSelectedOptionInt >= 7):
          /* skp to P */
          if (GG0170Ps.length > 0) {
            GG0170Ps[0].focus();
          }
          break;
      }
    }
    if (!isNaN(GG0170NSelectedOptionInt) && GG0170NSelectedOptionInt > 0) {
      switch (true) {
        case (GG0170NSelectedOptionInt <= 6):
          /* skip to O */
          if (GG0170Os.length > 0)
            GG0170Os[0].focus();
          break;
        case (GG0170NSelectedOptionInt >= 7):
          /* skp to P */
          if (GG0170Ps.length > 0) {
            GG0170Ps[0].focus();
          }
          break;
      }
    }
  }

  /* private function */
  function H0350_depends_on_GG0170Q(GG0170QAdmPerformance: any): void {
    switch (GG0170QAdmPerformance.prop('id')) {
      /* 315 = No */
      case 'GG0170Q_Admission_Performance_315': {
        let H0350s: any = $('select[id^=H0350_]');
        if (GG0170QAdmPerformance.prop('checked')) {
          H0350s.each(function () {
            $(this).prop('disabled', false);
          });
          H0350s[0].focus();
        }
        else {
          H0350s.each(function () {
            $(this).prop('disabled', true).val(-1);
          });
        }
      }
        break;

      /* 314 = Yes */
      case 'GG0170Q_Admission_Performance_314': {
        let GG0170Rs: any = $('select[id^=GG0170R_');
        if (GG0170QAdmPerformance.prop('checked')) {
          GG0170Rs.each(function () {
            $(this).prop('disabled', false)
          });
          GG0170Rs[0].focus();
        }
      }
        break;
    }
  }

  /* private function */
  function J1750_depends_on_J0510(J0510: any): void {
    let J1750s: any = $('input[id^=J1750]');
    if (J0510.val() == '345') { /*345 == 0. Does not apply */
      J1750s.each(function () {
        $(this).prop('disabled', false);
      });
      J1750s[0].focus();
    }
    else {
      J1750s.each(function () {
        $(this).prop('checked', false);
      });
    }
  }

  /*private function*/
  function AddMore(stage: string, questionKey: string) {
    let lastInputIdx: number = $('input[id^=' + questionKey + '_' + stage + ']').length;
    let lastInputDate: any = $('#' + questionKey + '_' + stage + '_' + lastInputIdx);
    let dateClone: any = lastInputDate.clone();
    dateClone.val('').focus();
    lastInputDate.append(dateClone);
  }

  /***************************************************************************
   * public functions exposing the private functions to outside of the closure
  ***************************************************************************/
  return {
    'isEmpty': isEmpty,
    'CommonUnlock': CommonUnlock,
    'Q12_Q23_blank_then_Lock_All': Q12_Q23_blank_then_Lock_All,
    'Q14B_enabled_if_Q14A_is_86': Q14B_enabled_if_Q14A_is_86,
    'Q16A_is_Home_then_Q17': Q16A_is_Home_then_Q17,
    'Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A': Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A,
    'Q42_Interrupted_then_Q43': Q42_Interrupted_then_Q43,
    'Q44C_is_Y_then_Q44D': Q44C_is_Y_then_Q44D,
    'Q44C_is_N_then_Q46': Q44C_is_N_then_Q46,
    'GG0170JKLMN_depends_on_GG0170I': GG0170JKLMN_depends_on_GG0170I,
    'GG0170P_depends_on_GG0170M_and_GG0170N': GG0170P_depends_on_GG0170M_and_GG0170N,
    'H0350_depends_on_GG0170Q': H0350_depends_on_GG0170Q,
    'J1750_depends_on_J0510': J1750_depends_on_J0510,
    'AddMore': AddMore
  }
})();