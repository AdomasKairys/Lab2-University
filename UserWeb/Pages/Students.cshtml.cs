using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System;
using UserWeb.Models;

namespace UserWeb.Pages
{
    public class StudentsModel : PageModel
    {
        private readonly ILogger _logger;
        public List<Student>? StudentList { get; private set; }
        public string? Error { get; private set; }

        public StudentsModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }
        public async Task OnGetAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("http://host.docker.internal:5083/Students"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        StudentList = JsonConvert.DeserializeObject<List<Student>>(apiResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message + " Inner exeption: " + ex.InnerException;
            }
        }
    }
}
