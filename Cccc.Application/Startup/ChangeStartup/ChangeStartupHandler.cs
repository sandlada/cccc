using System;
using System.IO;
using System.Security;

namespace Cccc.Application
{
    public sealed class ChangeStartupHandler
    {
        private readonly StartupRegistration startup;

        public ChangeStartupHandler(StartupRegistration startup)
        {
            this.startup = startup;
        }

        public ChangeStartupResult Handle(bool enabled)
        {
            try
            {
                return new() { State = startup.SetEnabled(enabled), Success = true };
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or SecurityException or ArgumentException)
            {
                return Failure(error);
            }
        }

        private static ChangeStartupResult Failure(Exception error)
        {
            return new() { Success = false, Error = error.GetType().Name };
        }
    }
}
