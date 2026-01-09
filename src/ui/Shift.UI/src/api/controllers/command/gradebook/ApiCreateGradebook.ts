import { ApiCommand } from "../ApiCommand";

type GradebookType = "Scores" | "Standards" | "ScoresAndStandards";

export class ApiCreateGradebook extends ApiCommand {
    constructor (
        gradebookId: string,
        organizationId: string,
        name: string,
        type: GradebookType,
        eventId: string | null,
        achievementId: string | null,
        frameworkId: string | null
    ) {
        super("CreateGradebook", gradebookId, {
            Tenant: organizationId,
            Name: name,
            Type: type,
            Event: eventId,
            Achievement: achievementId,
            Framework: frameworkId,
        });
    }
}