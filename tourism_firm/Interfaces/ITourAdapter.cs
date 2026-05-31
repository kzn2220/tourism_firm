using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tourism_firm.объекты;

namespace tourism_firm  
{
    public interface ITourAdapter
    {
        List<Tour> GetAllTours();
        List<Tour> GetAllToursForAdmin();
        List<Tour> SearchTours(string country, decimal? maxPrice, int? minSeats);
        Tour GetTourById(int tourId);
        bool AddTour(Tour tour);
        bool DeleteTour(int tourId);
        bool UpdateTour(Tour tour);
        bool DecreaseSeats(int tourId);
        bool IncreaseSeats(int tourId);
        bool UpdateTourSeats(int tourId, int newSeats);
    }
}
