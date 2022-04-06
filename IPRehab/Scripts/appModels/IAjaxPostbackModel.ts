import { IUserAnswer } from "../app/commonImport.js";

export interface IAjaxPostbackModel {
  EpisodeID: number;
  OldAnswers: Array<IUserAnswer>;
  UpdatedAnswers: Array<IUserAnswer>;
  NewAnswers: Array<IUserAnswer>;
}