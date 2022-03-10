import { IAjaxPostbackModel } from "../appModels/IAjaxPostbackModel.js";
import { IUserAnswer } from "../appModels/IUserAnswer.js";

export class AjaxPostbackModel implements IAjaxPostbackModel {
    EpisodeID: number;
    OldAnswers: IUserAnswer[];
    UpdatedAnswers: IUserAnswer[];
    NewAnswers: IUserAnswer[];

}