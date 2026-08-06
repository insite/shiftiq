export interface ApiCreateChatbotSessionResult {
    SessionId: string;
    Model: string | null | undefined;
    AvailableModels: string[] | null | undefined;
}
