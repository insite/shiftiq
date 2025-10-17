import { ApiCommand } from "../ApiCommand";

export class ApiUnlockGradebook extends ApiCommand {
    constructor (gradebookId: string) {
        super("UnlockGradebook", gradebookId, null);
    }
}