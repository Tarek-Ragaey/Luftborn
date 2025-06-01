import api from './api';
import type { LoginRequest, User } from '../types/auth';
import { jwtDecode } from 'jwt-decode';

export const login = async (credentials: LoginRequest) => {
    const response = await api.post('/Authentication/login', credentials);
    const { token } = response.data;
    localStorage.setItem('token', token);
    return decodeToken(token);
};

export const logout = () => {
    localStorage.removeItem('token');
};

export const getCurrentUser = (): User | null => {
    const token = localStorage.getItem('token');
    if (token) {
        return decodeToken(token);
    }
    return null;
};

const decodeToken = (token: string): User => {
    const decoded: any = jwtDecode(token);
    return {
        id: decoded.nameid,
        email: decoded.email,
        userName: decoded.name,
        roles: decoded.role || [],
    };
}; 