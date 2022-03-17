import { ICommonUtility } from "../appModels/ICommonUtility.js";

export class Utility implements ICommonUtility {
  isDate(aDate: Date): boolean {
    //throw new Error("Method not implemented.");
    return aDate instanceof Date && !isNaN(aDate.valueOf());
  }

  isEmpty($this: any): boolean {
    //throw new Error("Method not implemented.");
    if (typeof $this.val() !== 'undefined' && $this.val())
      return false;
    else
      return true;
  }

  isSameAnswer($this: any, oldAnswer: string, newAnswer: string): string {
    //throw new Error("Method not implemented.");
    const controlType = $this.prop('type');
    let rtnMsg: string = '';
    //!undefined or !NaN yield true
    if (+newAnswer <= 0)
      newAnswer = '';

    if ((controlType == 'radio' || controlType == 'checkbox') &&
        (newAnswer === oldAnswer && $this.prop('checked'))) {
      rtnMsg= 'radio/checkbox checked equal';
    }
    if ((controlType == 'radio' || controlType == 'checkbox') &&
        (newAnswer !== oldAnswer && !$this.prop('checked'))) {
      rtnMsg = 'radio/checkbox unchecked unequal';
    }
    if (!newAnswer && !oldAnswer && +newAnswer === +oldAnswer) {
      rtnMsg= 'both values are blank';
    }
    if (newAnswer === oldAnswer || +newAnswer === +oldAnswer) {
      rtnMsg= 'both non-blank values are equal';
    }
    if (newAnswer === undefined && oldAnswer === undefined) {
      rtnMsg= 'both are undefined';
    }
    return rtnMsg;
  }

  getControlValue($thisControl: any, valueSource: string = 'other'): any {
    //throw new Error("Method not implemented.");
    //console.log('$thisControl', $thisControl);
    let thisControlType: string = $thisControl.prop('type');
    let thisValue: any;
    switch (valueSource) {
      case "other": {
        switch (thisControlType) {
          case "select-one": {
            //true score is the selected option text because it starts with 1 to 6, 7, 9, 10 and 88
            let selectedOption: string = $('#' + $thisControl.prop('id') + ' option:selected').text();
            thisValue = parseInt(selectedOption);
            break;
          }

          case "radio":
          case "checkbox":
            if ($thisControl.prop('checked'))
              thisValue = 1;
            break;

          case "text": {
            let numberString: number = parseInt($thisControl.val());
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

  resetControlValue($thisControl: any, newValue: string) {
    //throw new Error("Method not implemented.");
    //console.log('$thisControl', $thisControl);
    let thisControlType: string = $thisControl.prop('type');
    switch (thisControlType) {
      case "select-one": {
        let newValueInt: number = parseInt(newValue);
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

  getTextPixels(someText: string, font: any) {
    //throw new Error("Method not implemented.");
    let canvas = document.createElement('canvas');
    let context = canvas.getContext("2d");
    context.font = font;
    let width = context.measureText(someText).width;
    return Math.ceil(width);
  }

  breakLongSentence(thisSelectElement: any) {
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