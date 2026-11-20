import "./AdminHomeLayout_Navigation_Search.css";

import {
    ChangeEvent,
    FocusEvent,
    KeyboardEvent,
    UIEvent,
    useEffect,
    useId,
    useLayoutEffect,
    useRef,
    useState
} from "react";
import { Spinner } from "react-bootstrap";
import { shiftClient } from "@/api/shiftClient";
import { ApiSearchViewModel } from "@/api/controllers/searches/ApiSearchViewModel";
import ActionLink from "@/components/ActionLink";
import Icon from "@/components/icon/Icon";
import { IconName } from "@/components/icon/IconName";
import { QueryResult } from "@/models/QueryResult";
import HelpButton from "@/components/helpbutton/HelpButton";

const _searchDelayMs = 300;
const _scrollBottomThresholdPx = 1;

interface ObjectTypeDescr {
    iconName: IconName;
    className: string;
    name?: string;
}

const _objectTypes: Record<string, ObjectTypeDescr> = {
    Person: { iconName: "user", className: "bg-info" },
    Group: { iconName: "users", className: "bg-success" },
    Bank: { iconName: "balance-scale", className: "bg-success" },
    Course: { iconName: "users-class", className: "bg-success" },
    Event: { iconName: "calendar-alt", className: "bg-success" },
    Form: { iconName: "check-square", className: "bg-success" },
    Achievement: { iconName: "trophy", className: "bg-success" },
    Gradebook: { iconName: "spell-check", className: "bg-success" },
    Logbook: { iconName: "pencil-ruler", className: "bg-success" },
    Program: { iconName: "graduation-cap", className: "bg-success" },
    Standard: { iconName: "ruler-triangle", className: "bg-success" },
    PlatformApp: { iconName: "toolbox", className: "bg-primary", name: "Platform App" },
    App: { iconName: "location", className: "bg-secondary2" },
};

const _defaultObjectType: ObjectTypeDescr = { iconName: "question", className: "bg-warning" };

interface Props {
    className?: string;
}

export default function AdminHomeLayout_Navigation_Search({ className }: Props) {
    const generatedId = useId().replaceAll(":", "");
    const listboxId = `${generatedId}-search-results`;

    const [searchText, setSearchText] = useState("");
    const [results, setResults] = useState<ApiSearchViewModel[]>([]);
    const [hasCompletedSearch, setHasCompletedSearch] = useState(false);
    const [selectedIndex, setSelectedIndex] = useState(0);
    const [isOpen, setIsOpen] = useState(false);
    const [loadedQuery, setLoadedQuery] = useState("");
    const [pageIndex, setPageIndex] = useState(-1);
    const [hasMoreResults, setHasMoreResults] = useState(false);
    const [isLoadingNextPage, setIsLoadingNextPage] = useState(false);

    const containerRef = useRef<HTMLDivElement>(null);
    const resultsPanelRef = useRef<HTMLUListElement>(null);
    const optionRefs = useRef<Array<HTMLLIElement | null>>([]);
    const hasFocusWithinRef = useRef(false);
    const isDismissedRef = useRef(false);
    const searchGenerationRef = useRef(0);
    const nextPageRequestRef = useRef<object | null>(null);
    const shouldResetScrollRef = useRef(false);
    const isKeyboardNavigationRef = useRef(false);

    useEffect(() => {
        const query = searchText.trim();
        if (!query) {
            return;
        }

        let isCancelled = false;
        const searchGeneration = searchGenerationRef.current;
        const timeoutId = setTimeout(async () => {
            try {
                const response = await shiftClient.search.search(query, 0);
                if (isCancelled || searchGeneration !== searchGenerationRef.current) {
                    return;
                }

                const nextResults = response?.rows ?? [];
                optionRefs.current = [];
                shouldResetScrollRef.current = true;
                setResults(nextResults);
                setLoadedQuery(query);
                setPageIndex(response?.pageIndex ?? 0);
                setHasMoreResults(canLoadMore(response, nextResults.length));
                setHasCompletedSearch(true);
                setSelectedIndex(0);
                setIsOpen(hasFocusWithinRef.current && !isDismissedRef.current);
            } catch {
                if (!isCancelled && searchGeneration === searchGenerationRef.current) {
                    optionRefs.current = [];
                    setResults([]);
                    setLoadedQuery("");
                    setPageIndex(-1);
                    setHasMoreResults(false);
                    setHasCompletedSearch(false);
                    setSelectedIndex(0);
                    setIsOpen(false);
                }
            }
        }, _searchDelayMs);

        return () => {
            isCancelled = true;
            clearTimeout(timeoutId);
        };
    }, [searchText]);

    useEffect(() => () => {
        searchGenerationRef.current += 1;
        nextPageRequestRef.current = null;
    }, []);

    useEffect(() => {
        function handleMouseDown(event: MouseEvent) {
            if (event.target instanceof Node && !containerRef.current?.contains(event.target)) {
                hasFocusWithinRef.current = false;
                isDismissedRef.current = true;
                setIsOpen(false);
            }
        }

        document.addEventListener("mousedown", handleMouseDown);
        return () => document.removeEventListener("mousedown", handleMouseDown);
    }, []);

    useEffect(() => {
        if (!isOpen || !resultsPanelRef.current) {
            return;
        }

        const isKeyboardNavigation = isKeyboardNavigationRef.current;
        isKeyboardNavigationRef.current = false;
        const optionCount = resultsPanelRef.current.querySelectorAll("[role='option']").length;

        if (isKeyboardNavigation && selectedIndex === 0) {
            resultsPanelRef.current.scrollTop = 0;
        } else if (isKeyboardNavigation && selectedIndex === optionCount - 1) {
            resultsPanelRef.current.scrollTop = Math.max(
                0,
                resultsPanelRef.current.scrollHeight - resultsPanelRef.current.clientHeight
            );
        } else {
            optionRefs.current[selectedIndex]?.scrollIntoView?.({ block: "nearest" });
        }
    }, [isOpen, selectedIndex]);

    useLayoutEffect(() => {
        if (shouldResetScrollRef.current && hasCompletedSearch && results.length > 0 && resultsPanelRef.current) {
            resultsPanelRef.current.scrollTop = 0;
            shouldResetScrollRef.current = false;
        }
    }, [hasCompletedSearch, results]);

    function handleChange(event: ChangeEvent<HTMLInputElement>) {
        const nextSearchText = event.target.value;

        searchGenerationRef.current += 1;
        nextPageRequestRef.current = null;
        shouldResetScrollRef.current = false;
        isDismissedRef.current = false;
        setSearchText(nextSearchText);
        setSelectedIndex(0);
        setLoadedQuery("");
        setPageIndex(-1);
        setHasMoreResults(false);
        setIsLoadingNextPage(false);

        if (!nextSearchText.trim()) {
            optionRefs.current = [];
            setResults([]);
            setHasCompletedSearch(false);
            setIsOpen(false);
        } else if (hasCompletedSearch && hasFocusWithinRef.current) {
            setIsOpen(true);
        }
    }

    async function handleResultsScroll(event: UIEvent<HTMLUListElement>) {
        const panel = event.currentTarget;
        const distanceFromBottom = panel.scrollHeight - panel.scrollTop - panel.clientHeight;
        if (distanceFromBottom > _scrollBottomThresholdPx) {
            return;
        }

        const query = searchText.trim();
        if (!hasCompletedSearch
            || !query
            || loadedQuery !== query
            || !hasMoreResults
            || nextPageRequestRef.current
        ) {
            return;
        }

        const requestToken = {};
        const searchGeneration = searchGenerationRef.current;
        const nextPageIndex = pageIndex + 1;
        nextPageRequestRef.current = requestToken;
        setIsLoadingNextPage(true);

        try {
            const response = await shiftClient.search.search(query, nextPageIndex);
            if (nextPageRequestRef.current !== requestToken || searchGeneration !== searchGenerationRef.current) {
                return;
            }

            const nextResults = response?.rows ?? [];
            const loadedResultCount = results.length + nextResults.length;
            setResults(currentResults => [...currentResults, ...nextResults]);
            setPageIndex(response?.pageIndex ?? nextPageIndex);
            setHasMoreResults(canLoadMore(response, loadedResultCount));
        } catch {
            // Keep the current results and allow another bottom scroll to retry this page.
        } finally {
            if (nextPageRequestRef.current === requestToken) {
                nextPageRequestRef.current = null;
                setIsLoadingNextPage(false);
            }
        }
    }

    function handleFocus() {
        hasFocusWithinRef.current = true;
        isDismissedRef.current = false;
        if (hasCompletedSearch) {
            setIsOpen(true);
        }
    }

    function handleBlur(event: FocusEvent<HTMLDivElement>) {
        if (!(event.relatedTarget instanceof Node) || !event.currentTarget.contains(event.relatedTarget)) {
            hasFocusWithinRef.current = false;
            isDismissedRef.current = true;
            setIsOpen(false);
        }
    }

    function handleKeyDown(event: KeyboardEvent<HTMLInputElement>) {
        if (event.key === "Escape") {
            event.preventDefault();
            isDismissedRef.current = true;
            setIsOpen(false);
            return;
        }

        if (results.length === 0) {
            return;
        }

        if (event.key === "ArrowDown") {
            event.preventDefault();
            isDismissedRef.current = false;
            setIsOpen(true);
            const nextSelectedIndex = Math.min(selectedIndex + 1, results.length - 1);
            isKeyboardNavigationRef.current = nextSelectedIndex !== selectedIndex;
            setSelectedIndex(nextSelectedIndex);
        } else if (event.key === "ArrowUp") {
            event.preventDefault();
            isDismissedRef.current = false;
            setIsOpen(true);
            const nextSelectedIndex = Math.max(selectedIndex - 1, 0);
            isKeyboardNavigationRef.current = nextSelectedIndex !== selectedIndex;
            setSelectedIndex(nextSelectedIndex);
        } else if (event.key === "Enter" && isOpen) {
            event.preventDefault();
            optionRefs.current[selectedIndex]?.querySelector<HTMLAnchorElement>("a")?.click();
        }
    }

    const selectedOptionId = isOpen && results.length > 0
        ? `${listboxId}-${selectedIndex}`
        : undefined;

    return (
        <>
            <div
                ref={containerRef}
                className={`AdminHomeLayout_Navigation_Search navbar-nav-left d-none d-lg-block ${className ?? ""}`}
                onFocus={handleFocus}
                onBlur={handleBlur}
            >
                <label className="search-control">
                    <span className="visually-hidden">Search Shift iQ</span>
                    <Icon style="solid" name="search" className="search-icon" />
                    <input
                        type="search"
                        className="form-control insite-text"
                        placeholder="Search Shift iQ"
                        value={searchText}
                        maxLength={50}
                        autoComplete="off"
                        autoCorrect="off"
                        autoCapitalize="none"
                        spellCheck={false}
                        role="combobox"
                        aria-autocomplete="list"
                        aria-expanded={isOpen}
                        aria-controls={listboxId}
                        aria-activedescendant={selectedOptionId}
                        onChange={handleChange}
                        onKeyDown={handleKeyDown}
                    />
                </label>

                {isOpen && hasCompletedSearch && results.length > 0 && (
                    <ul
                        ref={resultsPanelRef}
                        id={listboxId}
                        className="search-results shadow-lg"
                        role="listbox"
                        aria-busy={isLoadingNextPage}
                        onScroll={handleResultsScroll}
                    >
                        {results.map((result, index) => {
                            const optionId = `${listboxId}-${index}`;
                            const isSelected = index === selectedIndex;
                            const objectTypeDescr = _objectTypes[result.ObjectType] ?? _defaultObjectType;
                            const subtitle = [objectTypeDescr.name ?? result.ObjectType, ...(result.Attributes ?? [])]
                                .filter(value => value?.trim())
                                .join(" • ");

                            return (
                                <li
                                    ref={element => { optionRefs.current[index] = element; }}
                                    id={optionId}
                                    key={`${result.ObjectType}-${result.ObjectId}`}
                                    className={`search-result ${isSelected ? "active" : ""}`}
                                    role="option"
                                    aria-selected={isSelected}
                                >
                                    <span className={`result-icon ${objectTypeDescr.className}`}>
                                        <Icon style="solid" name={objectTypeDescr.iconName} />
                                    </span>
                                    <span className="result-content">
                                        <ActionLink href={result.Url} className="result-title">
                                            {result.Title}
                                        </ActionLink>
                                        <span className="result-subtitle">{subtitle}</span>
                                    </span>
                                </li>
                            );
                        })}
                        {isLoadingNextPage && (
                            <li className="loading-results" role="status">
                                <Spinner animation="border" size="sm" aria-hidden="true" />
                                <span>Loading more results...</span>
                            </li>
                        )}
                    </ul>
                )}

                {isOpen && hasCompletedSearch && results.length === 0 && (
                    <div id={listboxId} className="search-results shadow-lg" role="status">
                        <div className="no-results">No search results found</div>
                    </div>
                )}
            </div>

            <HelpButton className="d-none d-lg-block ms-2">
                <div className="m-2">
                    <b>Global Search</b>
                    <div className="mt-2 mb-2">
                        Search using keywords to find your Apps and manage your content and contacts across Shift iQ.
                    </div>
                    <div>
                        Combine words to find more specific results, such as <i className="bg-secondary px-2">create event</i>,
                        <i className="bg-secondary px-2">view gradebooks</i>, a course or assessment name, or a contact's first and last name.
                    </div>
                </div>
            </HelpButton>
        </>
    );
}

function canLoadMore(response: QueryResult<ApiSearchViewModel> | null, loadedResultCount: number) {
    if (!response
        || response.rowsPerPage <= 0
        || response.rows.length < response.rowsPerPage
    ) {
        return false;
    }

    return response.totalRowCount >= 0
        ? loadedResultCount < response.totalRowCount
        : response.rows.length >= response.rowsPerPage;
}
