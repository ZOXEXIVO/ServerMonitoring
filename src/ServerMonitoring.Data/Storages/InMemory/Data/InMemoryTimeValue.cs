using System;

namespace ServerMonitoring.Data.Storages.InMemory.Data
{
    public struct InMemoryTimeValue
    {
        public InMemoryTimeValue(double val, DateTime date)
        {
            Value = val;
            Date = date;
        }

        public double Value { get; set; }
        public DateTime Date { get; set; }
    }
}
