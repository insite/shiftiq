import { languageNames } from "@/helpers/language";
import { _achievementController } from "./controllers/achievement/_achievementController";
import { _commandController } from "./controllers/command/_commandController";
import { _cookieController } from "./controllers/cookie/_cookieController";
import { _eventController } from "./controllers/event/_eventController";
import { _fileController } from "./controllers/file/_fileController";
import { _gradebookController } from "./controllers/gradebook/_gradebookController";
import { _organizationController } from "./controllers/organization/_organizationController";
import { _peopleController } from "./controllers/people/_peopleController";
import { _periodController } from "./controllers/period/_periodController";
import { _standardController } from "./controllers/standard/_standardController";
import { _userController } from "./controllers/user/_userController";
import { _caseStatusController } from "./controllers/caseStatus/_caseStatusController";
import { fetchHelper } from "./fetchHelper";
import { ApiSiteSetting } from "./models/ApiSiteSetting";
import { _translationController } from "./controllers/translation/_translationController";

export const shiftClient = {
    me: {
        async context(refresh: boolean): Promise<ApiSiteSetting> {
            const result = await fetchHelper.get<ApiSiteSetting>("/me/context", [{ name: "refresh", value: refresh ? "true" : "false" }]);
            for (const language of result.SupportedLanguages) {
                if (!(language in languageNames)) {
                    throw new Error(`This language is not supported: ${language}`);
                }
            }
            return result;
        },
    },

    gradebook: _gradebookController,
    period: _periodController,
    achievement: _achievementController,
    event: _eventController,
    organization: _organizationController,
    file: _fileController,
    user: _userController,
    people: _peopleController,
    standard: _standardController,
    cookie: _cookieController,
    caseStatus: _caseStatusController,
    translation: _translationController,

    command: _commandController,
}