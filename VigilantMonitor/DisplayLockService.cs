using System;
using System.ComponentModel;

namespace VigilantMonitor
{
    public class DisplayLockService : IDisposable
    {
        private const NativeMethods.POWER_REQUEST_TYPE DisplayRequired = NativeMethods.POWER_REQUEST_TYPE.PowerRequestDisplayRequired;

        private readonly NativeMethods.SafePowerRequestHandle _powerRequest;

        public DisplayLockService()
        {
            var reasonContext = new NativeMethods.REASON_CONTEXT
            {
                Version = NativeMethods.REASON_CONTEXT.POWER_REQUEST_CONTEXT_VERSION,
                Flags = NativeMethods.REASON_CONTEXT.POWER_REQUEST_CONTEXT_SIMPLE_STRING,
                SimpleReasonString = "Vigilant Monitor"
            };
            _powerRequest = NativeMethods.PowerCreateRequest(reasonContext);
            if (_powerRequest.IsInvalid)
                throw new Win32Exception();
        }

        public void Dispose() => _powerRequest.Dispose();

        public void Lock()
        {
            if (!NativeMethods.PowerSetRequest(_powerRequest, DisplayRequired))
                throw new Win32Exception();
        }

        public void Unlock()
        {
            if (!NativeMethods.PowerClearRequest(_powerRequest, DisplayRequired))
                throw new Win32Exception();
        }
    }
}