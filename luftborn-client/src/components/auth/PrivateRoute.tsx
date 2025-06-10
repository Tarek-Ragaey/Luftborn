import React from 'react';
import { Navigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import type { RootState } from '../../store/store';

interface PrivateRouteProps {
    children: React.ReactNode;
    roles?: string[];
}

const PrivateRoute: React.FC<PrivateRouteProps> = ({ children, roles }) => {
    const { user } = useSelector((state: RootState) => state.auth);
    const isAuthenticated = !!user;

    if (!isAuthenticated) {
        return <Navigate to="/login" />;
    }

    if (roles && roles.length > 0) {
        const hasRequiredRole = roles.some(role => user?.roles.includes(role));
        if (!hasRequiredRole) {
            return <Navigate to="/articles" />;
        }
    }

    return <>{children}</>;
};

export default PrivateRoute; 