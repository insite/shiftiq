import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

const pageId = "b1b1d7da-8d09-42cb-b25b-6d3a17637462";

test("GET /api/workspace/pages-contents/<pageId>: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.pageContent.retrieve(pageId)).rejects.toThrowError(new ApiError(401, ""));
});

test("GET /api/workspace/pages-contents/<pageId>: authenticated", async () => {
    await global.login();

    const page = await shiftClient.pageContent.retrieve(pageId);

    expect(page).not.toBe(null);
    expect(page!.Title).toBe("The test page for React");
    expect(page!.Content["Body"]["Html"]?.en).toBe("The test body (HTML)");
    expect(page!.Content["Body"]["Text"]?.en).toBe("The test body (Text)");
    expect(page!.Content["Title"]["Text"]?.en).toBe("The test page for React");
    expect(page!.Blocks.length).toBeGreaterThanOrEqual(9);

    expect(page!.Blocks[0].BlockId).toBeTypeOf("string");
    expect(page!.Blocks[0].Title).toBe("Block 1");
    expect(page!.Blocks[0].Hook).toBe("Test hook 1");
    expect(page!.Blocks[0].BlockType).toBe("HeadingAndParagraphs");
    expect(page!.Blocks[0].Content["Heading"]["Text"]?.en).toBe("The test heading");
    expect(page!.Blocks[0].Content["Paragraphs"]["Html"]?.en).toBe("The test paragraphs (Html)");
    expect(page!.Blocks[0].Content["Paragraphs"]["Text"]?.en).toBe("The test paragraphs (Text)");

    expect(page!.Blocks[1].BlockId).toBeTypeOf("string");
    expect(page!.Blocks[1].Title).toBe("Block 2");
    expect(page!.Blocks[1].Hook).not.toBeTypeOf("string");
    expect(page!.Blocks[1].BlockType).toBe("HeadingAndParagraphsWithImage");
    expect(page!.Blocks[1].Content["Image URL:Alt"]["Text"]?.en).toBe("Alt 1");
    expect(page!.Blocks[1].Content["Image URL:Url"]["Text"]?.en).toBe("Url 1");

    expect(page!.Blocks[2].BlockId).toBeTypeOf("string");
    expect(page!.Blocks[2].Title).toBe("Block 3");
    expect(page!.Blocks[2].Hook).not.toBeTypeOf("string");
    expect(page!.Blocks[2].BlockType).toBe("ImageGallery");
    expect(page!.Blocks[2].Content["Image List:0.Alt"]["Text"]?.en).toBe("Alt 1");
    expect(page!.Blocks[2].Content["Image List:0.Url"]["Text"]?.en).toBe("Url 1");
    expect(page!.Blocks[2].Content["Image List:1.Alt"]["Text"]?.en).toBe("Alt 2");
    expect(page!.Blocks[2].Content["Image List:1.Url"]["Text"]?.en).toBe("Url 2");

    expect(page!.Blocks[3].BlockId).toBeTypeOf("string");
    expect(page!.Blocks[3].Title).toBe("Block 4");
    expect(page!.Blocks[3].BlockType).toBe("TwoColumns");

    expect(page!.Blocks[4].BlockId).toBeTypeOf("string");
    expect(page!.Blocks[4].Title).toBe("Block 5");
    expect(page!.Blocks[4].BlockType).toBe("LinkToAchievement");

    expect(page!.Blocks[5].BlockId).toBeTypeOf("string");
    expect(page!.Blocks[5].Title).toBe("Block 6");
    expect(page!.Blocks[5].BlockType).toBe("LinkToAssessment");

    expect(page!.Blocks[6].BlockId).toBeTypeOf("string");
    expect(page!.Blocks[6].Title).toBe("Block 7");
    expect(page!.Blocks[6].BlockType).toBe("LinkToCourse");

    expect(page!.Blocks[7].BlockId).toBeTypeOf("string");
    expect(page!.Blocks[7].Title).toBe("Block 8");
    expect(page!.Blocks[7].BlockType).toBe("LinkToForm");

    expect(page!.Blocks[8].BlockId).toBeTypeOf("string");
    expect(page!.Blocks[8].Title).toBe("Block 9");
    expect(page!.Blocks[8].BlockType).toBe("CourseSummary");
});

test("PUT /api/workspace/pages-contents/<pageId>: authenticated 1", async () => {
    await global.login();

    const result = await shiftClient.pageContent.modify(pageId, {
        Content: null,
        Blocks: [
            {
                BlockId: null,
                BlockIdNumber: 1,
                BlockType: "HeadingAndParagraphsWithImage",
                Title: "Block 10",
                Hook: "Hook 10",
                Content: {
                    "Image URL:Alt": {
                        Text: {
                            en: "Alt 1"
                        },
                    },
                    "Image URL:Url": {
                        Text: {
                            en: "Url 1"
                        }
                    }
                }
            },
            {
                BlockId: null,
                BlockIdNumber: 2,
                BlockType: "ImageGallery",
                Title: "Block 11",
                Hook: null,
                Content: {
                    "Image List:0.Alt": {
                        Text: {
                            en: "Alt 1"
                        },
                    },
                    "Image List:0.Url": {
                        Text: {
                            en: "Url 1"
                        }
                    },
                    "Image List:1.Alt": {
                        Text: {
                            en: "Alt 2"
                        },
                    },
                    "Image List:1.Url": {
                        Text: {
                            en: "Url 2"
                        }
                    },
                }
            }
        ],
        DeletedBlockIds: null
    });

    expect(result).not.toBe(null);
    expect(result!["1"]).toBeTypeOf("string");
    expect(result!["2"]).toBeTypeOf("string");

    const page = await shiftClient.pageContent.retrieve(pageId);

    expect(page).not.toBe(null);

    const block10 = page!.Blocks.find(x => x.BlockId === result!["1"])!;
    expect(block10).toBeDefined();
    expect(block10.Title).toBe("Block 10");
    expect(block10.Hook).toBe("Hook 10");
    expect(block10.BlockType).toBe("HeadingAndParagraphsWithImage");
    expect(block10.Content["Image URL:Alt"]["Text"]?.en).toBe("Alt 1");
    expect(block10.Content["Image URL:Url"]["Text"]?.en).toBe("Url 1");

    const block11 = page!.Blocks.find(x => x.BlockId === result!["2"])!;
    expect(block11.BlockId).toBeTypeOf("string");
    expect(block11.Title).toBe("Block 11");
    expect(block11.Hook).not.toBeTypeOf("string");
    expect(block11.BlockType).toBe("ImageGallery");
    expect(block11.Content["Image List:0.Alt"]["Text"]?.en).toBe("Alt 1");
    expect(block11.Content["Image List:0.Url"]["Text"]?.en).toBe("Url 1");
    expect(block11.Content["Image List:1.Alt"]["Text"]?.en).toBe("Alt 2");
    expect(block11.Content["Image List:1.Url"]["Text"]?.en).toBe("Url 2");

    await shiftClient.pageContent.modify(pageId, {
        Content: null,
        Blocks: null,
        DeletedBlockIds: [result!["1"], result!["2"]]
    });

    const page2 = await shiftClient.pageContent.retrieve(pageId);

    expect(page2).not.toBe(null);
    expect(page2!.Blocks.find(x => x.BlockId === result!["1"])!).not.toBeDefined();
    expect(page2!.Blocks.find(x => x.BlockId === result!["2"])!).not.toBeDefined();

});

test("PUT /api/workspace/pages-contents/<pageId>: authenticated 2", async () => {
    await global.login();

    await shiftClient.pageContent.modify(pageId, {
        Content: {
            Body: {
                Html: {
                    en: "The test body (HTML)",
                },
                Text: {
                    en: "The test body (Text)",
                },
            },
            Title: {
                Text: {
                    en: "The test page for React"
                }
            },
            Summary: {
                Html: {
                    en: "The test summary (Html)",
                },
                Text: {
                    en: "The test summary (Text)",
                },
            }
        },
        Blocks: null,
        DeletedBlockIds: null
    });

    const page = await shiftClient.pageContent.retrieve(pageId);

    expect(page).not.toBe(null);
    expect(page!.Title).toBe("The test page for React");
    expect(page!.Content["Body"]["Html"]?.en).toBe("The test body (HTML)");
    expect(page!.Content["Body"]["Text"]?.en).toBe("The test body (Text)");
    expect(page!.Content["Title"]["Text"]?.en).toBe("The test page for React");
    expect(page!.Content["Summary"]["Html"]?.en).toBe("The test summary (Html)");
    expect(page!.Content["Summary"]["Text"]?.en).toBe("The test summary (Text)");
});