using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Tag.request
{
    public class UpdateTagRequestDto
    {
        public Guid Id { get; set; }
        public string TagTitle { get; set; }
    }
}
