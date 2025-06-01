import api from './api';
import type { Article, CreateArticle, UpdateArticle } from '../types/article';

export const getArticles = async (page: number = 1, pageSize: number = 10) => {
    const response = await api.get(`/article?pageNumber=${page}&pageSize=${pageSize}`);
    return response.data;
};

export const getArticle = async (id: number) => {
    const response = await api.get(`/article/${id}`);
    return response.data;
};

export const createArticle = async (article: CreateArticle) => {
    const response = await api.post('/article', article);
    return response.data;
};

export const updateArticle = async (id: number, article: UpdateArticle) => {
    const response = await api.put(`/article/${id}`, article);
    return response.data;
};

export const deleteArticle = async (id: number) => {
    await api.delete(`/article/${id}`);
}; 