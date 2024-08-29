import { IAjaxPostbackModel, IUserAnswer } from "./commonImport.js";
import { IEpisodeScore } from "./../appModels/IEpisodeScore.js";

export class AjaxPostbackModel implements IAjaxPostbackModel {
    EpisodeID = 0;
    FacilityID = '';
    DeleteAnswers: IUserAnswer[] = [];
    UpdateAnswers: IUserAnswer[] = [];
    InsertAnswers: IUserAnswer[] = [];
    EpisodeScores: IEpisodeScore[];
}