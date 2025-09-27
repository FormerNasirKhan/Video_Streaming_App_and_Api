// Infrastructure/RelayCommand.cs
using System;
using System.Windows.Input;

namespace VideoPlayer.Wpf.Infrastructure
{
    /// <summary>
    /// Basic RelayCommand for WPF (object? parameter).
    /// Supports CanExecute, auto-refresh via CommandManager, and manual refresh.
    /// </summary>
    public sealed class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            _execute = _ => execute();
            _canExecute = canExecute is null ? null : new Predicate<object?>(_ => canExecute());
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        // WPF listens to CommandManager.RequerySuggested to refresh button state
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Manually force WPF to requery CanExecute for all commands.
        /// </summary>
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Strongly-typed RelayCommand&lt;T&gt; for safer parameter casting.
    /// </summary>
    public sealed class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Predicate<T?>? _canExecute;

        public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is null) return _canExecute?.Invoke(default) ?? true;
            if (parameter is T t) return _canExecute?.Invoke(t) ?? true;
            return false; // wrong parameter type
        }

        public void Execute(object? parameter)
        {
            T? value = parameter is T t ? t : default;
            _execute(value);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}
