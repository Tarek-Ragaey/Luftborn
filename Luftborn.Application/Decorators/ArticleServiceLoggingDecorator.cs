using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Luftborn.Application.IServices.Interfaces;
using Luftborn.Application.Models.Articles;

namespace Luftborn.Application.Decorators
{
    public class ArticleServiceLoggingDecorator : LoggingDecorator<IArticleService>, IArticleService
    {
        public ArticleServiceLoggingDecorator(IArticleService articleService, ILogger<IArticleService> logger)
            : base(articleService, logger)
        {
        }

        public async Task<(IEnumerable<ArticleDto> Articles, int TotalCount)> GetArticlesAsync(
            int pageNumber, int pageSize, string languageKey, string writerId = null)
        {
            try
            {
                LogMethodEntry(nameof(GetArticlesAsync), pageNumber, pageSize, languageKey, writerId);
                
                var result = await _service.GetArticlesAsync(pageNumber, pageSize, languageKey, writerId);
                
                LogMethodExit(nameof(GetArticlesAsync), new { result.TotalCount, Articles = result.Articles });
                
                return result;
            }
            catch (Exception ex)
            {
                LogError(nameof(GetArticlesAsync), ex, pageNumber, pageSize, languageKey, writerId);
                throw;
            }
        }

        public async Task<ArticleDto> GetArticleByIdAsync(int id, string languageKey)
        {
            try
            {
                LogMethodEntry(nameof(GetArticleByIdAsync), id, languageKey);
                
                var result = await _service.GetArticleByIdAsync(id, languageKey);
                
                LogMethodExit(nameof(GetArticleByIdAsync), result);
                
                return result;
            }
            catch (Exception ex)
            {
                LogError(nameof(GetArticleByIdAsync), ex, id, languageKey);
                throw;
            }
        }

        public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto, string writerId)
        {
            try
            {
                LogMethodEntry(nameof(CreateArticleAsync), createArticleDto, writerId);
                
                var result = await _service.CreateArticleAsync(createArticleDto, writerId);
                
                LogMethodExit(nameof(CreateArticleAsync), result);
                
                return result;
            }
            catch (Exception ex)
            {
                LogError(nameof(CreateArticleAsync), ex, createArticleDto, writerId);
                throw;
            }
        }

        public async Task<ArticleDto> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto)
        {
            try
            {
                LogMethodEntry(nameof(UpdateArticleAsync), id, updateArticleDto);
                
                var result = await _service.UpdateArticleAsync(id, updateArticleDto);
                
                LogMethodExit(nameof(UpdateArticleAsync), result);
                
                return result;
            }
            catch (Exception ex)
            {
                LogError(nameof(UpdateArticleAsync), ex, id, updateArticleDto);
                throw;
            }
        }

        public async Task<bool> DeleteArticleAsync(int id)
        {
            try
            {
                LogMethodEntry(nameof(DeleteArticleAsync), id);
                
                var result = await _service.DeleteArticleAsync(id);
                
                LogMethodExit(nameof(DeleteArticleAsync), result);
                
                return result;
            }
            catch (Exception ex)
            {
                LogError(nameof(DeleteArticleAsync), ex, id);
                throw;
            }
        }
    }
} 