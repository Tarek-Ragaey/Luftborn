import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import {
    AppBar,
    Box,
    Toolbar,
    Typography,
    Button,
    Container,
    IconButton,
    Menu,
    MenuItem,
} from '@mui/material';
import { AccountCircle } from '@mui/icons-material';
import type { RootState } from '../../store/store';
import { logout } from '../../store/authSlice';

interface LayoutProps {
    children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const { user } = useSelector((state: RootState) => state.auth);
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

    const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleLogout = () => {
        dispatch(logout());
        navigate('/login');
    };

    const isWriter = user?.roles.includes('Writer');
    const isSuperAdmin = user?.roles.includes('SuperAdmin');

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <AppBar position="static">
                <Toolbar>
                    <Typography
                        variant="h6"
                        component="div"
                        sx={{ flexGrow: 1, cursor: 'pointer' }}
                        onClick={() => navigate('/')}
                    >
                        Luftborn Articles
                    </Typography>

                    {user ? (
                        <>
                            <Button
                                color="inherit"
                                onClick={() => navigate('/articles')}
                            >
                                Articles
                            </Button>
                            {(isWriter || isSuperAdmin) && (
                                <Button
                                    color="inherit"
                                    onClick={() => navigate('/articles/new')}
                                >
                                    New Article
                                </Button>
                            )}
                            <IconButton
                                size="large"
                                onClick={handleMenu}
                                color="inherit"
                            >
                                <AccountCircle />
                            </IconButton>
                            <Menu
                                anchorEl={anchorEl}
                                open={Boolean(anchorEl)}
                                onClose={handleClose}
                            >
                                <MenuItem disabled>
                                    {user.email}
                                </MenuItem>
                                <MenuItem disabled>
                                    Role: {user.roles.join(', ')}
                                </MenuItem>
                                <MenuItem onClick={handleLogout}>
                                    Logout
                                </MenuItem>
                            </Menu>
                        </>
                    ) : (
                        <Button
                            color="inherit"
                            onClick={() => navigate('/login')}
                        >
                            Login
                        </Button>
                    )}
                </Toolbar>
            </AppBar>

            <Container component="main" sx={{ flex: 1, py: 3 }}>
                {children}
            </Container>

            <Box
                component="footer"
                sx={{
                    py: 3,
                    px: 2,
                    mt: 'auto',
                    backgroundColor: (theme) =>
                        theme.palette.mode === 'light'
                            ? theme.palette.grey[200]
                            : theme.palette.grey[800],
                }}
            >
                <Container maxWidth="sm">
                    <Typography variant="body2" color="text.secondary" align="center">
                        © {new Date().getFullYear()} Luftborn Articles. All rights reserved.
                    </Typography>
                </Container>
            </Box>
        </Box>
    );
};

export default Layout; 