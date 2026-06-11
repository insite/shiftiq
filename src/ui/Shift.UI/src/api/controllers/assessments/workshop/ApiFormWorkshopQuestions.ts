export interface ApiFormWorkshopQuestions {
    IsQuestionOrderMatch: boolean;
    StaticQuestionOrderVerified: string | null | undefined;
    VerifiedQuestions: {
        Sequence: number,
        Code: string | null | undefined;
        Tag: string | null | undefined;
        Text: string;
    }[] | null | undefined;
}