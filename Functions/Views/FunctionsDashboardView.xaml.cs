using Functions.Interfaces;
using System.ComponentModel;
using System.Windows;

namespace Functions.Views;

public partial class FunctionsDashboardView : Window
{
    private bool _closingRequested = false;

    public FunctionsDashboardView()
    {
        InitializeComponent();

        Closing += Close;
    }

    private void Close(object? sender, CancelEventArgs e)
    {
        if (_closingRequested)
            return;

        if (DataContext is IHasChanges context && context.HasChanges)
        {
            var result = MessageBox.Show(
                "У вас есть несохраненные изменения, вы уверены, что хотите выйти?",
                "Несохраненные изменения",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result is (MessageBoxResult.No or MessageBoxResult.Cancel))
            {
                e.Cancel = true;
            }
            else
            {
                _closingRequested = true;
            }
        }
    }
}