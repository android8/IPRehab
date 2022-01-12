export interface IUserAnswer {

  PatientName: string;
  PatientID: string;
  AnswerID: number;
  EpisodeID: number;
  OnsetDate: string;
  AdmissionDate: string;
  QuestionID: number;
  QuestionKey: string;
  StageID: number;
  StageName: string;
  AnswerCodeSetID: number;
  AnswerCodeSetDescription: string;
  AnswerSequenceNumber: number;
  
  //optional but required for text (date, ICD), number (therapy Hours), or text area type
  Description: string; 

  //user network ID
  AnswerByUserID: string; //user network ID
  LastUpdate: Date;


}

export interface AjaxPostbackModel {
  EpisodeID: number
  OldAnswers: Array<IUserAnswer>
  UpdatedAnswers: Array<IUserAnswer>
  NewAnswers: Array<IUserAnswer>
}


