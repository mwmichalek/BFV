using Meadow;
using PubSub;
using System.Threading;

namespace BFV.Appliance {
    class Program {
        static IApp app;
        public static void Main(string[] args) {
            if (args.Length > 0 && args[0] == "--exitOnDebug") return;

            // instantiate and run new meadow app
            app = new ApplianceApp();

            




            Thread.Sleep(Timeout.Infinite);
        }
    }
}
