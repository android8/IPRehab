export interface IUserAnswer {

  PatientName: string;
  PatientID: string;
  AnswerID: number;
  EpisodeID: number;
  OnsetDate: Date;
  AdmissionDate: Date;
  QuestionID: number;
  QuestionKey: string;
  MeasureID: number;
  MeasureName: string;
  AnswerCodeSetID: number;
  AnswerCodeSetDescription: string;
  AnswerSequenceNumber: number;
  
  //optional but required for text (date, ICD), number (therapy Hours), or text area type
  Description: string; 

  //user network ID
  AnswerByUserID: string; //user network ID
  LastUpdate: Date;
}


