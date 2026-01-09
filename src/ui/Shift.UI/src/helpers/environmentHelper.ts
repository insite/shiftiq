export const environmentHelper = {
    getIndicator(environmentName: string) {
        switch (environmentName.toLowerCase()) {
            case "production":
                return {
                    indicator: "success",
                    icon: "home",
                }
            case "sandbox":
                return {
                    indicator: "warning",
                    icon: "presentation",
                }
            case "development":
                return {
                    indicator: "danger",
                    icon: "laptop-code",
                }
            default:
                return {
                    indicator: "info",
                    icon: "robot",
                }
        }
    }
}