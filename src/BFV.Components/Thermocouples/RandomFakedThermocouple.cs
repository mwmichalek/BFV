﻿using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.Thermocouples {
    public class RandomFakedThermocouple : Thermocouple {

        private Random random = new Random();

        public RandomFakedThermocouple(ILogger logger) : base(logger) {
            CurrentState = new ThermocoupleState { Temperature = 70 };
        }

        public override void Refresh() {
            int integer = random.Next(-100, 100);
            decimal fraction = integer / 100;

            
        }


    }
}
