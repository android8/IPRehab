export interface ICommonUtility {
  isDate(aDate: Date): boolean;
  isEmpty($this: any): boolean;
  isSameAnswer($this: any, oldAnswer: string, newAnswer: string): string;
  getControlValue($this: any): number;
  resetControlValue($this: any, newValue: string)
  getTextPixels(someText: string, font: any);
  breakLongSentence(thisSelectElement);
}