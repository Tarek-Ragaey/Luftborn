import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import {
    Box,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Button,
    IconButton,
    Typography,
    Chip,
    Alert,
    CircularProgress,
} from '@mui/material';
import { Edit as EditIcon, Delete as DeleteIcon, Add as AddIcon } from '@mui/icons-material';
import type { AppDispatch, RootState } from '../../store/store';
import { fetchUsers, deleteUser } from '../../store/userSlice';

const UserList: React.FC = () => {
    const dispatch = useDispatch<AppDispatch>();
    const navigate = useNavigate();
    const { users, isLoading, error } = useSelector(
        (state: RootState) => state.users
    );
    const { user: currentUser } = useSelector((state: RootState) => state.auth);

    useEffect(() => {
        if (currentUser?.roles?.includes('Super Admin')) {
            dispatch(fetchUsers());
        }
    }, [dispatch, currentUser]);

    const handleDelete = async (id: string) => {
        if (window.confirm('Are you sure you want to delete this user?')) {
            await dispatch(deleteUser(id));
            dispatch(fetchUsers());
        }
    };

    if (!currentUser?.roles?.includes('Super Admin')) {
        return (
            <Box sx={{ p: 3 }}>
                <Alert severity="error">You don't have permission to access this page.</Alert>
            </Box>
        );
    }

    if (isLoading) {
        return (
            <Box sx={{ p: 3, display: 'flex', justifyContent: 'center' }}>
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Box sx={{ p: 3 }}>
                <Alert severity="error">{error}</Alert>
            </Box>
        );
    }

    return (
        <Box sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h4" component="h1">
                    Users
                </Typography>
                <Button
                    variant="contained"
                    color="primary"
                    startIcon={<AddIcon />}
                    onClick={() => navigate('/users/new')}
                >
                    Create User
                </Button>
            </Box>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
            )}

            {users && users.length > 0 ? (
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Username</TableCell>
                                <TableCell>Email</TableCell>
                                <TableCell>Roles</TableCell>
                                <TableCell align="right">Actions</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {users.map((user) => (
                                <TableRow key={user.id}>
                                    <TableCell>{user.userName}</TableCell>
                                    <TableCell>{user.email}</TableCell>
                                    <TableCell>
                                        <Box sx={{ display: 'flex', gap: 1 }}>
                                            {user.roles?.map((role) => (
                                                <Chip
                                                    key={role}
                                                    label={role}
                                                    color={role === 'Super Admin' ? 'error' : role === 'Writer' ? 'primary' : 'default'}
                                                    size="small"
                                                />
                                            ))}
                                        </Box>
                                    </TableCell>
                                    <TableCell align="right">
                                        <IconButton
                                            size="small"
                                            onClick={() => navigate(`/users/edit/${user.id}`)}
                                        >
                                            <EditIcon />
                                        </IconButton>
                                        {user.id !== currentUser?.userId && (
                                            <IconButton
                                                size="small"
                                                onClick={() => handleDelete(user.id)}
                                                color="error"
                                            >
                                                <DeleteIcon />
                                            </IconButton>
                                        )}
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            ) : (
                <Alert severity="info">No users found.</Alert>
            )}
        </Box>
    );
};

export default UserList; 