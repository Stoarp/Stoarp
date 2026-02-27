using System;
using ReactiveUI.Avalonia;
using Stoarp.ViewModels;

namespace Stoarp.Views;

public partial class RegisterView : ReactiveUserControl<RegisterViewModel>
{
    public RegisterView()
    {
        InitializeComponent();
    }
}