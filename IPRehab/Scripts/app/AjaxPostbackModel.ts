import { IAjaxPostbackModel } from "../appModels/IAjaxPostbackModel.js";
import { IUserAnswer } from "../appModels/IUserAnswer.js";

export class AjaxPostbackModel implements IAjaxPostbackModel {
  EpisodeID: number;
  FacilityID: string;
  OldAnswers: IUserAnswer[];
  UpdatedAnswers: IUserAnswer[];
  NewAnswers: IUserAnswer[];
}