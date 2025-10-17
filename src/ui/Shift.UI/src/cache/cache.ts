import { _searchCache } from "./_searchCache";
import { _textByIdCache } from "./_textByIdCache";

export const cache = {
    search: _searchCache,
    textById: _textByIdCache,

    clear() {
        this.search.clear();
        this.textById.clear();
    },

    onGradebookChange() {
        this.search.clearRows("search.gradebook");
    },
}