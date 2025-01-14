﻿using ReversePhoneLookup.Models.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Abstract.Repositories
{
    public interface IOperatorRepository
    {
        Task<int> AddOperatorAsync(Operator @operator, CancellationToken cancellationToken);
        Task<Operator> GetOperatorAsync(int id, CancellationToken cancellationToken);
        Task<Operator> GetOperatorAsync(string mcc, string mnc, string name, CancellationToken cancellationToken);
    }
}
