import { equal } from "assert";
import { ICommonUtility, EnumChangeEventArg, EnumDbCommandType } from "./commonImport.js";

//enum EnumGetControlValueBehavior {
//    Elaborated,
//    Simple
//}


export class Utility implements ICommonUtility {

    public dialogOptions() {
        const dialogOptions: any = {
            resizable: true,
            //height: ($(window).height() - 200),
            //width: '90%',
            classes: { 'ui-dialog': 'my-dialog', 'ui-dialog-titlebar': 'my-dialog-header' },
            modal: true,
            stack: true,
            sticky: true,
            position: { my: 'center', at: 'center', of: window },
            buttons: [
                {
                    text: "Close",
                    icon: "ui-icon-close",
                    click: function () {
                        $(this).dialog("close");
                    },
                    open: function (event, ui) {
                        $('.ui-widget-overlay').css({
                            'opacity': '0.5',
                            'filter': 'Alpha(Opacity = 50)',
                            'background-color': 'black'
                        });
                    },
                }]
        };
        return dialogOptions;
    }

    public isDate(aDate: Date): boolean {
        //throw new Error("Method not implemented.");
        return aDate instanceof Date && !isNaN(aDate.valueOf());
    }

    public isEmpty($this): boolean {
        //throw new Error("Method not implemented.");
        if (typeof $this.val() === 'undefined' || $this.val() === '' || $this.val() === undefined || $this.val() === null) {
            console.log($this.prop('id') + ' is empty');
            return true;
        }
        else {
            console.log($this.prop('id') + ' is not empty');
            return false;
        }
    }

    public isTheSame($thisPersistable, oldValue: string, currentValue: string): boolean {
        //throw new Error("Method not implemented.");
        const thisControlType: string = $thisPersistable.prop('type');
        const thisControlID: string = $thisPersistable.prop('id');
        let isChanged: boolean = false;
        let equalMsg = 'unchanged';

        //!undefined or !NaN yield true
        switch (thisControlType) {
            case "radio":
            case "checkbox":
                {
                    const isChecked: boolean = $thisPersistable.is(':checked');
                    switch (true) {
                        case currentValue == oldValue && !isChecked:
                        case currentValue != oldValue && isChecked:
                            isChanged = true;
                            equalMsg = 'changed';
                            break;
                        case currentValue == oldValue && isChecked:
                        case currentValue != oldValue && !isChecked:
                            isChanged = false;
                            break;
                    }
                }
                break;
        }

        equalMsg = thisControlType + ' ' + thisControlID + ' ' + equalMsg;
        console.log(equalMsg);

        return isChanged;
    }

    public getCRUD($thisPersistable: any, oldValue: string, currentValue: string): EnumDbCommandType {
        const thisControlType: string = $thisPersistable.prop('type');
        const oldValueIsEmpty: boolean = oldValue === undefined || oldValue === null || oldValue === '';
        switch (thisControlType) {
            case "radio":
            case "checkbox":
                const isChecked: boolean = $thisPersistable.is(':checked');
                switch (true) {
                    case currentValue === oldValue && !oldValueIsEmpty && !isChecked:
                        return EnumDbCommandType.Update;
                    case currentValue !== oldValue && isChecked: {
                        if (oldValueIsEmpty)
                            return EnumDbCommandType.Create;
                        else
                            return EnumDbCommandType.Update;
                    }
                }
                break;
            case "select-one":
                if (currentValue !== oldValue) {
                    if (oldValueIsEmpty) {
                        if (currentValue !== '-1')
                            return EnumDbCommandType.Create;
                    }
                    else {
                        if (currentValue !== '-1')
                            return EnumDbCommandType.Delete;
                        else
                            return EnumDbCommandType.Update;
                    }
                }
                break;
            case "text":
            case "date":
            case "textarea":
            case 'number':
                if (currentValue !== oldValue) {
                    if (oldValueIsEmpty)
                        return EnumDbCommandType.Create;
                    else {
                        if (currentValue === '')
                            return EnumDbCommandType.Delete;
                        else
                            return EnumDbCommandType.Update;
                    }
                }
                break;
            default:
                {
                    if (oldValue != currentValue) {
                        if (oldValue === null || oldValue === null)
                            return EnumDbCommandType.Create;
                        else
                            return EnumDbCommandType.Update;
                    }
                }
        }
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

    public getControlCurrentValue($thisControl) {
        //throw new Error("Method not implemented.");
        const thisControlType: string = $thisControl.prop('type');
        const thisControlID: string = $thisControl.prop('id');

        let thisValue: string = "";

        switch (thisControlType) {
            case "radio":
            case "checkbox":
                thisValue = $thisControl.val();
                break;
            default:
                thisValue = $thisControl.val();
                break;
        }

        console.log('(' + thisControlType + ') ' + thisControlID + ' value = ' + thisValue);
        return thisValue;
    }

    //when the controls, such as GG0170s and GG0130s, could have binding value and data-score are different
    public getControlScore(thisControl) {
        //throw new Error("Method not implemented.");
        const thisControlType: string = thisControl.prop('type');
        const thisControlID: string = thisControl.prop('id');

        let thisScore: string = "0";
        let hasDataScore: boolean;

        switch (thisControlType) {
            case "radio":
            case "checkbox": {
                //radio and checkbox val() returns the value regardless checked or not so use prop('checked') to ensure the checked value
                hasDataScore = $('[data-score]', thisControl) !== undefined;
                if (hasDataScore && thisControl.is(':checked'))
                    thisScore = thisControl.data('score');
                break;
            }
            case "select-one":
                hasDataScore = $('option:selected[data-score]', thisControl) !== undefined;
                if (hasDataScore)
                thisScore = $('option:selected[data-score]', thisControl).data('score');
                break;
            default:
                hasDataScore = $('[data-score]', thisControl).length !== 0;
                if (hasDataScore)
                    thisScore = thisControl.data('score');
                break;
        }

        console.log('(' + thisControlType + ') ' + thisControlID + ' , score =' + thisScore);
        return thisScore;
    }

    public resetControlValue($thisControl, newValue: string = '-1') {
        //throw new Error("Method not implemented.");
        //console.log('$thisControl', $thisControl);
        const thisControlType: string = $thisControl.prop('type');
        switch (thisControlType) {
            case "select-one": {
                const newValueInt: number = parseInt(newValue);
                if (isNaN(newValueInt)) {
                    $thisControl.val(-1);
                }
                else {
                    $thisControl.val(newValue);
                }
                console.log('changed ' + thisControlType + ' ' + $thisControl.prop('id') + ' value to ' + newValueInt);
                break;
            }
            case "checkbox":
            case "radio": {
                $thisControl.prop('checked', false);
                console.log('unchecked ' + thisControlType + $thisControl.prop('id'));
                break;
            }
            case "text":
            case "date": {
                $thisControl.val('');
                console.log('cleared ' + $thisControl.prop('id') + ' ' + thisControlType);
                break;
            }
            default:
                console.log('unknown ' + thisControlType + 'control type:  for ' + $thisControl.prop('id)'));
                break;
        }
    }

    public getTextPixels(someText: string, font) {
        //throw new Error("Method not implemented.");
        const canvas = document.createElement('canvas');
        const context = canvas.getContext("2d");
        context.font = font;
        const width = context.measureText(someText).width;
        return Math.ceil(width);
    }

    public breakLongSentence(thisSelectElement) {
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

    public scrollTo(thisElement: any) {
        let scrollAmount: number = thisElement.prop('offsetTop') + 15;
        if (thisElement.prop('id').indexOf('Q12') !== -1) scrollAmount = 0; //scroll up further by 15
        console.log('scroll to ' + thisElement.prop('id') + ', amount ' + scrollAmount, thisElement);
        $('html,body').animate({ scrollTop: scrollAmount }, 'fast');
        thisElement.trigger('focus');
    }
}