import { IUserAnswer } from "./IUserAnswer";

export interface IAjaxPostbackModel {
  EpisodeID: number
  OldAnswers: Array<IUserAnswer>
  UpdatedAnswers: Array<IUserAnswer>
  NewAnswers: Array<IUserAnswer>
}