import { Routes, Route, Navigate } from 'react-router-dom';
import LoginForm from '../components/auth/LoginForm';
import ArticleList from '../components/articles/ArticleList';
import ArticleForm from '../components/articles/ArticleForm';
import ArticleDetails from '../components/articles/ArticleDetails';
import UserList from '../components/users/UserList';
import UserForm from '../components/users/UserForm';
import PrivateRoute from '../components/auth/PrivateRoute';

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/login" element={<LoginForm />} />
            
            {/* Article Routes */}
            <Route
                path="/articles/new"
                element={
                    <PrivateRoute roles={['Writer', 'SuperAdmin']}>
                        <ArticleForm />
                    </PrivateRoute>
                }
            />
            <Route
                path="/articles/edit/:id"
                element={
                    <PrivateRoute roles={['Writer', 'SuperAdmin']}>
                        <ArticleForm />
                    </PrivateRoute>
                }
            />
            <Route
                path="/articles/:id"
                element={
                    <PrivateRoute>
                        <ArticleDetails />
                    </PrivateRoute>
                }
            />
            <Route
                path="/articles"
                element={
                    <PrivateRoute>
                        <ArticleList />
                    </PrivateRoute>
                }
            />

            {/* User Management Routes */}
            <Route
                path="/users/new"
                element={
                    <PrivateRoute roles={['SuperAdmin']}>
                        <UserForm />
                    </PrivateRoute>
                }
            />
            <Route
                path="/users/edit/:id"
                element={
                    <PrivateRoute roles={['SuperAdmin']}>
                        <UserForm />
                    </PrivateRoute>
                }
            />
            <Route
                path="/users"
                element={
                    <PrivateRoute roles={['SuperAdmin']}>
                        <UserList />
                    </PrivateRoute>
                }
            />

            <Route path="/" element={<Navigate to="/articles" replace />} />
        </Routes>
    );
};

export default AppRoutes; 