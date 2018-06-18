﻿using Centare.Extensions;
using IEvangelist.PhotoBooth.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace IEvangelist.PhotoBooth.Services
{
    public class TextMessagingService : ITextMessagingService
    {
        private readonly TwilioOptions _options;
        private readonly ILogger<TextMessagingService> _logger;

        public TextMessagingService(
            IOptions<TwilioOptions> twilioOptions,
            ILogger<TextMessagingService> logger)
        {
            _options = twilioOptions?.Value ?? throw new ArgumentNullException(nameof(twilioOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

           var accountId = Environment.GetEnvironmentVariable(_options.AccountIdKey) 
                ?? throw new ApplicationException($"Misconfigured environment variable. Missing {_options.AccountIdKey} value.");

            var authToken = Environment.GetEnvironmentVariable(_options.AuthTokenKey)
                ?? throw new ApplicationException($"Misconfigured environment variable. Missing {_options.AuthTokenKey} value.");

            TwilioClient.Init(accountId, authToken);
        }

        public async Task SendTextAsync(string toPhoneNumber, string body)
        {
            try
            {
                var message =
                    await MessageResource.CreateAsync(
                        to: toPhoneNumber,
                        from: _options.FromPhoneNumber,
                        body: body);

                _logger.LogInformation($"Texted {toPhoneNumber}: {body}.");
            }
            catch (Exception ex)
            {
                ex.TryLogException(_logger);
            }            
        }
    }
}