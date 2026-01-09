import { ListItem } from "@/models/listItem";
import Finder from "../../components/finder/Finder";
import { useState } from "react";
import { FinderSearchWindowData } from "@/components/finder/FinderSearchWindowData";

const testData: ListItem[] = [];
for (let i = 0; i < 1000; i++) {
    testData.push({ value: String(i), text: "User " + (i + 1) });
}

export default function TestFinder() {
    const [value, setValue] = useState("76");

    return (
        <Finder
            value={value}
            placeholder="Test Input"
            windowTitle="Users"
            columnHeaderTitle="Name"
            onChange={value => setValue(value)}
            onLoadText={loadText}
            onLoadItems={loadItems}
        />
    )
}

async function loadText(value: string) {
    const text = testData.find(x => x.value === value)?.text ?? "";

    return await new Promise<string>(resolve => setTimeout(() => resolve(text), 500));
}

async function loadItems(pageIndex: number, keyword: string): Promise<FinderSearchWindowData> {
    await new Promise<void>(resolve => setTimeout(resolve, 500));

    const items: ListItem[] = [];
    let start = pageIndex * 10;

    const allItems = !keyword ? testData : testData.filter(x => x.text.toLowerCase().includes(keyword.toLowerCase()));

    if (start >= allItems.length) {
        pageIndex = 0;
        start = 0;
    }

    for (let i = start; i < start + 10 && i < allItems.length; i++) {
        items.push(allItems[i]);
    }

    return {
        pageIndex,
        items,
        totalItemCount: allItems.length,
        itemsPerPage: 10
    };
}
