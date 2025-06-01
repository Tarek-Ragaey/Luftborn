export interface User {
    id: string;
    email: string;
    userName: string;
    roles: string[];
}

export interface AuthState {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    error: string | null;
}

export interface LoginRequest {
    email: string;
    password: string;
} 