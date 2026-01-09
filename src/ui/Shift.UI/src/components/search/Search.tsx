import { createContext, ReactNode, useCallback, useContext, useEffect, useState } from "react";
import { CacheKey } from "@/cache/cacheKey";
import { useStatusProvider } from "@/contexts/StatusProvider";
import { useLoadingProvider } from "@/contexts/LoadingProvider";
import { translate } from "@/helpers/translate";
import { GridColumn } from "./Grid";
import { BaseCriteria } from "./BaseCriteria";
import Search_Tabs from "./Search_Tabs";
import { useSearchDefaults } from "./useSearchDefaults";
import { SelectedTab } from "@/models/enums";
import { QueryResult } from "@/models/QueryResult";

export interface SearchContextData<Criteria extends BaseCriteria, Row extends object> {
    cacheKey: CacheKey | null;
    selectedTab: SelectedTab;
    columns: GridColumn[] | null;
    pageIndex: number;
    criteria: Criteria;
    defaultCriteria: Criteria;
    rows: Row[] | null;
    totalRowCount: number;
    rowsPerPage: number;
    isLoading: boolean;
    setPageIndex: (pageIndex: number) => void;
    setCriteria: (criteria: Criteria) => void;
    setSelectedTab: (selectedTab: SelectedTab) => void;
    setColumns: (columns: GridColumn[]) => void;
}

const SearchContext = createContext<SearchContextData<BaseCriteria, object>>({
    cacheKey: null,
    selectedTab: "result",
    columns: null,
    pageIndex: -1,
    criteria: {
        visibleColumns: [],
        sortByColumn: "",
    },
    defaultCriteria: {
        visibleColumns: [],
        sortByColumn: "",
    },
    rows: null,
    totalRowCount: -1,
    rowsPerPage: -1,
    isLoading: false,
    setPageIndex: () => {},
    setCriteria: () => {},
    setSelectedTab: () => {},
    setColumns: () => {},
});

interface Props<Criteria extends BaseCriteria, Row extends object> {
    cacheKey?: CacheKey;
    defaultSelectedTab?: SelectedTab;
    defaultPageIndex?: number;
    defaultCriteria: Criteria;
    resultElement: ReactNode;
    criteriaElement: ReactNode;
    downloadElement: ReactNode;
    onLoad: (pageIndex: number, criteria: Criteria, contextData: SearchContextData<Criteria, Row> | null) => Promise<QueryResult<Row>>;
}

export default function Search<Criteria extends BaseCriteria, Row extends object>({
    cacheKey,
    defaultSelectedTab,
    defaultPageIndex,
    defaultCriteria,
    resultElement,
    criteriaElement,
    downloadElement,
    onLoad
}: Props<Criteria, Row>)
{
    const { addError, removeError } = useStatusProvider();
    const { addLoading, removeLoading } = useLoadingProvider();

    const defaults = useSearchDefaults<Criteria, Row>(cacheKey, defaultSelectedTab, defaultPageIndex, defaultCriteria);

    const [selectedTab, setSelectedTab] = useState(defaults.selectedTab);
    const [columns, setColumns] = useState<GridColumn[] | null>(null);
    const [pageIndex, setPageIndex] = useState(-1);
    const [criteria, setCriteria] = useState(defaultCriteria);

    const [contextData, setContextData] = useState<SearchContextData<Criteria, Row>>({
        cacheKey: cacheKey ?? null,
        selectedTab: defaults.selectedTab,
        columns: null,
        pageIndex: defaults.pageIndex,
        criteria: defaults.criteria,
        defaultCriteria,
        rows: defaults.rows,
        totalRowCount: defaults.totalRowCount,
        rowsPerPage: defaults.rowsPerPage,
        isLoading: false,
        setPageIndex,
        setCriteria,
        setSelectedTab,
        setColumns,
    });

    // Preventing possibility of onLoad to be changed later
    // Otherwise if onLoad is dynamic then 'load' method will be recreated on each re-render, which is not good on the component initialization
    // because initial data will be loaded about twice but should be once
    const [storedOnLoad] = useState(() => onLoad);

    const load = useCallback(async (loadPageIndex: number, loadCriteria: Criteria, loadContextData: SearchContextData<Criteria, Row> | null) => {
        const { pageIndex: newPageIndex, rows, totalRowCount, rowsPerPage } = await storedOnLoad(loadPageIndex, loadCriteria, loadContextData);

        const newSelectedTab: SelectedTab = rows.length ? "result" : "criteria";

        setPageIndex(newPageIndex);
        setCriteria(loadCriteria);
        setSelectedTab(newSelectedTab);

        setContextData(prev => ({
            cacheKey: prev.cacheKey,
            selectedTab: newSelectedTab,
            columns: prev.columns,
            pageIndex: newPageIndex,
            criteria: loadCriteria,
            defaultCriteria: prev.defaultCriteria,
            rows,
            totalRowCount,
            rowsPerPage,
            isLoading: false,
            setPageIndex,
            setCriteria,
            setSelectedTab,
            setColumns,
        }));

        defaults.setData(
            newPageIndex,
            loadCriteria,
            rows,
            totalRowCount,
            rowsPerPage,
        );
    }, [storedOnLoad, defaults]);

    // Initial data load
    useEffect(() => {
        if (defaults.rows) {
            setPageIndex(defaults.pageIndex);
            setCriteria(defaults.criteria);
            return;
        }

        run();

        async function run() {
            addLoading();

            try {
                await load(defaults.pageIndex, defaults.criteria, null);
                removeError();
            } catch (e) {
                addError(e, translate("Failed to load data"));
            }

            removeLoading();
        };
    }, [addLoading, removeLoading, addError, removeError, load, defaults]);

    // Loading data when the criteria or the pageIndex were changed
    useEffect(() => {
        if (pageIndex < 0
            || contextData.isLoading
            || pageIndex === contextData.pageIndex && criteria === contextData.criteria
        ) {
            return;
        }

        const newPageIndex = criteria === contextData.criteria ? pageIndex : 0;

        setContextData({...contextData, isLoading: true});

        run();

        async function run () {
            try {
                await load(newPageIndex, criteria, contextData);
                removeError();
            } catch (e) {
                addError(e, translate("Failed to load data"));
                setContextData({...contextData, isLoading: false});
                setPageIndex(contextData.pageIndex);
                setCriteria(contextData.criteria);
            }
        }
    }, [load, addError, removeError, pageIndex, criteria, contextData]);

    useEffect(() => {
        setContextData(prev => {
            return prev.selectedTab === selectedTab
                ? prev
                : {
                    ...prev,
                    selectedTab
                };
        });
        defaults.setSelectedTab(selectedTab);
    }, [selectedTab, defaults]);

    useEffect(() => {
        setContextData(prev => {
            return prev.columns === columns
                ? prev
                : {
                    ...prev,
                    columns
                };
        });
    }, [columns]);

    return (
        <SearchContext.Provider value={contextData as unknown as SearchContextData<BaseCriteria, object>}>
            <Search_Tabs
                selectedTab={contextData.selectedTab}
                resultElement={resultElement}
                criteriaElement={criteriaElement}
                downloadElement={downloadElement}
                totalRowCount={contextData.totalRowCount}
                onSelectTab={setSelectedTab}
            />
        </SearchContext.Provider>
    );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useSearch<Criteria extends BaseCriteria, Row extends object>(): SearchContextData<Criteria, Row> {
    return useContext(SearchContext) as unknown as SearchContextData<Criteria, Row>;
}