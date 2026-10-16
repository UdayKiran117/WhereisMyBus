using Android.Util;
using Java.Util.Logging;
using Newtonsoft.Json;
using Org.Apache.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Text;
using System.Xml;

namespace WhereIsMyBus.Mobile.Services
{
    public class GenericService
    {
        private static readonly HttpClient Client = CreateClient();

        private static HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();

#if DEBUG
            // Local development only. Never accept arbitrary server certificates.
            handler.ServerCertificateCustomValidationCallback =
                (request, certificate, chain, errors) =>
                {
                    if (errors == SslPolicyErrors.None)
                        return true;

                    var uri = request.RequestUri;

                    return certificate is not null &&
                           uri is not null &&
                           uri.Scheme == "https" &&
                           uri.Port == 7081 &&
                           (uri.Host == "10.0.2.2" ||
                            uri.Host == "localhost") &&
                           certificate.GetNameInfo(
                               System.Security.Cryptography.X509Certificates
                                   .X509NameType.SimpleName,
                               false) == "localhost";
                };
#endif

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
        public async Task<List<T>> GetListObjectResultWithContent<T>(string url, StringContent content)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Uri completeurl = new Uri(url);
                    client.Timeout = TimeSpan.FromSeconds(5000);
                    HttpResponseMessage response = await client.PostAsync(completeurl, content);
                    //response.Wait();
                    //var webresult = response.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        
                        //log.Debug("The result after succedded is " + webresult.Content.ReadAsStringAsync().Result);
                        string responses = await response.Content.ReadAsStringAsync();
                        if (responses == "false")

                        {

                            return new List<T>();

                        }
                        return JsonConvert.DeserializeObject<List<T>>(responses);
                    }
                    else
                    {
                        
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                
                return null;
            }
        }
        public async Task<List<T>> GetListObjectResult<T>(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // client.Timeout = TimeSpan.FromSeconds(300);

                Uri completeurl = new Uri(url);

                //client.Timeout = TimeSpan.FromSeconds(1000); // Adjust the timeout duration as needed

                HttpResponseMessage response = await client.GetAsync(completeurl);
                //response.Wait();
                //var webresult = response.Result;
                if (response.IsSuccessStatusCode)
                {
                    string responses = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<T>>(responses);
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<IList<T>> GetIListObjectResult<T>(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Uri completeurl = new Uri(url);

                HttpResponseMessage response = await client.GetAsync(completeurl);
                //response.Wait();
                //var webresult = response.Result;
                if (response.IsSuccessStatusCode)
                {
                    string responses = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IList<T>>(responses);
                }
                else
                {
                    return null;
                }
            }
        }
        public async Task<T> GetScalarObjectResult<T>(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(300);
                    Uri completeurl = new Uri(url);
                    client.Timeout = TimeSpan.FromSeconds(5000);
                    HttpResponseMessage response = await client.GetAsync(completeurl);
                    //response.Wait();
                    //var webresult = response.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responses = await response.Content.ReadAsStringAsync();

                        var jsonresult = JsonConvert.DeserializeObject<T>(responses);

                        return jsonresult;
                    }
                    else
                    {
                        return default(T);
                    }
                }
                catch (Exception ex)
                {
                    
                    return default(T);
                }
            }
        }
        public async Task<T> GetScalarObjectResultWithContent<T>(string url, StringContent content, CancellationToken ct = default)
        {
            using (var client = Client)
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.Timeout = TimeSpan.FromSeconds(300);

                    Uri completeurl = new Uri(url);

                    client.Timeout = TimeSpan.FromSeconds(2000);
                    HttpResponseMessage response = await client.PostAsync(completeurl, content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string responses = await response.Content.ReadAsStringAsync();

                        var jsonresult = JsonConvert.DeserializeObject<T>(responses);

                        return jsonresult;
                    }
                    else
                    {
                        return default(T);
                    }
                }
                catch (Exception ex)
                {
                    return default(T);
                }
            }
        }

        public async Task GetScalarObjectResultWithContent(string url, StringContent content)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.Timeout = TimeSpan.FromSeconds(300);

                    Uri completeurl = new Uri(url);

                    client.Timeout = TimeSpan.FromSeconds(2000);

                    HttpResponseMessage response = await client.PostAsync(completeurl, content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string responses = await response.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        public async Task ExecuteScalar(string url, StringContent content)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Uri completeurl = new Uri(url);

                    HttpResponseMessage response = await client.PostAsync(completeurl, content);
                    //response.Wait();
                    //var webresult = response.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responses = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        //return default(T);    
                    }
                }
                catch (Exception ex)
                {
                    // return default(T);
                }
            }
        }
        public async Task ExecuteScalar(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Uri completeurl = new Uri(url);

                    HttpResponseMessage response = await client.GetAsync(completeurl);
                    //response.Wait();
                    //var webresult = response.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responses = await response.Content.ReadAsStringAsync();

                        // var jsonresult = JsonConvert.DeserializeObject<T>(responses);

                        //return jsonresult;
                    }
                    else
                    {
                        //return default(T);
                    }
                }
                catch (Exception ex)
                {
                    //return default(T);
                }
            }
        }
        public async Task GetScalarObjectResult(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear(); client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Uri completeurl = new Uri(url);

                    HttpResponseMessage response = await client.GetAsync(completeurl);
                    //response.Wait();
                    //var webresult = response.Result;

                    string responses = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            DataTable dt = new DataTable();
            PropertyInfo[] columns = null;

            if (Linqlist == null) return dt;

            foreach (T Record in Linqlist)
            {

                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type colType = GetProperty.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                               == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
                    }
                }

                DataRow dr = dt.NewRow();

                foreach (PropertyInfo pinfo in columns)
                {
                    dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                           (Record, null);
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        //public byte[] GetPdfBytesResultWithContent(string url, StringContent content)
        //{
        //    using (var client = new System.Net.Http.HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.Timeout = TimeSpan.FromSeconds(5000);
        //        Uri completeurl = new Uri(url);

        //        Task<HttpResponseMessage> response = client.PostAsync(completeurl, content);
        //        response.Wait();
        //        var webresult = response.Result;
        //        var w = response.ToString();
        //        if (webresult.IsSuccessStatusCode)
        //        {
        //            string responses = webresult.Content.ReadAsStringAsync().Result;
        //            byte[] rs = webresult.Content.ReadAsByteArrayAsync().Result;
        //            byte[] b = (byte[])JsonConvert.DeserializeObject<byte[]>(responses);
        //            string j = responses.ToString();
        //            return b;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}
        public byte[] GetPdfBytesResultWithContent(string url, StringContent content)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(5000);
                    Uri completeurl = new Uri(url);

                    Task<HttpResponseMessage> response = client.PostAsync(completeurl, content);
                    response.Wait();
                    var webresult = response.Result;
                    var w = response.ToString();

                    if (webresult.IsSuccessStatusCode)
                    {
                        string responses = webresult.Content.ReadAsStringAsync().Result;
                        byte[] rs = webresult.Content.ReadAsByteArrayAsync().Result;
                        byte[] b = (byte[])JsonConvert.DeserializeObject<byte[]>(responses);
                        string j = responses.ToString();
                        return b;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public byte[] GetReportResult(string url, StringContent content)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    Uri completeurl = new Uri(url);

                    Task<HttpResponseMessage> response = client.PostAsync(completeurl, content);
                    response.Wait();
                    var webresult = response.Result;
                    var w = response.ToString();
                    if (webresult.IsSuccessStatusCode)
                    {
                        string responses = webresult.Content.ReadAsStringAsync().Result;
                        var reportresult = JsonConvert.DeserializeObject<byte[]>(responses);
                        return reportresult;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    
                    return null;
                }
            }
        }

        public byte[] GetLogFile(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    Uri completeurl = new Uri(url);

                    Task<HttpResponseMessage> response = client.GetAsync(completeurl);
                    response.Wait();
                    var webresult = response.Result;
                    var w = response.ToString();
                    if (webresult.IsSuccessStatusCode)
                    {
                        string responses = webresult.Content.ReadAsStringAsync().Result;
                        var reportresult = JsonConvert.DeserializeObject<byte[]>(responses);
                        return reportresult;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    
                    return null;
                }
            }
        }

        public async Task<T> PostFormData<T>(string url, MultipartFormDataContent formData)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(10);

                    var response = await client.PostAsync(url, formData);

                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(json))
                        throw new Exception("API returned empty response.");

                    var result = JsonConvert.DeserializeObject<T>(json);

                    if (result == null)
                        throw new Exception("Failed to deserialize API response.");

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Single catch – wraps all errors
                throw new Exception("PostFormData failed.", ex);
            }
        }

        public async Task<T> DeleteObjectResult<T>(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(300);
                    Uri completeurl = new Uri(url);

                    // Use DeleteAsync instead of GetAsync
                    Task<HttpResponseMessage> response = client.DeleteAsync(completeurl);
                    response.Wait();
                    var webresult = response.Result;

                    if (webresult.IsSuccessStatusCode)
                    {
                        string responses = webresult.Content.ReadAsStringAsync().Result;
                        var jsonresult = JsonConvert.DeserializeObject<T>(responses);
                        return jsonresult;
                    }
                    else
                    {
                        return default(T);
                    }
                }
                catch (Exception ex)
                {
                    return default(T);
                }
            }
        }


        public byte[] GetFileBytes(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));

                    Uri completeurl = new Uri(url);

                    Task<HttpResponseMessage> response = client.GetAsync(completeurl);
                    response.Wait();
                    var webresult = response.Result;
                    if (webresult.IsSuccessStatusCode)
                    {
                        return webresult.Content.ReadAsByteArrayAsync().Result;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    
                    return null;
                }
            }
        }

        public async Task<T> PostObjectResult<T>(string url, HttpContent content)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(300);

                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(jsonString);
                    }
                }
                catch (Exception ex)
                {
                    // Log exception
                }

                return default(T);
            }
        }
        public byte[] GetGLPdfBytesResultWithContent(string url, StringContent content)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("XApiKey", "Intell@Banjara#2");

                client.Timeout = TimeSpan.FromMinutes(10);

                var response = client.PostAsync(url, content).Result;

                if (!response.IsSuccessStatusCode)
                    return null;

                return response.Content.ReadAsByteArrayAsync().Result;
            }
        }
    }
}
