import { IUserAnswer } from "./commonImport.js"

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
    OldAnswerCodeSetID: number;
    AnswerCodeSetID: number;
    AnswerCodeSetDescription: string;
    AnswerSequenceNumber: number;
    Description: string;
    AnswerByUserID: string;
    LastUpdate: Date;
    Score: number;
}