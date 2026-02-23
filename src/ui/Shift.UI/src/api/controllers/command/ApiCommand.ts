import { _ApiGradebookCommandName } from "./gradebook/_ApiGradebookCommandName";

export type ApiCommandName = _ApiGradebookCommandName;

export abstract class ApiCommand {
    constructor (
        public readonly commandName: ApiCommandName,
        public readonly aggregateId: string,
        public readonly data: object | null,
    ) {}
}