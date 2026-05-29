using Domain.Data;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FeedbackService
    {
        private readonly DataContext _context;

        public FeedbackService(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Feedback> GetFeedbacks(int pageNumber , int pageSize ,out int totalrecords)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageNumber = 5;


            // محاسبه تعداد آیتم‌هایی که باید رد شوند
            var skipCount = (pageNumber - 1) * pageSize;
            totalrecords =  _context.Feedbacks
               .Count();
            return  _context.Feedbacks
                .OrderByDescending(f => f.Cdate)
                .Skip(skipCount)
                .Take(pageSize)
                .AsNoTracking()
                .ToList();
            

        }
        public async Task<int> GetFeedbackCountAsync()
        {
            return await _context.Feedbacks.CountAsync();
        }
        public async Task AddFeedbackAsync(Feedback feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException(nameof(feedback));

            feedback.Cdate = DateTime.UtcNow;

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
        }
    }
}
