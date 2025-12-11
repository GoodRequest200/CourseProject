using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Data.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _context;
        public ImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Image?> GetByIdAsync(int id)
        {
            var entity = await _context.Images.FindAsync(id);
            if (entity == null) return null;

            return Image.Create(entity.FilePath).Value;
        }

        public async Task<IEnumerable<Image>> GetAllAsync()
        {
            var list = await _context.Images.ToListAsync();
            return list.Select(i => Image.Create(i.FilePath).Value);
        }

        public async Task<Image> CreateAsync(Image model)
        {
            //var entity = new Image
            //{
            //    FileName = model.FileName
            //};

            //_context.Images.Add(entity);
            //await _context.SaveChangesAsync();

            //return model;

            throw new NotSupportedException("Images cannot be updated.");
        }

        public async Task<Image> UpdateAsync(Image model)
        {
            throw new NotSupportedException("Images cannot be updated.");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Images.FindAsync(id);
            if (entity == null) return false;

            _context.Images.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
