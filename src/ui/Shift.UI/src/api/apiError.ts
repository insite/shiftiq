export class ApiError extends Error {
    constructor (
        public status: number,
        public statusMessage: string
    ) {
        super(`HTTP error. Status: ${status}. ${statusMessage}`);
    }
}