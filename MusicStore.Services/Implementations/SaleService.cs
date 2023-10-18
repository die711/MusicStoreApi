using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Utils;

namespace MusicStore.Services.Implementations;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<SaleService> _logger;
    private readonly IMapper _mapper;
    private readonly IConcertRepository _concertRepository;
    private readonly ICustomerRepository _customerRepository;

    public SaleService(ISaleRepository saleRepository, ILogger<SaleService> logger, IMapper mapper,
                       IConcertRepository concertRepository, ICustomerRepository customerRepository)
    {
        _saleRepository = saleRepository;
        _concertRepository = concertRepository;
        _customerRepository = customerRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<long>> AddAsync(string email, SaleDtoRequest request)
    {
        var response = new BaseResponseGeneric<long>();

        try
        {
           // await _saleRepository.CreateTransaction();
            
            var entity = _mapper.Map<Sale>(request);

            var customer = await _customerRepository.GetByEmailAsync(email);

            if (customer is null)
                throw new InvalidOperationException($"No se encontre el usuario con ese email {email}");

            // if (customer is null)
            // {
            //     //vamos a crear en esta misma operacion el registro de customer
            //     customer = new Customer
            //     {
            //         Email = request.Email,
            //         FullName = request.FullName
            //     };
            //     
            //     //Esto es cuando esperamos a que el registro se grabe primero para obtener el id creado de la base de datos
            //     customer.Id = await _customerRepository.AddAsync(customer);
            //
            // }
            
            entity.CustomerId = customer.Id;

            //entity.Customer = customer;

            var concert = await _concertRepository.FindByIdAsync(request.ConcertId);

            if (concert is null)
                throw new InvalidOperationException($"No se encontro el concierto con el ID {request.ConcertId}");

            entity.Total = entity.Quantity * concert.UnitPrice;

            response.Data = await _saleRepository.AddAsync(entity);
            await _saleRepository.UpdateAsync();

            response.Success = true;

        }
        catch (Exception ex)
        {
            //await _saleRepository.RollBackAsync();
            _logger.LogError(ex, "Error adding sale");
            response.ErrorMessage = ex.Message;
        }

        return response;
    }

    public async Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(DateTime dateStart, DateTime dateEnd, int page, int rows)
    {
        var response = new BaseResponsePagination<SaleDtoResponse>();
        
        try
        {
            var end = dateEnd.AddHours(23);

            Expression<Func<Sale, bool>> predicate = p => p.SaleDate >= dateStart && p.SaleDate <= end;

            var tuple = await _saleRepository.ListAsync(predicate,
                p => _mapper.Map<SaleDtoResponse>(p),
                x => x.OperationNumber,
                page, rows);

            response.Data = tuple.Collection;
            response.TotalPages = Utilities.GetTotalPages(tuple.Total, rows);

            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error al listar {Message}", ex.Message);
            response.ErrorMessage = ex.Message;
        }

        return response;
    }

    public async Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(string email, string? filter, int page, int rows)
    {

        var response = new BaseResponsePagination<SaleDtoResponse>();
        Expression<Func<Sale, bool>> predicate = p =>
            p.Customer.Email.Equals(email) && p.Concert.Title.Contains(filter ?? string.Empty);
        
        try
        {
            var tuple = await _saleRepository.ListAsync(predicate, 
                p => _mapper.Map<SaleDtoResponse>(p),
                x => x.OperationNumber,
                page, rows);

            response.Data = tuple.Collection;
            response.TotalPages = Utils.Utilities.GetTotalPages(tuple.Total, rows);

            response.Success = true;


        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error al listar {message}", ex.Message);
            response.ErrorMessage = "Error al listar las ventas";
        }

        return response;

    }

    public async Task<BaseResponseGeneric<SaleDtoResponse>> GetSaleAsync(long id)
    {
        var response = new BaseResponseGeneric<SaleDtoResponse>();

        try
        {
            var sale = await _saleRepository.FindByIdAsync(id);

            if (sale == null)
                throw new Exception("Venta no existe");

            response.Data = _mapper.Map<SaleDtoResponse>(sale);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,"Error al obtener la Venta {Message}", ex.Message);
            response.ErrorMessage = "Error al obtener la venta";
        }
        
        return response;
    }
}