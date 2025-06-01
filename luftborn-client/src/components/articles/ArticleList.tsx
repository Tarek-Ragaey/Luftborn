import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import {
    Box,
    Grid,
    Card,
    CardContent,
    Typography,
    Button,
    Chip,
    CardActions,
    IconButton,
    Alert,
    Fab,
    CircularProgress,
} from '@mui/material';
import { Edit as EditIcon, Add as AddIcon, Translate as TranslateIcon } from '@mui/icons-material';
import type { AppDispatch, RootState } from '../../store/store';
import { fetchArticles } from '../../store/articleSlice';
import type { Article } from '../../types/article';

const ArticleList: React.FC = () => {
    const dispatch = useDispatch<AppDispatch>();
    const navigate = useNavigate();
    const { articles, isLoading, error } = useSelector((state: RootState) => state.articles);
    const { user } = useSelector((state: RootState) => state.auth);

    const isWriter = user?.roles.includes('Writer');
    const isSuperAdmin = user?.roles.includes('Super Admin');
    const canCreateArticle = isWriter || isSuperAdmin;

    useEffect(() => {
        dispatch(fetchArticles({ page: 1, pageSize: 10 }));
    }, [dispatch]);

    const getTranslationTitle = (article: Article, languageKey: string) => {
        const translation = article.allTranslations.find(t => t.languageKey === languageKey);
        return translation?.title || 'No translation';
    };

    const hasTranslation = (article: Article, languageKey: string) => {
        return article.allTranslations.some(t => t.languageKey === languageKey);
    };

    if (!user) return null;

    if (isLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
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
                    Articles
                </Typography>
                {canCreateArticle && (
                    <Button
                        variant="contained"
                        color="primary"
                        startIcon={<AddIcon />}
                        onClick={() => navigate('/articles/new')}
                    >
                        Create Article
                    </Button>
                )}
            </Box>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
            )}

            <Grid container spacing={2}>
                {articles.map((article) => (
                    <Grid item xs={12} sm={6} md={4} key={article.id}>
                        <Card>
                            <CardContent>
                                <Typography variant="h6" component="h2" gutterBottom>
                                    {getTranslationTitle(article, 'en')}
                                </Typography>
                                <Typography color="textSecondary" gutterBottom>
                                    By {article.writerName}
                                </Typography>
                                <Box sx={{ mt: 2, display: 'flex', gap: 1 }}>
                                    <Chip
                                        icon={<TranslateIcon />}
                                        label="EN"
                                        color={hasTranslation(article, 'en') ? 'primary' : 'default'}
                                        size="small"
                                    />
                                    <Chip
                                        icon={<TranslateIcon />}
                                        label="AR"
                                        color={hasTranslation(article, 'ar') ? 'primary' : 'default'}
                                        size="small"
                                    />
                                    <Chip
                                        label={article.isPublished ? 'Published' : 'Draft'}
                                        color={article.isPublished ? 'success' : 'default'}
                                        size="small"
                                    />
                                </Box>
                            </CardContent>
                            <CardActions>
                                <Button
                                    size="small"
                                    onClick={() => navigate(`/articles/${article.id}`)}
                                >
                                    Read More
                                </Button>
                                {((isWriter && article.writerId === user?.userId) || isSuperAdmin) && (
                                    <IconButton
                                        size="small"
                                        onClick={() => navigate(`/articles/edit/${article.id}`)}
                                    >
                                        <EditIcon />
                                    </IconButton>
                                )}
                            </CardActions>
                        </Card>
                    </Grid>
                ))}
            </Grid>

            {articles.length === 0 && !error && (
                <Alert severity="info">No articles found.</Alert>
            )}

            {canCreateArticle && (
                <Fab
                    color="primary"
                    aria-label="add"
                    sx={{ position: 'fixed', bottom: 16, right: 16 }}
                    onClick={() => navigate('/articles/new')}
                >
                    <AddIcon />
                </Fab>
            )}
        </Box>
    );
};

export default ArticleList; 