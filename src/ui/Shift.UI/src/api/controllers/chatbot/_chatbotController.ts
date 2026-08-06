import { fetchHelper } from "@/api/fetchHelper";
import { ApiCreateChatbotSessionResult } from "./ApiCreateChatbotSessionResult";
import { ApiChatStreamEvent } from "./ApiChatStreamEvent";
import { ApiRetrieveSystemPromptResult } from "./ApiRetrieveSystemPromptResult";
import { ApiModifySystemPromptResult } from "./ApiModifySystemPromptResult";

export const _chatbotController = {
    async retrieveSystemPrompt(): Promise<ApiRetrieveSystemPromptResult | null> {
        return await fetchHelper.get("/api/chatbot/system-prompt");
    },

    async modifySystemPrompt(systemPrompt: string | null): Promise<ApiModifySystemPromptResult | null> {
        return await fetchHelper.put("/api/chatbot/system-prompt", {
            SystemPrompt: systemPrompt
        });
    },

    async modifyModel(model: string): Promise<void> {
        await fetchHelper.put("/api/chatbot/model", {
            Model: model
        });
    },

    async createSession(): Promise<ApiCreateChatbotSessionResult | null> {
        return await fetchHelper.post<ApiCreateChatbotSessionResult | null>("/api/chatbot", {});
    },

    stream(userInput: string): AsyncIterable<ApiChatStreamEvent> {
        return fetchHelper.postStream<ApiChatStreamEvent>("/api/chatbot/stream", {
            UserInput: userInput
        });
    },
}
