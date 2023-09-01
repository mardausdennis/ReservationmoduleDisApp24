using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisApp24.Services
{
    public interface INavigationService
    {
        void SetNavigation(INavigation navigation);
        INavigation GetNavigation();
    }
}

