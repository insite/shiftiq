import { ApiCommand } from "../ApiCommand";

export class ApiDeleteGradebook extends ApiCommand {
    constructor (gradebookId: string) {
        super("DeleteGradebook", gradebookId, null);
    }
}