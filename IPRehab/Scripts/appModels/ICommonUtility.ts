export interface ICommonUtility {
  isDate(aDate: Date): boolean;
  isEmpty($this: any): boolean;
  isTheSame($this: any, oldValue: string, currentValue: string): boolean;
  getCRUD($this: any, oldValue: string, currentValue: string): string;
  getControlValue($this: any): number;
  resetControlValue($this: any, newValue: string);
  getTextPixels(someText: string, font: any);
  breakLongSentence(thisSelectElement);
  scrollTo(anElement: any);
}