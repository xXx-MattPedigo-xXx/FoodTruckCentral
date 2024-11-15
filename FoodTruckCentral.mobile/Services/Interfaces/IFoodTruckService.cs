using FoodTruckCentral.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FoodTruckCentral.Mobile.Services.Interfaces
{
    public interface IFoodTruckService
    {
        // Search and Retrieval
        Task<List<FoodTruck>> SearchNearbyAsync(
            double latitude,
            double longitude,
            double radiusInMiles = 5,
            string searchQuery = null,
            List<string> categories = null);

        Task<FoodTruck> GetByIdAsync(string id);

        Task<List<FoodTruck>> GetFavoritesAsync();

        // Favorites Management
        Task<bool> AddToFavoritesAsync(string foodTruckId);
        Task<bool> RemoveFromFavoritesAsync(string foodTruckId);

        // Reviews and Ratings
        Task<bool> AddReviewAsync(string foodTruckId, Review review);
        Task<List<Review>> GetReviewsAsync(string foodTruckId, int page = 1, int pageSize = 10);

        // Categories
        Task<List<string>> GetCategoriesAsync();

        // Schedule
        Task<Schedule> GetScheduleAsync(string foodTruckId, DateTime date);
        Task<bool> IsOpenNowAsync(string foodTruckId);
    }
}
