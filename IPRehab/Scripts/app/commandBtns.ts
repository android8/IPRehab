//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

//don't need use strict because the script is loaded as module which by default is executed in strict mode
//'use strict';

/****************************************************************************
 * javaScript closure
 ***************************************************************************/

const commandBtnController = (function () {
    const dialogOptions: any = {
        resizable: true,
        //height: ($(window).height() - 200),
        //width: '90%',
        classes: { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' },
        modal: true,
        stack: true,
        sticky: true,
        position: { my: 'center', at: 'center', of: window },
        buttons: [{
            //    "Save": function () {
            //      //do something here
            //      let thisUrl: string = $('form').prop('action');
            //      let postBackModel: AjaxPostbackModel = new AjaxPostbackModel();
            //      postBackModel.NewAnswers = newAnswers;
            //      postBackModel.OldAnswers = oldAnswers;
            //      postBackModel.UpdatedAnswers = updatedAnswers;
            //      alert('ToDo: sending ajax postBackModel to ' + thisUrl);
            //    },
            text: "Close",
            //icon: "ui-icon-close",
            click: function () {
                $(this).dialog("close");
            }
        }]
    };

    /* private function */
    function addRipple(el) {
        /* addMaterial Design ripple effect to all .rehabAction buttons */
        //no need to retain a reference to the ripple instance, there's no need to assign it to a variable.
        //new MDCRipple(document.querySelector(el));
    }

    /* private function */
    function makeRequestUsingFormAction(thisRehabCommandButton) {
        const controller: string = thisRehabCommandButton.data('controller');
        const action: string = thisRehabCommandButton.data('action');
        const stage: string = thisRehabCommandButton.data('stage');
        const patientID: string = thisRehabCommandButton.attr('asp-route-patientid');
        const patientName: string = thisRehabCommandButton.data('patientname');
        const episodeID: string = thisRehabCommandButton.attr('asp-route-episodeid');
        const searchCriteria: string = thisRehabCommandButton.data('searchcriteria');
        const orderBy: string = thisRehabCommandButton.data('orderby')
        const pageNumber: string = thisRehabCommandButton.attr('asp-route-pagenumber');
        const admitDate: string = thisRehabCommandButton.attr('asp-route-admitDate');

        let stageLowerCase: string = stage.toLowerCase();
        switch (true) {
            case stageLowerCase.indexOf('patientlist') !== -1:
                stageLowerCase = 'patient;'
                break;
            case stageLowerCase.indexOf('followup') !== -1:
                stageLowerCase = 'follow up';
                break;
            case stageLowerCase === '':
                stageLowerCase = 'full';
                break;
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

        const pageTitleLowerCase: string = $(".pageTitle").data('systitle').toLowerCase();
        if (stageLowerCase === pageTitleLowerCase) {
            $('.spinnerContainer').hide();
            $('#dialog')
                .text('You are already in it')
                .dialog(dialogOptions, {
                    title: 'Warning'
                });
        }
        else {
            const thisUrl: string = thisRehabCommandButton.attr('data-href');
            console.log('this RehabCommandButton data-href', thisUrl);
            location.href = thisUrl;
        }
    }

    function slideCommands(triggerContainer: any, hidden: boolean) {
        const siblingContainers: any[] = [];
        triggerContainer.siblings().each(function () {
            const siblingContainer = $(this); //should be <div class="mdc-touch-target-wrapper">
            const el: any = {};
            el.h = siblingContainer;
            el.width = el.h.children().eq(0).width(); //should be <button>
            el.h.width(0);
            siblingContainers.push(el);
        });

        for (let i = 0; i < siblingContainers.length; i++) {
            const thisCommandbtn: any = siblingContainers[i];
            const target: string = hidden ? thisCommandbtn.width + "px" : "0px";
            thisCommandbtn.h.animate({ width: target });
        }
    }
    /****************************************************************************
     * public functions exposing addRipple() and makeRequest() to outside of the closure
    ****************************************************************************/
    return {
        'addRipple': addRipple,
        'makeRequestUsingFormAction': makeRequestUsingFormAction,
        'slideCommands': slideCommands
    }
})();
/****************************** end of closure *******************************/

$(function () {
    $('.rehabAction').each(function () {
        const thisRehabCommandButton = $(this);
        //call closure
        //commandBtnController.addRipple(thisRehabCommandButton);

        thisRehabCommandButton.on('click', function () {
            //call closure
            //commandBtnController.makeRequest(thisRehabCommandButton);
            commandBtnController.makeRequestUsingFormAction(thisRehabCommandButton);
        })
    });

    $('.commandTrigger').on('click', function () {
        const thisCommandTrigger: any = $(this);
        const thisContainer: any = thisCommandTrigger.parent(); //should be <div class="mdc-touch-target-wrapper">

        const hidden: boolean = thisCommandTrigger.data('hidden');
        commandBtnController.slideCommands(thisContainer, hidden);
        thisCommandTrigger.data('hidden', !hidden);
        if (hidden)
            thisCommandTrigger.prop('title', 'Show Commands');
        else
            thisCommandTrigger.prop('title', 'Hide Commands');
    })
});

