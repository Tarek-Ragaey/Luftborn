using System.Threading.Tasks;
using Luftborn.Application.Models.Articles;

namespace Luftborn.Application.IServices.Interfaces
{
    public interface IArticleService
    {
        Task<(IEnumerable<ArticleDto> Articles, int TotalCount)> GetArticlesAsync(int pageNumber, int pageSize, string languageKey, string writerId = null);
        Task<ArticleDto> GetArticleByIdAsync(int id, string languageKey);
        Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto, string writerId);
        Task<ArticleDto> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto);
        Task<bool> DeleteArticleAsync(int id);
    }
} 