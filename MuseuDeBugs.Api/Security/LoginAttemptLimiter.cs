namespace MuseuDeBugs.Api.Security
{
    public class LoginAttemptLimiter
    {
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(5);
        private readonly object _syncRoot = new();

        private int _failedAttempts;
        private DateTime? _blockedUntil;

        public bool IsBlocked()
        {
            lock (_syncRoot)
            {
                return _blockedUntil.HasValue && DateTime.UtcNow < _blockedUntil.Value;
            }
        }

        public void RegisterFailure()
        {
            lock (_syncRoot)
            {
                _failedAttempts++;

                if (_failedAttempts >= MaxFailedAttempts)
                {
                    _blockedUntil = DateTime.UtcNow.Add(BlockDuration);
                    _failedAttempts = 0;
                }
            }
        }

        public void RegisterSuccess()
        {
            lock (_syncRoot)
            {
                _failedAttempts = 0;
                _blockedUntil = null;
            }
        }
    }
}
