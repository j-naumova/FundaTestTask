using FundaTestTask.Application;

namespace FundaTestTask.ConsoleClient.ConsoleSpecific
{
    public class WorkerOptions : IWorkerOptions
    {
        public bool ResetDatabase => ConfigHelper.GetBoolProperty(nameof(ResetDatabase));                     
    }
}
