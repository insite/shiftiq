import { ApiCommand } from "../ApiCommand";

export class ApiRenameGradebook extends ApiCommand {
    constructor (gradebookId: string, name: string) {
        super("RenameGradebook", gradebookId, {
            Name: name,
        });
    }
}