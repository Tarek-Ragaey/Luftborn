using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Luftborn.Application.IServices.Interfaces;
using Luftborn.Application.Models.Articles;
using Luftborn.Application.IServices.Models.Common;
using Luftborn.Application.IServices.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Luftborn.API.Constants;

namespace Luftborn.Task.Controllers
{
    /// <summary>
    /// Controller for managing articles with multilingual support
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.SuperAdminOrWriter)]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// Retrieves a paginated list of all articles
        /// </summary>
        /// <param name="paginationParams">Pagination parameters including page number, page size, and language</param>
        /// <returns>A paginated list of articles with translations for the specified language</returns>
        /// <response code="200">Returns the paginated list of articles</response>
        /// <response code="400">If the pagination parameters are invalid</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = Roles.AllRoles)]

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

        /// <summary>
        /// Retrieves a paginated list of articles written by the authenticated user
        /// </summary>
        /// <param name="paginationParams">Pagination parameters including page number, page size, and language</param>
        /// <returns>A paginated list of articles authored by the current user</returns>
        /// <response code="200">Returns the paginated list of user's articles</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("my")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Retrieves a specific article by its ID
        /// </summary>
        /// <param name="id">The ID of the article to retrieve</param>
        /// <returns>The article with translations for the current language</returns>
        /// <response code="200">Returns the requested article</response>
        /// <response code="404">If the article is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = Roles.AllRoles)]

        public async Task<IActionResult> GetArticle(int id)
        {
            var languageKey = HttpContext.Items["LanguageKey"]?.ToString() ?? "en";
            var article = await _articleService.GetArticleByIdAsync(id, languageKey);
            
            if (article == null)
                return NotFound();

            return Ok(article);
        }

        /// <summary>
        /// Creates a new article
        /// </summary>
        /// <param name="createArticleDto">The article data to create</param>
        /// <returns>The newly created article</returns>
        /// <response code="201">Returns the newly created article</response>
        /// <response code="400">If the article data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto createArticleDto)
        {
            var writerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var article = await _articleService.CreateArticleAsync(createArticleDto, writerId);
            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        /// <summary>
        /// Updates an existing article
        /// </summary>
        /// <param name="id">The ID of the article to update</param>
        /// <param name="updateArticleDto">The updated article data</param>
        /// <returns>The updated article</returns>
        /// <response code="200">Returns the updated article</response>
        /// <response code="404">If the article is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticleDto updateArticleDto)
        {
            var article = await _articleService.UpdateArticleAsync(id, updateArticleDto);
            
            if (article == null)
                return NotFound();

            return Ok(article);
        }

        /// <summary>
        /// Deletes an article
        /// </summary>
        /// <param name="id">The ID of the article to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the article was successfully deleted</response>
        /// <response code="404">If the article is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var result = await _articleService.DeleteArticleAsync(id);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 