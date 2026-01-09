import { requestHelper } from "@/api/requestHelper";
import { shiftClient } from "@/api/shiftClient";
import { shiftConfig } from "@/helpers/shiftConfig";

declare global {
    // eslint-disable-next-line no-var
    var login: () => Promise<void>;

    // eslint-disable-next-line no-var
    var loginUser1: () => Promise<void>;

    // eslint-disable-next-line no-var
    var loginUser2: () => Promise<void>;

    // eslint-disable-next-line no-var
    var logout: () => Promise<void>;
}

global.login = async () => {
    await shiftClient.cookie.login("e01", "aleksey@shiftiq.com");
}

global.loginUser1 = async () => {
    await shiftClient.cookie.login("e01", "react1@shiftiq.com");
}

global.loginUser2 = async () => {
    await shiftClient.cookie.login("e01", "react2@shiftiq.com");
}

global.logout = async () => {
    await shiftClient.cookie.logout();
}

shiftConfig.isLocal = true;
shiftConfig.shiftApiHostUrl = "https://localhost:5000";

requestHelper.setThrowAuthError(true);