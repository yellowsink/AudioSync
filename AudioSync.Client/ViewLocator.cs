using System;
using AudioSync.Client.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AudioSync.Client
{
	public class ViewLocator : IDataTemplate
	{
		public bool SupportsRecycling => false;

		public IControl Build(object data)
		{
			var name = data.GetType().FullName!.Replace("ViewModel", "View");
			var type = Type.GetType(name);

			if (type != null)
				return (Control) Activator.CreateInstance(type)!;
			return new TextBlock { Text = "Not Found: " + name };
		}

		public bool Match(object data) => data is ViewModelBase;
	}
}