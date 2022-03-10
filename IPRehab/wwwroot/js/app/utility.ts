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

  getControlValue($this: any, valueSource: string = 'other'): any {
    //throw new Error("Method not implemented.");
    let thisControlType: string = $this.prop('type');
    let thisValue: any;
    switch (valueSource) {
      case "other": {
        switch (thisControlType) {
          case "select-one": {
            //true score is the selected option text because it starts with 1 to 6, 7, 9, 10 and 88
            let selectedOption: string = $('#' + $this.prop('id') + ' option:selected').text();
            thisValue = parseInt(selectedOption);
            break;
          }

          case "radio":
          case "checkbox":
            if ($this.prop('checked'))
              thisValue = 1;
            break;

          case "text": {
            let numberString: number = parseInt($this.val());
            if (!isNaN(numberString))
              thisValue = $this.val();
            else
              thisValue = numberString;
            break;
          }

          default: {
            thisValue = $this.val();
            break;
          }
        }
        break;
      }
      default: {
        if ((thisControlType == 'checkbox' || thisControlType == 'radio') && $this.prop('checked')) {
          thisValue = $this.val();
        }
        else {
          thisValue = $this.val();
        }
        break;
      }
    }
    return thisValue;
  }

  resetControlValue($thisControl: any, newValue: string) {
    //throw new Error("Method not implemented.");
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