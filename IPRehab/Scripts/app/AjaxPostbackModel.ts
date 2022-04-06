////import { IAjaxPostbackModel } from "../appModels/IAjaxPostbackModel.js";
////import { IUserAnswer } from "../appModels/IUserAnswer.js";

import { IAjaxPostbackModel, IUserAnswer } from "./commonImport.js";

export class AjaxPostbackModel implements IAjaxPostbackModel {
  EpisodeID = 0;
  FacilityID = '';
  OldAnswers: IUserAnswer[] = [];
  UpdatedAnswers: IUserAnswer[] = [];
  NewAnswers: IUserAnswer[] = [];
}