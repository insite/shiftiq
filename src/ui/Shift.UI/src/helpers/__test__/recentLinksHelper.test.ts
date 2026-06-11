import { afterEach, expect, test, vi } from "vitest";
import { recentLinksHelper } from "../recentLinksHelper";

afterEach(() => {
    localStorage.clear();
    vi.restoreAllMocks();
});

test("recentLinksHelper.getAll returns items in original order for valid legacy data", () => {
    const recentLinksKey = "recent-links-key";
    const items = [{
        pageUrl: "/ui/admin/events/classes/outline?event=1",
        observerName: "Class Event",
        pageTitle: "Class Event 1",
        key: "class-1",
    }, {
        pageUrl: "/ui/admin/courses/manage?course=2",
        observerName: "Course",
        pageTitle: "Course 2",
        key: "course-2",
    }];

    localStorage.setItem(recentLinksHelper.getStorageKey(recentLinksKey), JSON.stringify(items));

    expect(recentLinksHelper.getAll(recentLinksKey)).toEqual(items);
});

test("recentLinksHelper.getAll returns empty array when storage value is missing", () => {
    expect(recentLinksHelper.getAll("missing-key")).toEqual([]);
});

test("recentLinksHelper.getAll returns empty array for invalid json", () => {
    localStorage.setItem(recentLinksHelper.getStorageKey("invalid-json-key"), "{");

    expect(recentLinksHelper.getAll("invalid-json-key")).toEqual([]);
});

test("recentLinksHelper.getAll filters malformed entries", () => {
    localStorage.setItem(recentLinksHelper.getStorageKey("mixed-key"), JSON.stringify([{
        pageUrl: "/ui/admin/messages/outline?message=1",
        observerName: "Message",
        pageTitle: null,
        key: "message-1",
    }, {
        pageUrl: "",
        observerName: "Broken",
        pageTitle: "Broken",
        key: "broken-1",
    }, {
        pageUrl: "/ui/admin/messages/outline?message=2",
        observerName: null,
        pageTitle: "Broken 2",
        key: "broken-2",
    }]));

    expect(recentLinksHelper.getAll("mixed-key")).toEqual([{
        pageUrl: "/ui/admin/messages/outline?message=1",
        observerName: "Message",
        pageTitle: null,
        key: "message-1",
    }]);
});

test("recentLinksHelper.getAll reads from exact legacy storage key", () => {
    const getItemSpy = vi.spyOn(localStorage, "getItem");

    recentLinksHelper.getAll("legacy-key");

    expect(getItemSpy).toHaveBeenCalledWith("recentLinks.legacy-key");
});
