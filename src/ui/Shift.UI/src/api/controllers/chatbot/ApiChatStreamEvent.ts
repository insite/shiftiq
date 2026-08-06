export type ApiSummaryEvent = {
    Type: "Summary";
    Model: string;
    InputTokenCount: number;
    OutputTokenCount: number;
    Tools: string[];
}

export type ApiChatStreamEvent =
    | { Type: "Delta"; Text: string; }
    | { Type: "DirectText"; Text: string; }
    | { Type: "Table"; Text: string; }
    | ApiSummaryEvent;