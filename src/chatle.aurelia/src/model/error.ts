class Key {
    IDBCursorWithValue: string;
}

class ErrorMessage {
    errorMessage: string;
}

export class Error {
    subKey: Key;
    errors: Array<ErrorMessage>;
}

