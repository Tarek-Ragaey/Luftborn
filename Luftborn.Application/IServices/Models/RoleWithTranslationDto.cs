using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luftborn.Application.IServices.Models
{
    public class RoleWithTranslationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TranslatedName { get; set; }
    }
}
