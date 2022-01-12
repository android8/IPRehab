/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
//https://www.typescriptlang.org/docs/handbook/asp-net-core.html
$(function () {
    $('.rehabAction').each(function () {
        let $thisButton = $(this);
        //call closure
        //commandBtnController.addRipple($this);
        $thisButton.click(function () {
            //call closure
            //commandBtnController.makeRequest($this);
            commandBtnController.makeRequestUsingFormAction($thisButton);
        });
    });
    $('.commandTrigger').click(function () {
        const $this = $(this);
        const thisContainer = $this.parent(); //should be <div class="mdc-touch-target-wrapper">
        let hidden = $this.data('hidden');
        commandBtnController.slideCommands(thisContainer, hidden);
        $this.data('hidden', !hidden);
        if (hidden)
            $this.prop('title', 'Show Commands');
        else
            $this.prop('title', 'Hide Commands');
    });
});
/****************************************************************************
 * javaScript closure
 ***************************************************************************/
let commandBtnController = (function () {
    /* private function */
    function addRipple(el) {
        /* addMaterial Design ripple effect to all .rehabAction buttons */
        //no need to retain a reference to the ripple instance, there's no need to assign it to a variable.
        //new MDCRipple(document.querySelector(el));
    }
    /* private function */
    function makeRequest($this) {
        //get formaction attribute which is created by Tag Helper
        let thisUrl = $this.prop('formAction');
        ;
        /* data-attribute are all in lower case by covention */
        let controller = $this.data('controller');
        let action = $this.data('action');
        let stage = $this.data('stage');
        let stageLowerCase = stage.toLowerCase();
        let patientID = $this.data('patientid');
        let patientName = $this.data('patientname');
        let episodeID = $this.data('episodeid');
        let searchCriteria = $this.data('searchcriteria');
        let orderBy = $this.data('orderby');
        let pageNumber = $this.data('pagenumber');
        if (stageLowerCase.indexOf('patientlist') !== -1) {
            stageLowerCase = 'patient;';
        }
        //get queryparameter. this is not suitable if the querystring is encrypted
        //const urlParams = new URLSearchParams(window.location.search);
        //console.log('urlParams', urlParams);
        //const param_x = urlParams.get('stage');
        //console.log('param_x', param_x);
        //if (param_x == stageLowerCase || (stageLowerCase == 'Full' && param_x == '')) {
        //  alert('You are already in it');
        //  $('.spinnerContainer').hide();
        //}
        //else {
        //  location.href = thisUrl;
        //}
        let pageTitleLowerCase = $(".pageTitle").data('systitle').toLowerCase();
        if (stageLowerCase === pageTitleLowerCase) {
            alert('You are already in it');
            $('.spinnerContainer').hide();
        }
        else {
            thisUrl = location.protocol + '//' + location.host + '/' + controller + '/' + action + '?stage=' + stage + '&patientID=' + patientID + '&patientName=' + patientName + '&episodeID=' + episodeID + '&searchCriteria=' + searchCriteria + '&orderBy=' + orderBy + '&pageNumber=' + pageNumber;
            console.log('thisUrl', thisUrl);
            location.href = thisUrl;
        }
    }
    function makeRequestUsingFormAction($thisButton) {
        let stage = $thisButton.data('stage');
        let stageLowerCase = stage.toLowerCase();
        if (stageLowerCase.indexOf('patientlist') !== -1) {
            stageLowerCase = 'patient;';
        }
        if (stageLowerCase.indexOf('followup') !== -1)
            stageLowerCase = 'follow up';
        if (stageLowerCase == '')
            stageLowerCase = 'full';
        //get queryparameter. this is not suitable if the querystring is encrypted
        //const urlParams = new URLSearchParams(window.location.search);
        //console.log('urlParams', urlParams);
        //const param_x = urlParams.get('stage');
        //console.log('param_x', param_x);
        //if (param_x == stageLowerCase || (stageLowerCase == 'Full' && param_x == '')) {
        //  alert('You are already in it');
        //  $('.spinnerContainer').hide();
        //}
        //else {
        //  location.href = thisUrl;
        //}
        let pageTitleLowerCase = $(document).prop('title').toLowerCase();
        let alreadyThere = false;
        if (pageTitleLowerCase.indexOf('episode of care') !== -1) {
            pageTitleLowerCase = 'base';
        }
        if (pageTitleLowerCase === stageLowerCase)
            alreadyThere = true;
        let dialogOptions = {
            resizable: true,
            height: "auto",
            width: 400,
            modal: true,
            stack: true,
            sticky: true,
            position: { my: 'center', at: 'center', of: window },
            classes: { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' },
            buttons: [{
                    text: "Close",
                    //icon: "ui-icon-close",
                    click: function () {
                        $(this).dialog("close");
                    }
                }]
        };
        if (alreadyThere) {
            $('.spinnerContainer').hide();
            $('#dialog')
                .text('You are already in it')
                .dialog(dialogOptions, {
                title: 'Warning'
            });
        }
        else {
            let submitButton = $('#ajaxPost');
            if (submitButton.length == 0) {
                //submit doesn't exist on this page
                //get formaction attribute for this button and navigate away
                let thisUrl = $thisButton.prop('formAction');
                location.href = thisUrl;
            }
            else {
                //submit exists on this page
                //if submit is not disabled then the form is dirty
                if (!submitButton.is(":disabled")) {
                    $('.spinnerContainer').hide();
                    $('#dialog')
                        .text('Data is not saved. To save it, click Cancel to close this dialog window, then click the purple Save button on the upper left edge of the browser tab. To abandon the changes, click OK to continue going to the ' + stage + ' page')
                        .dialog(dialogOptions, {
                        title: 'Warning',
                        buttons: [{
                                text: "Ok",
                                click: function () {
                                    $(this).dialog("close");
                                    $('.spinnerContainer').show();
                                    var thisUrl = $thisButton.prop('formAction');
                                    $('.spinnerContainer').show();
                                    //navigate away
                                    location.href = thisUrl;
                                }
                            },
                            {
                                text: "Cancel",
                                click: function () {
                                    $('.spinnerContainer').hide();
                                    $(this).dialog("close");
                                }
                            }]
                    });
                }
                else {
                    let thisUrl = $thisButton.prop('formAction');
                    location.href = thisUrl;
                }
            }
        }
    }
    function slideCommands(triggerContainer, hidden) {
        let siblingContainers = [];
        triggerContainer.siblings().each(function () {
            let siblingContainer = $(this); //should be <div class="mdc-touch-target-wrapper">
            let el = {};
            el.h = siblingContainer;
            el.width = el.h.children().eq(0).width(); //should be <button>
            el.h.width(0);
            siblingContainers.push(el);
        });
        for (var i = 0; i < siblingContainers.length; i++) {
            let thisCommandbtn = siblingContainers[i];
            let target = hidden ? thisCommandbtn.width + "px" : "0px";
            thisCommandbtn.h.animate({ width: target });
        }
    }
    /****************************************************************************
     * public functions exposing addRipple() and makeRequest() to outside of the closure
    ****************************************************************************/
    return {
        'addRipple': addRipple,
        'makeRequest': makeRequest,
        'makeRequestUsingFormAction': makeRequestUsingFormAction,
        'slideCommands': slideCommands
    };
})();
export {};
//# sourceMappingURL=commandBtns.js.map