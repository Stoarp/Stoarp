using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Stoarp.ViewModels;

namespace Stoarp;

[RequiresUnreferencedCode(
    "ViewLocator uses reflection to resolve views.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var vmName = param.GetType().FullName!;
        var viewName = vmName.Replace(".ViewModels.", ".Views.")
                             .Replace("ViewModel", "View");
        var type = Type.GetType(viewName);

        if (type != null)
            return (Control)Activator.CreateInstance(type)!;

        return new TextBlock { Text = "Not Found: " + viewName };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
