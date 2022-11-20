using AutoStateTransitions.Misc;
using AutoStateTransitions.Models;
using AutoStateTransitions.Repos.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoStateTransitions.Repos
{
    public class RulesRepo : IRulesRepo, IDisposable
    {
        private IOptions<AppSettings> _appSettings;
        private IHelper _helper;
        private readonly IHttpClientFactory clientFactory;

        public RulesRepo(IOptions<AppSettings> appSettings, IHelper helper, IHttpClientFactory clientFactory)
        {
            _appSettings = appSettings;
            _helper = helper;
            this.clientFactory = clientFactory;
        }

        public async Task<RulesModel> ListRules(string wit)
        {
            string src = _appSettings.Value.SourceForRules;
            var client = clientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, src + "/rules." + wit.ToLower() + ".json");

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                RulesModel rules = JsonConvert.DeserializeObject<RulesModel>(json);
                return rules;

            }
            else
            {
                return new RulesModel();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RulesRepo()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _appSettings = null;
                _helper = null;
            }
        }
    }

}
