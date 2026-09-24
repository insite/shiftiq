import { renderToStaticMarkup } from "react-dom/server";
import { beforeEach, describe, expect, test, vi } from "vitest";

interface MockEditorProps {
    initialValue?: string;
    init?: {
        convert_urls?: boolean;
    };
    onEditorChange?: (html: string) => void;
}

const editorMock = vi.hoisted(() => {
    const renderedProps: MockEditorProps[] = [];

    return {
        renderedProps,
        reset() {
            renderedProps.length = 0;
        },
        component: vi.fn((props: MockEditorProps) => {
            renderedProps.push(props);
            return null;
        }),
    };
});

vi.mock("@tinymce/tinymce-react", () => ({ Editor: editorMock.component }));

import RichTextEditor_Html from "../RichTextEditor_Html";

describe("RichTextEditor_Html", () => {
    beforeEach(() => {
        editorMock.reset();
        editorMock.component.mockClear();
    });

    test("disables URL conversion and forwards absolute URLs unchanged", () => {
        const absoluteUrl = "http://localhost:3000/files/sites/bcpvpa/news/bcpvpa-update-newexecutivedirectorannounced-july092026.pdf";
        const html = `<a href="${absoluteUrl}" target="_blank" rel="noopener">Download</a>`;
        const onChange = vi.fn();

        renderToStaticMarkup(
            <RichTextEditor_Html
                disabled={false}
                html={html}
                disableUploadFile={false}
                supportedImageFileTypes={[".png"]}
                onUploadFile={async () => null}
                onChange={onChange}
            />
        );

        const editorProps = editorMock.renderedProps[0];

        expect(editorProps.init?.convert_urls).toBe(false);
        expect(editorProps.initialValue).toBe(html);

        editorProps.onEditorChange?.(html);

        expect(onChange).toHaveBeenCalledWith(html);
    });
});
