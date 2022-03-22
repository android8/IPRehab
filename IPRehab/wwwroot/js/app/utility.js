/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
export class Utility {
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
        if (controlID.indexOf('K0520B') != -1 && equalMsg != '')
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
            case (currentValue && !oldValue):
                console.log('(C)reate current value = ' + currentValue + ' because old value = blank');
                return 'C';
            case (oldValue && !currentValue):
                console.log('(D)elete old value = ' + oldValue + ' because current value = blank');
                return 'D1';
            case (oldValue == currentValue && controlType == 'checkbox' && !checked):
                console.log('(D)elete old value = ' + oldValue + ', current value = ' + currentValue + ' but unchecked');
                return 'D2';
            case ((currentValue && oldValue) && (currentValue !== oldValue)):
                console.log('(U)pdate old value = ' + oldValue + ' because new value = ' + currentValue);
                return 'U';
                break;
        }
    }
    getControlValue($thisControl, valueSource = 'other') {
        //throw new Error("Method not implemented.");
        //console.log('$thisControl', $thisControl);
        let thisControlType = $thisControl.prop('type');
        let thisValue;
        switch (valueSource) {
            case "other": {
                switch (thisControlType) {
                    case "select-one": {
                        //true score is the selected option text because it starts with 1 to 6, 7, 9, 10 and 88
                        let selectedOption = $('#' + $thisControl.prop('id') + ' option:selected').text();
                        thisValue = parseInt(selectedOption);
                        break;
                    }
                    case "radio":
                    case "checkbox":
                        if ($thisControl.prop('checked'))
                            thisValue = 1;
                        break;
                    case "text": {
                        let numberString = parseInt($thisControl.val());
                        if (!isNaN(numberString))
                            thisValue = $thisControl.val();
                        else
                            thisValue = numberString;
                        break;
                    }
                    default: {
                        thisValue = $thisControl.val();
                        break;
                    }
                }
                break;
            }
            default: {
                if ((thisControlType == 'checkbox' || thisControlType == 'radio') && $thisControl.prop('checked')) {
                    thisValue = $thisControl.val();
                }
                else {
                    thisValue = $thisControl.val();
                }
                break;
            }
        }
        return thisValue;
    }
    resetControlValue($thisControl, newValue) {
        //throw new Error("Method not implemented.");
        //console.log('$thisControl', $thisControl);
        let thisControlType = $thisControl.prop('type');
        switch (thisControlType) {
            case "select-one": {
                let newValueInt = parseInt(newValue);
                if (isNaN(newValueInt))
                    $thisControl.val(-1).change();
                else
                    $thisControl.val(newValue);
                break;
            }
            case "checkbox":
            case "radio": {
                $thisControl.prop('checked', false);
                break;
            }
            case "text": {
                $thisControl.val('');
                break;
            }
        }
    }
    getTextPixels(someText, font) {
        //throw new Error("Method not implemented.");
        let canvas = document.createElement('canvas');
        let context = canvas.getContext("2d");
        context.font = font;
        let width = context.measureText(someText).width;
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
}
//# sourceMappingURL=utility.js.map