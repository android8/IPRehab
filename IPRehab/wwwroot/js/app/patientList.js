/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />
//https://www.typescriptlang.org/docs/handbook/asp-net-core.html
$(function () {
    pageLoad();
});
function pageLoad() {
    const urlParams = new URLSearchParams(window.location.search.substring(1));
    const previousCriteria = urlParams.get('criteria'); //get criteria in the querystring
    const previousSearchInputValue = $("#searchCriteria").val().toString(); //get criteria from the input ellement
    //$('#previousCriteria').text(previousCriteria);
    if (previousSearchInputValue == '')
        $('#searchCriteria').val(previousCriteria); //only set the val of the input when the input has no value set by the server
    $('#search').on('click', function () {
        let host = location.host;
        let searchCriteria = $('#searchCriteria').val().toString();
        let href = '';
        let $this = $(this);
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
export {};
//# sourceMappingURL=patientList.js.map