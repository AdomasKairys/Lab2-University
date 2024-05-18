using Microsoft.Extensions.Diagnostics.HealthChecks;
using CoursesAPI.Models;
using Newtonsoft.Json;

namespace CoursesAPI.Health
{
    public class GetHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new()) {
            
            try
            {
                List<Course> coursesList;
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("http://host.docker.internal:5081/api/Courses"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        coursesList = JsonConvert.DeserializeObject<List<Course>>(apiResponse);
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
