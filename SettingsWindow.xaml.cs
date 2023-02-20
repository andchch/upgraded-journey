using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace kursovaya;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        
        var manager = new INIManager(Environment.CurrentDirectory + "\\settings.ini");

        hpsSlider.Value = double.Parse(manager.GetPrivateString("weights", "hasHPS"));
        adcSlider.Value = double.Parse(manager.GetPrivateString("weights", "numOfADCChannels"));
        voltageSlider.Value = double.Parse(manager.GetPrivateString("weights", "voltage"));
        ddrSlider.Value = double.Parse(manager.GetPrivateString("weights", "hasDDR"));
        nobSlider.Value = double.Parse(manager.GetPrivateString("weights", "numOfButtons"));
        nosSlider.Value = double.Parse(manager.GetPrivateString("weights", "numOfSwitches"));
        ledSlider.Value = double.Parse(manager.GetPrivateString("weights", "numOfLED"));
        gpioSlider.Value = double.Parse(manager.GetPrivateString("weights", "numOfGPIO"));
        vgaSlider.Value = double.Parse(manager.GetPrivateString("weights", "hasVGA"));
        ethSlider.Value = double.Parse(manager.GetPrivateString("weights", "hasEth"));
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var manager = new INIManager(Environment.CurrentDirectory + "\\settings.ini");

        var hps = Math.Round(hpsSlider.Value, 2);
        var adc = Math.Round(adcSlider.Value, 2);
        var voltage = Math.Round(voltageSlider.Value, 2);
        var ddr = Math.Round(ddrSlider.Value, 2);
        var nob = Math.Round(nobSlider.Value, 2);
        var nos = Math.Round(nosSlider.Value, 2);
        var led = Math.Round(ledSlider.Value, 2);
        var gpio = Math.Round(gpioSlider.Value, 2);
        var vga = Math.Round(vgaSlider.Value, 2);
        var eth = Math.Round(ethSlider.Value, 2);

        manager.WritePrivateString("weights", "hasHPS", hps.ToString());
        manager.WritePrivateString("weights", "numOfADCChannels", adc.ToString());
        manager.WritePrivateString("weights", "voltage", voltage.ToString());
        manager.WritePrivateString("weights", "hasDDR", ddr.ToString());
        manager.WritePrivateString("weights", "numOfButtons", nob.ToString());
        manager.WritePrivateString("weights", "numOfSwitches", nos.ToString());
        manager.WritePrivateString("weights", "numOfLED", led.ToString());
        manager.WritePrivateString("weights", "numOfGPIO", gpio.ToString());
        manager.WritePrivateString("weights", "hasVGA", vga.ToString());
        manager.WritePrivateString("weights", "hasEth", eth.ToString());

        Close();
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9,]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void hpsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        hpstb.Text = Math.Round(e.NewValue, 2).ToString();
    }

    private void adcSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        adctb.Text = Math.Round(e.NewValue, 2).ToString();
    }

    private void voltageSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        vtb.Text = Math.Round(e.NewValue, 2).ToString();
    }

    private void ddrSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ddrtb.Text = Math.Round(e.NewValue, 2).ToString();
    }

    private void nobSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        btb.Text = Math.Round(e.NewValue, 2).ToString();
    }

    private void nosSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        stb.Text = Math.Round(e.NewValue, 2).ToString();
    }

    private void ledSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ledtb.Text = Math.Round(e.NewValue, 2).ToString();
    }

    private void gpioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        gpiotb.Text = Math.Round(e.NewValue, 2).ToString();
    }

    private void vgaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        vgatb.Text = Math.Round(e.NewValue, 2).ToString();
    }

    private void ethSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ethtb.Text = Math.Round(e.NewValue, 2).ToString();
    }
}