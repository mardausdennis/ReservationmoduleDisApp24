using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisApp24.Models
{
    public class NavigationService
    {
        private INavigation _navigation;

        public void SetNavigation(INavigation navigation)
        {
            _navigation = navigation;
        }

        public INavigation GetNavigation()
        {
            if (_navigation == null)
            {
                throw new InvalidOperationException("Navigation not set.");
            }
            return _navigation;
        }
    }

}
