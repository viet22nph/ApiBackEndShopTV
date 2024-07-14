using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Account
{
    public class ExternalAuthDto
    {

        public string? Provider { get; set; }
        public string? IdToken { get; set; }
    }
}
