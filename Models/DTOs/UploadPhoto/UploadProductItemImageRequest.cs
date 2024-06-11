using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.UploadPhoto
{
    public class UploadProductItemImageRequest
    {
        public ICollection<IFormFile> File { get; set; } 
        public Guid ProductItemId { get; set; }

    }
}
