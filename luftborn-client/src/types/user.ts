export interface User {
    id: string;
    userName: string;
    email: string;
    roles: string[];
}

export interface CreateUser {
    userName: string;
    email: string;
    password: string;
    roles: string[];
}

export interface UpdateUser {
    id: string;
    userName: string;
    email: string;
    roles: string[];
    password?: string;
} 