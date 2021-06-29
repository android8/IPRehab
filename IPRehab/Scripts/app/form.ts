/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
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