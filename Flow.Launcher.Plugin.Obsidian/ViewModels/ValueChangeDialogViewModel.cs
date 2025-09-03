using System;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class ValueChangeDialogViewModel(
    string valueToChange,
    string operationTitle,
    string valueName,
    Func<string, ResultMessage>? condition)
    : BaseModel
{
    public delegate void RequestCloseEventHandler();

    public delegate void ValueChangedEventHandler(string newValue);

    private string _errorMessage = string.Empty;
    private Visibility _errorMessageVisibility = Visibility.Collapsed;
    private string _newValueInput = valueToChange;
    private bool _valueHasChanged;

    [UsedImplicitly] // For design-time data
    public ValueChangeDialogViewModel() : this(string.Empty, "Change value", "value", null) { }

    public string NewValueInput
    {
        get => _newValueInput;
        set
        {
            if (value == _newValueInput)
            {
                return;
            }

            _newValueInput = value;
            ValidateCommand.NotifyCanExecuteChanged();
            _valueHasChanged = true;
        }
    }

    public string OperationTitle => operationTitle;

    public string ValueName => valueName;

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (value == _errorMessage)
            {
                return;
            }

            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public Visibility ErrorMessageVisibility
    {
        get => _errorMessageVisibility;
        set
        {
            if (value == _errorMessageVisibility)
            {
                return;
            }

            _errorMessageVisibility = value;
            OnPropertyChanged();
        }
    }

    public event RequestCloseEventHandler? RequestClose;
    public event ValueChangedEventHandler? ValueChanged;

    [RelayCommand(CanExecute = nameof(CanExecuteValidate))]
    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(NewValueInput))
        {
            return;
        }

        ValueChanged?.Invoke(NewValueInput);
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void CancelOperation() => RequestClose?.Invoke();

    private bool CanExecuteValidate()
    {
        if (!_valueHasChanged)
        {
            // This doesn't show an error when the window is open.
            return false;
        }

        if (condition is null)
        {
            HideErrorMessage();
            return true;
        }

        ResultMessage result = condition.Invoke(NewValueInput);
        if (result.IsSuccess)
        {
            HideErrorMessage();
        }
        else
        {
            ShowErrorMessage(result.ErrorMessage);
        }

        return result.IsSuccess;
    }

    private void ShowErrorMessage(string message)
    {
        ErrorMessage = message;
        ErrorMessageVisibility = Visibility.Visible;
    }

    private void HideErrorMessage()
    {
        ErrorMessage = string.Empty;
        ErrorMessageVisibility = Visibility.Collapsed;
    }
}
