﻿using System;
using System.Net.Http;
using BonVoyage.Clients;
using BonVoyage.Infrastructure;

namespace BonVoyage
{
    public class FoursquareUserlessContext : IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <seealso href="https://developer.foursquare.com/overview/auth#userless" />
        public FoursquareUserlessContext(HttpMessageHandler handler, UserlessAccessSettings settings)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var userlessHandler = HttpClientFactory.CreatePipeline(handler, new DelegatingHandler[]
            {
                new QueryAppenderHandler("client_id", settings.ClientId),
                new QueryAppenderHandler("client_secret", settings.ClientSecret)
            });

            _httpClient = new HttpClient(userlessHandler, false)
            {
                BaseAddress = new Uri(Constants.FoursquareApiBaseUrl)
            };

            Categories = new CategoriesClient(_httpClient);
            Venues = new VenuesClient(_httpClient);
            Photos = new PhotosClient(_httpClient);
        }

        public CategoriesClient Categories { get; }
        public VenuesClient Venues { get; }
        public PhotosClient Photos { get; }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}