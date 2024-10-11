using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenOperations.Models.Commands
{
    public interface ICommandAsync
    {
        Task ExecuteAsync();
    }
}
