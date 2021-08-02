using iRacingSimulator;
using System.Collections.Generic;
using System.Linq;
using static iRacingSdkWrapper.SdkWrapper;

namespace iRacingReplayControl
{
    class Driver
    {
        private static Dictionary<int, Driver> _drivers;
        private readonly iRacingSimulator.Drivers.Driver _driver;

        static Driver()
        {
            Sim.Instance.SessionInfoUpdated += OnSessionInfoUpdate;
            UpdateDriversDictionary();
        }

        public static Driver FromCarIdx(int carIdx)
        {
            return _drivers[carIdx]; ;
        }

        static private void UpdateDriversDictionary()
        {
            _drivers = Sim.Instance.Drivers.ToDictionary(driver => driver.Id, driver => new Driver(driver));
        }

        static private void OnSessionInfoUpdate(object sender, SessionInfoUpdatedEventArgs e)
        {
            UpdateDriversDictionary();
        }

        public Driver(iRacingSimulator.Drivers.Driver driver)
        {
            _driver = driver;
        }

        public int CarNumber => int.Parse(_driver.CarNumber);
        public string ShortName => _driver.ShortName;
    }
}