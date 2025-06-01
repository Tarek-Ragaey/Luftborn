import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import api from '../api/axios';

interface User {
    id: string;
    userName: string;
    email: string;
    roles: string[];
}

interface UserState {
    users: User[];
    currentUser: User | null;
    totalPages: number;
    currentPage: number;
    isLoading: boolean;
    error: string | null;
}

const initialState: UserState = {
    users: [],
    currentUser: null,
    totalPages: 1,
    currentPage: 1,
    isLoading: false,
    error: null,
};

export const fetchUsers = createAsyncThunk(
    'users/fetchUsers',
    async () => {
        const response = await api.get<User[]>('/user');
        // Log the response to help with debugging
        console.log('Users API response:', response.data);
        return response.data;
    }
);

export const fetchUser = createAsyncThunk(
    'users/fetchUser',
    async (id: string) => {
        const response = await api.get<User>(`/user/${id}`);
        return response.data;
    }
);

export const createUser = createAsyncThunk(
    'users/createUser',
    async (user: { userName: string; email: string; password: string; roles: string[] }) => {
        const response = await api.post<User>('/user', user);
        return response.data;
    }
);

export const updateUser = createAsyncThunk(
    'users/updateUser',
    async ({ id, user }: { id: string; user: { userName: string; email: string; password?: string; roles: string[] } }) => {
        const response = await api.put<User>(`/user/${id}`, user);
        return response.data;
    }
);

export const deleteUser = createAsyncThunk(
    'users/deleteUser',
    async (id: string) => {
        await api.delete(`/user/${id}`);
        return id;
    }
);

const userSlice = createSlice({
    name: 'users',
    initialState,
    reducers: {
        clearError: (state) => {
            state.error = null;
        },
        clearCurrentUser: (state) => {
            state.currentUser = null;
        },
    },
    extraReducers: (builder) => {
        builder
            // Fetch Users
            .addCase(fetchUsers.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchUsers.fulfilled, (state, action) => {
                state.isLoading = false;
                state.users = action.payload;
                // Since we're not paginating on the backend anymore
                state.totalPages = 1;
                state.currentPage = 1;
            })
            .addCase(fetchUsers.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to fetch users';
            })
            // Fetch Single User
            .addCase(fetchUser.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchUser.fulfilled, (state, action) => {
                state.isLoading = false;
                state.currentUser = action.payload;
            })
            .addCase(fetchUser.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to fetch user';
            })
            // Create User
            .addCase(createUser.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(createUser.fulfilled, (state, action) => {
                state.isLoading = false;
                state.users.unshift(action.payload);
            })
            .addCase(createUser.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to create user';
            })
            // Update User
            .addCase(updateUser.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(updateUser.fulfilled, (state, action) => {
                state.isLoading = false;
                const index = state.users.findIndex(user => user.id === action.payload.id);
                if (index !== -1) {
                    state.users[index] = action.payload;
                }
                if (state.currentUser?.id === action.payload.id) {
                    state.currentUser = action.payload;
                }
            })
            .addCase(updateUser.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to update user';
            })
            // Delete User
            .addCase(deleteUser.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(deleteUser.fulfilled, (state, action) => {
                state.isLoading = false;
                state.users = state.users.filter(user => user.id !== action.payload);
                if (state.currentUser?.id === action.payload) {
                    state.currentUser = null;
                }
            })
            .addCase(deleteUser.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to delete user';
            });
    },
});

export const { clearError, clearCurrentUser } = userSlice.actions;
export default userSlice.reducer; 