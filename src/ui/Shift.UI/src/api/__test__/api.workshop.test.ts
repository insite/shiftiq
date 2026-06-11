import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";
import { ApiSpecWorkshop } from "../controllers/assessments/workshop/ApiSpecWorkshop";
import { ApiSpecWorkshopInput } from "../controllers/assessments/workshop/ApiSpecWorkshopInput";
import { ApiSpecWorkshopSet } from "../controllers/assessments/workshop/ApiSpecWorkshopSet";
import { ApiFormWorkshopSection } from "../controllers/assessments/workshop/ApiFormWorkshopSection";
import { ApiWorkshopQuestionComments } from "../controllers/assessments/workshop/ApiWorkshopQuestionComments";

const frameworkId = "fd4ce3ef-4418-44b9-babc-0e76dd1f7c17";
const areaId1 = "ee29c864-0b85-4abd-afa1-dc4bb0ff1b8f";
const areaId2 = "b4d75c15-2488-42bc-a569-c74adff169c0";
const competencyId1 = "32e963ca-3e4e-42a5-b60f-84886ee21c9b";
const competencyId2 = "45e61f4a-d7a7-4708-8060-0948a3a2499a";

const immutableBankId = "4e699b7d-85d8-49cd-aabd-e8440fda3a41";
const immutableSpecificationId = "9a9c93f9-8a0b-4d4a-9808-5453f71d552c";
const immutableFormId = "ee8bbcae-a48a-4191-bf71-4652c5cb1714";
const immutableSetId2 = "cd8e0e23-7baf-41f9-97fb-03f8cdf161a0";
const immutableSectionId1 = "0a6953f9-8a6c-44b1-8465-58eac1bcf0cd";
const immutableFieldId11 = "649fecba-7347-47e9-bc14-b47868b4840a";
const immutableQuestionId11 = "ab764f85-91a4-4b65-b19c-ba398c4493a7";
const immutableQuestionId12 = "248825ae-5d0e-4776-a2af-ccbe3b173b06";
const immutableQuestionId21 = "7381231d-9da2-4848-9a6c-df99aef5da0d";
const immutableQuestionId22 = "acde5404-ad7d-4c6d-9e69-105b42e404fa";
const immutableCriterionId1 = "bcfcb1b1-081f-42e7-a74e-16be5b3991d4";
const immutableCriterionId2 = "0f31f76b-182c-4d15-a789-318a39e207e7";

const mutableBankId = "0e02271f-e2a6-4957-b457-65f008d8720c";
const mutableSpecificationId = "5d3954f2-4815-4576-b7c8-38365ff0b4b1";
const mutableFormId = "af52d704-ee2d-413c-bd18-49fc980f8adb";
const mutableSetId1 = "494d11cf-6ea7-4bf6-a2df-127eaf202a3a";
const mutableSectionId1 = "5af69ec8-739e-463e-a39d-5339d4269d9b";
const mutableQuestionId11 = "57570f99-0e5c-44d4-9c42-c91ca18a3f13";

test("/api/assessments/workshops: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.workshop.retrieveForm(immutableFormId, null, null)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.workshop.retrieveSpec(immutableSpecificationId, null, null)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.workshop.collectImages(immutableBankId)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.workshop.addQuestion(mutableBankId, mutableSpecificationId, mutableSetId1, competencyId1, "QuickMultipleChoice")).rejects.toThrowError(new ApiError(401, ""));
});

test("GET /api/assessments/workshops/forms/<formId>: authenticated", async () => {
    await global.login();

    const form = await shiftClient.workshop.retrieveForm(immutableFormId, null, null);

    expect(form).not.toBe(null);
    expect(form!.BankId.toLowerCase()).toBe(immutableBankId);
    expect(form!.Details.SpecificationName).toBe("Spec 1");
    expect(form!.Details.SpecificationType).toBe("Static");
    expect(form!.Details.FormName).toBe("Form 1");
    expect(form!.Details.ThirdPartyAssessmentIsEnabled).toBe(false);
    expect(form!.Details.Standard.StandardId.toLowerCase()).toBe(frameworkId);
    expect(form!.QuestionData.TotalQuestionCount).toBe(4);
    expect(form!.QuestionData.FirstSectionId.toLowerCase()).toBe(immutableSectionId1);
    expect(form!.QuestionData.FirstSectionQuestions.length).toBe(2);
    expect(form!.Comments.length).toBeGreaterThan(0);
    expect(form!.Attachments.length).toBeGreaterThan(0);
    expect(form!.QuestionData.FirstSectionQuestions.filter(x => x.QuestionId.toLowerCase() === immutableQuestionId11 || x.QuestionId.toLowerCase() === immutableQuestionId12).length).toBe(2);
});

test("GET /api/assessments/workshops/forms/<formId>/sections/<sectionId>: authenticated", async () => {
    await global.login();

    const section = await shiftClient.workshop.retrieveSection(immutableFormId, immutableSectionId1);

    expect(section).not.toBe(null);
    expect(section!.Standards.length).toBeGreaterThan(0);
    expect(section!.Questions.length).toBe(2);

    const question1 = findQuestion(section!, immutableQuestionId11);
    const question2 = findQuestion(section!, immutableQuestionId12);

    expect(question1.FieldId?.toLowerCase()).toBe(immutableFieldId11);
    expect(question1.QuestionTitle.en).toBe("Question 1");
    expect(question1.Options?.length).toBe(3);
    expect(question2.QuestionTitle.en).toBe("Question 2");
    expect(question2.Options?.length).toBe(3);
});

test("GET /api/assessments/workshops/specs/<specificationId>: authenticated", async () => {
    await global.login();

    const spec = await shiftClient.workshop.retrieveSpec(immutableSpecificationId, null, null);

    expect(spec).not.toBe(null);
    expect(spec!.BankId.toLowerCase()).toBe(immutableBankId);
    expect(spec!.Details.SpecName).toBe("Spec 1");
    expect(spec!.Details.FrameworkId?.toLowerCase()).toBe(frameworkId);
    expect(spec!.Details.Criteria.length).toBe(2);
    expect(spec!.Details.Criteria[0].StandardIds.length).toBeGreaterThan(0);
    expect(spec!.QuestionData.TotalQuestionCount).toBe(4);
    expect(spec!.QuestionData.FirstSectionQuestions.length).toBe(2);
    expect(spec!.Comments.length).toBeGreaterThan(0);
    expect(spec!.Attachments.length).toBeGreaterThan(0);
    expect(spec!.QuestionData.FirstSectionQuestions.filter(x => x.QuestionId.toLowerCase() === immutableQuestionId11 || x.QuestionId.toLowerCase() === immutableQuestionId12).length).toBe(2);

    const criterion1 = spec!.Details.Criteria.find(x => x.CriterionId === immutableCriterionId1);
    expect(criterion1).toBeDefined();
    expect(criterion1!.StandardIds).toStrictEqual([areaId1]);
    expect(criterion1!.Title).toBe("Set 1");
    expect(criterion1!.Weight).toBe(4000);
    expect(criterion1!.Competencies[0]).toBeDefined();
    expect(criterion1!.Competencies[0].StandardId).toBe(competencyId1);

    const criterion2 = spec!.Details.Criteria.find(x => x.CriterionId === immutableCriterionId2);
    expect(criterion2).toBeDefined();
    expect(criterion2!.StandardIds).toStrictEqual([areaId2]);
    expect(criterion2!.Title).toBe("Set 2");
    expect(criterion2!.Weight).toBe(6000);
    expect(criterion2!.Competencies[0]).toBeDefined();
    expect(criterion2!.Competencies[0].StandardId).toBe(competencyId2);
});

test("GET /api/assessments/workshops/specs/<specificationId>/sets/<setId>: authenticated", async () => {
    await global.login();

    const set = await shiftClient.workshop.retrieveSpecSet(immutableSpecificationId, immutableSetId2);

    expect(set).not.toBe(null);
    expect(set!.AreaId?.toLowerCase()).toBe(areaId2);
    expect(set!.Questions.length).toBe(2);
    expect(set!.Questions.map(x => x.QuestionId.toLowerCase())).toContain(immutableQuestionId21);
    expect(set!.Questions.map(x => x.QuestionId.toLowerCase())).toContain(immutableQuestionId22);
});

test("GET /api/assessments/workshops/banks/<bankId>/question-change-dates: authenticated", async () => {
    await global.login();

    const dates = await shiftClient.workshop.collectQuestionChangeDates(immutableBankId);

    expect(dates).not.toBe(null);
});

test("GET /api/assessments/workshops/banks/<bankId>/images: authenticated", async () => {
    await global.login();

    const images = await shiftClient.workshop.collectImages(immutableBankId);

    expect(images).not.toBe(null);
    expect(images!.length).toBeGreaterThan(0);

    const image = images!.find(x => x.Attachment?.Title === "Linux2");
    expect(image).toBeDefined();
    expect(image!.Url).toBeTypeOf("string");
    expect(image!.Environment).toBeTypeOf("string");
    expect(image!.Attachment?.Condition).toBe("Copy");
    expect(image!.Attachment?.PublicationStatus).toBeTypeOf("string");
});

test("POST /api/assessments/workshops/banks/<bankId>/specifications/<specificationId>/sets/<setId>/questions: authenticated", async () => {
    await global.login();

    const before = await getMutableSet();
    const created = await shiftClient.workshop.addQuestion(mutableBankId, mutableSpecificationId, mutableSetId1, competencyId1, "QuickMultipleChoice");

    expect(created).not.toBe(null);
    expect(created!.QuestionId).toBeTypeOf("string");
    expect(created!.Questions.length).toBe(before.Questions.length + 1);
    expect(created!.Questions.some(x => x.QuestionId.toLowerCase() === created!.QuestionId!.toLowerCase())).toBe(true);
});

test("POST /api/assessments/workshops/banks/<bankId>/specifications/<specificationId>/sets/<setId>/questions/<questionId>/duplicate: authenticated", async () => {
    await global.login();

    const before = await getMutableSet();
    const duplicated = await shiftClient.workshop.duplicateQuestion(mutableBankId, mutableSpecificationId, mutableSetId1, mutableQuestionId11);

    expect(duplicated).not.toBe(null);
    expect(duplicated!.QuestionId).toBeTypeOf("string");
    expect(duplicated!.Questions.length).toBe(before.Questions.length + 1);

    const question = findQuestion(duplicated!, duplicated!.QuestionId!);
    expect(question.QuestionTitle.en).toContain("Mutable Question 1");
    expect(question.Options?.length).toBeGreaterThan(0);
});

test("PUT /api/assessments/workshops/banks/<bankId>/questions/<questionId>: authenticated", async () => {
    await global.login();

    const duplicated = await shiftClient.workshop.duplicateQuestion(mutableBankId, mutableSpecificationId, mutableSetId1, mutableQuestionId11);
    expect(duplicated).not.toBe(null);

    const duplicatedQuestionId = duplicated!.QuestionId!;
    const title = createUniqueText("Mutable Title");
    const code = createUniqueText("CODE");
    const tag = createUniqueText("TAG");
    const reference = createUniqueText("REF");

    const titleHtml = await shiftClient.workshop.modifyQuestion(mutableBankId, duplicatedQuestionId, "Title", null, title);
    await shiftClient.workshop.modifyQuestion(mutableBankId, duplicatedQuestionId, "Code", null, code);
    await shiftClient.workshop.modifyQuestion(mutableBankId, duplicatedQuestionId, "Tag", null, tag);
    await shiftClient.workshop.modifyQuestion(mutableBankId, duplicatedQuestionId, "Reference", null, reference);
    await shiftClient.workshop.modifyQuestion(mutableBankId, duplicatedQuestionId, "Flag", null, "Yellow");

    expect(titleHtml).toContain(title);

    const updatedSet = await getMutableSet();
    const updatedQuestion = findQuestion(updatedSet, duplicatedQuestionId);

    expect(updatedQuestion.QuestionTitle.en).toBe(title);
    expect(updatedQuestion.QuestionTitleHtml).toContain(title);
    expect(updatedQuestion.QuestionCode).toBe(code);
    expect(updatedQuestion.QuestionTag).toBe(tag);
    expect(updatedQuestion.QuestionReference).toBe(reference);
    expect(updatedQuestion.QuestionFlag).toBe("Yellow");
});

test("PUT /api/assessments/workshops/banks/<bankId>/questions/<questionId>/options/<optionNumber>: authenticated", async () => {
    await global.login();

    const duplicated = await shiftClient.workshop.duplicateQuestion(mutableBankId, mutableSpecificationId, mutableSetId1, mutableQuestionId11);
    expect(duplicated).not.toBe(null);

    const duplicatedQuestionId = duplicated!.QuestionId!;
    const duplicatedOptionNumber = duplicated!.Questions.find(x => x.QuestionId === duplicatedQuestionId)!.Options![0].Number;
    const optionTitle = createUniqueText("Mutable Option");
    const optionTitleHtml = await shiftClient.workshop.modifyOption(mutableBankId, duplicatedQuestionId, duplicatedOptionNumber, "Title", null, optionTitle);
    await shiftClient.workshop.modifyOption(mutableBankId, duplicatedQuestionId, duplicatedOptionNumber, "Points", null, "7500");

    expect(optionTitleHtml).toContain(optionTitle);

    const updatedSet = await getMutableSet();
    const updatedQuestion = findQuestion(updatedSet, duplicatedQuestionId);
    const updatedOption = updatedQuestion.Options?.find(x => x.Number === duplicatedOptionNumber);

    expect(updatedOption).toBeDefined();
    expect(updatedOption!.TitleHtml).toContain(optionTitle);
    expect(updatedOption!.Points).toBe(7500);
});

test("POST /api/assessments/workshops/banks/<bankId>/questions/<questionId>/comments and PUT /comments/<commentId>/showhide: authenticated", async () => {
    await global.login();

    const duplicated = await shiftClient.workshop.duplicateQuestion(mutableBankId, mutableSpecificationId, mutableSetId1, mutableQuestionId11);
    expect(duplicated).not.toBe(null);

    const duplicatedQuestionId = duplicated!.QuestionId!;
    const commentText = createUniqueText("Mutable question comment");
    const commentResult = await shiftClient.workshop.postQuestionComment(mutableBankId, duplicatedQuestionId, "Administrator", "Green", commentText);

    expect(commentResult).not.toBe(null);

    const createdComment = findComment(commentResult!, commentText);
    expect(createdComment.IsHidden).toBe(false);

    await shiftClient.workshop.showHideComment(mutableBankId, createdComment.CommentId, true);

    const hiddenSet = await getMutableSet();
    const hiddenQuestion = findQuestion(hiddenSet, duplicatedQuestionId);
    const hiddenComment = hiddenQuestion.Comments.find(x => x.CommentId === createdComment.CommentId);
    expect(hiddenComment).toBeDefined();
    expect(hiddenComment!.IsHidden).toBe(true);

    await shiftClient.workshop.showHideComment(mutableBankId, createdComment.CommentId, false);

    const visibleSet = await getMutableSet();
    const visibleQuestion = findQuestion(visibleSet, duplicatedQuestionId);
    const visibleComment = visibleQuestion.Comments.find(x => x.CommentId === createdComment.CommentId);
    expect(visibleComment).toBeDefined();
    expect(visibleComment!.IsHidden).toBe(false);
});

test("POST /api/assessments/workshops/banks/<bankId>/fields/<fieldId>/comments: authenticated", async () => {
    await global.login();

    const initialSection = await getMutableSection();
    const fieldId = initialSection.Questions[0].FieldId!;

    const commentText = createUniqueText("Mutable field comment");
    const result = await shiftClient.workshop.postFieldComment(mutableBankId, fieldId, "Administrator", "Blue", commentText);

    expect(result).not.toBe(null);
    expect(findComment(result!, commentText).Text).toContain(commentText);

    const section = await getMutableSection();
    const question = findQuestionByFieldId(section, fieldId);
    expect(question.Comments.some(x => x.Text.includes(commentText))).toBe(true);
});

test("PUT /api/assessments/workshops/forms/<formId>/third-party-assessment: authenticated", async () => {
    await global.login();

    const initial = await shiftClient.workshop.retrieveForm(mutableFormId, null, null);
    expect(initial).not.toBe(null);

    const baseline = initial!.Details.ThirdPartyAssessmentIsEnabled;
    const toggled = !baseline;

    try {
        await shiftClient.workshop.modifyThirdPartyAssessment(mutableFormId, toggled);

        const updated = await shiftClient.workshop.retrieveForm(mutableFormId, null, null);
        expect(updated).not.toBe(null);
        expect(updated!.Details.ThirdPartyAssessmentIsEnabled).toBe(toggled);
    } finally {
        await shiftClient.workshop.modifyThirdPartyAssessment(mutableFormId, baseline);
    }

    const restored = await shiftClient.workshop.retrieveForm(mutableFormId, null, null);
    expect(restored).not.toBe(null);
    expect(restored!.Details.ThirdPartyAssessmentIsEnabled).toBe(baseline);
});

test("POST /api/assessments/workshops/forms/<formId>/verify-static-question-order: authenticated", async () => {
    await global.login();

    const result = await shiftClient.workshop.verifyStaticQuestionOrder(mutableFormId);

    expect(result).not.toBe(null);
    expect(result!.StaticQuestionOrderVerified).toBeTypeOf("string");
    expect(result!.VerifiedQuestions).not.toBe(null);
    expect(result!.VerifiedQuestions!.length).toBe(2);
    expect(result!.IsQuestionOrderMatch).toBe(true);
});

test("PUT /api/assessments/workshops/specs/<specificationId>: authenticated", async () => {
    await global.login();

    const original = await shiftClient.workshop.retrieveSpec(mutableSpecificationId, null, null);
    expect(original).not.toBe(null);

    const updatedInput = createSpecInput(original!, {
        formLimit: original!.Details.FormLimit + 1,
        questionLimit: original!.Details.QuestionLimit + 2,
        weights: [6000, 4000]
    });

    try {
        const saved = await shiftClient.workshop.modifySpec(mutableSpecificationId, updatedInput);
        expect(saved).toBe(true);

        const updated = await shiftClient.workshop.retrieveSpec(mutableSpecificationId, null, null);
        expect(updated).not.toBe(null);
        expect(updated!.Details.FormLimit).toBe(updatedInput.FormLimit);
        expect(updated!.Details.QuestionLimit).toBe(updatedInput.QuestionLimit);
        expect(updated!.Details.Criteria.map(x => x.Weight)).toEqual([6000, 4000]);
    } finally {
        await shiftClient.workshop.modifySpec(mutableSpecificationId, createSpecInput(original!));
    }

    const restored = await shiftClient.workshop.retrieveSpec(mutableSpecificationId, null, null);
    expect(restored).not.toBe(null);
    expect(restored!.Details.FormLimit).toBe(original!.Details.FormLimit);
    expect(restored!.Details.QuestionLimit).toBe(original!.Details.QuestionLimit);
    expect(restored!.Details.Criteria.map(x => x.Weight)).toEqual(original!.Details.Criteria.map(x => x.Weight));
});

test("POST /api/assessments/workshops/banks/<bankId>/fields/<fieldId>/replace: authenticated", async () => {
    await global.login();

    const initialSection = await getMutableSection();
    const fieldId = initialSection.Questions[0].FieldId!;
    const replaced = await shiftClient.workshop.replaceFieldQuestion(mutableBankId, fieldId, "NewQuestionAndSurplus");

    expect(replaced).not.toBe(null);
    expect(replaced!.Questions.length).toBe(initialSection.Questions.length);
    expect(replaced!.Questions.find(x => x.FieldId?.toLowerCase() === fieldId.toLowerCase())).toBeUndefined();
});

async function getMutableSet(): Promise<ApiSpecWorkshopSet> {
    const set = await shiftClient.workshop.retrieveSpecSet(mutableSpecificationId, mutableSetId1);
    expect(set).not.toBe(null);
    return set!;
}

async function getMutableSection(): Promise<ApiFormWorkshopSection> {
    const section = await shiftClient.workshop.retrieveSection(mutableFormId, mutableSectionId1);
    expect(section).not.toBe(null);
    return section!;
}

function findQuestion<T extends { QuestionId: string }>(container: { Questions: T[] }, questionId: string): T {
    const question = container.Questions.find(x => x.QuestionId.toLowerCase() === questionId.toLowerCase());
    expect(question).toBeDefined();
    return question!;
}

function findQuestionByFieldId(section: ApiFormWorkshopSection, fieldId: string) {
    const question = section.Questions.find(x => x.FieldId?.toLowerCase() === fieldId.toLowerCase());
    expect(question).toBeDefined();
    return question!;
}

function findComment(result: ApiWorkshopQuestionComments, text: string) {
    const comment = result.Comments.find(x => x.Text.includes(text));
    expect(comment).toBeDefined();
    return comment!;
}

function createUniqueText(prefix: string) {
    return `${prefix} ${Date.now()} ${Math.random().toString(36).slice(2, 8)}`;
}

function createSpecInput(
    spec: ApiSpecWorkshop,
    overrides?: {
        formLimit?: number;
        questionLimit?: number;
        weights?: number[];
    }
): ApiSpecWorkshopInput {
    return {
        FormLimit: overrides?.formLimit ?? spec.Details.FormLimit,
        QuestionLimit: overrides?.questionLimit ?? spec.Details.QuestionLimit,
        Criteria: spec.Details.Criteria.map((criterion, index) => ({
            CriterionId: criterion.CriterionId,
            Weight: overrides?.weights?.[index] ?? criterion.Weight,
            Competencies: criterion.Competencies.map(competency => ({
                StandardId: competency.StandardId,
                Tax1Count: competency.Tax1Count ?? null,
                Tax2Count: competency.Tax2Count ?? null,
                Tax3Count: competency.Tax3Count ?? null,
            }))
        }))
    };
}
