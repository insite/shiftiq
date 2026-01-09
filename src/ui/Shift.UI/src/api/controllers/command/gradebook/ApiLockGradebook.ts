import { ApiCommand } from "../ApiCommand";

export class ApiLockGradebook extends ApiCommand {
    constructor (gradebookId: string) {
        super("LockGradebook", gradebookId, null);
    }
}