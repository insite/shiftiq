import { languageNames } from "@/helpers/language";
import { _achievementController } from "./controllers/records/achievements/_achievementController";
import { _commandController } from "./controllers/command/_commandController";
import { _cookieController } from "./controllers/accounts/cookies/_cookieController";
import { _eventController } from "./controllers/events/_eventController";
import { _fileController } from "./controllers/assets/files/_fileController";
import { _gradebookController } from "./controllers/records/gradebooks/_gradebookController";
import { _organizationController } from "./controllers/accounts/organizations/_organizationController";
import { _peopleController } from "./controllers/people/_peopleController";
import { _periodController } from "./controllers/records/periods/_periodController";
import { _standardController } from "./controllers/standards/_standardController";
import { _userController } from "./controllers/accounts/users/_userController";
import { _caseStatusController } from "./controllers/workflows/caseStatuses/_caseStatusController";
import { fetchHelper } from "./fetchHelper";
import { ApiSiteSetting } from "./models/ApiSiteSetting";
import { _translationController } from "./controllers/assets/translations/_translationController";
import { _pageContentController } from "./controllers/sites/pageContents/_pageContentController";
import { _timelineController } from "./controllers/timeline/_timelineController";
import { _workshopController } from "./controllers/assessments/workshop/_workshopController";
import { _dashboardController } from "./controllers/platform/dashboard/_dashboardController";
import { _maintenanceController } from "./controllers/platform/maintenance/_maintenanceController";

export const shiftClient = {
    me: {
        async context(refresh: boolean): Promise<ApiSiteSetting> {
            const result = await fetchHelper.get<ApiSiteSetting>("/api/me/context", [{ name: "refresh", value: refresh ? "true" : "false" }]);
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
    pageContent: _pageContentController,
    workshop: _workshopController,
    dashboard: _dashboardController,
    maintenance: _maintenanceController,

    command: _commandController,
    timeline: _timelineController,
}