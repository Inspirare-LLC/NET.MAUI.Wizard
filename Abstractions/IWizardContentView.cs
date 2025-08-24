using System.ComponentModel;

namespace NET.MAUI.Wizard.Abstractions
{
    public interface IWizardContentView
    {
        Task<bool> OnNextAsync(IWizardViewModel viewModel);
        Task<bool> OnPreviousAsync(IWizardViewModel viewModel);
        Task OnAppearingAsync();
        Task OnDisappearingAsync();
    }
}
