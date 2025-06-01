import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import {
    AppBar,
    Toolbar,
    Typography,
    Button,
    Box,
} from '@mui/material';
import { logout } from '../../store/authSlice';
import type { RootState } from '../../store/store';

const Navigation: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const dispatch = useDispatch();
    const { user } = useSelector((state: RootState) => state.auth);

    const handleLogout = () => {
        dispatch(logout());
        navigate('/login');
    };

    if (!user) return null;

    const isSuperAdmin = user.roles.includes('Super Admin');
    const canCreateArticle = user.roles.includes('Writer') || user.roles.includes('Super Admin');

    return (
        <AppBar position="static">
            <Toolbar>
                <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                    Luftborn Task
                </Typography>
                <Box sx={{ display: 'flex', gap: 2 }}>
                    <Button
                        color="inherit"
                        onClick={() => navigate('/articles')}
                        sx={{ color: location.pathname === '/articles' ? 'secondary.main' : 'inherit' }}
                    >
                        Articles
                    </Button>
                    {isSuperAdmin && (
                        <Button
                            color="inherit"
                            onClick={() => navigate('/users')}
                            sx={{ color: location.pathname === '/users' ? 'secondary.main' : 'inherit' }}
                        >
                            Users
                        </Button>
                    )}
                    <Button color="inherit" onClick={handleLogout}>
                        Logout
                    </Button>
                </Box>
            </Toolbar>
        </AppBar>
    );
};

export default Navigation; 