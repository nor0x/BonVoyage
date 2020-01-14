using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BonVoyage.Clients;
using BonVoyage.Models;

namespace BonVoyage
{
    public static class VenueCategoryExtensions
    {
        public static Task<IReadOnlyCollection<CompactVenue>> SearchVenues(this VenueCategory category, 
            string placeName)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            
            var client = new VenuesClient(category.HttpClient);
            return client.Search(placeName, category.Id);
        }

        public static Task<IReadOnlyCollection<CompactVenue>> SearchVenues(this VenueCategory category,
            string placeName, int limit, bool explore = false)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            var client = new VenuesClient(category.HttpClient);
            return client.Search(placeName, category.Id, limit, explore);
        }

        public static Task<IReadOnlyCollection<CompactVenue>> SearchVenues(this VenueCategory category,
            Location location, int radius, int limit = 50, bool explore = false)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            var client = new VenuesClient(category.HttpClient);
            return client.Search(location, category.Id, radius, limit, explore);
        }
    }
}