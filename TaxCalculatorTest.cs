namespace tax_calculator;

[TestClass]
public class TaxCalculatorTest
{
    private readonly Dictionary<Commodity, double> _defaultRates = new() {
        {Commodity.Default, 0.25},
        {Commodity.Alcohol, 0.25},
        {Commodity.Food, 0.12},
        {Commodity.FoodServices, 0.12},
        {Commodity.Literature, 0.06},
        {Commodity.Transport, 0.06},
        {Commodity.CulturalServices, 0.06}
    };

    [TestMethod]
    public void CheckDefaultTaxRate()
    {
        ITaxCalculator ts = new TaxCalculator();
        Assert.AreEqual(ts.GetStandardTaxRate(Commodity.Default), _defaultRates[Commodity.Default], "Default value is wrong!");
        Assert.AreEqual(ts.GetStandardTaxRate(Commodity.Alcohol), _defaultRates[Commodity.Alcohol], "Default value is wrong!");
        Assert.AreEqual(ts.GetStandardTaxRate(Commodity.Food), _defaultRates[Commodity.Food], "Default value is wrong!");
        Assert.AreEqual(ts.GetStandardTaxRate(Commodity.FoodServices), _defaultRates[Commodity.FoodServices], "Default value is wrong!");
        Assert.AreEqual(ts.GetStandardTaxRate(Commodity.Literature), _defaultRates[Commodity.Literature], "Default value is wrong!");
        Assert.AreEqual(ts.GetStandardTaxRate(Commodity.Transport), _defaultRates[Commodity.Transport], "Default value is wrong!");
        Assert.AreEqual(ts.GetStandardTaxRate(Commodity.CulturalServices), _defaultRates[Commodity.CulturalServices], "Default value is wrong!");
    }

    [TestMethod]
    public void CheckMultipleInstances()
    {
        ITaxCalculator ts1 = new TaxCalculator();
        ITaxCalculator ts2 = new TaxCalculator();
        ITaxCalculator ts3 = new TaxCalculator();

        const double newFood1 = 0.05;
        const double newFood2 = 0.15;
        const double newAlcohol2 = 0.2;
        const double newAlcohol3 = 0.19;

        ts1.SetCustomTaxRate(Commodity.Food, newFood1);
        ts2.SetCustomTaxRate(Commodity.Food, newFood2);
        ts2.SetCustomTaxRate(Commodity.Alcohol, newAlcohol2);
        ts3.SetCustomTaxRate(Commodity.Alcohol, newAlcohol3);
        
        Assert.AreEqual(ts1.GetCurrentTaxRate(Commodity.Food), newFood1, "Multiple instances interfere between each other.");
        Assert.AreEqual(ts1.GetCurrentTaxRate(Commodity.Alcohol), _defaultRates[Commodity.Alcohol], "Multiple instances interfere between each other.");
        Assert.AreEqual(ts2.GetCurrentTaxRate(Commodity.Food), newFood2, "Multiple instances interfere between each other.");
        Assert.AreEqual(ts2.GetCurrentTaxRate(Commodity.Alcohol), newAlcohol2, "Multiple instances interfere between each other.");
        Assert.AreEqual(ts3.GetCurrentTaxRate(Commodity.Food), _defaultRates[Commodity.Food], "Multiple instances interfere between each other.");
        Assert.AreEqual(ts3.GetCurrentTaxRate(Commodity.Alcohol), newAlcohol3, "Multiple instances interfere between each other.");
    }

    [TestMethod]
    public void CheckChronologicalAdding()
    {
        ITaxCalculator ts = new TaxCalculator();

        const double food1 = 0.05;
        const double food2 = 0.06;
        const double food3 = 0.07;

        DateTime date = DateTime.Now.AddDays(-1);
        
        ts.SetCustomTaxRate(Commodity.Food, food1);
        Assert.AreEqual(ts.GetTaxRateForDateTime(Commodity.Food, date), _defaultRates[Commodity.Food], "Error in chronological saving.");

        date = DateTime.Now;
        ts.SetCustomTaxRate(Commodity.Food, food2);
        ts.SetCustomTaxRate(Commodity.Food, food3);

        Assert.AreEqual(ts.GetTaxRateForDateTime(Commodity.Food, date), food1, "Error in chronological saving.");
    }
}