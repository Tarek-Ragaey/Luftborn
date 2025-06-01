import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import {
    Box,
    Paper,
    TextField,
    Button,
    Typography,
    FormControlLabel,
    Switch,
    Alert,
    CircularProgress,
} from '@mui/material';
import type { AppDispatch, RootState } from '../../store/store';
import { createArticle, updateArticle, fetchArticle } from '../../store/articleSlice';
import type { Article } from '../../types/article';

interface ArticleFormProps {
    mode?: 'create' | 'edit' | 'view';
}

interface FormData {
    title: string;
    content: string;
    isPublished: boolean;
}

const ArticleForm: React.FC<ArticleFormProps> = ({ mode = 'create' }) => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const dispatch = useDispatch<AppDispatch>();
    const { currentArticle, isLoading, error } = useSelector((state: RootState) => state.articles);
    const { user } = useSelector((state: RootState) => state.auth);

    const [formData, setFormData] = useState<FormData>({
        title: '',
        content: '',
        isPublished: false,
    });

    useEffect(() => {
        if (id && (mode === 'edit' || mode === 'view')) {
            dispatch(fetchArticle(parseInt(id)));
        }
    }, [dispatch, id, mode]);

    useEffect(() => {
        if (currentArticle && (mode === 'edit' || mode === 'view')) {
            const mainTranslation = currentArticle.allTranslations.find(t => t.languageKey === 'en') || currentArticle.allTranslations[0];
            if (mainTranslation) {
                setFormData({
                    title: mainTranslation.title,
                    content: mainTranslation.content,
                    isPublished: currentArticle.isPublished,
                });
            }
        }
    }, [currentArticle, mode]);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value, checked } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: name === 'isPublished' ? checked : value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (mode === 'edit' && id) {
                await dispatch(updateArticle({ id: parseInt(id), article: formData })).unwrap();
            } else {
                await dispatch(createArticle(formData)).unwrap();
            }
            navigate('/articles');
        } catch (error) {
            console.error('Failed to save article:', error);
        }
    };

    if (isLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
                <CircularProgress />
            </Box>
        );
    }

    const isViewMode = mode === 'view';
    const canEdit = mode === 'edit' && (user?.roles.includes('Writer') || user?.roles.includes('Super Admin'));

    return (
        <Box sx={{ p: 3 }}>
            <Paper sx={{ p: 3 }}>
                <Typography variant="h5" component="h1" gutterBottom>
                    {mode === 'edit' ? 'Edit Article' : mode === 'view' ? 'View Article' : 'Create Article'}
                </Typography>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                <form onSubmit={handleSubmit}>
                    <TextField
                        fullWidth
                        label="Title"
                        name="title"
                        value={formData.title}
                        onChange={handleInputChange}
                        margin="normal"
                        required
                        disabled={isViewMode}
                    />

                    <TextField
                        fullWidth
                        label="Content"
                        name="content"
                        value={formData.content}
                        onChange={handleInputChange}
                        margin="normal"
                        required
                        multiline
                        rows={4}
                        disabled={isViewMode}
                    />

                    <FormControlLabel
                        control={
                            <Switch
                                checked={formData.isPublished}
                                onChange={handleInputChange}
                                name="isPublished"
                                disabled={isViewMode}
                            />
                        }
                        label="Published"
                        sx={{ mt: 2 }}
                    />

                    <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
                        {!isViewMode && (
                            <Button
                                type="submit"
                                variant="contained"
                                color="primary"
                                disabled={isLoading}
                            >
                                {mode === 'edit' ? 'Update' : 'Create'}
                            </Button>
                        )}
                        {isViewMode && canEdit && (
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={() => navigate(`/articles/edit/${id}`)}
                            >
                                Edit
                            </Button>
                        )}
                        <Button
                            variant="outlined"
                            onClick={() => navigate('/articles')}
                        >
                            Back to Articles
                        </Button>
                    </Box>
                </form>
            </Paper>
        </Box>
    );
};

export default ArticleForm; 