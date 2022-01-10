using System;
using System.Windows.Input;

namespace GraphChatApp.ViewModel
{
	public class ShowMessageEditorCommand : ICommand
	{
		private readonly MessageViewModel viewModel;

		public ShowMessageEditorCommand(MessageViewModel messageViewModel)
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
			var mvm = viewModel as MessageViewerViewModel;
			if (mvm == null)
				return;
			mvm.AddResponse();
		}

		public event EventHandler? CanExecuteChanged;
	}
}