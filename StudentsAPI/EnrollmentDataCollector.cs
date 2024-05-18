using Newtonsoft.Json;
using Plain.RabbitMQ;
using StudentsAPI.Data;
using StudentsAPI.Models;

namespace StudentsAPI
{
    public class EnrollmentDataCollector : IHostedService
    {
        private readonly ISubscriber subscriber;
        private readonly IServiceProvider serviceProvider;
        public EnrollmentDataCollector(IServiceProvider serviceProvider, ISubscriber subscriber)
        {
            this.subscriber = subscriber;
            this.serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            subscriber.Subscribe(ProcessMesage);
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        private bool ProcessMesage(string message, string routingKey)
        {
            var enrollment = JsonConvert.DeserializeObject<Enrollment>(message);
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<StudentsAPIContext>();
                _context.Enrollment.Add(enrollment);
                _context.SaveChanges();
            }
            return true;
        }
    }
}
