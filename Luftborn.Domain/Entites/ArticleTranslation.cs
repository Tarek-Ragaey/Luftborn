using System;

namespace Luftborn.Domain.Entites
{
    public class ArticleTranslation
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LanguageKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Article Article { get; set; }

        public ArticleTranslation()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
} 