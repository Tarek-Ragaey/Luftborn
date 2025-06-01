using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Luftborn.Application.IServices.Interfaces;
using Luftborn.Application.Models.Articles;
using Luftborn.Application.IServices.Models.Common;
using Luftborn.Application.IServices.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Luftborn.Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Writer,Super Admin")]

    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetArticles([FromQuery] PaginationParams paginationParams)
        {
            // Get language from HttpContext.Items (set by middleware)
            var languageKey = HttpContext.Items["LanguageKey"]?.ToString() ?? "en";

            var (articles, totalCount) = await _articleService.GetArticlesAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                languageKey
            );

            var paginationHeader = new PaginationHeader(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                totalCount,
                (int)System.Math.Ceiling(totalCount / (double)paginationParams.PageSize)
            );

            Response.AddPaginationHeader(paginationHeader);

            return Ok(articles);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyArticles([FromQuery] PaginationParams paginationParams)
        {
            var writerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var languageKey = HttpContext.Items["LanguageKey"]?.ToString() ?? "en";

            var (articles, totalCount) = await _articleService.GetArticlesAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                languageKey,
                writerId
            );

            var paginationHeader = new PaginationHeader(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                totalCount,
                (int)System.Math.Ceiling(totalCount / (double)paginationParams.PageSize)
            );

            Response.AddPaginationHeader(paginationHeader);

            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            var languageKey = HttpContext.Items["LanguageKey"]?.ToString() ?? "en";
            var article = await _articleService.GetArticleByIdAsync(id, languageKey);
            
            if (article == null)
                return NotFound();

            return Ok(article);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto createArticleDto)
        {
            var writerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var article = await _articleService.CreateArticleAsync(createArticleDto, writerId);
            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticleDto updateArticleDto)
        {
            var article = await _articleService.UpdateArticleAsync(id, updateArticleDto);
            
            if (article == null)
                return NotFound();

            return Ok(article);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var result = await _articleService.DeleteArticleAsync(id);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 