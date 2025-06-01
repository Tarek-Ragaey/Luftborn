import React, { useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { Container, CssBaseline, ThemeProvider } from '@mui/material';
import { theme } from './theme';
import LoginForm from './components/auth/LoginForm';
import Navigation from './components/layout/Navigation';
import ArticleList from './components/articles/ArticleList';
import ArticleForm from './components/articles/ArticleForm';
import UserList from './components/users/UserList';
import UserForm from './components/users/UserForm';
import type { RootState } from './store/store';

const ProtectedRoute: React.FC<{
    children: React.ReactNode;
    allowedRoles?: string[];
}> = ({ children, allowedRoles }) => {
    const { user } = useSelector((state: RootState) => state.auth);

    if (!user) {
        return <Navigate to="/login" replace />;
    }

    if (allowedRoles && allowedRoles.length > 0) {
        const hasRequiredRole = allowedRoles.some(role => 
            user.roles.includes(role)
        );
        
        if (!hasRequiredRole) {
            return <Navigate to="/articles" replace />;
        }
    }

    return <>{children}</>;
};

const App: React.FC = () => {
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <Router>
                <div style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
                    <Navigation />
                    <Container component="main" sx={{ flex: 1, py: 3 }}>
                        <Routes>
                            <Route path="/login" element={<LoginForm />} />
                            <Route
                                path="/"
                                element={<Navigate to="/articles" replace />}
                            />
                            <Route
                                path="/articles"
                                element={
                                    <ProtectedRoute>
                                        <ArticleList />
                                    </ProtectedRoute>
                                }
                            />
                            <Route
                                path="/articles/new"
                                element={
                                    <ProtectedRoute allowedRoles={['Writer', 'Super Admin']}>
                                        <ArticleForm />
                                    </ProtectedRoute>
                                }
                            />
                            <Route
                                path="/articles/:id"
                                element={
                                    <ProtectedRoute>
                                        <ArticleForm mode="view" />
                                    </ProtectedRoute>
                                }
                            />
                            <Route
                                path="/articles/edit/:id"
                                element={
                                    <ProtectedRoute allowedRoles={['Writer', 'Super Admin']}>
                                        <ArticleForm mode="edit" />
                                    </ProtectedRoute>
                                }
                            />
                            <Route
                                path="/users"
                                element={
                                    <ProtectedRoute allowedRoles={['Super Admin']}>
                                        <UserList />
                                    </ProtectedRoute>
                                }
                            />
                            <Route
                                path="/users/new"
                                element={
                                    <ProtectedRoute allowedRoles={['Super Admin']}>
                                        <UserForm />
                                    </ProtectedRoute>
                                }
                            />
                            <Route
                                path="/users/edit/:id"
                                element={
                                    <ProtectedRoute allowedRoles={['Super Admin']}>
                                        <UserForm />
                                    </ProtectedRoute>
                                }
                            />
                        </Routes>
                    </Container>
                </div>
            </Router>
        </ThemeProvider>
    );
};

export default App;
