using Microsoft.EntityFrameworkCore;
using SagaPatternMichael.Orchestration.Data;

namespace SagaPatternMichael.Orchestration.Services
{
    public interface IOrchestrationService
    {
        Task AddMsg(EventBox eventBox);
        Task AddMsgError(EventErrorBox eventBox);
        Task Remove(EventBox eventBox);
        Task RemoveError(EventErrorBox eventBox);
        Task<List<EventBox>> GetEvents();
        Task<List<EventErrorBox>> GetEventsError();
    }
    public class OrchestrationService : IOrchestrationService
    {
        private readonly OrchestrationMichaelContext _context;

        public OrchestrationService(OrchestrationMichaelContext orchestrationMichaelContext)
        {
            _context = orchestrationMichaelContext;
        }
        public async Task AddMsg(EventBox eventBox)
        {
            await _context.EventBoxes.AddAsync(eventBox);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(EventBox eventBox)
        {
            _context.EventBoxes.Remove(eventBox);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EventBox>> GetEvents() 
            => await _context.EventBoxes.ToListAsync();

        public async Task<List<EventErrorBox>> GetEventsError()
         => await _context.EventErrorBoxes.ToListAsync();

        public async Task AddMsgError(EventErrorBox eventBox)
        {
            await _context.EventErrorBoxes.AddAsync(eventBox);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveError(EventErrorBox eventBox)
        {
            _context.EventErrorBoxes.Remove(eventBox);
            await _context.SaveChangesAsync();
        }
    }
}
