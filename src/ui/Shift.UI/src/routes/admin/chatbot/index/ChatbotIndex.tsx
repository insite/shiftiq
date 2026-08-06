import { ApiChatStreamEvent, ApiSummaryEvent } from "@/api/controllers/chatbot/ApiChatStreamEvent";
import "./ChatbotIndex.css";

import { shiftClient } from "@/api/shiftClient";
import Alert from "@/components/Alert";
import TextBox from "@/components/TextBox";
import { getErrorDescription } from "@/contexts/status/StatusProviderContext";
import { urlHelper } from "@/helpers/urlHelper";
import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { Spinner } from "react-bootstrap";
import Markdown, { Components } from "react-markdown";
import remarkGfm from "remark-gfm";
import ComboBox from "@/components/combobox/ComboBox";
import { ListItem } from "@/models/listItem";
import ActionLink from "@/components/ActionLink";

type TextBlockType = "user" | "llm" | "direct" | "table" | "error";

interface TextBlock {
    key: number;
    type: TextBlockType,
    text: string;
    summary?: {
        model: string;
        inputTokenCount: number;
        outputTokenCount: number;
        tools: string[];
    }
}

export default function ChatbotIndex() {
    const [blocks, setBlocks] = useState<TextBlock[]>([]);
    const [newBlock, setNewBlock] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const [availableModels, setAvailableModels] = useState<ListItem[]>([]);
    const [model, setModel] = useState<string | null>(null);
    const [isChanging, setIsChanging] = useState(false);

    const keyCounterRef = useRef(blocks.length + 1);
    const textboxRef = useRef<HTMLInputElement>(null);
    const autoscrollRef = useRef(true);

    const components: Components = {
        a: ({ node: _, ...props }) => (
            <a
                {...props}
                href={props.href ? urlHelper.getFileUrlByNavigateUrl(props.href) : props.href}
                target="_blank"
            />
        ),
        table: ({ node: _, ...props }) => (
            <table
                {...props}
                className="table table-striped"
            />
        ),
    };

    useEffect(() => {
        run();

        async function run() {
            const result = await shiftClient.chatbot.createSession();

            if (!result?.AvailableModels || !result.Model) {
                return;
            }

            setAvailableModels(result.AvailableModels.map(m => ({
                value: m,
                text: m
            })));

            setModel(result.Model);
        }        
    }, []);

    useLayoutEffect(() => {
        if (autoscrollRef.current) {
            window.scrollTo({
                top: document.body.scrollHeight,
                behavior: "smooth"
            });
        }
    }, [blocks, newBlock, isLoading]);

    async function handleChangeModel(value: string | null) {
        if (!value) {
            return;
        }

        setIsChanging(true);

        try {
            await shiftClient.chatbot.modifyModel(value);
            setModel(value);
            textboxRef.current?.focus();
        } catch (error) {
            const errorMessage = getErrorDescription(error, null);
            addBlock("error", `Error: ${errorMessage}`);
        } finally {
            setIsChanging(false);
        }
    }

    function handleTextBoxKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
        if (e.key !== "Enter") {
            return;
        }

        const userInput = e.currentTarget.value;

        e.currentTarget.value = "";

        addBlock("user", userInput);

        sendRequest(userInput);
    }

    async function sendRequest(userInput: string) {
        let text = "";

        autoscrollRef.current = isPartiallyInViewport(textboxRef.current);

        setNewBlock(null);
        setIsLoading(true);

        let errorMessage: string | null = null;

        try {
            const stream = shiftClient.chatbot.stream(userInput);

            for await (const ev of stream) {
                autoscrollRef.current = isPartiallyInViewport(textboxRef.current);

                text = processEvent(text, ev);
            }
        }
        catch (error) {
            errorMessage = getErrorDescription(error, null);
        }
        finally {
            if (text) {
                addBlock("llm", text);
            }
            setIsLoading(false);
        }

        if (errorMessage) {
            addBlock("error", `Error: ${errorMessage}`);
        }
    }

    function processEvent(text: string, ev: ApiChatStreamEvent): string {
        switch (ev.Type) {
            case "Delta":
                text += ev.Text;
                setNewBlock(text);
                break;
            case "DirectText":
                if (text) {
                    addBlock("llm", text);
                    text = "";
                }
                addBlock("direct", ev.Text);
                break;
            case "Table":
                if (text) {
                    addBlock("llm", text);
                }
                addBlock("table", ev.Text);
                break;
            case "Summary":
                text = addSummary(text, ev);
                break;
            default:
                throw new Error("Unknown type")
        }
        return text;
    }

    function addSummary(text: string, summary: ApiSummaryEvent): string {
        if (text) {
            addBlock("llm", text);
        }
        setBlocks(prev => {
            const block = prev[prev.length - 1];
            return [
                ...prev.filter(x => x !== block),
                {
                    ...block,
                    summary: {
                        model: summary.Model,
                        inputTokenCount: summary.InputTokenCount,
                        outputTokenCount: summary.OutputTokenCount,
                        tools: summary.Tools,
                    }
                }
            ];
        });
        return "";
    }

    function addBlock(type: TextBlockType, text: string) {
        setBlocks(prev => [
            ...prev,
            {
                key: keyCounterRef.current++,
                type: type,
                text: text
            }
        ]);
    }

    return (
        <>
            <div className="chatbot-session mb-5">
                {blocks.map(({ key, type, text, summary }) => (
                    <div key={key} className={`textblock textblock-${type}`}>
                        {type !== "error" ? (
                            <>
                                <Markdown remarkPlugins={[remarkGfm]} components={components}>
                                    {text}
                                </Markdown>
                                {summary && (
                                    <>
                                        <span className="badge bg-success">{summary.model}</span>
                                        {summary.tools.map(t => (
                                            <span className="badge bg-primary ms-1">{t}</span>
                                        ))}
                                        <span className="badge bg-info ms-1">i: {summary.inputTokenCount}</span>
                                        <span className="badge bg-info ms-1">o: {summary.outputTokenCount}</span>
                                        <span className="badge bg-info ms-1">t: {summary.inputTokenCount + summary.outputTokenCount}</span>
                                    </>
                                )}
                            </>
                        ) : (
                            <Alert alertType="error" hideIcon>
                                {text}
                            </Alert>
                        )}
                    </div>
                ))}
                {isLoading && (
                    <div className={`textblock textblock-llm`}>
                        <Markdown remarkPlugins={[remarkGfm]} components={components}>
                            {newBlock}
                        </Markdown>
                        <Spinner animation="border" role="status" size="sm" className="me-3" />
                    </div>
                )}
            </div>
            <div className="chatbot-input">
                <TextBox
                    ref={textboxRef}
                    autoFocus
                    placeholder="Ask something about Shift iQ data"
                    readOnly={isLoading || isChanging}
                    onKeyDown={handleTextBoxKeyDown}
                />
                {availableModels.length > 0 && model && (
                    <ComboBox
                        items={availableModels}
                        value={model}
                        disabled={isLoading || isChanging}
                        onChange={handleChangeModel}
                    />
                )}
            </div>
            {availableModels.length > 0 && model && (
                <div className="chatbot-system-prompt form-text">
                    <ActionLink href="/client/admin/chatbot/system-prompt">
                        Modify System Prompt
                    </ActionLink>
                </div>
            )}
        </>
    );
}

function isPartiallyInViewport(el: HTMLElement | null) {
    if (!el) {
        return false;
    }

    const rect = el.getBoundingClientRect();

    return rect.bottom > 0 && rect.top < window.innerHeight;
}