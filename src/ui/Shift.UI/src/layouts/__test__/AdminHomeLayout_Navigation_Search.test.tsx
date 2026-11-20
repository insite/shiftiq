import { act } from "react";
import type { ReactNode } from "react";
import { createRoot } from "react-dom/client";
import { afterEach, beforeEach, describe, expect, test, vi } from "vitest";
import type { ApiSearchViewModel } from "@/api/controllers/searches/ApiSearchViewModel";
import type { QueryResult } from "@/models/QueryResult";

declare global {
    // eslint-disable-next-line no-var
    var IS_REACT_ACT_ENVIRONMENT: boolean | undefined;
}

globalThis.IS_REACT_ACT_ENVIRONMENT = true;

const searchMock = vi.hoisted(() => vi.fn());

vi.mock("@/api/shiftClient", () => ({
    shiftClient: {
        search: {
            search: searchMock,
        }
    }
}));

vi.mock("@/components/ActionLink", () => ({
    default: ({ href, className, children }: { href?: string; className?: string; children?: ReactNode }) => (
        <a href={href} className={className}>{children}</a>
    )
}));

import AdminHomeLayout_Navigation_Search from "../AdminHomeLayout_Navigation_Search";

const personResult = createResult({
    ObjectType: "Person",
    ObjectId: "person-1",
    Title: "Michelle Morgan",
    Url: "/ui/admin/contacts/people/edit?contact=person-1",
    Attributes: ["michelle@example.com", "EMP-100"],
});

const toolResult = createResult({
    ObjectType: "Tool",
    ObjectId: "tool-1",
    Title: "Create an Assessment",
    Url: "/ui/admin/assessments/create",
    Attributes: null,
});

describe("AdminHomeLayout_Navigation_Search", () => {
    const scrollIntoViewMock = vi.fn();

    beforeEach(() => {
        vi.useFakeTimers();
        searchMock.mockReset();
        scrollIntoViewMock.mockReset();
        Object.defineProperty(HTMLElement.prototype, "scrollIntoView", {
            configurable: true,
            value: scrollIntoViewMock,
        });
    });

    afterEach(() => {
        vi.useRealTimers();
        document.body.innerHTML = "";
        delete (HTMLElement.prototype as Partial<HTMLElement>).scrollIntoView;
    });

    test("debounces non-whitespace searches and selects the first returned result", async () => {
        searchMock.mockResolvedValue(createQueryResult([personResult]));
        const view = await renderSearch();
        const input = getInput(view.container);

        expect(input.getAttribute("autocomplete")).toBe("off");
        expect(input.getAttribute("autocorrect")).toBe("off");
        expect(input.getAttribute("autocapitalize")).toBe("none");
        expect(input.getAttribute("spellcheck")).toBe("false");
        expect(input.getAttribute("aria-expanded")).toBe("false");
        expect(view.container.querySelector(".search-results")).toBeNull();

        await focusAndType(input, "   ");
        await advanceTimer(300);
        expect(searchMock).not.toHaveBeenCalled();

        await typeText(input, " m ");
        expect(vi.getTimerCount()).toBe(1);
        await advanceTimer(299);
        expect(searchMock).not.toHaveBeenCalled();
        expect(view.container.querySelector(".search-results")).toBeNull();

        await advanceTimer(1);

        expect(searchMock).toHaveBeenCalledOnce();
        expect(searchMock).toHaveBeenCalledWith("m", 0);
        expect(input.getAttribute("aria-expanded")).toBe("true");

        const options = getOptions(view.container);
        expect(options).toHaveLength(1);
        expect(options[0].getAttribute("aria-selected")).toBe("true");
        expect(input.getAttribute("aria-activedescendant")).toBe(options[0].id);

        await view.unmount();
    });

    test("renders result links, subtitles, and object type icons", async () => {
        searchMock.mockResolvedValue(createQueryResult([personResult, toolResult]));
        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "search");
        await advanceTimer(300);

        const listbox = view.container.querySelector("[role='listbox']");
        const links = Array.from(view.container.querySelectorAll<HTMLAnchorElement>(".result-title"));

        expect(listbox?.id).toBe(input.getAttribute("aria-controls"));
        expect(links[0].textContent).toBe("Michelle Morgan");
        expect(links[0].getAttribute("href")).toBe(personResult.Url);
        expect(links[1].textContent).toBe("Create an Assessment");
        expect(links[1].getAttribute("href")).toBe(toolResult.Url);
        expect(view.container.textContent).toContain("Person • michelle@example.com • EMP-100");
        expect(view.container.textContent).toContain("Tool");
        expect(view.container.querySelector(".result-icon.bg-info .fa-user")).not.toBeNull();
        expect(view.container.querySelector(".result-icon.bg-warning .fa-question")).not.toBeNull();
        expect(scrollIntoViewMock).toHaveBeenCalledWith({ block: "nearest" });

        await view.unmount();
    });

    test("clamps arrow selection, retains textbox focus, and opens the selected link with Enter", async () => {
        const thirdResult = createResult({
            ObjectType: "Tool",
            ObjectId: "tool-2",
            Title: "View Assessments",
            Url: "/ui/admin/assessments/search",
            Attributes: undefined,
        });
        searchMock.mockResolvedValue(createQueryResult([personResult, toolResult, thirdResult]));
        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "assessment");
        await advanceTimer(300);

        const resultsPanel = view.container.querySelector<HTMLUListElement>("[role='listbox']")!;
        setPanelDimensions(resultsPanel, 300, 100);
        scrollIntoViewMock.mockReset();

        await pressKey(input, "ArrowUp");
        expect(getSelectedOption(view.container).textContent).toContain("Michelle Morgan");

        await pressKey(input, "ArrowDown");
        expect(getSelectedOption(view.container).textContent).toContain("Create an Assessment");
        expect(scrollIntoViewMock).toHaveBeenCalledOnce();
        expect(scrollIntoViewMock).toHaveBeenCalledWith({ block: "nearest" });

        resultsPanel.scrollTop = 100;
        await pressKey(input, "ArrowUp");
        expect(getSelectedOption(view.container).textContent).toContain("Michelle Morgan");
        expect(resultsPanel.scrollTop).toBe(0);

        await pressKey(input, "ArrowDown");
        expect(getSelectedOption(view.container).textContent).toContain("Create an Assessment");

        await pressKey(input, "ArrowDown");
        expect(getSelectedOption(view.container).textContent).toContain("View Assessments");
        expect(resultsPanel.scrollTop).toBe(200);

        await pressKey(input, "ArrowDown");
        const selectedOption = getSelectedOption(view.container);
        expect(selectedOption.textContent).toContain("View Assessments");
        expect(resultsPanel.scrollTop).toBe(200);
        expect(document.activeElement).toBe(input);

        const selectedLink = selectedOption.querySelector("a")!;
        const clickHandler = vi.fn((event: Event) => event.preventDefault());
        selectedLink.addEventListener("click", clickHandler);

        await pressKey(input, "Enter");
        expect(clickHandler).toHaveBeenCalledOnce();
        expect(document.activeElement).toBe(input);

        await view.unmount();
    });

    test("keeps previous results interactive while replacement results are debounced and loading", async () => {
        const replacementRequest = deferred<QueryResult<ApiSearchViewModel> | null>();
        const replacementResult = createResult({
            ObjectType: "Tool",
            ObjectId: "tool-2",
            Title: "View Assessments",
            Url: "/ui/admin/assessments/search",
            Attributes: undefined,
        });
        searchMock
            .mockResolvedValueOnce(createQueryResult([personResult, toolResult]))
            .mockReturnValueOnce(replacementRequest.promise);

        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "assessment");
        await advanceTimer(300);
        await pressKey(input, "ArrowDown");
        expect(getSelectedOption(view.container).textContent).toContain("Create an Assessment");

        const retainedPanel = view.container.querySelector<HTMLUListElement>("[role='listbox']")!;
        retainedPanel.scrollTop = 120;

        await typeText(input, "assessments");
        expect(input.getAttribute("aria-expanded")).toBe("true");
        expect(view.container.textContent).toContain("Michelle Morgan");
        expect(getSelectedOption(view.container).textContent).toContain("Michelle Morgan");
        expect(retainedPanel.scrollTop).toBe(120);

        await advanceTimer(299);
        expect(view.container.textContent).toContain("Create an Assessment");
        expect(retainedPanel.scrollTop).toBe(120);

        await advanceTimer(1);
        expect(searchMock).toHaveBeenCalledTimes(2);
        expect(view.container.textContent).toContain("Create an Assessment");
        expect(retainedPanel.scrollTop).toBe(120);

        await pressKey(input, "ArrowDown");
        const retainedLink = getSelectedOption(view.container).querySelector("a")!;
        const clickHandler = vi.fn((event: Event) => event.preventDefault());
        retainedLink.addEventListener("click", clickHandler);
        await pressKey(input, "Enter");
        expect(clickHandler).toHaveBeenCalledOnce();
        await pressKey(input, "ArrowUp");
        expect(getSelectedOption(view.container).textContent).toContain("Michelle Morgan");

        await act(async () => {
            replacementRequest.resolve(createQueryResult([replacementResult]));
            await flushPromises();
        });

        expect(view.container.textContent).not.toContain("Michelle Morgan");
        expect(getSelectedOption(view.container).textContent).toContain("View Assessments");
        expect(view.container.querySelector<HTMLUListElement>("[role='listbox']")?.scrollTop).toBe(0);

        await typeText(input, "   ");
        expect(input.getAttribute("aria-expanded")).toBe("false");
        expect(view.container.querySelector("[role='listbox']")).toBeNull();

        await view.unmount();
    });

    test("scrolls to the bottom after a query replaces a longer result set with a shorter one", async () => {
        const thirdResult = createResult({
            ObjectType: "Action",
            ObjectId: "action-1",
            Title: "Manage Enrollments",
            Url: "/ui/admin/contacts/enrollments",
            Attributes: [],
        });
        searchMock
            .mockResolvedValueOnce(createQueryResult([personResult, toolResult, thirdResult]))
            .mockResolvedValueOnce(createQueryResult([personResult, toolResult]));

        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "manage");
        await advanceTimer(300);
        expect(getOptions(view.container)).toHaveLength(3);

        await typeText(input, "assessment");
        await advanceTimer(300);
        expect(getOptions(view.container)).toHaveLength(2);

        const resultsPanel = view.container.querySelector<HTMLUListElement>("[role='listbox']")!;
        setPanelDimensions(resultsPanel, 300, 100);
        scrollIntoViewMock.mockReset();

        await pressKey(input, "ArrowDown");

        expect(getSelectedOption(view.container).textContent).toContain("Create an Assessment");
        expect(resultsPanel.scrollTop).toBe(200);
        expect(scrollIntoViewMock).not.toHaveBeenCalled();

        await view.unmount();
    });

    test("closes on outside interaction or Escape and reopens cached results on focus", async () => {
        const pendingRequest = deferred<QueryResult<ApiSearchViewModel> | null>();
        searchMock
            .mockResolvedValueOnce(createQueryResult([personResult, toolResult]))
            .mockReturnValueOnce(pendingRequest.promise);
        const view = await renderSearch();
        const input = getInput(view.container);
        const outsideButton = document.createElement("button");
        document.body.appendChild(outsideButton);

        await focusAndType(input, "person");
        await advanceTimer(300);
        await pressKey(input, "ArrowDown");

        await act(async () => {
            outsideButton.dispatchEvent(new MouseEvent("mousedown", { bubbles: true }));
            outsideButton.focus();
        });

        expect(input.getAttribute("aria-expanded")).toBe("false");
        expect(view.container.querySelector("[role='listbox']")).toBeNull();

        await act(async () => input.focus());
        expect(input.getAttribute("aria-expanded")).toBe("true");
        expect(getSelectedOption(view.container).textContent).toContain("Create an Assessment");

        await pressKey(input, "Escape");
        expect(input.getAttribute("aria-expanded")).toBe("false");
        expect(input.hasAttribute("aria-activedescendant")).toBe(false);
        expect(document.activeElement).toBe(input);

        await typeText(input, "people");
        await advanceTimer(300);
        expect(input.getAttribute("aria-expanded")).toBe("true");

        await pressKey(input, "Escape");
        await act(async () => {
            pendingRequest.resolve(createQueryResult([personResult]));
            await flushPromises();
        });
        expect(input.getAttribute("aria-expanded")).toBe("false");
        expect(view.container.querySelector("[role='listbox']")).toBeNull();

        await view.unmount();
    });

    test("ignores stale responses", async () => {
        const firstRequest = deferred<QueryResult<ApiSearchViewModel> | null>();
        const secondRequest = deferred<QueryResult<ApiSearchViewModel> | null>();
        searchMock
            .mockReturnValueOnce(firstRequest.promise)
            .mockReturnValueOnce(secondRequest.promise);

        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "m");
        await advanceTimer(300);
        await typeText(input, "mi");
        await advanceTimer(300);

        await act(async () => {
            secondRequest.resolve(createQueryResult([toolResult]));
            await flushPromises();
        });
        expect(view.container.textContent).toContain("Create an Assessment");

        const resultsPanel = view.container.querySelector<HTMLUListElement>("[role='listbox']")!;
        resultsPanel.scrollTop = 80;

        await act(async () => {
            firstRequest.resolve(createQueryResult([personResult]));
            await flushPromises();
        });
        expect(view.container.textContent).toContain("Create an Assessment");
        expect(view.container.textContent).not.toContain("Michelle Morgan");
        expect(resultsPanel.scrollTop).toBe(80);

        await view.unmount();
    });

    test("loads and appends the next page at the bottom without changing selection or scroll position", async () => {
        const nextPageRequest = deferred<QueryResult<ApiSearchViewModel> | null>();
        const nextPageResult = createResult({
            ObjectType: "Action",
            ObjectId: "action-1",
            Title: "Manage Enrollments",
            Url: "/ui/admin/contacts/enrollments",
            Attributes: [],
        });
        searchMock
            .mockResolvedValueOnce(createQueryResult([personResult, toolResult], {
                totalRowCount: -1,
                rowsPerPage: 2,
            }))
            .mockReturnValueOnce(nextPageRequest.promise);

        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "manage");
        await advanceTimer(300);
        await pressKey(input, "ArrowDown");

        const resultsPanel = view.container.querySelector<HTMLUListElement>("[role='listbox']")!;
        setPanelDimensions(resultsPanel, 300, 100);

        await scrollResults(resultsPanel, 150);
        expect(searchMock).toHaveBeenCalledOnce();

        await scrollResults(resultsPanel, 200);
        expect(searchMock).toHaveBeenCalledTimes(2);
        expect(searchMock).toHaveBeenLastCalledWith("manage", 1);
        expect(resultsPanel.getAttribute("aria-busy")).toBe("true");
        expect(view.container.querySelector(".loading-results")?.textContent).toContain("Loading more results...");

        await scrollResults(resultsPanel, 200);
        expect(searchMock).toHaveBeenCalledTimes(2);

        await act(async () => {
            nextPageRequest.resolve(createQueryResult([nextPageResult], {
                pageIndex: 1,
                totalRowCount: -1,
                rowsPerPage: 2,
            }));
            await flushPromises();
        });

        expect(getOptions(view.container)).toHaveLength(3);
        expect(getOptions(view.container)[2].textContent).toContain("Manage Enrollments");
        expect(getSelectedOption(view.container).textContent).toContain("Create an Assessment");
        expect(resultsPanel.scrollTop).toBe(200);
        expect(resultsPanel.getAttribute("aria-busy")).toBe("false");
        expect(view.container.querySelector(".loading-results")).toBeNull();

        await scrollResults(resultsPanel, 200);
        expect(searchMock).toHaveBeenCalledTimes(2);

        await view.unmount();
    });

    test("does not request another page when the known total is already loaded", async () => {
        searchMock.mockResolvedValue(createQueryResult([personResult, toolResult], {
            totalRowCount: 2,
            rowsPerPage: 2,
        }));
        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "person");
        await advanceTimer(300);

        const resultsPanel = view.container.querySelector<HTMLUListElement>("[role='listbox']")!;
        setPanelDimensions(resultsPanel, 200, 100);
        await scrollResults(resultsPanel, 100);

        expect(searchMock).toHaveBeenCalledOnce();

        await view.unmount();
    });

    test("retains results after a next-page failure and retries on another bottom scroll", async () => {
        const failedRequest = deferred<QueryResult<ApiSearchViewModel> | null>();
        searchMock
            .mockResolvedValueOnce(createQueryResult([personResult, toolResult], {
                totalRowCount: -1,
                rowsPerPage: 2,
            }))
            .mockReturnValueOnce(failedRequest.promise)
            .mockResolvedValueOnce(createQueryResult([], {
                pageIndex: 1,
                totalRowCount: -1,
                rowsPerPage: 2,
            }));
        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "assessment");
        await advanceTimer(300);

        const resultsPanel = view.container.querySelector<HTMLUListElement>("[role='listbox']")!;
        setPanelDimensions(resultsPanel, 200, 100);
        await scrollResults(resultsPanel, 100);

        await act(async () => {
            failedRequest.reject(new Error("failed"));
            await flushPromises();
        });

        expect(getOptions(view.container)).toHaveLength(2);
        expect(resultsPanel.getAttribute("aria-busy")).toBe("false");

        await scrollResults(resultsPanel, 100);
        expect(searchMock).toHaveBeenCalledTimes(3);
        expect(searchMock).toHaveBeenLastCalledWith("assessment", 1);
        expect(getOptions(view.container)).toHaveLength(2);

        await scrollResults(resultsPanel, 100);
        expect(searchMock).toHaveBeenCalledTimes(3);

        await view.unmount();
    });

    test("ignores a next-page response after the search query changes", async () => {
        const staleNextPageRequest = deferred<QueryResult<ApiSearchViewModel> | null>();
        const staleNextPageResult = createResult({
            ObjectType: "Action",
            ObjectId: "action-stale",
            Title: "Stale Result",
            Url: "/stale",
            Attributes: [],
        });
        searchMock
            .mockResolvedValueOnce(createQueryResult([personResult, toolResult], {
                totalRowCount: -1,
                rowsPerPage: 2,
            }))
            .mockReturnValueOnce(staleNextPageRequest.promise)
            .mockResolvedValueOnce(createQueryResult([toolResult]));
        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "first");
        await advanceTimer(300);

        const resultsPanel = view.container.querySelector<HTMLUListElement>("[role='listbox']")!;
        setPanelDimensions(resultsPanel, 200, 100);
        await scrollResults(resultsPanel, 100);

        await typeText(input, "second");
        expect(resultsPanel.getAttribute("aria-busy")).toBe("false");
        await advanceTimer(300);
        expect(searchMock).toHaveBeenLastCalledWith("second", 0);

        await act(async () => {
            staleNextPageRequest.resolve(createQueryResult([staleNextPageResult], {
                pageIndex: 1,
                totalRowCount: -1,
                rowsPerPage: 2,
            }));
            await flushPromises();
        });

        expect(view.container.textContent).toContain("Create an Assessment");
        expect(view.container.textContent).not.toContain("Stale Result");
        expect(getOptions(view.container)).toHaveLength(1);

        await view.unmount();
    });

    test("shows and retains an accessible empty state for null and empty responses", async () => {
        const emptyRequest = deferred<QueryResult<ApiSearchViewModel> | null>();
        searchMock
            .mockResolvedValueOnce(null)
            .mockReturnValueOnce(emptyRequest.promise)
            .mockRejectedValueOnce(new Error("failed"));

        const view = await renderSearch();
        const input = getInput(view.container);

        await focusAndType(input, "missing");
        await advanceTimer(299);
        expect(view.container.querySelector(".search-results")).toBeNull();

        await advanceTimer(1);
        expect(input.getAttribute("aria-expanded")).toBe("true");
        expect(input.hasAttribute("aria-activedescendant")).toBe(false);
        expect(view.container.querySelector("[role='status']")?.textContent).toBe("No search results found");
        expect(view.container.querySelector("[role='status']")?.id).toBe(input.getAttribute("aria-controls"));

        await pressKey(input, "ArrowDown");
        await pressKey(input, "ArrowUp");
        await pressKey(input, "Enter");
        expect(getOptions(view.container)).toHaveLength(0);
        expect(input.hasAttribute("aria-activedescendant")).toBe(false);
        expect(document.activeElement).toBe(input);

        await pressKey(input, "Escape");
        expect(view.container.querySelector(".search-results")).toBeNull();

        const outsideButton = document.createElement("button");
        document.body.appendChild(outsideButton);
        await act(async () => outsideButton.focus());
        await act(async () => input.focus());
        expect(view.container.querySelector("[role='status']")?.textContent).toBe("No search results found");

        await typeText(input, "still missing");
        expect(view.container.querySelector("[role='status']")?.textContent).toBe("No search results found");
        await advanceTimer(300);
        expect(view.container.querySelector("[role='status']")?.textContent).toBe("No search results found");

        await act(async () => {
            emptyRequest.resolve(createQueryResult([]));
            await flushPromises();
        });
        expect(view.container.querySelector("[role='status']")?.textContent).toBe("No search results found");

        await typeText(input, "   ");
        expect(input.getAttribute("aria-expanded")).toBe("false");
        expect(view.container.querySelector(".search-results")).toBeNull();

        await typeText(input, "failed");
        await advanceTimer(300);
        expect(input.getAttribute("aria-expanded")).toBe("false");
        expect(view.container.querySelector(".search-results")).toBeNull();

        await view.unmount();
    });

    test("opens and toggles the search help popup without using the help text as a tooltip", async () => {
        const view = await renderSearch();
        const helpButton = getHelpButton(view.container);

        expect(helpButton.title).not.toContain("Search using keywords");
        expect(view.container.querySelector(".search-help-popup")).toBeNull();

        await click(helpButton);

        expect(view.container.querySelector(".search-help-popup")?.textContent).toContain(
            "Search using keywords to find your Apps and manage your content and contacts across Shift iQ."
        );

        await click(helpButton);
        expect(view.container.querySelector(".search-help-popup")).toBeNull();

        await view.unmount();
    });

    test("closes the search help popup on outside interaction and Escape", async () => {
        const view = await renderSearch();
        const helpButton = getHelpButton(view.container);
        const outsideButton = document.createElement("button");
        document.body.appendChild(outsideButton);

        await click(helpButton);
        expect(view.container.querySelector(".search-help-popup")).not.toBeNull();

        await click(outsideButton);
        expect(view.container.querySelector(".search-help-popup")).toBeNull();

        await click(helpButton);
        await act(async () => outsideButton.focus());
        expect(view.container.querySelector(".search-help-popup")).toBeNull();

        await click(helpButton);
        await act(async () => {
            document.body.dispatchEvent(new KeyboardEvent("keydown", { key: "Escape", bubbles: true }));
        });

        expect(view.container.querySelector(".search-help-popup")).toBeNull();
        expect(document.activeElement).toBe(helpButton);

        await view.unmount();
    });
});

async function renderSearch(): Promise<{
    container: HTMLDivElement;
    unmount: () => Promise<void>;
}> {
    const container = document.createElement("div");
    document.body.appendChild(container);
    const root = createRoot(container);

    await act(async () => {
        root.render(<AdminHomeLayout_Navigation_Search />);
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

function getInput(container: HTMLElement): HTMLInputElement {
    return container.querySelector<HTMLInputElement>("input[role='combobox']")!;
}

function getOptions(container: HTMLElement): HTMLLIElement[] {
    return Array.from(container.querySelectorAll<HTMLLIElement>("[role='option']"));
}

function getSelectedOption(container: HTMLElement): HTMLLIElement {
    return container.querySelector<HTMLLIElement>("[role='option'][aria-selected='true']")!;
}

function getHelpButton(container: HTMLElement): HTMLButtonElement {
    return container.querySelector<HTMLButtonElement>(".HelpButton .IconButton")!;
}

async function click(element: HTMLElement) {
    await act(async () => {
        element.click();
    });
}

async function focusAndType(input: HTMLInputElement, value: string) {
    await act(async () => input.focus());
    await typeText(input, value);
}

async function typeText(input: HTMLInputElement, value: string) {
    await act(async () => {
        input.value = value;
        getReactInputProps(input).onChange({ target: input });
    });
}

async function pressKey(input: HTMLInputElement, key: string) {
    await act(async () => {
        input.dispatchEvent(new KeyboardEvent("keydown", { key, bubbles: true, cancelable: true }));
    });
}

function setPanelDimensions(panel: HTMLUListElement, scrollHeight: number, clientHeight: number) {
    Object.defineProperty(panel, "scrollHeight", { configurable: true, value: scrollHeight });
    Object.defineProperty(panel, "clientHeight", { configurable: true, value: clientHeight });
}

async function scrollResults(panel: HTMLUListElement, scrollTop: number) {
    await act(async () => {
        panel.scrollTop = scrollTop;
        panel.dispatchEvent(new Event("scroll", { bubbles: true }));
        await flushPromises();
    });
}

function getReactInputProps(input: HTMLInputElement): {
    onChange: (event: { target: HTMLInputElement }) => void;
} {
    const propsKey = Object.keys(input).find(key => key.startsWith("__reactProps$"));
    if (!propsKey) {
        throw new Error("React input props were not found");
    }

    return (input as unknown as Record<string, {
        onChange: (event: { target: HTMLInputElement }) => void;
    }>)[propsKey];
}

async function advanceTimer(milliseconds: number) {
    await act(async () => {
        await vi.advanceTimersByTimeAsync(milliseconds);
        await flushPromises();
    });
}

function createResult(overrides: ApiSearchViewModel): ApiSearchViewModel {
    return overrides;
}

function createQueryResult(
    rows: ApiSearchViewModel[],
    overrides?: Partial<Omit<QueryResult<ApiSearchViewModel>, "rows">>
): QueryResult<ApiSearchViewModel> {
    return {
        pageIndex: 0,
        rows,
        totalRowCount: rows.length,
        rowsPerPage: 20,
        ...overrides,
    };
}

function deferred<T>() {
    let resolve!: (value: T) => void;
    let reject!: (reason?: unknown) => void;
    const promise = new Promise<T>((promiseResolve, promiseReject) => {
        resolve = promiseResolve;
        reject = promiseReject;
    });
    return { promise, resolve, reject };
}

async function flushPromises() {
    await Promise.resolve();
    await Promise.resolve();
}
