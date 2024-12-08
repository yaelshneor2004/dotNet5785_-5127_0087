

using DalApi;
using 
using System.Diagnostics;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get;
    public static double CalculateDistance(double? lat1, double? lon1, double lat2, double lon2)
    {
        if (lat1 == null || lon1 == null)
        {
            throw new BO.BlNullPropertyException("Latitude and Longitude cannot be null.");
        }

        const double R = 6371e3; // Radius of the Earth in meters
        var phi1 = lat1.Value * Math.PI / 180; // φ, λ in radians
        var phi2 = lat2 * Math.PI / 180;
        var deltaPhi = (lat2 - lat1.Value) * Math.PI / 180;
        var deltaLambda = (lon2 - lon1.Value) * Math.PI / 180;

        var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distance = R * c; // in meters

        return distance;
    }
    public static BO.MyCallStatusByVolunteer DetermineCallStatus(DateTime? maxFinishTime)
    {
        // Use the updated system clock from ClockManager
        var remainingTime = maxFinishTime - ClockManager.Now;

        if (remainingTime <= s_dal.Config.RiskRange)
        {
            return BO.MyCallStatusByVolunteer.AtRisk;
        }

        return BO.MyCallStatusByVolunteer.InProgress; 
    }


}


