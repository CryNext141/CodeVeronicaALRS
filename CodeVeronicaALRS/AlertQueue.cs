using ALRS.Models;
using CodeVeronicaALRS.Middleware;
using System.Threading.Channels;

namespace CodeVeronicaALRS
{
    public class AlertQueue : IAlertQueue
    {
        private readonly Channel<Alert> _channel;

        public AlertQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _channel = Channel.CreateBounded<Alert>(options);
        }

        public void EnqueueAlert(Alert alert)
        {
            if (!_channel.Writer.TryWrite(alert))
            {
            }
        }

        public async Task<Alert> DequeueAlertAsync(CancellationToken cancellationToken)
        {
            var alert = await _channel.Reader.ReadAsync(cancellationToken);
            return alert;
        }
    }
}
