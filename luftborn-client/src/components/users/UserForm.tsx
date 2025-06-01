import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import {
    Box,
    Paper,
    TextField,
    Button,
    Typography,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Alert,
    CircularProgress,
} from '@mui/material';
import type { SelectChangeEvent } from '@mui/material';
import type { AppDispatch, RootState } from '../../store/store';
import { createUser, updateUser, fetchUser, clearCurrentUser } from '../../store/userSlice';

const UserForm: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const dispatch = useDispatch<AppDispatch>();
    const { currentUser, isLoading, error } = useSelector((state: RootState) => state.users);

    const [formData, setFormData] = useState({
        userName: '',
        email: '',
        password: '',
        roles: [] as string[],
    });

    useEffect(() => {
        if (id) {
            dispatch(fetchUser(id));
        }
        return () => {
            dispatch(clearCurrentUser());
        };
    }, [dispatch, id]);

    useEffect(() => {
        if (id && currentUser) {
            setFormData({
                userName: currentUser.userName,
                email: currentUser.email,
                password: '', // Don't populate password for editing
                roles: currentUser.roles,
            });
        }
    }, [id, currentUser]);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };

    const handleRolesChange = (event: SelectChangeEvent<string[]>) => {
        setFormData({
            ...formData,
            roles: event.target.value as string[],
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (id) {
                // For update, only include password if it was changed
                const updateData = {
                    ...formData,
                    password: formData.password || undefined,
                };
                await dispatch(updateUser({ id, user: updateData })).unwrap();
            } else {
                await dispatch(createUser(formData)).unwrap();
            }
            navigate('/users');
        } catch (error) {
            console.error('Failed to save user:', error);
        }
    };

    if (isLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Box sx={{ p: 3 }}>
            <Paper sx={{ p: 3 }}>
                <Typography variant="h5" component="h1" gutterBottom>
                    {id ? 'Edit User' : 'Create User'}
                </Typography>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                <form onSubmit={handleSubmit}>
                    <TextField
                        fullWidth
                        label="Username"
                        name="userName"
                        value={formData.userName}
                        onChange={handleInputChange}
                        margin="normal"
                        required
                    />

                    <TextField
                        fullWidth
                        label="Email"
                        name="email"
                        type="email"
                        value={formData.email}
                        onChange={handleInputChange}
                        margin="normal"
                        required
                    />

                    <TextField
                        fullWidth
                        label={id ? "New Password (leave blank to keep current)" : "Password"}
                        name="password"
                        type="password"
                        value={formData.password}
                        onChange={handleInputChange}
                        margin="normal"
                        required={!id}
                    />

                    <FormControl fullWidth margin="normal">
                        <InputLabel id="roles-label">Roles</InputLabel>
                        <Select
                            labelId="roles-label"
                            multiple
                            value={formData.roles}
                            onChange={handleRolesChange}
                            label="Roles"
                            required
                        >
                            <MenuItem value="Super Admin">Super Admin</MenuItem>
                            <MenuItem value="Writer">Writer</MenuItem>
                            <MenuItem value="User">User</MenuItem>
                        </Select>
                    </FormControl>

                    <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
                        <Button
                            type="submit"
                            variant="contained"
                            color="primary"
                            disabled={isLoading}
                        >
                            {id ? 'Update' : 'Create'}
                        </Button>
                        <Button
                            variant="outlined"
                            onClick={() => navigate('/users')}
                        >
                            Cancel
                        </Button>
                    </Box>
                </form>
            </Paper>
        </Box>
    );
};

export default UserForm; 