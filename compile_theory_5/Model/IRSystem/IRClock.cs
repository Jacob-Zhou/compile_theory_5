using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace compile_theory_5.Model.IRSystem
{
	class IRClock
	{
		private int interval = 1000;
		private Action actions = null;
		private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.DataBind);

		public IRClock()
		{
			timer.Interval = TimeSpan.FromMilliseconds(interval);
			timer.Tick += IRClockTick;
		}

		public Action Actions
		{
			get
			{
				return actions;
			}

			set
			{
				actions = value;
			}
		}

		public int Interval
		{
			get
			{
				return interval;
			}

			set
			{
				interval = value;
				timer.Interval = TimeSpan.FromMilliseconds(interval);
			}
		}

		public void AddAction(Action a)
		{
			actions += a;
		}

		private void IRClockTick(object sender, EventArgs e)
		{
			actions();
		}

		public void RemoveAction(Action a)
		{
			actions -= a;
		}

		public void Start()
		{
			timer.Start();
		}

		public void Stop()
		{
			timer.Stop();
		}
	}
}
