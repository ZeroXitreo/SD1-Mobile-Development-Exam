using Exam.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Exam.Services
{
    public class ApiConnectionService<TEntity> where TEntity : IIdentification
    {
        public static readonly Uri Uri = new Uri("https://fast-castle-50377.herokuapp.com/");
        private static HttpClient _client;
        private static Auth _auth { get; } = new Auth()
        {
            identifier = "student",
            password = "Student1234",
        };

        public static Task<HttpClient> Client => GetClientAsync();
        public readonly string path;

        public ApiConnectionService(string relativePath)
        {
            path = relativePath;
        }

        private static async Task<HttpClient> GetClientAsync()
        {
            if (_client == null)
            {
                await RefreshClientToken();
            }
            return _client;
        }

        private static async Task RefreshClientToken()
        {
            if (_client == null)
            {
                _client = new HttpClient();
            }
            StringContent content = new StringContent(JsonConvert.SerializeObject(_auth), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(Uri + "auth/local", content);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                JsonConvert.PopulateObject(responseContent, _auth);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _auth.jwt);
                //_client.DefaultRequestHeaders.Add("Authorization", auth.jwt);
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<TReturn> LoginWrapper<TReturn>(Func<Task<HttpResponseMessage>> callback)
        {
            HttpResponseMessage response = await callback();
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TReturn>(responseContent);
            }
            else
            {
                await RefreshClientToken();
                response = await callback();
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TReturn>(responseContent);
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<List<TEntity>> Get()
        {
            return await LoginWrapper<List<TEntity>>(async () =>
            {
                return await (await Client).GetAsync(new Uri(Uri, path));
            });
        }

        public async Task<TEntity> Get(string id)
        {
            return await LoginWrapper<TEntity>(async () =>
            {
                return await (await Client).GetAsync(new Uri(Uri, path + "/" + id));
            });
        }

        public async Task<List<TEntity>> Get(List<KeyValuePair<string, string>> parameters)
        {
            List<string> fixedParameters = new List<string>();

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                fixedParameters.Add(HttpUtility.UrlEncode(parameter.Key) + "=" + HttpUtility.UrlEncode(parameter.Value));
            }

            string completeParameters = string.Join("&", fixedParameters);

            return await LoginWrapper<List<TEntity>>(async () =>
            {
                return await (await Client).GetAsync(new Uri(new Uri(Uri, path), "?" + completeParameters));
            });
        }

        public async Task<TEntity> Create(TEntity entity)
        {
            return await LoginWrapper<TEntity>(async () =>
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(entity, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }), Encoding.UTF8, "application/json");
                return await (await Client).PostAsync(new Uri(Uri, path), content);
            });
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            return await LoginWrapper<TEntity>(async () =>
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(entity, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }), Encoding.UTF8, "application/json");
                return await (await Client).PutAsync(new Uri(Uri, path + "/" + entity.id), content);
            });
        }

        public async Task<TEntity> Delete(TEntity entity)
        {
            return await LoginWrapper<TEntity>(async () =>
            {
                return await (await Client).DeleteAsync(new Uri(Uri, path + "/" + entity.id));
            });
        }
    }
}
