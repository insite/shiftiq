interface QuestionPerTagAndTaxonomy {
    tag: string;
    taxonomy: number;
    count: number;
}

interface QuestionPerIntItem {
    item: number;
    count: number;
}

interface QuestionPerStringItem {
    item: string;
    count: number;
}

interface Standard {
    setStandardCode: string | null;
    questionStandardCode: string | null;
    questions: number;
    taxonomies: (number | null)[];
}

interface SubCompetency {
    setStandardCode: string | null;
    questionStandardCode: string | null;
    questionSubCode: string | null;
    questions: number;
}

interface TagAndTaxonomy {
    tag: string;
    countPerTaxonomy: number[];
    questions: number;
}

export interface FormWorkshopQuestionStatistics {
    questionPerTagAndTaxonomy: QuestionPerTagAndTaxonomy[];
    questionPerTaxonomy: QuestionPerIntItem[];
    questionPerDifficulty: QuestionPerIntItem[];
    questionPerGAC: QuestionPerStringItem[];
    questionPerCode: QuestionPerStringItem[];
    questionPerLIG: QuestionPerStringItem[];
    taxonomies: number[];
    standards: Standard[];
    subCompetencies: SubCompetency[];
    tagAndTaxonomy: TagAndTaxonomy[];
}