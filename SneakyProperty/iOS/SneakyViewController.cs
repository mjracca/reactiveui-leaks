using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using ReactiveUI;

namespace SneakyProperty.iOS
{
	partial class SneakyViewController : ReactiveViewController
	{
		private SneakyViewModel viewModel;

		public SneakyViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.viewModel = new SneakyViewModel ();
			Back.TouchUpInside += this.DismissSneakyViewController;
		}

		protected override void Dispose (bool disposing)
		{
			this.viewModel = null;
			Back.TouchUpInside -= this.DismissSneakyViewController;

			base.Dispose (disposing);
			this.View.TerminatorDispose ();
		}

		private void DismissSneakyViewController(object sender, EventArgs e)
		{
			this.DismissViewController(true, () => Dispose());
		}
	}
}