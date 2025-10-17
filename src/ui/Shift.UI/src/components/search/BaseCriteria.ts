import { ObjectIndexer } from "@/models/ObjectIndexer";

export interface BaseCriteria {
    visibleColumns: string[];
    sortByColumn: string | null;
}

export function cleanCriteria(criteria: object, defaultCriteria: object): object {
    const result: ObjectIndexer = {};

    for (const key in criteria) {
        if (key in defaultCriteria && (criteria as ObjectIndexer)[key] !== (defaultCriteria as ObjectIndexer)[key]) {
            result[key] = (criteria as ObjectIndexer)[key];
        }
    }

    return result;
}

export function hydrateCriteria<Criteria>(criteria: Criteria | null | undefined, defaultCriteria: Criteria): Criteria {
    if (!criteria) {
        return defaultCriteria;
    }
    
    const result = {...defaultCriteria} as ObjectIndexer;

    for (const key in criteria) {
        if (key in result) {
            result[key] = criteria[key];
        }
    }

    return result as Criteria;
}