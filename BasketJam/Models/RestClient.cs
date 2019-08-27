using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BasketJam.Models
{
    public class RestClient
    {
        private HttpClient client;
        public const string ApiUri = "http://ec2-54-208-166-6.compute-1.amazonaws.com/";
        public const string MediaTypeJson = "application/json";
        public const string RequestMsg = "Request has not been processed";
        public static string ReasonPhrase { get; set; }
        public RestClient()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(ApiUri);
           // this.client.DefaultRequestHeaders.Accept.Clear();
           // this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeJson));
        }
        public async Task<List<U>> RunAsyncGetAll<T, U>(dynamic uri)
        {
            HttpResponseMessage response = await this.client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<U>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new ApplicationException(response.ReasonPhrase);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                throw new Exception(response.ReasonPhrase);
            }

            throw new Exception(RequestMsg);
        }

        public async Task<U> RunAsyncPost<T, U>(string uri, T entity)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(uri, entity).Result;
            ReasonPhrase = response.ReasonPhrase;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<U>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new ApplicationException(response.ReasonPhrase);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                throw new Exception(response.ReasonPhrase);
            }

            throw new Exception(RequestMsg);
        }
        public async Task<U> RunAsyncGet<T, U>(dynamic uri, dynamic data)
        {
            HttpResponseMessage response = await this.client.GetAsync(uri + "/" + data);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<U>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new ApplicationException(response.ReasonPhrase);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                throw new Exception(response.ReasonPhrase);
            }

            throw new Exception(RequestMsg);
        }
    }
}
