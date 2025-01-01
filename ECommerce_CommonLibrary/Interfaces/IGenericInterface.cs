﻿using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce_CommonLibrary.Responses;
using System.Linq.Expressions;

namespace ECommerce_CommonLibrary.Interfaces
{
    public interface IGenericInterface<T> where T : class
    {
        Task<Responses.Response> CreateAsync(T entity);
        Task<Responses.Response> UpdateAsync(T entity);
        Task<Responses.Response> DeleteAsync(T entity);
        Task<IEnumerable<Responses.Response>> GetAllAsync();
        Task<T> FindByIdAsync(int id);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);

    }
}
