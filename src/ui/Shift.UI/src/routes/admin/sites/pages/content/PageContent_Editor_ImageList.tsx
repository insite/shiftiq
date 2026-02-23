import "./PageContent_Editor_ImageList.css";

import { Fragment } from "react";
import Button from "@/components/Button";
import { BlockFieldName } from "./models/BlockFieldName";
import Icon from "@/components/icon/Icon";
import TextBox from "@/components/TextBox";
import FileTextBox from "@/components/filetextbox/FileTextBox";
import { translate } from "@/helpers/translate";
import { BlockState } from "./models/BlockState";
import { BlockImageValue } from "./models/BlockImageValue";
import { usePageContent_Provider } from "./PageContent_Provider";
import Alert from "@/components/Alert";

interface Props {
    block: BlockState;
    fieldName: BlockFieldName;
}

export default function PageContent_Editor_ImageList({ block, fieldName }: Props) {
    const field = block.contentFields.find(x => x.fieldName === fieldName);
    const images: BlockImageValue[] | undefined = field?.fieldValue as BlockImageValue[];

    const { readOnly, modifyBlockField } = usePageContent_Provider();

    function handleAddImage() {
        const newImages = images ? [...images] : [];
        newImages.push({
            key: newImages.reduce((max, cur) => max < cur.key ? cur.key : max, 0) + 1,
            alt: null,
            url: null,
        });
        modifyBlockField(block.blockId, fieldName, newImages);
    }

    function handleModifyAlt(index: number, alt: string) {
        const newImages = [...images!];
        newImages![index].alt = alt;
        modifyBlockField(block.blockId, fieldName, newImages);
    }

    function handleModifyUrl(index: number, url: string) {
        const newImages = [...images!];
        newImages![index].url = url;
        modifyBlockField(block.blockId, fieldName, newImages);
    }

    function handleDelete(index: number) {
        const image = images![index];
        if ((image.alt || image.url)
            && !window.confirm(`Are you sure to delete image N${index + 1}?`)
        ) {
            return;
        }

        const newImages = [...images!];
        newImages.splice(index, 1);
        modifyBlockField(block.blockId, fieldName, newImages);
    }

    return (
        <div className="form-group mb-3">
            <label className="form-label">
                {fieldName}
                <button
                    type="button"
                    title={translate("Add Image")}
                    className="btn btn-sm btn-icon btn-light ms-2"
                    disabled={readOnly}
                    onClick={handleAddImage}
                >
                    <Icon style="Solid" name="plus-circle" />
                </button>
            </label>
            {images?.length ? (
                <div className="PageContent_Editor_ImageList">
                    {images.map((v, index) => (
                        <Fragment key={v.key}>
                            {index + 1}.
                            <div className="d-flex gap-2">
                                <TextBox
                                    placeholder={translate("Img Alt Text")}
                                    className="w-25"
                                    maxLength={64}
                                    defaultValue={v.alt ?? ""}
                                    readOnly={readOnly}
                                    onBlur={e => handleModifyAlt(index, e.target.value)}
                                />
                                <FileTextBox
                                    placeholder={translate("Img URL")}
                                    className="w-75"
                                    maxLength={512}
                                    defaultValue={v?.url ?? ""}
                                    readOnly={readOnly}
                                    onBlur={value => handleModifyUrl(index, value)}
                                />
                            </div>
                            <Button
                                variant="delete-icon"
                                type="button"
                                disabled={readOnly}
                                onClick={() => handleDelete(index)}
                            />
                        </Fragment>
                    ))}
                </div>
            ) : (
                <Alert alertType="information">
                    {translate("There are no added images. Click the plus button to add an image.")}
                </Alert>
            )}
        </div>
    );
}