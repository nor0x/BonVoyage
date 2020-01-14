﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BonVoyage.Models;
using Newtonsoft.Json.Linq;

namespace BonVoyage.Clients
{
    public class VenuesClient : BaseClient
    {
        public VenuesClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <param name="placeName">
        /// The name of a place in the world (i.e. San Fransisco, CA). Used to pass as the value for 'near' parameter.
        /// </param>
        /// <param name="categoryId">
        /// The id of the category to limit results to. If specifying a top-level category, all sub-categories will also match the query.
        /// </param>
        /// <seealso href="https://developer.foursquare.com/docs/venues/search" />
        public Task<IReadOnlyCollection<CompactVenue>> Search(string placeName, string categoryId)
        {
            return Search(placeName, categoryId, 50);
        }

        /// <param name="placeName">
        /// The name of a place in the world (i.e. San Fransisco, CA). Used to pass as the value for 'near' parameter.
        /// </param>
        /// <param name="categoryId">
        /// The id of the category to limit results to. If specifying a top-level category, all sub-categories will also match the query.
        /// </param>
        /// <param name="limit">The number of search results. Min: 1, Max: 50</param>
        /// <seealso href="https://developer.foursquare.com/docs/venues/search" />
        public async Task<IReadOnlyCollection<CompactVenue>> Search(string placeName, string categoryId, int limit = 50, bool explore = false)
        {
            if (placeName == null) throw new ArgumentNullException(nameof(placeName));
            if (categoryId == null) throw new ArgumentNullException(nameof(categoryId));
            if (limit < 1) throw new ArgumentOutOfRangeException(nameof(limit), limit, "Cannot be lower than 1");
            if (limit > 50) throw new ArgumentOutOfRangeException(nameof(limit), limit, "Cannot be greater than 50");
            string request = $"v2/venues/search?near={placeName}&categoryId={categoryId}&limit={limit.ToString(CultureInfo.InvariantCulture)}";
            if(explore)
            {
                request = request.Replace("v2/venues/search", "v2/venues/explore");
            }
            using (var response = await HttpClient.GetAsync($"v2/venues/search?near={placeName}&categoryId={categoryId}&limit={limit.ToString(CultureInfo.InvariantCulture)}").ConfigureAwait(false))
            {
                var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jObject = DeserializeObject<JObject>(resultAsString);
                var venues = DeserializeObject<IEnumerable<CompactVenue>>(jObject["response"]["venues"].ToString());

                return new ReadOnlyCollection<CompactVenue>(venues.ToList());
            }
        }
        /// <param name="location">
        /// The location coordinates (latitude and longitude) in the world (i.e. 37.77,-122.41). Used to pass as the value for 'll' parameter.
        /// </param>
        /// <param name="categoryId">
        /// The id of the category to limit results to. If specifying a top-level category, all sub-categories will also match the query.
        /// </param>
        /// <param name="limit">The number of search results. Min: 1, Max: 50</param>
        /// <seealso href="https://developer.foursquare.com/docs/venues/search" />
        public async Task<IReadOnlyCollection<CompactVenue>> Search(Location location, string categoryId, int radius = 0, int limit = 50, bool explore = false)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));
            if (categoryId == null) throw new ArgumentNullException(nameof(categoryId));
            if (limit < 1) throw new ArgumentOutOfRangeException(nameof(limit), limit, "Cannot be lower than 1");
            if (limit > 50) throw new ArgumentOutOfRangeException(nameof(limit), limit, "Cannot be greater than 50");

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            string request = $"v2/venues/search?ll={location.Lat.ToString(nfi)},{location.Lng.ToString(nfi)}&categoryId={categoryId}&limit={limit.ToString(CultureInfo.InvariantCulture)}";
            if(radius != 0)
            {
                request += $"&radius={radius}";
            }
            if (explore)
            {
                request = request.Replace("v2/venues/search", "v2/venues/explore");
            }
            using (var response = await HttpClient.GetAsync(request).ConfigureAwait(false))
            {
                var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jObject = DeserializeObject<JObject>(resultAsString);
                var venues = DeserializeObject<IEnumerable<CompactVenue>>(jObject["response"]["venues"].ToString());

                return new ReadOnlyCollection<CompactVenue>(venues.ToList());
            }
        }
    }
}