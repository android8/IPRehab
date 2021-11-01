/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

/* jquery plugin dependsOn*/
/* https://dstreet.github.io/dependsOn */
/* http://emranahmed.github.io/Form-Field-Dependency */

$(function () {
  const stage: string = $(document).prop('title').toLowerCase();

  $('iput[id^="Q42_' + stage + '"]').each(function () {
    $(this).change(function () {
      formController.Q42_Interrupted_then_Q43(stage, $(this));
    });
  })
  $('#btnMoreQ42').each(function () {
    $(this).click(function () {
        formController.AddMoreQ42Q43(stage, 'Q42')
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
  function Q12_Q23_blank_then_Lock_All(stage: string): boolean {
    let Q12_isNull: boolean = $("input[id^='Q12_" + stage + "']").val() == null;
    let Q23_isNull: boolean = $("input[id^='Q23_" + stage + "']").val() == null;

    if (Q12_isNull || Q23_isNull) {
      $('.persistable').each(function () {
        let $this: any = $(this);
        if ($this.prop("id").indexOf('Q12') < 0 || $this.prop("id").indexOf('Q23') < 0) {
          $this.prop("disabled", true); 
        }
      });
      return false;
    }
    else {
      $("input[id^='Q14A']").prop('disabled', false).focus();
      return false;
    }
  }

  /* private function */
  function Q16A_is_Home_then_Q17(stage: string): boolean {
    let Q16A_is_Home: boolean = $("input[id^='Q16']").val() == 94 /*1. Home */;
    if (Q16A_is_Home) {
      $("input[id^='Q17']").prop('disabled', false).focus();
      return false;
    }
  }

  /* private function */
  function Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A(stage: string): boolean {
    let Q24A_is_set: boolean = false;
    $('input[id ^= "Q21A"]').each(function () {
      if ($(this).val() == '123.45') {
        $('input[id^="Q24A_' + stage + '_86"]').prop('checked', true);
        Q24A_is_set = true;
        return; //break out each()
      }
    });

    if (!Q24A_is_set) {
      $('input[id^="Q22"]').each(function () {
        if ($(this).val() == '123.45') {
          $('input[id^="Q24A_' + stage + '_86"]').prop('checked', true);
          Q24A_is_set = true;
          return; //break out each()
        }
      });
    }

    if (!Q24A_is_set) {
      $('input[id^="Q24"]').each(function () {
        if ($(this).val() == '123.45') {
          $('input[id^="Q24A_' + stage + '_86"]').prop('checked', true);
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
  function Q42_Interrupted_then_Q43(stage: string, $this: any): boolean {
    let Q42_Interrupted: boolean = $this.val() != null;
    let thisQ43ID: string = $this.prop('id').replace('42', '43');
    if (Q42_Interrupted) {
      if ($('#' + thisQ43ID).length == 0) {
        let dateClone: any = $this;
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
  function AddMoreQ42Q43(stage: string, questionKey: string) {
    /* add interrup date control */
    let lastInputIdx: number = $('input[id^="' + questionKey + '_' + stage + '"]').length;
    let lastInputDate: any = $('#' + questionKey + '_' + stage + '_' + lastInputIdx);
    let dateClone: any = lastInputDate.clone();
    dateClone.val(null).focus();
  }

  /* private function */
  function Q44C_is_Y_then_Q44D(stage: string): boolean {
    /*codeset ID 86(Y) 87(N)*/
    let Q44C_Y: any = $('input[id^="Q44C' + stage + '_86"]');
    if (Q44C_Y.prop('checked')) {
      $('iput[id^="Q44D"]').prop('disabled', false).focus();
      $('iput[id^="Q44E"]').prop('disabled', false).focus();
      $('iput[id^="Q45"]').prop('disabled', false).focus();
      return false;
    }
    return true;
  }

  /* private function */
  function Q44C_is_N_then_Q46(stage: string): boolean {
    /*codeset ID 87(N)*/
    let Q44C_N: any = $('input[id%="Q44C' + stage + '_87]');
    if (Q44C_N.prop('checked')) {
      if (confirm('Q44D and Q44E answers will be resetted')) {
        $('iput[id^="Q44D"]').val(-1);
        $('iput[id^="Q44D"]').prop('disabled', true).focus();
        $('iput[id^="Q44E"]').val(-1);
        $('iput[id^="Q44E"]').prop('disabled', true).focus();
        $('iput[id^="Q46"]').prop('disabled', false).focus();
      }
    }
    return true;
  }

  /****************************************************************************
 * public functions exposing the private functions to outside of the closure
***************************************************************************/
  return {
    'Q12_Q23_blank_then_Lock_All_else_Q14A': Q12_Q23_blank_then_Lock_All,
    'Q16A_is_Home_Then_Q17': Q16A_is_Home_then_Q17,
    'Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A': Q21A_Q21B_Q22_Q24_is_Arthritis_then_Q24A,
    'Q42_Interrupted_then_Q43': Q42_Interrupted_then_Q43,
    'AddMoreQ42Q43': AddMoreQ42Q43,
    'Q44C_is_Y_then_Q44D': Q44C_is_Y_then_Q44D,
    'Q44C_is_N_then_Q46': Q44C_is_N_then_Q46
  }
})();