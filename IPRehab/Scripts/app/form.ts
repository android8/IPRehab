﻿/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />

import { IUserAnswer, AjaxPostbackModel } from "../appModels/IUserAnswer";

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
  pageLoad();
});

function pageLoad(): void {
  let commandBtnScope = $('.commandBtn');
  $.each($('input[type="checkbox"]', commandBtnScope), function () {
    let $this = $(this);
    if ($this.prop("checked")) {
      setRehabBtns($this.parent());
    }
  });

  $('.persistable').change(function () { $('#submit').removeAttr('disabled') });

  //handle rehab action checkbox
  $('input[type="checkbox"]').click(function () {
    let $this: any = $(this);
    let targetScope: any = $this.parent();

    if ($this.prop("checked")) {
      setRehabBtns(targetScope);
    }
    else {
      resetRehabBtns(targetScope);
    }
  })

  /* jump to section anchor */
  $('.gotoSection').click(function () {
    let $this = $(this);
    let anchorId = $this.data("anchorid");
    scrollToAnchor(anchorId);
  });

  /* slide section nav */
  $("#questionTab").hover(
    function () {
      $('#questionTab').css({ 'left': '0px', 'transition-duration': '1s' });
    },
    function () {
      $('#questionTab').css({ 'left': '-230px', 'transition-duration': '1s' });
    }
  );

  checkRules();

  /* collect all persistable input values */
  $('#submit').click(function () {
    alert('collecting answers');
  });
}

/* scroll to an anchor */
function scrollToAnchor(aid) {
  let aTag: any = $('a[name="' + aid + '"]');
  $('html,body').animate({ scrollTop: aTag.offset().top-15 }, 'fast');
}

function setRehabBtns(targetScope: any) {
  let currentIdx: number = 0;
  $.each($('.rehabAction', targetScope), function () {
    let $this = $(this);
    let newTitle: string = $this.attr('title').replace(/Edit/g, 'Create');
    let newHref: string = $this.attr('href').replace(/Edit/g, 'Create');
    $this.attr('title', newTitle);
    $this.attr('href', newHref);
    currentIdx++;
    let newClass: string = $this.attr('class') + ' createActionCmd' + currentIdx.toString();;
    $this.attr('class', newClass);
  });
}

function resetRehabBtns(targetScope: any) {
  let cmdBtns: string[] = ['primary', 'info', 'secondary', 'success','warning'];
  let currentIdx: number = 0;
  $.each($('.rehabAction', targetScope), function () {
    let $this = $(this);
    let newTitle: string = $this.attr('title').replace(/Create/g, 'Edit');
    let newHref: string = $this.attr('href').replace(/Create/g, 'Edit');
    $this.attr('title', newTitle);
    $this.attr('href', newHref);
    let resetClass: string = '';
    resetClass = 'badge badge-' + cmdBtns[currentIdx] + ' rehabAction';
    currentIdx++;
    $this.attr('class', resetClass);
  });
}

function checkRules() {
  let q44c_is_1: boolean = $('#Q44C_86').prop("checked");
  let q44c_is_0: boolean = $('#Q44C_87').prop("checked");
  let q44d_is_1: boolean = $('#Q44D_').val() == '1';
  let q46: any = $('#Q46_').val();

  if (!q44c_is_1 && !q44c_is_0) {
    /* Q44c is not answered */
    $('#Q44D_').attr('disabled', 'true');
    $('#Q45_').attr('disabled', 'true');
  }
  if (q44c_is_1 && q44d_is_1)
  {
    /*Q44C = 1 and Q44D = 1*/
    $('#Q45_').attr('disabled', 'false');
  }
  else
  {
    if (q44c_is_0) {
      $('#Q44D_').attr('disabled', 'false');
      $('#Q46_').focus();
    }
  }

  /* interrupted */
  let q42_is_interrupted: boolean = $('#Q42-INTRRUPT_86').prop('checked');
  
  if (q42_is_interrupted) {
    $('#Q43_').attr('disabled', 'false');
    $('#Q43_').focus();
  }
}

