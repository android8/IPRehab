import { EnumDbCommandType } from "../app/commonImport.js";
export interface ICommonUtility {
    dialogOptions(): any;
    isDate(aDate: Date): boolean;
    isEmpty($this: any): boolean;
    getCRUD($this: any, oldValue: string, currentValue: string): EnumDbCommandType;
    getControlCurrentValue($this: any): string;
    getControlScore(thisControl)
    resetControlValue($this: any, newValue: string);
    getTextPixels(someText: string, font: any);
    breakLongSentence(thisSelectElement);
    scrollTo(anElement: any);
}