using System;
using ReactiveUI;

namespace SneakyProperty
{
	public class SneakyViewModel : ReactiveObject
	{
		private bool _IsSneaky;

		public bool IsSneaky
		{
			get { return _IsSneaky; }
			set { this.RaiseAndSetIfChanged(ref _IsSneaky, value); }
		}

		public SneakyViewModel ()
		{
		}
	}
}