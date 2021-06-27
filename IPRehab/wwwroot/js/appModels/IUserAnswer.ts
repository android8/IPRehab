export interface IUserAnswer {

  FacilityID: string;
  FacilityRelationshipID: string;
  FiscalYear: string;
  UserID: string;
  QuestionID: string;
  QuestionKey: string;
  UserAnswerID: string; /* record primary key */
  AnswerCodeSetID: string;
  OtherDescription: string;
  AnswerSetID: string;
}

export interface AjaxPostbackModel {
  OldAnswers: Array<IUserAnswer>
  NewAnswers: Array<IUserAnswer>
}


