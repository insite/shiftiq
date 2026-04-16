import { ApiWorkshopComment } from "./ApiWorkshopComment";

export interface ApiWorkshopQuestionComments {
    Comments: ApiWorkshopComment[]
    CandidateCommentCount: number;
}