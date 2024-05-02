import { IUserAnswer } from "../app/commonImport.js";
import { IEpisodeScore } from "./IEpisodeScore.js";

export interface IAjaxPostbackModel {
    EpisodeID: number;
    OldAnswers: Array<IUserAnswer>;
    UpdatedAnswers: Array<IUserAnswer>;
    NewAnswers: Array<IUserAnswer>;
    EpisodeScores: Array<IEpisodeScore>;
}