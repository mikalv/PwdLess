﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PwdLess.Auth.Services
{
    /// <summary>
    /// Handles running the HTTP Actions defined in configuration.
    /// </summary>
    public interface IActionService
    {
        Task<string> BeforeSendingNonce(string identifier);
        Task<string> BeforeSendingToken(string token);
    }

    public class ActionService : IActionService
    {
        private IConfigurationRoot _config;

        public ActionService(IConfigurationRoot config)
        {
            _config = config;
        }

        public async Task<string> BeforeSendingNonce(string identifier)
        {
            string uri = _config["PwdLess:Actions:BeforeSendingNonce"];
            HttpContent content = new StringContent($"{{\"identifier\":\"{identifier}\"}}", 
                                            Encoding.UTF8,
                                            "application/json");
            HttpResponseMessage response;

            // send the POST request
            using (var client = new HttpClient())
                response = await client.PostAsync(uri, content);

            // throw an exception if not successful
            if (!response.IsSuccessStatusCode)
                throw new Exception("Unsuccessful status code recieved from action.");
            else
                return await response.Content.ReadAsStringAsync();

        }

        public async Task<string> BeforeSendingToken(string token)
        {
            string uri = _config["PwdLess:Actions:BeforeSendingToken"];
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response;

            // send the GET request with auth header
            using (var client = new HttpClient() { DefaultRequestHeaders = { Authorization = authHeader } })
                response = await client.GetAsync(uri);

            // throw an exception if not successful
            if (!response.IsSuccessStatusCode)
                throw new Exception("Unsuccessful status code recieved from action.");
            else
                return await response.Content.ReadAsStringAsync();
        }
    }
}
