import { fetchHelper } from "@/api/fetchHelper";
import { ApiCommand, ApiCommandName } from "./ApiCommand";
import { cache } from "@/cache/cache";

function updateCache(commandName: ApiCommandName) {
    switch (commandName) {
        case "LockGradebook":
        case "UnlockGradebook":
        case "RenameGradebook":
        case "DeleteGradebook":
        case "CreateGradebook":
            cache.onGradebookChange();
            break;
        default:
            throw new Error(`Cannot update the cache, unknown command: ${commandName}`);
    }
}

export const _commandController = {
    async send(command: ApiCommand) {
        const body = {
            AggregateId: command.aggregateId,
            ...command.data
        };

        const result = await fetchHelper.post<string>("/api/timeline/commands", body, [{
            name: "command",
            value: command.commandName
        }]);

        updateCache(command.commandName);

        return result;
    },
}