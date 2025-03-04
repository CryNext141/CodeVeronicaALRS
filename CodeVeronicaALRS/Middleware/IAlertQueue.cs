using ALRS.Models;

namespace CodeVeronicaALRS.Middleware
{
    public interface IAlertQueue
    {
        void EnqueueAlert(Alert alert);
        Task<Alert> DequeueAlertAsync(CancellationToken cancellationToken);
    }
}
