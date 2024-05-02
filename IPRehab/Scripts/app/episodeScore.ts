import { IEpisodeScore } from "./../appModels/IEpisodeScore.js";

export class EpisodeScore implements IEpisodeScore {
    Measure: string;
    Description: string;
    Score: number;
}