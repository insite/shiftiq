import Alert from "@/components/Alert";
import Button from "@/components/Button";
import FormCard from "@/components/form/FormCard";
import { numberHelper } from "@/helpers/numberHelper";
import { translate } from "@/helpers/translate";
import { useDeleteForm } from "@/hooks/useDeleteForm";
import { ReactNode, useState } from "react";

interface DeleteModel {
    consequences: {
        name: string;
        count: number;
    }[]
}

interface Props<Model extends DeleteModel> {
    entityName: string;
    children: (model: Model | null) => ReactNode | null,
    onLoad: (id: string) => Promise<Model>,
    onDelete: (id: string) => Promise<boolean>,
}

export default function DeleteForm<Model extends DeleteModel>({
    entityName,
    children,
    onLoad,
    onDelete
}: Props<Model>) {
    const {
        model,
        backUrl,
        isSaving,
        handleDelete,
    } = useDeleteForm(onLoad, onDelete);

    const [isConfirmed, setIsConfirmed] = useState(false);

    return (
        <div className="d-flex gap-6 align-items-start">

            <div className="w-100">
                {children(model)}

                <Alert alertType="error">
                    <strong>Confirm:</strong> Are you absolutely certain you want to delete this {entityName} and all its related data?
                </Alert>

                <div className="mb-3">
                    <label>
                        <input
                            type="checkbox"
                            checked={isConfirmed}
                            disabled={!model || isSaving}
                            onChange={e => setIsConfirmed(e.target.checked)}
                        />
                        <span className="text-danger ms-1">
                            Yes, I understand the consequences, delete this {entityName}.
                        </span>
                    </label>
                </div>

                <div className="d-flex gap-2">
                    <Button
                        variant="delete"
                        disabled={!model || !isConfirmed}
                        isLoading={isSaving}
                        onClick={handleDelete}
                    />
                    <Button variant="cancel" href={backUrl} />
                </div>
            </div>

            <FormCard hasShadow={false} title={translate("Consequences")} className="w-100">
                <Alert alertType="warning">
                    <strong>Warning:</strong> This action <strong>cannot be undone</strong>.
                    This will permanently remove the {entityName} and its related data from all forms, queries, and reports.
                    Here is a summary of the data that will be removed if you proceed:
                </Alert>
                <table className="table table-striped table-bordered table-metrics">
                    <tbody>
                        <tr>
                            <th>Data</th>
                            <th className="text-end" style={{ width: "80px" }}>
                                Items
                            </th>
                        </tr>
                        {model && model.consequences.map(({ name, count }) => (
                            <tr key={name}>
                                <td>{name}</td>
                                <td className="text-end">
                                    {numberHelper.formatInt(count)}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </FormCard>

        </div>
    );
}