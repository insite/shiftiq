function getCookie(name: string): string | null {
    const nameEQ = name + "=";
    const decodedCookie = decodeURIComponent(document.cookie);
    const ca = decodedCookie.split(";");

    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) === " ") {
            c = c.substring(1);
        }
        if (c.indexOf(nameEQ) === 0) {
            return c.substring(nameEQ.length, c.length);
        }
    }

    return null; 
}

export const cookieHelper = {
    getTheme() : "dark" | "light" {
        const themeCookie = getCookie("Shift.UI.ThemeMode");
        return themeCookie?.toLowerCase() === "dark" ? "dark" : "light";
    },
}