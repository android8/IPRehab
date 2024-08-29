import { IUserAnswer } from "../app/commonImport.js";
import { IEpisodeScore } from "./IEpisodeScore.js";

export interface IAjaxPostbackModel {
    EpisodeID: number;
    DeleteAnswers: Array<IUserAnswer>;
    UpdateAnswers: Array<IUserAnswer>;
    InsertAnswers: Array<IUserAnswer>;
    EpisodeScores: Array<IEpisodeScore>;
}