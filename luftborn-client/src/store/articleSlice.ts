import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import api from '../api/axios';
import type { Article } from '../types/article';

interface ArticleState {
    articles: Article[];
    currentArticle: Article | null;
    isLoading: boolean;
    error: string | null;
    totalPages: number;
    currentPage: number;
}

const initialState: ArticleState = {
    articles: [],
    currentArticle: null,
    isLoading: false,
    error: null,
    totalPages: 1,
    currentPage: 1,
};

export const fetchArticles = createAsyncThunk(
    'articles/fetchArticles',
    async ({ page, pageSize }: { page: number; pageSize: number }) => {
        const response = await api.get<Article[]>('/article', {
            params: { page, pageSize }
        });
        return response.data;
    }
);

export const fetchArticle = createAsyncThunk(
    'articles/fetchArticle',
    async (id: number) => {
        const response = await api.get<Article>(`/article/${id}`);
        return response.data;
    }
);

export const createArticle = createAsyncThunk(
    'articles/createArticle',
    async (article: Partial<Article>) => {
        const response = await api.post<Article>('/article', article);
        return response.data;
    }
);

export const updateArticle = createAsyncThunk(
    'articles/updateArticle',
    async ({ id, article }: { id: number; article: Partial<Article> }) => {
        const response = await api.put<Article>(`/article/${id}`, article);
        return response.data;
    }
);

export const deleteArticle = createAsyncThunk(
    'articles/deleteArticle',
    async (id: number) => {
        await api.delete(`/article/${id}`);
        return id;
    }
);

const articleSlice = createSlice({
    name: 'articles',
    initialState,
    reducers: {
        clearCurrentArticle: (state) => {
            state.currentArticle = null;
        },
    },
    extraReducers: (builder) => {
        builder
            // Fetch Articles
            .addCase(fetchArticles.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchArticles.fulfilled, (state, action) => {
                state.isLoading = false;
                state.articles = action.payload;
            })
            .addCase(fetchArticles.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to fetch articles';
            })
            // Fetch Single Article
            .addCase(fetchArticle.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchArticle.fulfilled, (state, action) => {
                state.isLoading = false;
                state.currentArticle = action.payload;
            })
            .addCase(fetchArticle.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to fetch article';
            })
            // Create Article
            .addCase(createArticle.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(createArticle.fulfilled, (state, action) => {
                state.isLoading = false;
                state.articles.push(action.payload);
            })
            .addCase(createArticle.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to create article';
            })
            // Update Article
            .addCase(updateArticle.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(updateArticle.fulfilled, (state, action) => {
                state.isLoading = false;
                const index = state.articles.findIndex(a => a.id === action.payload.id);
                if (index !== -1) {
                    state.articles[index] = action.payload;
                }
                if (state.currentArticle?.id === action.payload.id) {
                    state.currentArticle = action.payload;
                }
            })
            .addCase(updateArticle.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to update article';
            })
            // Delete Article
            .addCase(deleteArticle.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(deleteArticle.fulfilled, (state, action) => {
                state.isLoading = false;
                state.articles = state.articles.filter(article => article.id !== action.payload);
                if (state.currentArticle?.id === action.payload) {
                    state.currentArticle = null;
                }
            })
            .addCase(deleteArticle.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.error.message || 'Failed to delete article';
            });
    },
});

export const { clearCurrentArticle } = articleSlice.actions;

export default articleSlice.reducer; 