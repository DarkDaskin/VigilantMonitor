using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;

namespace VigilantMonitor
{
    public sealed class AudioMeterService : IDisposable, IMMNotificationClient
    {
        private readonly MMDeviceEnumerator _deviceEnumerator = new();
        private MMDevice _device;

        public float? CurrentVolume
        {
            get 
            {
                // Retry once
                for (var attempt = 0; ; attempt++)
                {
                    try
                    {
                        return _device.AudioMeterInformation.MasterPeakValue;
                    }
                    catch (Exception)
                    {
                        if (attempt > 0)
                            break;

                        _device.Dispose();
                        _device = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    }                    
                }

                return null;
            }
        }

        public AudioMeterService()
        {
            _deviceEnumerator.RegisterEndpointNotificationCallback(this);
            _device = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        public void Dispose()
        {
            _device.Dispose();
            _deviceEnumerator.UnregisterEndpointNotificationCallback(this);
            _deviceEnumerator.Dispose();
        }

        void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
        }

        void IMMNotificationClient.OnDeviceAdded(string pwstrDeviceId)
        {
        }

        void IMMNotificationClient.OnDeviceRemoved(string deviceId)
        {
        }

        void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (flow == DataFlow.Render && role == Role.Multimedia)
            {
                _device.Dispose();
                _device = _deviceEnumerator.GetDevice(defaultDeviceId);
            }
        }

        void IMMNotificationClient.OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
        }
    }
}