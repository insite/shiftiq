export interface WorkshopQuestionOption {
    number: number;
    letter: string;
    titleMarkdown: string | null;
    titleHtml: string | null;
    points: number;
    isTrue: boolean | null;
    columns: {
        textMarkdown: string | null;
        textHtml: string | null;
    }[] | null,
}