﻿using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;

namespace MusicStore.Repositories.Implementations;

public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(MusicStoreDbContext context) : base(context)
    {
        
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await Context.Set<Customer>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email);
    }
}