import { IAjaxPostbackModel, IUserAnswer } from "./commonImport.js";

export class AjaxPostbackModel implements IAjaxPostbackModel {
  EpisodeID = 0;
  FacilityID = '';
  OldAnswers: IUserAnswer[] = [];
  UpdatedAnswers: IUserAnswer[] = [];
  NewAnswers: IUserAnswer[] = [];
}