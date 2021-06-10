/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />

import { IUserAnswer, AjaxPostbackModel } from "../appModels/IUserAnswer";

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

$(function () {
   pageLoad();
});

function pageLoad(): void {
  const urlParams: any = new URLSearchParams(window.location.search.substring(1));
  const previousCriteria: string = urlParams.get('criteria'); //get criteria in the querystring
  //$('#previousCriteria').text(previousCriteria);

  $('#searchCriteria').val(previousCriteria);
  $('#search').on('click', function () {
    let host: string = location.host;
    let searchCriteria: string = $('#searchCriteria').val().toString();
    let href: string = '';
    let $this: any = $(this);
    if (host.indexOf('localhost') != -1) {
      href = '/Patient/Index?criteria=' + searchCriteria;
      //alert('local searchCriteria: ' + href);
    }
    else {
      href = '/IPRehabMetrics/Patient/Index?criteria=' + searchCriteria;
      //alert('remote searchCriteria: ' + href);
    }
    //$('#previousCriteria').text('');
    $('#recordCount').text('');
    $this.attr('href', href);
  });
}