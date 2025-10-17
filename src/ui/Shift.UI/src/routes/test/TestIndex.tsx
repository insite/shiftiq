import Button from "@/components/Button";
import TestBasicControls from "./TestBasicControls";
import { Tab, Tabs } from "react-bootstrap";
import FormCard from "@/components/form/FormCard";
import TestValidation from "./TestValidation";
import TestNewFeatures from "./TestNewFeatures";
import TestFileUpload from "./TestFileUpload";
import TestRichTextEditor from "./TestRichTextEditor";

export default function TestIndex() {
    return (
        <>
            <Tabs defaultActiveKey="RichTextEditor" transition={false}>
                <Tab eventKey="BasicControls" title="Basic Controls">
                    <FormCard>
                        <form onSubmit={e => {
                            e.preventDefault();

                            for (const [name, value] of new FormData(e.currentTarget)) {
                                console.log(name, ":", value)
                            }
                        }}>
                            <TestBasicControls />

                            <div>
                                <Button variant="save" />
                            </div>
                        </form>
                    </FormCard>
                </Tab>
                <Tab eventKey="Validation" title="Validation">
                    <FormCard>
                        <TestValidation />
                    </FormCard>
                </Tab>
                <Tab eventKey="NewFeatures" title="New Features">
                    <FormCard>
                        <TestNewFeatures />
                    </FormCard>
                </Tab>
                <Tab eventKey="FileUpload" title="File Upload">
                    <FormCard>
                        <TestFileUpload />
                    </FormCard>
                </Tab>
                <Tab eventKey="RichTextEditor" title="Rich Text Editor">
                    <FormCard>
                        <TestRichTextEditor />
                    </FormCard>
                </Tab>
            </Tabs>
        </>
    )
}