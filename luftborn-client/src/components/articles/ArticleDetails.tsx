import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import {
    Box,
    Paper,
    Typography,
    Button,
    Chip,
    ToggleButton,
    ToggleButtonGroup,
    Alert,
    IconButton,
} from '@mui/material';
import { Edit as EditIcon, ArrowBack as ArrowBackIcon } from '@mui/icons-material';
import type { AppDispatch, RootState } from '../../store/store';
import { fetchArticle } from '../../store/articleSlice';
import type { Translation } from '../../types/article';

const ArticleDetails: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const dispatch = useDispatch<AppDispatch>();
    const { currentArticle: article, isLoading, error } = useSelector((state: RootState) => state.articles);
    const { user } = useSelector((state: RootState) => state.auth);
    const [selectedLanguage, setSelectedLanguage] = useState<string>('en');

    const isWriter = user?.roles.includes('Writer');
    const isSuperAdmin = user?.roles.includes('SuperAdmin');
    const canEdit = (isWriter && article?.writerId === user?.id) || isSuperAdmin;

    useEffect(() => {
        if (id) {
            dispatch(fetchArticle(parseInt(id)));
        }
    }, [dispatch, id]);

    const handleLanguageChange = (
        _: React.MouseEvent<HTMLElement>,
        newLanguage: string,
    ) => {
        if (newLanguage !== null) {
            setSelectedLanguage(newLanguage);
        }
    };

    const getTranslation = (translations: Translation[] | undefined): Translation | null => {
        if (!translations || translations.length === 0) return null;
        return translations.find(t => t.languageKey === selectedLanguage) || translations[0];
    };

    if (isLoading) {
        return (
            <Box sx={{ p: 3, display: 'flex', justifyContent: 'center' }}>
                <Typography>Loading...</Typography>
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

    if (!article) {
        return (
            <Box sx={{ p: 3 }}>
                <Alert severity="warning">Article not found</Alert>
            </Box>
        );
    }

    const translation = getTranslation(article.allTranslations);

    return (
        <Box sx={{ p: 3, maxWidth: 1200, mx: 'auto' }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <IconButton onClick={() => navigate('/articles')}>
                        <ArrowBackIcon />
                    </IconButton>
                    <Typography variant="h4" component="h1">
                        {translation?.title || 'No title in selected language'}
                    </Typography>
                </Box>
                <Box sx={{ display: 'flex', gap: 2 }}>
                    <ToggleButtonGroup
                        value={selectedLanguage}
                        exclusive
                        onChange={handleLanguageChange}
                        aria-label="language selection"
                    >
                        <ToggleButton value="en" aria-label="english">
                            EN
                        </ToggleButton>
                        <ToggleButton value="ar" aria-label="arabic">
                            AR
                        </ToggleButton>
                    </ToggleButtonGroup>
                    {canEdit && (
                        <Button
                            startIcon={<EditIcon />}
                            variant="contained"
                            onClick={() => navigate(`/articles/edit/${article.id}`)}
                        >
                            Edit
                        </Button>
                    )}
                </Box>
            </Box>

            <Paper sx={{ p: 4, mb: 3 }}>
                <Box sx={{ mb: 3, display: 'flex', gap: 2, alignItems: 'center' }}>
                    <Typography variant="subtitle1" color="textSecondary">
                        By {article.writerName}
                    </Typography>
                    <Chip
                        label={article.isPublished ? 'Published' : 'Draft'}
                        color={article.isPublished ? 'success' : 'default'}
                    />
                </Box>

                {translation ? (
                    <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap' }}>
                        {translation.content}
                    </Typography>
                ) : (
                    <Alert severity="info">No content available in selected language</Alert>
                )}
            </Paper>

            <Box sx={{ mt: 4 }}>
                <Typography variant="h6" gutterBottom>
                    Available Translations
                </Typography>
                <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
                    {article.allTranslations.map((t) => (
                        <Chip
                            key={t.languageKey}
                            label={t.languageKey.toUpperCase()}
                            onClick={() => setSelectedLanguage(t.languageKey)}
                            color={t.languageKey === selectedLanguage ? 'primary' : 'default'}
                        />
                    ))}
                </Box>
            </Box>
        </Box>
    );
};

export default ArticleDetails; 