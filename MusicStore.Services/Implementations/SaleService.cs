using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.DataAccess;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Entities.Info;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Utils;

namespace MusicStore.Services.Implementations;

public class SaleService : ISaleService
{
    private readonly MusicStoreDbContext _context;
    private readonly ILogger<SaleService> _logger;
    private readonly IMapper _mapper;

    public SaleService(MusicStoreDbContext context, ILogger<SaleService> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<long>> AddAsync(string email, SaleDtoRequest request)
    {
        var response = new BaseResponseGeneric<long>();

        var entity = _mapper.Map<Sale>(request);

        var customer = await _context.Set<Customer>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email);

        if (customer is null)
            throw new InvalidOperationException($"No se encontre el usuario con ese email {email}");

        entity.CustomerId = customer.Id;

        var concert = await _context.Set<Concert>().FindAsync(request.ConcertId);

        if (concert is null)
            throw new InvalidOperationException($"No se encontro el concierto con el ID {request.ConcertId}");

        entity.Total = entity.Quantity * concert.UnitPrice;

        // Sale numbering logic from repository
        entity.SaleDate = DateTime.Now;
        var lastNumber = await _context.Set<Sale>().CountAsync() + 1;
        entity.OperationNumber = $"{lastNumber:00000}";

        await _context.Set<Sale>().AddAsync(entity);
        await _context.SaveChangesAsync();

        response.Data = entity.Id;
        response.Success = true;

        return response;
    }

    public async Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(DateTime dateStart, DateTime dateEnd, int page, int rows)
    {
        var response = new BaseResponsePagination<SaleDtoResponse>();

        var end = dateEnd.AddHours(23);

        Expression<Func<Sale, bool>> predicate = p => p.SaleDate >= dateStart && p.SaleDate <= end;

        var query = _context.Set<Sale>()
            .Include(p => p.Concert)
            .ThenInclude(p => p.Genre)
            .Include(p => p.Customer)
            .Where(predicate);

        var collection = await query
            .OrderBy(x => x.OperationNumber)
            .Skip((page - 1) * rows)
            .Take(rows)
            .AsNoTracking()
            .Select(p => _mapper.Map<SaleDtoResponse>(p))
            .ToListAsync();

        var total = await query.CountAsync();

        response.Data = collection;
        response.TotalPages = Utilities.GetTotalPages(total, rows);

        response.Success = true;

        return response;
    }

    public async Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(string email, string? filter, int page, int rows)
    {

        var response = new BaseResponsePagination<SaleDtoResponse>();
        Expression<Func<Sale, bool>> predicate = p =>
            p.Customer.Email.Equals(email) && p.Concert.Title.Contains(filter ?? string.Empty);

        var query = _context.Set<Sale>()
            .Include(p => p.Concert)
            .ThenInclude(p => p.Genre)
            .Include(p => p.Customer)
            .Where(predicate);

        var collection = await query
            .OrderBy(x => x.OperationNumber)
            .Skip((page - 1) * rows)
            .Take(rows)
            .AsNoTracking()
            .Select(p => _mapper.Map<SaleDtoResponse>(p))
            .ToListAsync();

        var total = await query.CountAsync();

        response.Data = collection;
        response.TotalPages = Utils.Utilities.GetTotalPages(total, rows);

        response.Success = true;

        return response;

    }

    public async Task<BaseResponseGeneric<SaleDtoResponse>> GetSaleAsync(long id)
    {
        var response = new BaseResponseGeneric<SaleDtoResponse>();

        var sale = await _context.Set<Sale>()
            .Include(p => p.Concert)
            .ThenInclude(p => p.Genre)
            .Include(p => p.Customer)
            .Where(p => p.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (sale == null)
            throw new Exception("Venta no existe");

        response.Data = _mapper.Map<SaleDtoResponse>(sale);
        response.Success = true;

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<ReportDtoResponse>>> GetReportSaleAsync(DateTime dateStart, DateTime dateEnd)
    {
        var response = new BaseResponseGeneric<ICollection<ReportDtoResponse>>();

        var list = await _context.Set<ReportInfo>()
            .FromSqlRaw("Exec uspReportSales {0}, {1}", dateStart, dateEnd)
            .ToListAsync();

        response.Data = list.Select(p => _mapper.Map<ReportDtoResponse>(p)).ToList();
        response.Success = true;


        return response;

    }
}