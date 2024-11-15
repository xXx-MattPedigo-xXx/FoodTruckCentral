using System.Diagnostics;
using System.Net.Http.Json;
using FoodTruckCentral.Mobile.Services.Interfaces;

namespace FoodTruckCentral.Mobile.Services.Implementations
{
    public class FoodTruckService : IFoodTruckService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly IConnectivity _connectivity;
        private readonly ISecureStorage _secureStorage;
        private readonly IGeolocation _geolocation;
        private readonly DatabaseContext _dbContext;

        public FoodTruckService(
            HttpClient httpClient,
            IAuthService authService,
            IConnectivity connectivity,
            ISecureStorage secureStorage,
            IGeolocation geolocation,
            DatabaseContext dbContext)
        {
            _httpClient = httpClient;
            _authService = authService;
            _connectivity = connectivity;
            _secureStorage = secureStorage;
            _geolocation = geolocation;
            _dbContext = dbContext;
        }

        public async Task<List<FoodTruck>> SearchNearbyAsync(
            double latitude,
            double longitude,
            double radiusInMiles = 5,
            string searchQuery = null,
            List<string> categories = null)
        {
            try
            {
                // Check internet connectivity
                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    return await SearchOfflineAsync(latitude, longitude, radiusInMiles);
                }

                // Build query parameters
                var queryParams = new Dictionary<string, string>
                {
                    ["lat"] = latitude.ToString(),
                    ["lon"] = longitude.ToString(),
                    ["radius"] = radiusInMiles.ToString()
                };

                if (!string.IsNullOrEmpty(searchQuery))
                    queryParams["q"] = searchQuery;

                if (categories?.Any() == true)
                    queryParams["categories"] = string.Join(",", categories);

                // Create the query string
                var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

                // Make API request
                var response = await _httpClient.GetAsync($"/api/foodtrucks/search?{queryString}");
                if (response.IsSuccessStatusCode)
                {
                    var trucks = await response.Content.ReadFromJsonAsync<List<FoodTruck>>();

                    // Update local cache
                    await UpdateLocalCacheAsync(trucks);

                    // Mark favorites
                    await MarkFavoritesAsync(trucks);

                    return trucks;
                }

                return new List<FoodTruck>();
            }
            catch (Exception ex)
            {
                // Log error
                Debug.WriteLine($"Error searching food trucks: {ex}");
                return new List<FoodTruck>();
            }
        }

        private async Task<List<FoodTruck>> SearchOfflineAsync(
            double latitude,
            double longitude,
            double radiusInMiles)
        {
            // Get cached food trucks from local database
            var trucks = await _dbContext.FoodTrucks
                .Where(ft =>
                    CalculateDistance(
                        latitude,
                        longitude,
                        ft.Location.Latitude,
                        ft.Location.Longitude) <= radiusInMiles)
                .ToListAsync();

            return trucks;
        }

        private double CalculateDistance(
            double lat1,
            double lon1,
            double lat2,
            double lon2)
        {
            // Haversine formula for calculating distance between two points
            var R = 3959; // Earth's radius in miles
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRad(double degrees) => degrees * Math.PI / 180;

        public async Task<bool> AddToFavoritesAsync(string foodTruckId)
        {
            try
            {
                // Add to server
                var response = await _httpClient.PostAsync(
                    $"/api/foodtrucks/{foodTruckId}/favorite", null);

                if (response.IsSuccessStatusCode)
                {
                    // Update local database
                    var truck = await _dbContext.FoodTrucks
                        .FirstOrDefaultAsync(ft => ft.Id == foodTruckId);

                    if (truck != null)
                    {
                        truck.IsFavorite = true;
                        await _dbContext.SaveChangesAsync();
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding to favorites: {ex}");
                return false;
            }
        }

        public async Task<List<FoodTruck>> GetFavoritesAsync()
        {
            try
            {
                if (_connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    // Get from server
                    var response = await _httpClient.GetAsync("/api/foodtrucks/favorites");
                    if (response.IsSuccessStatusCode)
                    {
                        var trucks = await response.Content
                            .ReadFromJsonAsync<List<FoodTruck>>();

                        // Update local cache
                        await UpdateLocalCacheAsync(trucks);
                        return trucks;
                    }
                }

                // Fall back to local cache
                return await _dbContext.FoodTrucks
                    .Where(ft => ft.IsFavorite)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting favorites: {ex}");
                return new List<FoodTruck>();
            }
        }

        private async Task UpdateLocalCacheAsync(List<FoodTruck> trucks)
        {
            foreach (var truck in trucks)
            {
                var existing = await _dbContext.FoodTrucks
                    .FirstOrDefaultAsync(ft => ft.Id == truck.Id);

                if (existing != null)
                {
                    _dbContext.Entry(existing).CurrentValues.SetValues(truck);
                }
                else
                {
                    await _dbContext.FoodTrucks.AddAsync(truck);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsOpenNowAsync(string foodTruckId)
        {
            var schedule = await GetScheduleAsync(foodTruckId, DateTime.Now);
            if (schedule == null || schedule.IsClosed)
                return false;

            var currentTime = DateTime.Now.TimeOfDay;
            return currentTime >= schedule.OpenTime &&
                   currentTime <= schedule.CloseTime;
        }
    }
}
