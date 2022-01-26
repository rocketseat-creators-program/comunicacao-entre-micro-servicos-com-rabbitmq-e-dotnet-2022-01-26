using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertStore.SeedWork.Interfaces
{
    public interface IUseCase<TInput, TOutput>
    {
        Task<TOutput> Handle(TInput input);
    }

    public interface IUseCase<TOutput>
    {
        Task<TOutput> Handle();
    }
}
