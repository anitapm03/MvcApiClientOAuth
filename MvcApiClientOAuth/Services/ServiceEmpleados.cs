using MvcApiClientOAuth.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;

namespace MvcApiClientOAuth.Services
{
    public class ServiceEmpleados
    {
        private string UrlApiEmpleados;
        private MediaTypeWithQualityHeaderValue Header;

        //obj para recuperar httpcontext y user y su claim
        private IHttpContextAccessor httpContextAccessor;

        public ServiceEmpleados(IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            this.UrlApiEmpleados =
                configuration.GetValue<string>("ApiUrls:ApiEmpleados");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetTokenAsync
            (string username, string password)
        {
            using(HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel loginModel = new LoginModel
                {
                    UserName = username, Password = password
                };
                string jsonData = JsonConvert.SerializeObject(loginModel);
                StringContent content = new StringContent
                    (jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await
                    client.PostAsync(request, content);

                if(responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient()) 
            {
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage responseMessage = await
                    client.GetAsync(request);
                if (responseMessage.IsSuccessStatusCode)
                {
                    T data = await 
                        responseMessage.Content.ReadAsAsync<T>();
                    
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        //TENDREMOS UN METODO GENERICO QUE RECIBIRA EL REQUEST 
        //Y EL TOKEN
        private async Task<T> CallApiAsync<T>
            (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }


        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            string request = "api/empleados";
            List<Empleado> empleados = await
                this.CallApiAsync<List<Empleado>>(request);
            return empleados;
        }

        public async Task<Empleado> FindEmpleadoAsync
            (int id)
        {
            string token =
                this.httpContextAccessor.HttpContext.User
                .FindFirst(x => x.Type == "TOKEN").Value;

            string request = "api/empleados/" + id;
            Empleado empleado = await
                this.CallApiAsync<Empleado>(request, token);
            return empleado;
        }

        public async Task<Empleado> GetPerfilAsync()
        {
            string token =
                this.httpContextAccessor.HttpContext.User
                .FindFirst(x => x.Type == "TOKEN").Value;

            string request = "api/empleados/perfilempleado";
            Empleado empleado = await
                this.CallApiAsync<Empleado>(request, token);
            return empleado;
        }

        public async Task<List<Empleado>> GetCompisAsync()
        {
            string token =
                this.httpContextAccessor.HttpContext.User
                .FindFirst(x => x.Type == "TOKEN").Value;

            string request = "api/empleados/getcompisempleado";

            List<Empleado> compis = await this.CallApiAsync
                <List<Empleado>>(request, token);
            return compis;
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            string request = "api/empleados/oficios";
            List<string> oficios = await
                this.CallApiAsync<List<string>>(request);
            return oficios;
        }

        private string TransformCollectionToQuery
            (List<string> collection)
        {
            string result = "";
            foreach (string elem in collection)
            {
                result += "oficio=" + elem + "&";
            }
            result = result.TrimEnd('&');
            return result;
        }

        public async Task<List<Empleado>>
            GetEmpleadosOficioAsync(List<string> oficios)
        {
            string request = "api/empleados/empleadosoficio";
            string data = this.TransformCollectionToQuery(oficios);
            List<Empleado> empleados = await
                this.CallApiAsync<List<Empleado>>(request + "?" + data);
            return empleados;
        }

        public async Task SubirSalarioAsync
            (int incremento, List<string> oficios)
        {
            string request = 
                "api/empleados/subirsalariooficio/" + incremento;

            string data = this.TransformCollectionToQuery(oficios);
            
            using(HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.PutAsync(request+"?"+data,null);

            }
        }
    }
}
