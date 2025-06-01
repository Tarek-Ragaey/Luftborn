using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Luftborn.Domain.Entites
{
    public class Article
    {
        public int Id { get; set; }
        public string WriterId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        
        public virtual IdentityUser Writer { get; set; }
        public virtual ICollection<ArticleTranslation> Translations { get; set; }

        public Article()
        {
            Translations = new HashSet<ArticleTranslation>();
            CreatedAt = DateTime.UtcNow;
        }
    }
} 