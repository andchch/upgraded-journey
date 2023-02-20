using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace kursovaya;

public partial class GiveWindow : Window
{
    public GiveWindow()
    {
        InitializeComponent();
    }

    public string Name
    {
        get { return nametb.Text; }
    }

    public int Num
    {
        get { return int.Parse(numtb.Text); }
    }

    private void givebt_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
    }
    
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
    
    private void NameValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^a-zA-z ]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}