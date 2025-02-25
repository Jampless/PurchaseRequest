using PRSystem.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using PRSystem.Model;
using System.Data.Odbc;

namespace PRSystem.Services
{
    public class SAPService
    {
        public List<OGLAccounts>? gLAccounts { get; set; }
        private const string Url = "https://hanaservernbfi:50000/b1s/v1/";
      
        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            string jsonRequestBody = JsonConvert.SerializeObject(loginRequest);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url + "Login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonRequestBody);
            }

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = await streamReader.ReadToEndAsync();
                    var responseInstance = JsonConvert.DeserializeObject<LoginResponse>(result);
                    return responseInstance;
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, etc.)
                throw new ApplicationException("An error occurred while logging into SAP.", ex);
            }
        }

        public async Task<bool> LogoutAsync()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url + "Logout");
            httpWebRequest.Method = "POST"; // POST request for logout
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                // If the status code is OK (200), then the logout was successful
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    // Optionally clear session or authentication tokens if stored
                    // Example: ClearCookies();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, etc.)
                throw new ApplicationException("An error occurred while logging out of SAP.", ex);
            }
        }

        public async Task<string> PostPurchaseRequestAsync(PurchaseRequest purchaseRequest, string sessionId)
        {
            var jsonRequestBody = JsonConvert.SerializeObject(purchaseRequest);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url + "PurchaseRequests");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={sessionId}");
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonRequestBody);
            }

            // Console.WriteLine($"JSON Request Body: {jsonRequestBody}");

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = await streamReader.ReadToEndAsync();
                    // Parse the response to extract DocNum
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(result);
                    string docNum = responseObject?.DocNum;

                    //Console.WriteLine($"Response: {result}");
                    //Console.WriteLine($"Extracted DocNum: {docNum}");
                    return docNum;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        var errorResponse = await streamReader.ReadToEndAsync();
                        Console.WriteLine($"Response Error: {errorResponse}");
                        return errorResponse;
                    }
                }
                throw new ApplicationException("An error occurred while posting the purchase request.", ex);
            }

        }

        public async Task<List<Items>> GetItemMasterDataAsync(string sessionId)
        {
            List<Items> items = new List<Items>();
            string url = Url + "Items?$filter=startswith(ItemCode, '199') or startswith(ItemCode, '299')&$select=ItemName, ItemCode, U_ID007, U_ID011, InventoryUOM";
            int pageCount = 0;

            while (!string.IsNullOrEmpty(url))
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Cookie", $"B1SESSION={sessionId}");
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                httpWebRequest.ServicePoint.Expect100Continue = false;

                try
                {
                    using (var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync())
                    {
                        if (httpResponse.StatusCode != HttpStatusCode.OK)
                        {
                            throw new ApplicationException($"Error fetching Stock Transfer: {httpResponse.StatusDescription}");
                        }

                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = await streamReader.ReadToEndAsync();
                            //Console.WriteLine("Response received: " + result);
                            var response = JsonConvert.DeserializeObject<ItemsResponse>(result);
                            items.AddRange(response.Value);

                            if (!string.IsNullOrEmpty(response.OdataNextLink))
                            {
                                if (Uri.IsWellFormedUriString(response.OdataNextLink, UriKind.Absolute))
                                {
                                    url = response.OdataNextLink;
                                }
                                else
                                {
                                    url = new Uri(new Uri(url), response.OdataNextLink).ToString();
                                }
                            }
                            else
                            {
                                url = null;
                            }

                            pageCount++;
                            Console.WriteLine($"Fetched page {pageCount}, total stock transfer fetched: {items.Count}");
                            Console.WriteLine($"Next URL: {url}");
                        }
                    }
                }
                catch (WebException webEx)
                {
                    using (var streamReader = new StreamReader(webEx.Response.GetResponseStream()))
                    {
                        var errorResponse = await streamReader.ReadToEndAsync();
                        throw new ApplicationException($"An error occurred while fetching Stock Transfer: {errorResponse}", webEx);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while fetching Stock Transfer.", ex);
                }
            }

            Console.WriteLine($"Total stock transfer fetched: {items.Count}");
            return items;
        }

        public async Task<List<Suppliers>> GetSuppliers(string sessionId)
        {
            List<Suppliers> items = new List<Suppliers>();
            string url = Url + "BusinessPartners?$filter=startswith(CardCode, 'LTS-')&$select=CardCode,CardName";
            int pageCount = 0;

            while (!string.IsNullOrEmpty(url))
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Cookie", $"B1SESSION={sessionId}");
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                httpWebRequest.ServicePoint.Expect100Continue = false;

                try
                {
                    using (var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync())
                    {
                        if (httpResponse.StatusCode != HttpStatusCode.OK)
                        {
                            throw new ApplicationException($"Error fetching Stock Transfer: {httpResponse.StatusDescription}");
                        }

                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = await streamReader.ReadToEndAsync();
                            //Console.WriteLine("Response received: " + result);
                            var response = JsonConvert.DeserializeObject<SuppliersResponse>(result);
                            items.AddRange(response.Value);

                            if (!string.IsNullOrEmpty(response.OdataNextLink))
                            {
                                if (Uri.IsWellFormedUriString(response.OdataNextLink, UriKind.Absolute))
                                {
                                    url = response.OdataNextLink;
                                }
                                else
                                {
                                    url = new Uri(new Uri(url), response.OdataNextLink).ToString();
                                }
                            }
                            else
                            {
                                url = null;
                            }

                            pageCount++;
                            Console.WriteLine($"Fetched page {pageCount}, total stock transfer fetched: {items.Count}");
                            Console.WriteLine($"Next URL: {url}");
                        }
                    }
                }
                catch (WebException webEx)
                {
                    using (var streamReader = new StreamReader(webEx.Response.GetResponseStream()))
                    {
                        var errorResponse = await streamReader.ReadToEndAsync();
                        throw new ApplicationException($"An error occurred while fetching Stock Transfer: {errorResponse}", webEx);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while fetching Stock Transfer.", ex);
                }
            }

            Console.WriteLine($"Total stock transfer fetched: {items.Count}");
            return items;
        }

    }
}
