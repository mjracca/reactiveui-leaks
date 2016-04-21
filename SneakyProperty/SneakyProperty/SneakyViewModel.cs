using System;
using ReactiveUI;

namespace SneakyProperty
{
	public class SneakyViewModel : ReactiveObject
	{
		private bool isSneaky;

		public SneakyViewModel ()
		{
            this.RaiseAndSetIfChanged(ref isSneaky, true);
        }
	}
}