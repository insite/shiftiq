interface QuestionPerTagAndTaxonomy {
    Tag: string;
    Taxonomy: number;
    Count: number;
}

interface QuestionPerIntItem {
    Item: number;
    Count: number;
}

interface QuestionPerStringItem {
    Item: string;
    Count: number;
}

interface Standard {
    SetStandardCode: string | null | undefined;
    QuestionStandardCode: string | null | undefined;
    Questions: number;
    Taxonomies: (number | null | undefined)[];
}

interface SubCompetency {
    SetStandardCode: string | null | undefined;
    QuestionStandardCode: string | null | undefined;
    QuestionSubCode: string | null | undefined;
    Questions: number;
}

interface TagAndTaxonomy {
    Tag: string;
    CountPerTaxonomy: number[];
}

export interface ApiFormWorkshopQuestionStatistics {
    QuestionPerTagAndTaxonomyArray: QuestionPerTagAndTaxonomy[];
    QuestionPerTaxonomyArray: QuestionPerIntItem[];
    QuestionPerDifficultyArray: QuestionPerIntItem[];
    QuestionPerGACArray: QuestionPerStringItem[];
    QuestionPerCodeArray: QuestionPerStringItem[];
    QuestionPerLIGArray: QuestionPerStringItem[];
    Taxonomies: number[];
    Standards: Standard[];
    SubCompetencies: SubCompetency[];
    TagAndTaxonomyArray: TagAndTaxonomy[];
}