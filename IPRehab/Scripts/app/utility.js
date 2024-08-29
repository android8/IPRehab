import { EnumDbCommandType } from "./commonImport.js";
//enum EnumGetControlValueBehavior {
//    Elaborated,
//    Simple
//}
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
        const thisControlType = $this.prop('type');
        const thisControlID = $this.prop('id');
        let isChanged = false;
        let equalMsg = 'unchanged';
        //!undefined or !NaN yield true
        if (currentValue !== oldValue) { //compare string
            isChanged = true;
            equalMsg = 'changed';
        }
        equalMsg = thisControlType + ' ' + thisControlID + ' ' + equalMsg;
        console.log(equalMsg);
        return isChanged;
    }
    getCRUD($this, oldValue, currentValue) {
        const controlType = $this.prop('type');
        const checked = $this.prop('checked');
        switch (true) {
            case (currentValue && !oldValue):
                console.log('(C)reate. current value = ' + currentValue + ', old value = ' + oldValue);
                return EnumDbCommandType.Create;
            case (oldValue && !currentValue):
                console.log('(D)elete. current value = ' + currentValue + ', old value = ' + oldValue);
                return EnumDbCommandType.Delete;
            case ((currentValue && oldValue) && (currentValue !== oldValue)):
                console.log('(U)pdate. current value = ' + currentValue + ', old value = ' + oldValue);
                return EnumDbCommandType.Update;
        }
    }
    getControlValue($thisControl) {
        //throw new Error("Method not implemented.");
        const thisControlType = $thisControl.prop('type');
        const thisControlID = $thisControl.prop('id');
        let thisValue = "";
        let thisScore = "";
        switch (thisControlType) {
            case "radio":
            case "checkbox": {
                let selectedOption = $('#' + thisControlID + " :selected");
                //radio and checkbox val() returns the value regardless checked or not so use prop('checked') to ensure the checked value
                //if ($thisControl.prop('checked'))   
                thisScore = selectedOption.data("score");
                thisValue = $thisControl.val();
                break;
            }
            default:
                thisValue = $thisControl.val();
                thisScore = thisValue;
                break;
        }
        console.log('(' + thisControlType + ') ' + thisControlID + ' value = ' + thisValue + ', score =' + thisScore);
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
                    $thisControl.val(-1).trigger('change');
                }
                else {
                    $thisControl.val(newValue).trigger('change');
                }
                console.log('changed ' + thisControlType + ' ' + $thisControl.prop('id') + ' value to ' + newValueInt);
                break;
            }
            case "checkbox":
            case "radio": {
                $thisControl.prop('checked', false).trigger('change');
                console.log('unchecked ' + thisControlType + $thisControl.prop('id'));
                break;
            }
            case "text":
            case "date": {
                $thisControl.val('').trigger('change');
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
        thisElement.trigger('focus');
    }
}
//# sourceMappingURL=utility.js.map