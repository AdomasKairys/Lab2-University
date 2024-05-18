using Microsoft.Extensions.Diagnostics.HealthChecks;
using StudentsAPI.Models;
using Newtonsoft.Json;

namespace StudentsAPI.Health
{
    public class GetHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new()) {
            
            try
            {
                List<Student> studentList;
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("http://host.docker.internal:5082/api/Students"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        studentList = JsonConvert.DeserializeObject<List<Student>>(apiResponse);
                    }
                }
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                string exeption = ex.Message + " Inner exeption: " + ex.InnerException;
                return HealthCheckResult.Unhealthy(exeption);
            }
        }
    }
}
