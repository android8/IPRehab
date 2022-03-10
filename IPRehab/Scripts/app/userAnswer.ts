import { IUserAnswer } from "../appModels/IUserAnswer.js";

export class UserAnswer implements IUserAnswer {
    PatientName: string;
    PatientID: string;
    AnswerID: number;
    EpisodeID: number;
    OnsetDate: Date;
    AdmissionDate: Date;
    QuestionID: number;
    QuestionKey: string;
    MeasureID: number;
    MeasureName: string;
    AnswerCodeSetID: number;
    AnswerCodeSetDescription: string;
    AnswerSequenceNumber: number;
    Description: string;
    AnswerByUserID: string;
    LastUpdate: Date;
}