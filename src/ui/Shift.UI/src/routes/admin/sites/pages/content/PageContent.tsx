import ContentEdior from "@/components/contenteditor/ContentEditor";
import { ContentEditorValues } from "@/components/contenteditor/ContentEditorValues";
import PageContent_Context, { usePageContent_Provider } from "./PageContent_Provider";
import { BlockState } from "./models/BlockState";
import { shiftClient } from "@/api/shiftClient";
import { pageBlockAdapter } from "./pageBlockAdapter";
import { pageContentAdapter } from "./pageContentAdapter";
import { ContentEditorResult } from "@/components/contenteditor/ContentEditorResult";
import { ApiPageContentModifyModel } from "@/api/controllers/pageContent/ApiPageContentModifyModel";
import { useSearchParams } from "react-router";
import { usePageProvider } from "@/contexts/page/PageProviderContext";
import { blockTypeNameList } from "./blockTypeNameList";

export default function PageContent() {
    return (
        <PageContent_Context>
            <PageContentInternal />
        </PageContent_Context>
    );
}

function PageContentInternal() {
    const { selectedBlockId, blocks, deletedBlockIds, isDirty, initBlocks, setReadOnly, clearDirty } = usePageContent_Provider();
    const [searchParams] = useSearchParams();
    const defaultSelectedBlockId = searchParams.get("block");
    const { setActionSubtitle, setBreadcrumbItemPath } = usePageProvider();

    function getBackUrl(pageId: string, selectedFieldName: string): string {
        let url = `/ui/admin/sites/pages/outline?id=${pageId}&panel=content&tab=${encodeURIComponent(selectedFieldName)}`;
        if (typeof selectedBlockId === "string") {
            const selectedBlock = blocks.find(x => x.blockId === selectedBlockId)!;
            const blockTitle = selectedBlock.blockTitle || blockTypeNameList[selectedBlock.blockType];
            url += `&nav=${encodeURIComponent(blockTitle)}`;
        }
        return url;
    }

    async function handleSave(pageId: string, result: ContentEditorResult): Promise<boolean> {
        let replacedBlockIds: Record<number, string> | null;

        setReadOnly(true);
        try {
            replacedBlockIds = await save(pageId, blocks, deletedBlockIds, result);
        } finally {
            setReadOnly(false);
        }

        if (!replacedBlockIds) {
            return false;
        }

        clearDirty(replacedBlockIds);
        return true;
    }

    function handleTabSelected(pageId: string, selectedFieldName: string) {
        setBreadcrumbItemPath("/ui/admin/sites/pages/outline", getBackUrl(pageId, selectedFieldName));
    }

    return (
        <ContentEdior
            isDirty={isDirty}
            defaultValues={async id => {
                const [title, blocks, values] = await load(id);
                setActionSubtitle(title);
                initBlocks(blocks, defaultSelectedBlockId?.toLowerCase() ?? null);
                return values;
            }}
            onSave={handleSave}
            onGetBackUrl={getBackUrl}
            onTabSelected={handleTabSelected}
        />  
    );
}

async function save(id: string, blocks: BlockState[], deletedBlockIds: string[], result: ContentEditorResult): Promise<Record<number, string> | null> {
    const model: ApiPageContentModifyModel = {
        Content: null,
        Blocks: null,
        DeletedBlockIds: deletedBlockIds.length > 0 ? deletedBlockIds : null,
    };

    pageContentAdapter.addModifiedContent(model, result);
    pageBlockAdapter.addModifiedBlocks(model, blocks);

    if (!model.Content && !model.Blocks && !model.DeletedBlockIds) {
        return null;
    }

    return await shiftClient.pageContent.modify(id, model);
}

async function load(id: string): Promise<[string, BlockState[], ContentEditorValues]> {
    const model = await shiftClient.pageContent.retrieve(id);
    if (!model) {
        throw new Error(`Page with ID = ${id} does not exist`);
    }

    const blocks = pageBlockAdapter.getBlocks(model);
    const values = pageContentAdapter.getContentEditorValues(model);

    return [model.Title, blocks, values];
}