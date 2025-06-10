import api from './api';
import type { CreateUser, UpdateUser } from '../types/user';

export const getUsers = async (page: number = 1, pageSize: number = 10) => {
    const response = await api.get(`/user?pageNumber=${page}&pageSize=${pageSize}`);
    return response.data;
};

export const getUser = async (id: string) => {
    const response = await api.get(`/user/${id}`);
    return response.data;
};

export const createUser = async (user: CreateUser) => {
    const response = await api.post('/user', user);
    return response.data;
};

export const updateUser = async (id: string, user: UpdateUser) => {
    const response = await api.put(`/user/${id}`, user);
    return response.data;
};

export const deleteUser = async (id: string) => {
    await api.delete(`/user/${id}`);
}; 