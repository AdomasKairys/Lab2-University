using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plain.RabbitMQ
{
    public interface ISubscriber : IDisposable
    {
        void Subscribe(Func<string, string, bool> callback);
        void SubscribeAsync(Func<string, string, Task<bool>> callback);
    }
}