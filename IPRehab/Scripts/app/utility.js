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
                }
            ]
        };
        return dialogOptions;
    }
    isDate(aDate) {
        //throw new Error("Method not implemented.");
        return aDate instanceof Date && !isNaN(aDate.valueOf());
    }
    isEmpty($this) {
        //throw new Error("Method not implemented.");
        if (typeof $this.val() === 'undefined' || $this.val() === '' || $this.val() === undefined || $this.val() === null) {
            return true;
        }
        else {
            return false;
        }
    }
    getCRUD($thisPersistable, oldAnswer, newAnswer) {
        const thisControlType = $thisPersistable.prop('type');
        const noOld = +oldAnswer <= 0 || oldAnswer === undefined || oldAnswer === null || oldAnswer === '';
        const hadOld = +oldAnswer > 0;
        const noNew = +newAnswer <= 0 || newAnswer === undefined || newAnswer === null || newAnswer === '';
        const hasNew = +newAnswer > 0;
        ;
        console.log('CRUD for ' + $thisPersistable.prop('id') + '(' + thisControlType + ')');
        switch (thisControlType) {
            case "radio":
            case "checkbox": {
                const isChecked = $thisPersistable.is(':checked');
                if (hadOld && hasNew) {
                    if (+oldAnswer !== +newAnswer) {
                        if (isChecked) {
                            console.log('different old answer (' + oldAnswer + ') and new answer (' + newAnswer + ') CHECKED', EnumDbCommandType[EnumDbCommandType.Update]);
                            return EnumDbCommandType.Update;
                        }
                        if (!isChecked) {
                            console.log('different old answer (' + oldAnswer + ') and new answer (' + newAnswer + ') UNCHECKED', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                            return EnumDbCommandType.Unchanged;
                        }
                    }
                    if (+oldAnswer === +newAnswer) {
                        if (isChecked) {
                            console.log('same old answer (' + oldAnswer + ') and new checkbox answer (' + newAnswer + ') CHECKED', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                            return EnumDbCommandType.Unchanged;
                        }
                        if (!isChecked) {
                            if (thisControlType === "checkbox") {
                                console.log('same old answer (' + oldAnswer + ') and new checkbox answer (' + newAnswer + ') UNCHECKED', EnumDbCommandType[EnumDbCommandType.Delete]);
                                return EnumDbCommandType.Delete;
                            }
                            if (thisControlType === "radio") {
                                console.log('same old answer (' + oldAnswer + ') and new radio answer (' + newAnswer + ') UNCHECKED', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                                return EnumDbCommandType.Unchanged; //do not delete so that the mutually exexclusive radio will update using the same answer ID
                            }
                        }
                    }
                }
                if (noOld && noNew) {
                    console.log('no old answer and no new answer', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                    return EnumDbCommandType.Unchanged; //do nothing
                }
                if (noOld && hasNew) {
                    if (isChecked) {
                        console.log('no old answer ' + ' but new CHECKED answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Create]);
                        return EnumDbCommandType.Create; //insert new answer
                    }
                    if (!isChecked) {
                        console.log('no old answer and new but UNCHECKED answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                        return EnumDbCommandType.Unchanged; //do nothing
                    }
                }
                if (hadOld && noNew) {
                    console.log('old answer (' + oldAnswer + ') but new answer is blank', EnumDbCommandType.Delete);
                    return EnumDbCommandType.Delete; //delete old answer  
                }
                console.log('other');
                return EnumDbCommandType.Unchanged;
            }
            case "select-one": {
                if (hadOld && hasNew) {
                    if (+newAnswer === +oldAnswer) {
                        console.log('same old answer (' + oldAnswer + ') and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                        return EnumDbCommandType.Unchanged;
                    }
                    if (+newAnswer !== +oldAnswer) {
                        console.log('different old answer (' + oldAnswer + ') and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Update]);
                        return EnumDbCommandType.Update;
                    }
                }
                if (noOld && noNew) {
                    console.log('no old answer and no new answer', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                    return EnumDbCommandType.Unchanged;
                }
                if (noOld && hasNew) {
                    console.log('no old answer and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Create]);
                    return EnumDbCommandType.Create;
                }
                if (hadOld && noNew) {
                    console.log('old answer (' + oldAnswer + ') and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Delete]);
                    return EnumDbCommandType.Delete;
                }
                console.log('other');
                return EnumDbCommandType.Unchanged;
            }
            case "date": {
                const newDateString = newAnswer;
                const oldDateString = oldAnswer;
                let newConvertedDate, oldConvertedDate;
                if (hadOld && hasNew) {
                    //compare date type
                    newConvertedDate = new Date(newDateString);
                    oldConvertedDate = new Date(oldDateString);
                    if (newConvertedDate !== oldConvertedDate) {
                        console.log('different old date (' + oldConvertedDate + 'and new date (' + newConvertedDate + ')', EnumDbCommandType[EnumDbCommandType.Update]);
                        return EnumDbCommandType.Update;
                    }
                    else {
                        console.log('same old date (' + oldConvertedDate + ') and new date (' + newConvertedDate + ')', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                        return EnumDbCommandType.Unchanged;
                    }
                }
                //compare date string
                if (noOld && noNew) {
                    console.log('both old date string and new date string are empty', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                    return EnumDbCommandType.Unchanged;
                }
                if (noOld && hasNew) {
                    console.log('no old date string but has new date string (' + newDateString + ')', EnumDbCommandType[EnumDbCommandType.Create]);
                    return EnumDbCommandType.Create;
                }
                if (hadOld && noNew) {
                    console.log('old date string (' + oldDateString + ') but no new date string', EnumDbCommandType[EnumDbCommandType.Delete]);
                    return EnumDbCommandType.Delete;
                }
                console.log('other');
                return EnumDbCommandType.Unchanged;
            }
            case 'number': {
                if (hadOld && hasNew) {
                    if (+newAnswer !== +oldAnswer) {
                        console.log('different old answer (' + oldAnswer + ') and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Update]);
                        return EnumDbCommandType.Update;
                    }
                    else {
                        console.log('same old answer (' + oldAnswer + ') and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                        return EnumDbCommandType.Unchanged;
                    }
                }
                if (noOld && noNew) {
                    console.log('no old answer and no new answer', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                    return EnumDbCommandType.Unchanged;
                }
                if (noOld && hasNew) {
                    console.log('no old answer and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Create]);
                    return EnumDbCommandType.Create;
                }
                if (hadOld && noNew) {
                    console.log('old answer (' + oldAnswer + ') no new answer', EnumDbCommandType[EnumDbCommandType.Delete]);
                    return EnumDbCommandType.Delete;
                }
                console.log('other');
                return EnumDbCommandType.Unchanged;
            }
            //case "text":
            //case "textarea":
            default: {
                if (hadOld && hasNew) {
                    if (oldAnswer === newAnswer) {
                        console.log('same old answer (' + oldAnswer + ') and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                        return EnumDbCommandType.Unchanged;
                    }
                    if (oldAnswer !== newAnswer) {
                        console.log('different old answer (' + oldAnswer + ') and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Update]);
                        return EnumDbCommandType.Update;
                    }
                }
                if (noOld && noNew) {
                    console.log('no old answer and no new answer', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                    return EnumDbCommandType.Unchanged;
                }
                if (noOld && hasNew) {
                    console.log('no old answer and has new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Unchanged]);
                    return EnumDbCommandType.Unchanged;
                }
                if (hadOld && noNew) {
                    console.log('old answer (' + oldAnswer + ') and new answer (' + newAnswer + ')', EnumDbCommandType[EnumDbCommandType.Delete]);
                    return EnumDbCommandType.Delete;
                }
                console.log('other');
                return EnumDbCommandType.Unchanged;
            }
        }
    }
    getControlCurrentValue($thisControl) {
        //throw new Error("Method not implemented.");
        const thisControlType = $thisControl.prop('type');
        const thisControlID = $thisControl.prop('id');
        let thisValue = "";
        switch (thisControlType) {
            /* these type of control values are populated from answer description column in the WebAPI, so just get .val() */
            case 'text':
            case 'date':
            case 'textarea':
            case 'number':
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
    getControlScore(thisControl) {
        //throw new Error("Method not implemented.");
        const thisControlType = thisControl.prop('type');
        const thisControlID = thisControl.prop('id');
        let thisScore = "0";
        let hasDataScore;
        switch (thisControlType) {
            case "radio":
            case "checkbox": {
                //radio and checkbox val() returns the value regardless checked or not so use prop('checked') to ensure the checked value
                hasDataScore = $('[data-score]', thisControl).length > 0;
                if (hasDataScore && thisControl.is(':checked'))
                    thisScore = thisControl.data('score');
                break;
            }
            case "select-one":
                hasDataScore = $('option:selected[data-score]', thisControl).length > 0;
                if (hasDataScore)
                    thisScore = $('option:selected[data-score]', thisControl).data('score');
                break;
            default:
                hasDataScore = $('[data-score]', thisControl).length > 0;
                if (hasDataScore)
                    thisScore = thisControl.data('score');
                break;
        }
        console.log('(' + thisControlType + ') ' + thisControlID + ' , score =' + thisScore);
        return thisScore;
    }
    resetControlValue($thisControl, newValue = '-1') {
        //throw new Error("Method not implemented.");
        //console.log('$thisControl', $thisControl);
        const thisControlType = $thisControl.prop('type');
        switch (thisControlType) {
            case "select-one": {
                const newValueInt = parseInt(newValue);
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
    //public scrollTo(thisElement: any) {
    //    let scrollAmount: number = thisElement.prop('offsetTop') + 15;
    //    if (thisElement.prop('id').indexOf('Q12') !== -1) scrollAmount = 0; //scroll up further by 15
    //    console.log('scroll to ' + thisElement.prop('id') + ', amount ' + scrollAmount, thisElement);
    //    $('html,body').animate({ scrollTop: scrollAmount }, 'fast');
    //    thisElement.trigger('focus');
    //}
    scrollTo(elementId) {
        const element = document.querySelector('#' + elementId);
        // Get the size and position of our element in the viewport
        const rect = element.getBoundingClientRect();
        // The top offset of our element is the top position of the element in the viewport plus the amount the body is scrolled
        let offsetTop = rect.top + document.body.scrollTop;
        console.log('scroll to ' + elementId + ' with amount ' + offsetTop);
        // Now we can scroll the window to this position
        window.scrollTo(0, offsetTop - 15);
        document.getElementById(elementId).focus(); /* use focus() because this is not a jquery element */
    }
}
//# sourceMappingURL=utility.js.map