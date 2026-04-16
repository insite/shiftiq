import { WorkshopFlag } from "./WorkshopEnums";

export interface WorkshopComment {
    commentId: string;
    authorName: string;
    postedOn: string;
    subject: string | null;
    text: string;
    category: string | null;
    flag: WorkshopFlag;
    eventFormat: string | null;
    isHidden: boolean;
}