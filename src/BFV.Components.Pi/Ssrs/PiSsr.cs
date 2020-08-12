using BFV.Common;
using BFV.Components.Ssrs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;

namespace BFV.Components.Pi.Ssrs {
    public class PiSsr : Ssr {

        private Dictionary<Location, int> _pinLookup = new Dictionary<Location, int> {
            { Location.HLT, 23 },
            { Location.BK, 24 }
        };

        public PiSsr(ILogger<Ssr> logger) : base(logger) {
            //TODO: Wire up SSR GPIO

        }

        private static GpioController _controller = new GpioController();
        private int _pinNumber = int.MinValue;

        protected override void Toggle(bool isOn) {
            if (_pinNumber == int.MinValue) {
                _pinNumber = _pinLookup[Location];
                _controller.OpenPin(_pinNumber, PinMode.Output);
                _controller.Write(_pinNumber, PinValue.Low);
            }

            if (_pinNumber != int.MinValue)
                _controller.Write(_pinNumber, isOn ? PinValue.Low : PinValue.High);

            base.Toggle(isOn);
        }
    }

    public enum SsrPin {
        HLT = 23, //4,
        BK = 24 //5
    }
}
