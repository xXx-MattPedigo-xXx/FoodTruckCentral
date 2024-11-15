using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTruckCentral.Mobile.Services.Interfaces
{
    public interface INavigationService
    {
        Task InitializeAsync();
        Task NavigateToAsync(string route);
        Task NavigateToAsync(string route, IDictionary<string, object> parameters);
        Task GoBackAsync();
        Task NavigateToMainAsync();
        Task NavigateToLoginAsync();
        Shell CurrentShell { get; }
    }

}
