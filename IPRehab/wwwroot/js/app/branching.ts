/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

/* jquery plugin dependsOn*/
/* https://dstreet.github.io/dependsOn */
/* http://emranahmed.github.io/Form-Field-Dependency */

$(function () {
  const stage: string = $('.pageTitle').text().replace(' ', '_');

  if (stage !== 'Full') {
    /* on ready */
    branchingController.CommonUnlock(stage);
  }
  else {
    $('.persistable').prop("disabled", true);
  }

  /* on click */
  $('button[id^=btnMore]').each(function () {
    $(this).click(function () {
      let questionKey: string = $(this).data('questionkey');
      branchingController.AddMore(stage, questionKey);
    });
  });

  /* on change */
  $("input[id^=Q12_" + stage + "], input[id ^= Q23_" + stage + "]").each(
    function () {
      let $this = $(this);
      $this.change(function () {
        let Q12: any = $("input[id^=Q12_" + stage + "]");
        let Q23: any = $("input[id^=Q23_" + stage + "]");
        if (branchingController.isEmpty(Q12) && branchingController.isEmpty(Q23)) {
          alert('Q12 and Q23 can not be empty');
        }
        //branchingController.Q12_Q23_blank_then_Lock_All(stage);
        branchingController.CommonUnlock(stage);
      });
    }
  );

  /* on change */
  $('input[id^=Q14A_' + stage + ']').each(function () {
    $(this).change(function () {
      branchingController.Q14B_enabled_if_Q14A_is_86(stage);
    });
  });

  /* on change of dropdown Q16A*/
  $("select[id^=Q16A_" + stage + "]").each(function () {
    $(this).change(function () {
      branchingController.Q16A_is_Home_then_Q17(stage);
    });
  });

  /* on change */
  $('input[id^=Q42_' + stage + ']').each(function () {
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

  /* on click */
  $('select[id^=GG0170I_]').each(function () {
    $(this).change(function () {
      branchingController.GG0170JKLMN_depends_on_GG0170I($(this));
    });
  });

  /* on click */
  $('select[id^=GG0170M_]').each(function () {
    $(this).change(function () {
      branchingController.GG0170P_depends_on_GG0170M_and_GG0170N();
    });
  });

  /* on click */
  $('select[id^=GG0170N_]').each(function () {
    $(this).change(function () {
      branchingController.GG0170P_depends_on_GG0170M_and_GG0170N();
    });
  });

  /* on click */
  $('input[id^=GG0170Q_]').each(function () {
    $(this).change(function () {
      branchingController.H0350_depends_on_GG0170Q($(this));
    });
  });

  /* on click */
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

    GG0170JKLMN_depends_on_GG0170I($('#GG0170I_Admission_Performance_1'));
    GG0170P_depends_on_GG0170M_and_GG0170N();
    H0350_depends_on_GG0170Q($('#GG0170Q_Admission_Performance_315'));
    H0350_depends_on_GG0170Q($('#GG0170Q_Admission_Performance_314'));
    J1750_depends_on_J0510($('#J0510_' + stage + '_1'));
  }

  /* private function */
  function Q12_Q23_blank_then_Lock_All(stage: string): void {
    let Q12: any = $("input[id^=Q12_" + stage + "]");
    let Q23: any = $("input[id^=Q23_" + stage + "]");

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
    let Q14AYes: any = $('#Q14A_' + stage + '_86');//codeset id 86 == yes
    let Q14ANo: any = $('#Q14A_' + stage + '_87');//codeset id 87 = no
    let Q14Bs: any = $('input[id^=Q14B_' + stage + ']');
    if ((Q14AYes.length > 0 && !Q14AYes.prop('checked')) && (Q14ANo.length > 0 && !Q14ANo.prop('checked'))) {
      if (Q14Bs.length > 0) {
        Q14Bs.each(function () {
          $(this).prop('checked', false);
          $(this).prop('disabled', true);
        });
      }
    }

    if (Q14AYes.length > 0 && Q14AYes.prop('checked')) {
      if (Q14Bs.length > 0) {
        Q14Bs.removeAttr('disabled');
        Q14Bs[0].focus();
      }
    }

    if (Q14ANo.length > 0 && Q14ANo.prop('checked')) {
      if (Q14Bs.length > 0) {
        Q14Bs.each(function () {
          $(this).prop("checked", false).prop('disabled', true);
        });
      }
    }
  }

  /* private function */
  function Q16A_is_Home_then_Q17(stage: string): void {
    const Q16A: any = $("select[id^=Q16A_" + stage + "]");
    const Q17: any = $("select[id^=Q17_" + stage + "]");
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
    const GG0170JKLs: any = $('select[id^=GG0170J], select[id^=GG0170K], select[id^=GG0170L]');
    const GG0170Ms: any = $('select[id^=GG0170M]');
    const GG0170Ns: any = $('select[id^=GG0170N]');
    let GG0170ISelectedOption: any = $('#' + GG0170I.prop('id') + ' option:selected').text();
    let GG0170ISelectedOptionInt: number = parseInt(GG0170ISelectedOption);

    if (GG0170ISelectedOptionInt > 0) {
      switch (true) {
        case (GG0170ISelectedOptionInt >= 7):
          {
            /* lock and clear J K L, advance to M, unlock M N */
            if (GG0170Ms.length > 0) {
              if (GG0170JKLs.length > 0) {
                GG0170JKLs.each(function () {
                  $(this).prop('disabled', true).val(-1).change();
                });
              }
              if (GG0170Ns.length > 0) {
                GG0170Ns.each(function () {
                  $(this).prop('disabled', false).val(-1).change();
                });
              }
              if (GG0170Ms.length > 0) {
                GG0170Ms.each(function () {
                  $(this).prop('disabled', false).val(-1).change();
                });
                GG0170Ms[0].focus();
              }
            }
          }
          break;
        case (GG0170ISelectedOptionInt <= 6):
          {
            /* unlock and clear J K L, lock and clear M N */
            if (GG0170JKLs.length > 0) {
              GG0170JKLs.each(function () {
                $(this).prop('disabled', false).val(-1).change();
              });
            }
            if (GG0170Ms.length > 0) {
              GG0170Ms.each(function () {
                $(this).prop('disabled', true).val(-1).change();
              });
            }
            if (GG0170Ns.length > 0) {
              GG0170Ns.each(function () {
                $(this).prop('disabled', true).val(-1).change();
              });
            }
          }
          break;
      }
    }
    else {
      /* NaN or  <= 0 */
      /* lock and clear J K L M */
      if (GG0170JKLs.length > 0) {
        GG0170JKLs.each(function () {
          $(this).prop('disabled', true).val(-1).change();
        });
      }
      if (GG0170Ms.length > 0) {
        GG0170Ms.each(function () {
          $(this).prop('disabled', true).val(-1).change();
        });
      }
      if (GG0170Ns.length > 0) {
        GG0170Ns.each(function () {
          $(this).prop('disabled', true).val(-1).change();
        });
      }
    }
  }

  /* private function */
  function GG0170P_depends_on_GG0170M_and_GG0170N(): void {
    let GG0170MSelectedOption: any = $('#GG0170M_Admission_Performance_1 option:selected').text();
    let GG0170MSelectedOptionInt: number = parseInt(GG0170MSelectedOption);
    let GG0170NSelectedOption: any = $('#GG0170N_Admission_Performance_1 option:selected').text();
    let GG0170NSelectedOptionInt: number = parseInt(GG0170NSelectedOption);
    let GG0170Ps: any = $('select[id^=GG0170P]');

    if (GG0170MSelectedOptionInt > 0 || GG0170NSelectedOptionInt > 0) {
      switch (true) {
        case (GG0170MSelectedOptionInt <= 6 || GG0170NSelectedOptionInt <= 6):
          /* lock and clear P */
          if (GG0170Ps.length > 0) {
            GG0170Ps.each(function () {
              $(this).prop('disabled', true).val(-1).change();
            })
          }
          break;
        case (GG0170MSelectedOptionInt >= 7 || GG0170NSelectedOptionInt >= 7):
          /* unlock and clear P */
          if (GG0170Ps.length > 0) {
            GG0170Ps.each(function () {
              $(this).prop('disabled', false).val(-1).change();
            })
            GG0170Ps[0].focus();
          }
          break;
      }
    }
    else {
      /* lock and clear P */
      if (GG0170Ps.length > 0) {
        GG0170Ps.each(function () {
          $(this).prop('disabled', true).val(-1).change();
        });
      }
    }
  }

  /* private function */
  function H0350_depends_on_GG0170Q(GG0170QAdmPerformance: any): void {
    switch (GG0170QAdmPerformance.prop('id')) {
      case 'GG0170Q_Admission_Performance_315': {
        let H0350s: any = $('select[id^=H0350]');
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

      case 'GG0170Q_Admission_Performance_314': {
        let GG0170Rs: any = $('#GG0170R_Admission_Performance_314');
        if (GG0170QAdmPerformance.prop('checked')) {
          GG0170Rs.each(function () {
            $(this).prop('disabled', false)
          });
          GG0170Rs[0].focus();
        }
        else {
          GG0170Rs.each(function () {
            $(this).prop('disabled', true).val(-1);
          });
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