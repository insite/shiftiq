import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import ComboBox from "@/components/combobox/ComboBox";
import FormCard from "@/components/form/FormCard";
import FormField from "@/components/form/FormField";
import TextArea from "@/components/TextArea";
import TextBox from "@/components/TextBox";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";
import { useState } from "react";

export default function TestAggregate() {
    const [isRequesting, setIsRequesting] = useState(false);
    const [aggregateId, setAggregateId] = useState("7f919c01-adf4-b771-973e-06aaa9618a26");
    const [aggregateState, setAggregateState] = useState("");

    const [isModifyingQuestion, setIsModifyingQuestion] = useState(false);
    const [questionId, setQuestionId] = useState("13de9c01-19c2-f976-9123-631cfa73c932");
    const [questionField, setQuestionField] = useState("Title");
    const [questionColumnIndex, setQuestionColumnIndex] = useState("");
    const [questionValue, setQuestionValue] = useState("Question N1<br/>![Distro2](https://local-ita.insite.com/files/Assessments/261425/Attachments/Distro2.jpg)");
    const [questionResult, setQuestionResult] = useState("");

    const [isModifyingOption, setIsModifyingOption] = useState(false);
    const [optionNumber, setOptionNumber] = useState("37");
    const [optionField, setOptionField] = useState("Title");
    const [optionColumnIndex, setOptionColumnIndex] = useState("");
    const [optionValue, setOptionValue] = useState("**Option 1**");
    const [optionResult, setOptionResult] = useState("");

    const { addError, removeError } = useStatusProvider();

    async function handleClick() {
        setIsRequesting(true);

        try {
            const result = await shiftClient.timeline.retrieve(aggregateId, "BankAggregate");
            setAggregateState(result ? JSON.stringify(result, null, 2) : "Not Found");

            removeError();
        } catch (err) {
            addError(err);
        } finally {
            setIsRequesting(false);
        }
    }

    async function handlModifyQuestion() {
        if (!questionField) {
            window.alert("Question field is empty");
            return;
        }

        let columnIndex: number | null = parseInt(questionColumnIndex);
        if (isNaN(columnIndex)) {
            columnIndex = null;
        }

        setIsModifyingQuestion(true);

        try {
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            const result = await shiftClient.workshop.modifyQuestion(aggregateId, questionId, questionField as any, columnIndex, questionValue);
            setQuestionResult(result ?? "");

            removeError();
        } catch (err) {
            addError(err);
        } finally {
            setIsModifyingQuestion(false);
        }
    }

    async function handlModifyOption() {
        let columnIndex: number | null = parseInt(optionColumnIndex);
        if (isNaN(columnIndex)) {
            columnIndex = null;
        }

        setIsModifyingOption(true);

        try {
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            const result = await shiftClient.workshop.modifyOption(aggregateId, questionId, parseInt(optionNumber), optionField as any, columnIndex, optionValue);
            setOptionResult(result ?? "");

            removeError();
        } catch (err) {
            addError(err);
        } finally {
            setIsModifyingOption(false);
        }
    }

    return (
        <div className="d-flex gap-4">
            <div style={{ flex: "1 1 0" }}>
                <FormCard>
                    <div className="d-flex mb-3 gap-2">
                        <TextBox
                            value={aggregateId}
                            className="d-inline w-50"
                            onChange={e => setAggregateId(e.target.value)}
                        />

                        <Button
                            type="button"
                            variant="request"
                            isLoading={isRequesting}
                            text="Request BankAggregate State"
                            onClick={handleClick}
                        />
                    </div>
                    <div>
                        <TextArea
                            readOnly
                            rows={20}
                            value={aggregateState}
                        />
                    </div>
                </FormCard>
            </div>
            <div style={{ flex: "1 1 0" }}>
                <FormCard>
                    <div className="d-flex gap-4">
                        <div style={{ flex: "1 1 0" }}>
                            <FormField label="Question Id">
                                <TextBox
                                    value={questionId}
                                    onChange={e => setQuestionId(e.target.value)}
                                />
                            </FormField>
                            <FormField label="Field">
                                <ComboBox
                                    items={[
                                        { value: "", text: "" },
                                        { value: "Title", text: "Title" },
                                        { value: "Code", text: "Code" },
                                        { value: "Flag", text: "Flag" },
                                    ]}
                                    value={questionField}
                                    onChange={value => setQuestionField(value ?? "")}
                                />
                            </FormField>
                            <FormField label="ColumnIndex">
                                <TextBox
                                    value={questionColumnIndex}
                                    onChange={e => setQuestionColumnIndex(e.target.value)}
                                />
                            </FormField>
                            <FormField label="Value">
                                <TextBox
                                    value={questionValue}
                                    onChange={e => setQuestionValue(e.target.value)}
                                />
                            </FormField>

                            <Button
                                type="button"
                                variant="request"
                                isLoading={isModifyingQuestion}
                                text="Modify Question"
                                onClick={handlModifyQuestion}
                            />

                            <FormField label="Result" className="mt-3">
                                <TextBox value={questionResult} readOnly />
                            </FormField>
                        </div>
                        <div style={{ flex: "1 1 0" }}>
                            <FormField label="Option Number">
                                <TextBox
                                    value={optionNumber}
                                    onChange={e => setOptionNumber(e.target.value)}
                                />
                            </FormField>
                            <FormField label="Field">
                                <ComboBox
                                    items={[
                                        { value: "", text: "" },
                                        { value: "Title", text: "Title" },
                                        { value: "ColumnTitle", text: "ColumnTitle" },
                                        { value: "Points", text: "Points" },
                                    ]}
                                    value={optionField}
                                    onChange={value => setOptionField(value ?? "")}
                                />
                            </FormField>
                            <FormField label="ColumnIndex">
                                <TextBox
                                    value={optionColumnIndex}
                                    onChange={e => setOptionColumnIndex(e.target.value)}
                                />
                            </FormField>
                            <FormField label="Value">
                                <TextBox
                                    value={optionValue}
                                    onChange={e => setOptionValue(e.target.value)}
                                />
                            </FormField>

                            <Button
                                type="button"
                                variant="request"
                                isLoading={isModifyingOption}
                                text="Modify Option"
                                onClick={handlModifyOption}
                            />

                            <FormField label="Result" className="mt-3">
                                <TextBox value={optionResult} readOnly />
                            </FormField>
                        </div>
                    </div>
                </FormCard>
            </div>
        </div>
    );
}