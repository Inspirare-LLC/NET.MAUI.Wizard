using System.ComponentModel;

namespace NET.MAUI.Wizard.Abstractions
{
    public interface IWizardContentView
    {
        Task<bool> OnNext(IWizardViewModel viewModel);
        Task<bool> OnPrevious(IWizardViewModel viewModel);
        Task OnAppearing();
        Task OnDisappearing();
    }
}
