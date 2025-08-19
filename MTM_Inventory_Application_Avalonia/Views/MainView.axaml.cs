using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;

namespace MTM_Inventory_Application_Avalonia.Views;

public partial class MainView : UserControl
{
    private SizeToContent _savedSizeToContent = SizeToContent.Manual;
    private double _savedWidth;
    private double _savedHeight;
    private bool _hasSavedSize;

    public MainView()
    {
        InitializeComponent();
        DataContextChanged += MainView_DataContextChanged;
        AttachedToVisualTree += MainView_AttachedToVisualTree;
        DetachedFromVisualTree += MainView_DetachedFromVisualTree;
    }

    private void MainView_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        AttachToVm(DataContext as INotifyPropertyChanged);

        // Apply sizing once on attach
        if (DataContext is MTM_Inventory_Application_Avalonia.ViewModels.MainViewModel vm)
        {
            ApplyLoginSizing(vm.IsLoginVisible);
        }
    }

    private void MainView_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        AttachToVm(null);
    }

    private void MainView_DataContextChanged(object? sender, EventArgs e)
    {
        AttachToVm(DataContext as INotifyPropertyChanged);
        if (DataContext is MTM_Inventory_Application_Avalonia.ViewModels.MainViewModel vm)
        {
            ApplyLoginSizing(vm.IsLoginVisible);
        }
    }

    private INotifyPropertyChanged? _currentVm;

    private void AttachToVm(INotifyPropertyChanged? vm)
    {
        if (_currentVm is not null)
        {
            _currentVm.PropertyChanged -= VmOnPropertyChanged;
        }
        _currentVm = vm;
        if (_currentVm is not null)
        {
            _currentVm.PropertyChanged += VmOnPropertyChanged;
        }
    }

    private void VmOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MTM_Inventory_Application_Avalonia.ViewModels.MainViewModel.IsLoginVisible)
            && sender is MTM_Inventory_Application_Avalonia.ViewModels.MainViewModel vm)
        {
            ApplyLoginSizing(vm.IsLoginVisible);
        }
    }

    private void ApplyLoginSizing(bool isLoginVisible)
    {
        if (VisualRoot is not Window window)
            return;

        if (isLoginVisible)
        {
            if (!_hasSavedSize)
            {
                _savedSizeToContent = window.SizeToContent;
                _savedWidth = window.Width;
                _savedHeight = window.Height;
                _hasSavedSize = true;
            }

            // Let the window size to the LoginView overlay
            window.SizeToContent = SizeToContent.WidthAndHeight;
        }
        else
        {
            // Restore prior Manual sizing if we have concrete dimensions.
            if (_hasSavedSize && !double.IsNaN(_savedWidth) && !double.IsNaN(_savedHeight))
            {
                window.SizeToContent = _savedSizeToContent;
                window.Width = _savedWidth;
                window.Height = _savedHeight;
            }
            else
            {
                // Size to the main content's desired size.
                window.SizeToContent = SizeToContent.WidthAndHeight;
            }
        }
    }
}
