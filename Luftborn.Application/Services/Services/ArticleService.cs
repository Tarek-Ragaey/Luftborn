using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Luftborn.Application.IServices.Interfaces;
using Luftborn.Application.Models.Articles;
using Luftborn.Domain.Entites;
using Luftborn.Domain.Repositories;

namespace Luftborn.Application.Services.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<(IEnumerable<ArticleDto> Articles, int TotalCount)> GetArticlesAsync(int pageNumber, int pageSize, string languageKey, string writerId = null)
        {
            var (articles, totalCount) = await _articleRepository.GetArticlesAsync(pageNumber, pageSize, writerId);
            
            var articleDtos = articles.Select(article => MapToArticleDto(article, languageKey)).ToList();
            
            return (articleDtos, totalCount);
        }

        public async Task<ArticleDto> GetArticleByIdAsync(int id, string languageKey)
        {
            var article = await _articleRepository.GetArticleByIdAsync(id);
            if (article == null)
                return null;

            return MapToArticleDto(article, languageKey);
        }

        public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto, string writerId)
        {
            var article = new Article
            {
                WriterId = writerId,
                IsPublished = createArticleDto.IsPublished,
                Translations = new List<ArticleTranslation>
                {
                    new ArticleTranslation
                    {
                        Title = createArticleDto.Title,
                        Content = createArticleDto.Content,
                        LanguageKey = createArticleDto.LanguageKey
                    }
                }
            };

            article = await _articleRepository.CreateArticleAsync(article);
            return MapToArticleDto(article, createArticleDto.LanguageKey);
        }

        public async Task<ArticleDto> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto)
        {
            var article = await _articleRepository.GetArticleByIdAsync(id);
            if (article == null)
                return null;

            if (updateArticleDto.IsPublished.HasValue)
            {
                article.IsPublished = updateArticleDto.IsPublished.Value;
            }

            var translation = await _articleRepository.GetArticleTranslationAsync(id, updateArticleDto.LanguageKey);
            if (translation == null)
            {
                translation = new ArticleTranslation
                {
                    ArticleId = id,
                    LanguageKey = updateArticleDto.LanguageKey
                };
                article.Translations.Add(translation);
            }

            translation.Title = updateArticleDto.Title ?? translation.Title;
            translation.Content = updateArticleDto.Content ?? translation.Content;
            translation.UpdatedAt = DateTime.UtcNow;

            article.UpdatedAt = DateTime.UtcNow;
            
            article = await _articleRepository.UpdateArticleAsync(article);
            return MapToArticleDto(article, updateArticleDto.LanguageKey);
        }

        public async Task<bool> DeleteArticleAsync(int id)
        {
            return await _articleRepository.DeleteArticleAsync(id);
        }

        private ArticleDto MapToArticleDto(Article article, string languageKey)
        {
            var currentTranslation = article.Translations
                .FirstOrDefault(t => t.LanguageKey == languageKey);

            return new ArticleDto
            {
                Id = article.Id,
                WriterId = article.WriterId,
                WriterName = article.Writer?.UserName,
                WriterEmail = article.Writer?.Email,
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt,
                IsPublished = article.IsPublished,
                CurrentTranslation = currentTranslation != null ? new ArticleTranslationDto
                {
                    Id = currentTranslation.Id,
                    Title = currentTranslation.Title,
                    Content = currentTranslation.Content,
                    LanguageKey = currentTranslation.LanguageKey,
                    CreatedAt = currentTranslation.CreatedAt,
                    UpdatedAt = currentTranslation.UpdatedAt
                } : null,
                AllTranslations = article.Translations.Select(t => new ArticleTranslationDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Content = t.Content,
                    LanguageKey = t.LanguageKey,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList()
            };
        }
    }
} 