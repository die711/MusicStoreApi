using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;

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
}