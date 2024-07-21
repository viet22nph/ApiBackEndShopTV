using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Tag
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string TagTitle { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
