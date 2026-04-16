import FormCard from "@/components/form/FormCard";
import { Nav, Tab } from "react-bootstrap";
import PageContent_Blocks_Edit from "./PageContent_Blocks_Edit";
import PageContent_Blocks_New from "./PageContent_Blocks_New";
import { usePageContent_Provider } from "./PageContent_Provider";
import { translate } from "@/helpers/translate";
import { blockTypeNameList } from "./blockTypeNameList";

export default function PageContent_Blocks() {
    const { blocks, selectedBlockId, selectBlock } = usePageContent_Provider();

    return (
        <Tab.Container
            transition={false}
            activeKey={selectedBlockId ?? "new"}
            onSelect={key => selectBlock(key === "new" ? null : key)}
        >
            <div className="row">
                <div className="col-md-2">
                    <Nav variant="pills" className="flex-column mb-0">
                        {blocks.map(({ blockId, blockTitle, blockType }) => (
                            <Nav.Item key={blockId}>
                                <Nav.Link eventKey={blockId} className="text-center">
                                    {blockTitle || blockTypeNameList[blockType]}
                                </Nav.Link>
                            </Nav.Item>
                        ))}
                        <Nav.Item>
                            <Nav.Link eventKey="new">
                                {translate("New")}
                            </Nav.Link>
                        </Nav.Item>
                    </Nav>
                </div>
                <div className="col-md-10 align-items-stretch">
                    <FormCard
                        hasShadow={false}
                        hasBottomMargin={false}
                        className="h-100"
                        bodyClassName="py-3"
                    >
                        <Tab.Content>
                            {blocks.map(b => (
                                <Tab.Pane key={b.blockId} eventKey={b.blockId}>
                                    <PageContent_Blocks_Edit block={b} selected={selectedBlockId === b.blockId} />
                                </Tab.Pane>
                            ))}
                            <Tab.Pane eventKey="new">
                                <PageContent_Blocks_New selected={selectedBlockId === null} />
                            </Tab.Pane>
                        </Tab.Content>
                    </FormCard>
                </div>
            </div>
        </Tab.Container>
    );
}