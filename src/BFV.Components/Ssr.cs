using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
//using Windows.Devices.Gpio;

namespace BFV.Components {
    public class Ssr : StateComponent<SsrState>,
                       ILocatableComponent,
                       IComponentStateChangePublisher<SsrState>,
                       IComponentStateRequestSubscriber<SsrState> {

        private Dictionary<Location, int> _pinLookup = new Dictionary<Location, int> {
            { Location.HLT, 23 },
            { Location.BK, 24 }
        };

        private readonly ILogger _logger;

        private int _pinNumber { get; set; }

        private int _dutyCycleInMillis = 2000;

        //private GpioPin _pin;

        private int _millisOn = 0;

        private int _millisOff = 2000;

        private Action<ComponentStateChange<SsrState>> _publishSsrStateChanged;

        public Ssr(ILogger logger) {
            _logger = logger;

            //TODO: Wire up SSR GPIO
            //var gpio = GpioController.GetDefault();
            //if (gpio != null) {
            //    _pin = gpio.OpenPin(_pinNumber);
            //    _pin.SetDriveMode(GpioPinDriveMode.Output);
            //    _pin.Write(GpioPinValue.Low);
            //}


        }

        public Location Location { get; set; }

        public void ComponentStateRequestOccurred(ComponentStateRequest<SsrState> stateRequest) {
            if (stateRequest.Location == Location) {
                PriorState = CurrentState;
                CurrentState = stateRequest.UpdateState(CurrentState.Clone());

                if (PriorState.Percentage != CurrentState.Percentage) CalculateDurations();
                if (!PriorState.IsEngaged && CurrentState.IsEngaged) Start();
                if (PriorState.IsEngaged && !CurrentState.IsEngaged) Stop();

                _publishSsrStateChanged(CreateComponentStateChange());
            }
        }

        public void ComponentStateChangePublisher(Action<ComponentStateChange<SsrState>> publishStateChange) {
            _publishSsrStateChanged = publishStateChange;
        }

        private void CalculateDurations() {
            // Calculate On and Off durations

            decimal fraction = ((decimal)CurrentState.Percentage / 100.0m);
            _millisOn = (int)(fraction * (decimal)_dutyCycleInMillis);
            _millisOff = _dutyCycleInMillis - _millisOn;

            //Logger.LogInformation($"SSR: {Id} - CALC PERC {CurrentState.Percentage}, FRACTION {fraction}, MILLISON {_millisOn}, MILLISOFF {_millisOff}");
        }

        private void Start() {
            //CurrentState = CurrentState.Engage(true);
            //Task.Run(() => Run());
        }

        private void Stop() {
            //CurrentState = CurrentState.Engage(false);
            //ProposedState = null;
        }

        //private void Run() {
        //    while (CurrentState.IsEngaged) {
        //        // Something is causing a random blip.
        //        if (CurrentState.Percentage != 0 && _millisOn > 0) {
        //            On();
        //            Thread.Sleep(_millisOn);
        //        }
        //        if (CurrentState.Percentage != 100 && _millisOff > 0) {
        //            Off();
        //            Thread.Sleep(_millisOff);
        //        }

        //        if (ProposedState != null) {
        //            PriorState = CurrentState;
        //            CurrentState = ProposedState;
        //            ProposedState = null;
        //            CalculateDurations();

        //            //SendNotification();
        //        }
        //    }
        //}

        //private void On() {
        //    if (!CurrentState.IsFiring) {
        //        //Logger.LogInformation($"SSR: {Id} - ON {_millisOn}");

        //        //_pin?.Write(GpioPinValue.High);

        //        PriorState = CurrentState;
        //        CurrentState = CurrentState.Fire(true);
        //        CurrentState.IsFiring = true;

        //        //SendNotification();
        //    }
        //}

        //private void Off() {
        //    if (CurrentState.IsFiring) {
        //        //Logger.LogInformation($"SSR: {Id} - OFF {_millisOff}");

        //        //_pin?.Write(GpioPinValue.Low);

        //        PriorState = CurrentState;
        //        CurrentState = CurrentState.Fire(false);

        //        //SendNotification();
        //    }
        //}

        //private void SendNotification() {
        //    _eventHandler.ComponentStateChangeFiring<SsrState>(new ComponentStateChange<SsrState> {
        //        CurrentState = CurrentState.Clone(),
        //        PriorState = PriorState.Clone()
        //    });
        //}
    }

    public enum SsrPin {
        HLT = 23, //4,
        BK = 24 //5
    }

}












