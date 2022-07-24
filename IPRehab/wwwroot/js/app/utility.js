/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
var EnumGetControlValueBehavior;
(function (EnumGetControlValueBehavior) {
    EnumGetControlValueBehavior[EnumGetControlValueBehavior["Elaborated"] = 0] = "Elaborated";
    EnumGetControlValueBehavior[EnumGetControlValueBehavior["Simple"] = 1] = "Simple";
})(EnumGetControlValueBehavior || (EnumGetControlValueBehavior = {}));
export class Utility {
    dialogOptions() {
        const dialogOptions = {
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
        return dialogOptions;
    }
    isDate(aDate) {
        //throw new Error("Method not implemented.");
        return aDate instanceof Date && !isNaN(aDate.valueOf());
    }
    isEmpty($this) {
        //throw new Error("Method not implemented.");
        if (typeof $this.val() !== 'undefined' && $this.val())
            return false;
        else
            return true;
    }
    isTheSame($this, oldValue, currentValue) {
        //throw new Error("Method not implemented.");
        const controlType = $this.prop('type');
        const controlID = $this.prop('id');
        let equalMsg = '';
        //!undefined or !NaN yield true
        switch (controlType) {
            case "radio":
            case "checkbox":
                if (currentValue === oldValue && $this.prop('checked')) {
                    equalMsg = controlType + ' ' + controlID + ' checked equal';
                }
                if (currentValue !== oldValue && !$this.prop('checked')) {
                    equalMsg = controlType + ' ' + controlID + ' unchecked unequal';
                }
                break;
            default:
                if (!currentValue && !oldValue && +currentValue === +oldValue) {
                    equalMsg = controlType + ' ' + controlID + 'both values are blank';
                }
                if (currentValue === oldValue || +currentValue === +oldValue) {
                    equalMsg = ' ' + controlID + 'both non-blank values are equal';
                }
                if (currentValue === undefined && oldValue === undefined) {
                    equalMsg = controlType + ' ' + controlID + 'both are undefined';
                }
                break;
        }
        if (controlID.indexOf('K0520B') !== -1 && equalMsg !== '')
            console.log(equalMsg);
        if (equalMsg !== '') {
            return true;
        }
        else {
            return false;
        }
    }
    getCRUD($this, oldValue, currentValue) {
        const controlType = $this.prop('type');
        const checked = $this.prop('checked');
        switch (true) {
            case (currentValue !== oldValue && +oldValue === 0 && controlType === 'number'):
                console.log('(C)reate current value = ' + currentValue + ' because old value 0 is blank equivalent');
                return 'C';
            case (currentValue && !oldValue):
                console.log('(C)reate current value = ' + currentValue + ' because old value = blank');
                return 'C';
            case (oldValue && !currentValue):
                console.log('(D)elete old value = ' + oldValue + ' because current value = blank');
                return 'D1';
            case (oldValue === currentValue && controlType === 'checkbox' && !checked):
                console.log('(D)elete old value = ' + oldValue + ', current value = ' + currentValue + ' but unchecked');
                return 'D2';
            case ((currentValue && oldValue) && (currentValue !== oldValue)):
                console.log('(U)pdate old value = ' + oldValue + ' because new value = ' + currentValue);
                return 'U';
                break;
        }
    }
    getControlValue($thisControl, behavior = EnumGetControlValueBehavior.Elaborated /*use other if no valueSource */) {
        //throw new Error("Method not implemented.");
        const thisControlType = $thisControl.prop('type');
        let thisValue;
        if (behavior !== EnumGetControlValueBehavior.Elaborated) {
            thisValue = $thisControl.val();
        }
        else {
            switch (thisControlType) {
                case "select-one": {
                    //use the selected option text and parse the starting text to int
                    const selectedOption = $('#' + $thisControl.prop('id') + ' option:selected').text();
                    //console.log('selected option text = ', selectedOption);
                    thisValue = parseInt(selectedOption);
                    if (isNaN(thisValue))
                        thisValue = 0;
                    break;
                }
                case "radio":
                case "checkbox":
                    if ($thisControl.prop('checked')) {
                        //console.log(behavior + ' get control value for ' + $thisControl.prop('id') + ' prop("checked") = ', $thisControl.prop('checked'));
                        thisValue = 1;
                    }
                    break;
                case "text": {
                    const numberString = parseInt($thisControl.val());
                    if (!isNaN(numberString)) {
                        //console.log(behavior + ' get control value for ' + $thisControl.prop('id') + ' = ' + $thisControl.val());
                        thisValue = $thisControl.val();
                    }
                    else
                        thisValue = numberString;
                    break;
                }
                default: {
                    //console.log(behavior + ' get control value for ' + $thisControl.prop('id') + ' = ', $thisControl.val());
                    thisValue = $thisControl.val();
                    break;
                }
            }
        }
        console.log('(' + thisControlType + ') ' + $thisControl.prop('id') + ' = ' + thisValue + ' (with ' + EnumGetControlValueBehavior[behavior] + ' behavior)');
        return thisValue;
    }
    resetControlValue($thisControl, newValue = '-1') {
        //throw new Error("Method not implemented.");
        //console.log('$thisControl', $thisControl);
        const thisControlType = $thisControl.prop('type');
        console.log('resetting ' + thisControlType + ' control type ' + $thisControl.prop('id'));
        switch (thisControlType) {
            case "select-one": {
                const newValueInt = parseInt(newValue);
                if (isNaN(newValueInt)) {
                    $thisControl.val(-1).change();
                }
                else {
                    $thisControl.val(newValue).change();
                }
                console.log('changed ' + thisControlType + ' ' + $thisControl.prop('id') + ' value to ' + newValueInt);
                break;
            }
            case "checkbox":
            case "radio": {
                $thisControl.prop('checked', false).change();
                console.log('unchecked ' + thisControlType + $thisControl.prop('id'));
                break;
            }
            case "text":
            case "date": {
                $thisControl.val('').change();
                console.log('cleared ' + $thisControl.prop('id') + ' ' + thisControlType);
                break;
            }
            default:
                console.log('unknown ' + thisControlType + 'control type:  for ' + $thisControl.prop('id)'));
                break;
        }
    }
    getTextPixels(someText, font) {
        //throw new Error("Method not implemented.");
        const canvas = document.createElement('canvas');
        const context = canvas.getContext("2d");
        context.font = font;
        const width = context.measureText(someText).width;
        return Math.ceil(width);
    }
    breakLongSentence(thisSelectElement) {
        throw new Error("Method not implemented.");
        //  console.log('thisSelectElement', thisSelectElement);
        //  let maxLength: number = 50;
        //  let longTextOptionDIV = thisSelectElement.next('div.longTextOption');
        //  console.log('longTextOptionDIV', longTextOptionDIV);
        //  let thisSelectWidth = thisSelectElement[0].clientWidth;
        //  let thisScope: any = thisSelectElement;
        //  let selectedValue: number = parseInt(thisSelectElement.prop('value'));
        //  if (selectedValue <= 0) {
        //    longTextOptionDIV.text('');
        //  }
        //  else {
        //    $.each($('option:selected', thisScope), function () {
        //      let $thisOption = $(this);
        //      let regX = new RegExp("([\\w\\s]{" + (maxLength - 2) + ",}?\\w)\\s?\\b", "g")
        //      let oldText: string = $thisOption.text();
        //      let font = $thisOption.css('font');
        //      let oldTextInPixel = getTextPixels(oldText, font);
        //      console.log('oldTextInPixel', oldTextInPixel);
        //      console.log('thisSelectWidth', thisSelectWidth);
        //      longTextOptionDIV.text('');
        //      if (oldTextInPixel > thisSelectWidth) {
        //        let newStr = oldText.replace(regX, "$1\n");
        //        newStr = newStr.trim();
        //        let startWithNumber = $.isNumeric(newStr.substring(0, 1));
        //        if (startWithNumber) {
        //          newStr = newStr.substring(newStr.indexOf(" ") + 1);
        //        }
        //        console.log('old ->', oldText);
        //        console.log('new ->', newStr);
        //        longTextOptionDIV.text(newStr);
        //        longTextOptionDIV.removeClass("invisible");
        //      }
        //    });
        //  }
    }
    scrollTo(thisElement) {
        let scrollAmount = thisElement.prop('offsetTop') + 15;
        if (thisElement.prop('id').indexOf('Q12') !== -1)
            scrollAmount = 0; //scroll up further by 15
        console.log('scroll to ' + thisElement.prop('id') + ', amount ' + scrollAmount, thisElement);
        $('html,body').animate({ scrollTop: scrollAmount }, 'fast');
        thisElement.focus();
    }
}
//# sourceMappingURL=utility.js.map