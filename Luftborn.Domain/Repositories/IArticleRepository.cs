using System.Collections.Generic;
using System.Threading.Tasks;
using Luftborn.Domain.Entites;

namespace Luftborn.Domain.Repositories
{
    public interface IArticleRepository
    {
        Task<(IEnumerable<Article> Articles, int TotalCount)> GetArticlesAsync(int pageNumber, int pageSize, string writerId = null);
        Task<Article> GetArticleByIdAsync(int id);
        Task<Article> CreateArticleAsync(Article article);
        Task<Article> UpdateArticleAsync(Article article);
        Task<bool> DeleteArticleAsync(int id);
        Task<bool> ArticleExistsAsync(int id);
        Task<ArticleTranslation> GetArticleTranslationAsync(int articleId, string languageKey);
    }
} 