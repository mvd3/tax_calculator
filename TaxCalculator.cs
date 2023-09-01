using System;
using System.Collections.Generic;
using System.Linq;

//The focus should be on clean, simple and easy to read code 
//Everything but the public interface may be changed
namespace TaxCalculatorInterviewTests
{
    /// <summary>
    /// This is the public inteface used by our client and may not be changed
    /// </summary>
    public interface ITaxCalculator
    {
        double GetStandardTaxRate(Commodity commodity);
        void SetCustomTaxRate(Commodity commodity, double rate);
        double GetTaxRateForDateTime(Commodity commodity, DateTime date);
        double GetCurrentTaxRate(Commodity commodity);
    }

    /// <summary>
    /// Implements a tax calculator for our client.
    /// The calculator has a set of standard tax rates that are hard-coded in the class.
    /// It also allows our client to remotely set new, custom tax rates.
    /// Finally, it allows the fetching of tax rate information for a specific commodity and point in time.
    /// TODO: We know there are a few bugs in the code below, since the calculations look messed up every now and then.
    ///       There are also a number of things that have to be implemented.
    /// </summary>
    public class TaxCalculator : ITaxCalculator
    {
        private const double DEFAULT_RATE = 0.25;
        private readonly Dictionary<Commodity, double> _standardRates = new() 
        {
            {Commodity.Default, 0.25},
            {Commodity.Alcohol, 0.25},
            {Commodity.Food, 0.12},
            {Commodity.FoodServices, 0.12},
            {Commodity.Literature, 0.06},
            {Commodity.Transport, 0.06},
            {Commodity.CulturalServices, 0.06}
        };
        private Dictionary<Commodity, List<TaxRate>> _rates = new();


        public TaxCalculator()
        {
            foreach (Commodity c in Enum.GetValues(typeof(Commodity)))
                _rates[c] = new()
                {
                    new TaxRate(DateTime.MinValue, _standardRates.ContainsKey(c) ? _standardRates[c] : DEFAULT_RATE)
                };
        }

        /// <summary>
        /// Get the standard tax rate for a specific commodity.
        /// </summary>
        public double GetStandardTaxRate(Commodity commodity)
        {
            return _rates.ContainsKey(commodity) ? _rates[commodity].First().Value : DEFAULT_RATE;
        }


        /// <summary>
        /// This method allows the client to remotely set new custom tax rates.
        /// When they do, we save the commodity/rate information as well as the UTC timestamp of when it was done.
        /// NOTE: Each instance of this object supports a different set of custom rates, since we run one thread per customer.
        /// </summary>
        public void SetCustomTaxRate(Commodity commodity, double rate)
        {
            //TODO: support saving multiple custom rates for different combinations of Commodity/DateTime
            //TODO: make sure we never save duplicates, in case of e.g. clock resets, DST etc - overwrite old values if this happens
            if (rate < 0)
            {
                Console.WriteLine($"Error, tax rate can't be negative, value {rate}!");
                return;
            } else if (rate > 1)
                Console.WriteLine($"Info, tax rate is over 100%, value {rate}");

            if (!_rates.ContainsKey(commodity)) // Someone might be malicous and expand the enum with a additional value
                _rates[commodity] = new()
                    {
                        new TaxRate(DateTime.MinValue, DEFAULT_RATE)
                    };

            if (_rates[commodity].First().Value == rate) // No need to stack same values
                return;

            _rates[commodity].Insert(0, new TaxRate(DateTime.Now, rate));
        }

        /// <summary>
        /// Gets the tax rate that is active for a specific point in time (in UTC).
        /// A custom tax rate is seen as the currently active rate for a period from its starting timestamp until a new custom rate is set.
        /// If there is no custom tax rate for the specified date, use the standard tax rate.
        /// </summary>
        public double GetTaxRateForDateTime(Commodity commodity, DateTime date)
        {
            //TODO: implement
            return _rates[commodity]
                .First(x => DateTime.Compare(date, x.StartingFrom) > 0)
                .Value;
        }

        /// <summary>
        /// Gets the tax rate that is active for the current point in time.
        /// A custom tax rate is seen as the currently active rate for a period from its starting timestamp until a new custom rate is set.
        /// If there is no custom tax currently active, use the standard tax rate.
        /// </summary>
        public double GetCurrentTaxRate(Commodity commodity)
        {
            //TODO: implement
            return _rates[commodity].First().Value;
        }
    }

    public enum Commodity
    {
        //PLEASE NOTE: THESE ARE THE ACTUAL TAX RATES THAT SHOULD APPLY, WE JUST GOT THEM FROM THE CLIENT!
        Default,            //25%
        Alcohol,            //25%
        Food,               //12%
        FoodServices,       //12%
        Literature,         //6%
        Transport,          //6%
        CulturalServices    //6%
    }

    public readonly struct TaxRate
    {
        public DateTime StartingFrom { get; }
        public double Value { get; }
        public TaxRate(DateTime startingFrom, double value)
        {
            StartingFrom = startingFrom;
            Value = value;
        }
    }
}
