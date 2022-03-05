export interface ICommonUtility {
  isDate(aDate: Date): boolean;
  isEmpty($this: any): boolean;
  getControlValue($this: any): number;
  resetControlValue($this: any, newValue: string)
  getTextPixels(someText: string, font: any);
  breakLongSentence(thisSelectElement);
}