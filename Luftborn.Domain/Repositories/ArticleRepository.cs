using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Luftborn.Domain.Entites;

namespace Luftborn.Domain.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Article> Articles, int TotalCount)> GetArticlesAsync(int pageNumber, int pageSize, string writerId = null)
        {
            var query = _context.Articles
                .Include(a => a.Translations)
                .Include(a => a.Writer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(writerId))
            {
                query = query.Where(a => a.WriterId == writerId);
            }

            var totalCount = await query.CountAsync();

            var articles = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (articles, totalCount);
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            return await _context.Articles
                .Include(a => a.Translations)
                .Include(a => a.Writer)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Article> CreateArticleAsync(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<Article> UpdateArticleAsync(Article article)
        {
            _context.Entry(article).State = EntityState.Modified;
            foreach (var translation in article.Translations)
            {
                _context.Entry(translation).State = translation.Id == 0 
                    ? EntityState.Added 
                    : EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<bool> DeleteArticleAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
                return false;

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ArticleExistsAsync(int id)
        {
            return await _context.Articles.AnyAsync(a => a.Id == id);
        }

        public async Task<ArticleTranslation> GetArticleTranslationAsync(int articleId, string languageKey)
        {
            return await _context.Set<ArticleTranslation>()
                .FirstOrDefaultAsync(t => t.ArticleId == articleId && t.LanguageKey == languageKey);
        }
    }
} 