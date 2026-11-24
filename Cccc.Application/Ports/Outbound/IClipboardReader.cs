using System.Threading;
using System.Threading.Tasks;

namespace Cccc.Application
{
    public interface IClipboardReader
    {
        uint CurrentSequence { get; }

        Task<ClipboardContent> ReadAsync(uint expectedSequence, long deadlineMilliseconds,
            CancellationToken cancellationToken);
    }
}
