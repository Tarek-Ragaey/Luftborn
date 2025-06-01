import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import api from '../api/axios';

interface User {
    userId: string;
    email: string;
    roles: string[];
    token: string;
}

interface AuthState {
    user: User | null;
    isLoading: boolean;
    error: string | null;
}

interface LoginCredentials {
    email: string;
    password: string;
}

interface LoginResponse {
    isSuccess: boolean;
    message: string;
    token: string;
    refreshToken: string;
    userId: string;
    email: string;
    roles: string[];
    errors: Record<string, string[]>;
}

const initialState: AuthState = {
    user: null,
    isLoading: false,
    error: null,
};

// Try to load user from localStorage on startup
const savedUser = localStorage.getItem('user');
if (savedUser) {
    try {
        const user = JSON.parse(savedUser);
        initialState.user = user;
        // Set the token in axios headers
        api.defaults.headers.common['Authorization'] = `Bearer ${user.token}`;
    } catch (e) {
        localStorage.removeItem('user');
    }
}

export const login = createAsyncThunk(
    'auth/login',
    async (credentials: LoginCredentials) => {
        const response = await api.post<LoginResponse>('/authentication/login', credentials);
        
        // Log the response to see what we're getting
        console.log('Login response:', response.data);
        
        // Extract the role from the response
        const { userId, email, roles, token } = response.data;
        
        // Create the user object with the exact role string from the response
        const user = {
            userId,
            email,
            roles: roles.map(role => role.trim()), // Trim any whitespace
            token
        };
        
        // Log the user object we're creating
        console.log('Created user object:', user);
        
        // Save user to localStorage
        localStorage.setItem('user', JSON.stringify(user));
        
        // Set default auth header
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        
        return user;
    }
);

export const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        logout: (state) => {
            state.user = null;
            state.error = null;
            localStorage.removeItem('user');
            delete api.defaults.headers.common['Authorization'];
        },
        setUser: (state, action: PayloadAction<User>) => {
            state.user = action.payload;
            localStorage.setItem('user', JSON.stringify(action.payload));
            api.defaults.headers.common['Authorization'] = `Bearer ${action.payload.token}`;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(login.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(login.fulfilled, (state, action) => {
                state.isLoading = false;
                state.user = action.payload;
                state.error = null;
            })
            .addCase(login.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Login failed';
            });
    },
});

export const { logout, setUser } = authSlice.actions;

export default authSlice.reducer; 