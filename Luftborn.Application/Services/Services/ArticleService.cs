using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Luftborn.Application.IServices.Interfaces;
using Luftborn.Application.Models.Articles;
using Luftborn.Domain.Entites;
using Luftborn.Domain.Repositories;
using Microsoft.IdentityModel.Tokens;

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
                IsPublished = createArticleDto.IsPublished
            };
           foreach (var translation in createArticleDto.Translations)
            {
                article.Translations.Add(new ArticleTranslation
                {
                    Title = translation.Value.Title,
                    Content = translation.Value.Content,
                    LanguageKey = translation.Key,
                    CreatedAt = DateTime.UtcNow
                });
            }

            article = await _articleRepository.CreateArticleAsync(article);
            return MapToArticleDto(article);
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

            article.Translations = new List<ArticleTranslation>();
            foreach (var translation in updateArticleDto.Translations)
            {
                article.Translations.Add(new ArticleTranslation
                {
                    Title = translation.Value.Title,
                    Content = translation.Value.Content,
                    LanguageKey = translation.Key,
                    CreatedAt = DateTime.UtcNow
                });
            }

            article.UpdatedAt = DateTime.UtcNow;
            
            article = await _articleRepository.UpdateArticleAsync(article);
            return MapToArticleDto(article);
        }

        public async Task<bool> DeleteArticleAsync(int id)
        {
            return await _articleRepository.DeleteArticleAsync(id);
        }

        private ArticleDto MapToArticleDto(Article article, string languageKey = null)
        {
            ArticleTranslation currentTranslation = null;
            if (!languageKey.IsNullOrEmpty())
             currentTranslation = article.Translations
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