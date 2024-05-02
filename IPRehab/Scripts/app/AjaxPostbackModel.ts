import { IAjaxPostbackModel, IUserAnswer } from "./commonImport.js";
import { IEpisodeScore } from "./../appModels/IEpisodeScore.js";

export class AjaxPostbackModel implements IAjaxPostbackModel {
    EpisodeID = 0;
    FacilityID = '';
    OldAnswers: IUserAnswer[] = [];
    UpdatedAnswers: IUserAnswer[] = [];
    NewAnswers: IUserAnswer[] = [];
    EpisodeScores: IEpisodeScore[];
}