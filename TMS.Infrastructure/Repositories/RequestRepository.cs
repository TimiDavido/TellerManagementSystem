using Microsoft.EntityFrameworkCore;
using TMS.Application.Interfaces;
using TMS.Domain.Entities;
using TMS.Domain.Enums;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _context;

        public RequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Request> AddRequestAsync(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<List<Request>> GetPendingRequestAsync()
        {
            return await _context.Requests.Where(r => r.Status == RequestStatus.Pending).OrderBy(r => r.SubmittedAt).ToListAsync();
        }

        public async Task<Request?> GetRequestByIdAsync(Guid requestId)
        {
            return await _context.Requests.FirstOrDefaultAsync(r => r.Id == requestId);   
        }

        public async Task<List<Request>> GetRequestByUserIdAsync(Guid userId)
        {
            return await _context.Requests.Where(r => r.UserId == userId).OrderByDescending(r => r.SubmittedAt).ToListAsync();
        }

        public async Task SaveReviewAsync(Request request, RequestApprovalLog log)
        {
            _context.RequestApprovalLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
