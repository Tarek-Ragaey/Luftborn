using System;
using System.Collections.Generic;

namespace Luftborn.Application.Models.Articles
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string WriterId { get; set; }
        public string WriterName { get; set; }
        public string WriterEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public ArticleTranslationDto CurrentTranslation { get; set; }
        public ICollection<ArticleTranslationDto> AllTranslations { get; set; }
    }

    public class ArticleTranslationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LanguageKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateArticleDto
    {

        public Dictionary<string, ArticleTranslationContentDto> Translations { get; set; }

        public bool IsPublished { get; set; }
    }

    public class ArticleTranslationContentDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class UpdateArticleDto
    {
        public Dictionary<string, ArticleTranslationContentDto> Translations { get; set; }
        public bool? IsPublished { get; set; }
    }
} 