import { act } from "react";
import type { ReactNode } from "react";
import { createRoot } from "react-dom/client";
import { afterEach, beforeEach, describe, expect, test, vi } from "vitest";
import type { WorkshopImage } from "@/contexts/workshop/models/WorkshopImage";

declare global {
    // eslint-disable-next-line no-var
    var IS_REACT_ACT_ENVIRONMENT: boolean | undefined;
}

globalThis.IS_REACT_ACT_ENVIRONMENT = true;

const collectImagesMock = vi.hoisted(() => vi.fn());
const getImagesMock = vi.hoisted(() => vi.fn((images: WorkshopImage[]) => images));
const getErrorDescriptionMock = vi.hoisted(() => vi.fn(() => "Something went wrong"));

vi.mock("@/api/shiftClient", () => ({
    shiftClient: {
        workshop: {
            collectImages: collectImagesMock,
        }
    }
}));

vi.mock("../workshopQuestionAdapter", () => ({
    workshopQuestionAdapter: {
        getImages: getImagesMock,
    }
}));

vi.mock("@/contexts/status/StatusProviderContext", () => ({
    getErrorDescription: getErrorDescriptionMock,
}));

vi.mock("@/contexts/workshop/WorkshopQuestionProviderContext", () => ({
    useWorkshopQuestionProvider: () => ({
        bankId: "bank-1",
    })
}));

vi.mock("@/helpers/translate", () => ({
    translate: (value: string) => value,
}));

vi.mock("@/components/Alert", () => ({
    default: ({ children }: { children?: ReactNode }) => (
        <div role="alert">{children}</div>
    )
}));

vi.mock("react-bootstrap", () => {
    function Modal({ children, show }: { children?: ReactNode; show?: boolean }) {
        return show ? <div data-testid="modal">{children}</div> : null;
    }

    Modal.Header = function ModalHeader({ children }: { children?: ReactNode }) {
        return <div>{children}</div>;
    };

    Modal.Title = function ModalTitle({ children }: { children?: ReactNode }) {
        return <h5>{children}</h5>;
    };

    Modal.Body = function ModalBody({ children }: { children?: ReactNode }) {
        return <div>{children}</div>;
    };

    return {
        Modal,
        Spinner: ({ children }: { children?: ReactNode }) => <span role="status">{children}</span>,
    };
});

import WorkshopQuestions_SelectFile from "../WorkshopQuestions_SelectFile";

describe("WorkshopQuestions_SelectFile", () => {
    beforeEach(() => {
        collectImagesMock.mockReset();
        getImagesMock.mockClear();
        getErrorDescriptionMock.mockClear();
    });

    afterEach(() => {
        document.body.innerHTML = "";
    });

    test("shows loading and then renders the empty state", async () => {
        let resolveRequest: ((value: WorkshopImage[]) => void) | null = null;
        collectImagesMock.mockReturnValue(new Promise<WorkshopImage[]>(resolve => {
            resolveRequest = resolve;
        }));

        const view = await renderSelectFiles();

        expect(view.container.textContent).toContain("Loading...");

        await act(async () => {
            resolveRequest?.([]);
            await flushPromises();
        });

        expect(view.container.textContent).toContain("No Images");

        await view.unmount();
    });

    test("shows the error state when loading fails", async () => {
        collectImagesMock.mockRejectedValue(new Error("failed"));

        const view = await renderSelectFiles();

        expect(view.container.querySelector("[role='alert']")?.textContent).toContain("Something went wrong");

        await view.unmount();
    });

    test("renders attached image metadata and the environment badge", async () => {
        collectImagesMock.mockResolvedValue([
            createImage({
                fileName: "attached-image.png",
                environment: "Production",
                attachment: {
                    title: "Front Cover",
                    number: "100.2",
                    condition: null,
                    publicationStatus: "Published",
                    dimension: "1200 x 900",
                },
            }),
        ]);

        const view = await renderSelectFiles();

        expect(view.container.textContent).toContain("Asset Title");
        expect(view.container.textContent).toContain("Front Cover");
        expect(view.container.textContent).toContain("Asset #");
        expect(view.container.textContent).toContain("100.2");
        expect(view.container.textContent).toContain("Condition");
        expect(view.container.textContent).toContain("Unassigned");
        expect(view.container.textContent).toContain("Publication Status");
        expect(view.container.textContent).toContain("Published");
        expect(view.container.textContent).toContain("File Name");
        expect(view.container.textContent).toContain("attached-image.png");
        expect(view.container.textContent).toContain("Dimension");
        expect(view.container.textContent).toContain("1200 x 900");

        const badge = view.container.querySelector(".icon-wrapper .badge");

        expect(badge?.textContent).toContain("Bank Production");
        expect(badge?.className).toContain("bg-success");

        await view.unmount();
    });

    test("computes dimensions for unattached images after load and selects the image", async () => {
        const onSelect = vi.fn();
        collectImagesMock.mockResolvedValue([
            createImage({
                fileName: "diagram.png",
                url: "https://example.com/diagram.png",
                environment: "External",
                attachment: null,
            }),
        ]);

        const view = await renderSelectFiles({ onSelect });
        const img = view.container.querySelector("img") as HTMLImageElement | null;

        expect(img).not.toBeNull();
        expect(view.container.textContent).toContain("0 x 0");

        await act(async () => {
            img!.dispatchEvent(new Event("load", { bubbles: true }));
            await flushPromises();
        });

        expect(view.container.textContent).toContain("0 x 0");
        expect(img?.style.opacity).toBe("1");

        await act(async () => {
            img!.dispatchEvent(new MouseEvent("click", { bubbles: true }));
            await flushPromises();
        });

        expect(onSelect).toHaveBeenCalledWith("https://example.com/diagram.png", "diagram.png", true);

        await view.unmount();
    });

    test("shows a fallback icon for broken images and no longer allows selection", async () => {
        const onSelect = vi.fn();
        collectImagesMock.mockResolvedValue([
            createImage({
                fileName: "broken.png",
                url: "https://example.com/broken.png",
                environment: "Sandbox",
                attachment: null,
            }),
        ]);

        const view = await renderSelectFiles({ onSelect });
        const img = view.container.querySelector("img") as HTMLImageElement | null;

        await act(async () => {
            img!.dispatchEvent(new Event("error", { bubbles: true }));
            await flushPromises();
        });

        const fallbackIcon = view.container.querySelector(".no-upload");
        expect(fallbackIcon?.className).toContain("fa-file-xmark");
        expect(view.container.querySelector("img")).toBeNull();
        expect(onSelect).not.toHaveBeenCalled();

        await view.unmount();
    });
});

async function renderSelectFiles(options?: {
    onSelect?: (fileUrl: string, documentName: string, isImage: boolean) => void;
    onClose?: () => void;
}): Promise<{
    container: HTMLDivElement;
    unmount: () => Promise<void>;
}> {
    const container = document.createElement("div");
    document.body.appendChild(container);

    const root = createRoot(container);

    await act(async () => {
        root.render(
            <WorkshopQuestions_SelectFile
                onSelect={options?.onSelect ?? (() => {})}
                onClose={options?.onClose ?? (() => {})}
            />
        );
        await flushPromises();
    });

    return {
        container,
        async unmount() {
            await act(async () => {
                root.unmount();
                await flushPromises();
            });
        }
    };
}

function createImage(overrides?: Partial<WorkshopImage>): WorkshopImage {
    return {
        fileName: overrides?.fileName ?? "image.png",
        url: overrides?.url ?? "https://example.com/image.png",
        environment: overrides?.environment ?? "External",
        attachment: overrides?.attachment === undefined ? null : overrides.attachment,
    };
}

async function flushPromises() {
    await Promise.resolve();
    await Promise.resolve();
}
