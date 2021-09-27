export interface IUserAnswer {

  AnswerID: number;
  EpisodeID: number;
  QuestionID: number;
  StageID: number;
  AnswerCodeSetID: number;
  AnswerSequenceNumber: number;
  Description: string; //optional but required for text (date, ICD), number (therapy Hours), or text area type
  AnswerByUserID: number;
  LastUpdate: Date;
}

export interface AjaxPostbackModel {
  OldAnswers: Array<IUserAnswer>
  UpdatedAnswers: Array<IUserAnswer>
  NewAnswers: Array<IUserAnswer>
}


