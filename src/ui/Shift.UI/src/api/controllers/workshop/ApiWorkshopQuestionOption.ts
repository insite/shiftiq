export interface ApiWorkshopQuestionOption {
    Number: number;
    Letter: string;
    TitleMarkdown: string | null | undefined;
    TitleHtml: string | null | undefined;
    Points: number;
    IsTrue: boolean | null | undefined;
    Columns: {
        TextMarkdown: string | null | undefined;
        TextHtml: string | null | undefined;
    }[] | null,
}