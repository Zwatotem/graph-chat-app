using System;
using System.Windows.Input;

namespace GraphChatApp.ViewModel
{
	public class ShowMessageEditorCommand : ICommand
	{
		private readonly MessageViewModelBase viewModel;

		public ShowMessageEditorCommand(MessageViewModelBase messageViewModel)
		{
			this.viewModel = messageViewModel;
		}

		public bool CanExecute(object? parameter)
		{
			return true;
			return viewModel.CanShowMessageEditor;
		}

		public void Execute(object? parameter)
		{
			var mvm = viewModel as MessageViewModel;
			if (mvm == null)
				return;
			mvm.AddResponse();
		}

		public event EventHandler? CanExecuteChanged;
	}
}