import ContentEdior from "@/components/contenteditor/ContentEditor";
import { ContentEditorValues } from "@/components/contenteditor/ContentEditorValues";
import PageContent_Context, { usePageContent_Provider } from "./PageContent_Provider";
import { BlockState } from "./models/BlockState";
import { shiftClient } from "@/api/shiftClient";
import { pageBlockAdapter } from "./pageBlockAdapter";
import { pageContentAdapter } from "./pageContentAdapter";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { ContentEditorResult } from "@/components/contenteditor/ContentEditorResult";
import { ApiPageContentModifyModel } from "@/api/controllers/pageContent/ApiPageContentModifyModel";

export default function PageContent() {
    return (
        <PageContent_Context>
            <PageContentInternal />
        </PageContent_Context>
    );
}

function PageContentInternal() {
    const { blocks, deletedBlockIds: removedBlockIds, initBlocks, setReadOnly } = usePageContent_Provider();
    const { setActionSubtitle } = useSiteProvider();

    return (
        <ContentEdior
            defaultValues={async id => {
                const [title, blocks, values] = await load(id);
                setActionSubtitle(title);
                initBlocks(blocks);
                return values;
            }}
            onSave={async (id, result) => {
                setReadOnly(true);
                try {
                    return await save(id, blocks, removedBlockIds, result);
                } finally {
                    setReadOnly(false);
                }
            }}
        />  
    );
}

async function save(id: string, blocks: BlockState[], removedBlockIds: string[], result: ContentEditorResult): Promise<boolean> {
    const model: ApiPageContentModifyModel = {
        Content: null,
        Blocks: null,
        DeletedBlockIds: removedBlockIds.length > 0 ? removedBlockIds : null,
    };

    pageContentAdapter.addModifiedContent(model, result);
    pageBlockAdapter.addModifiedBlocks(model, blocks);

    if (!model.Content && !model.Blocks && !model.DeletedBlockIds) {
        return false;
    }

    await shiftClient.pageContent.modify(id, model);

    return true;
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