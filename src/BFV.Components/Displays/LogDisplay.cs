using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.Displays {
    public class LogDisplay : Display {

        public LogDisplay(ILogger logger) : base(logger) {

        }

    }
}
