using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
namespace Data.Repos.TagRepo
{
    internal class TagRepository: ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckTagTitleExistsAsync(string tagTitle)
        {
            return await _context.Set<Tag>().AnyAsync(x=> x.TagTitle.ToLower() == tagTitle.ToLower());
        }
    }
}
