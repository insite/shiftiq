import { useEffect, useState } from "react";
import { useStatusProvider } from "@/contexts/StatusProvider";
import { useLoadingProvider } from "@/contexts/LoadingProvider";

let _counter = 1;

const _actions: Map<number, unknown> = new Map();

interface Result<T> {
    isLoaded: boolean;
    runLoad: () => T;
}

interface ResultWithPromise<T> {
    isLoaded: boolean;
    runLoad: () => Promise<T>;
}

interface ResultBoth<T> {
    isLoaded: boolean;
    runLoad: () => T | Promise<T>;
}

export function useLoadAction<T>(action: () => T): Result<T>;
export function useLoadAction<T>(action: () => Promise<T>): ResultWithPromise<T>;

export function useLoadAction<T>(action: () => T | Promise<T>): ResultBoth<T> {
    const [isLoaded, setIsLoaded] = useState(false);

    const { addError, removeError } = useStatusProvider();
    const { addLoading, removeLoading } = useLoadingProvider();

    const [result] = useState(() => {
        const key = _counter++;
        return {
            run() {
                let storedValue = _actions.get(key);
                if (!storedValue) {
                    _actions.set(key, storedValue = action());

                    if (storedValue instanceof Promise) {
                        addLoading();

                        storedValue
                            .then(() => {
                                setIsLoaded(true);
                                removeError();
                            })
                            .catch(err => addError(err, "Failed to load data"))
                            .finally(() => removeLoading());
                    }
                }
                return storedValue as T | Promise<T>;
            },

            delete() {
                _actions.delete(key);
            }
        }
    });

    useEffect(() => () => result.delete(), [result]);

    return {
        isLoaded,
        runLoad: result.run
    };
}