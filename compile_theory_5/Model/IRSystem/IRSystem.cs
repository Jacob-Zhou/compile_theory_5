using compile_theory_5.Model.IRSystem.IRViewModel;
using compile_theory_5.Model.IRSystem.IRInterpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using System.Windows.Controls;

namespace compile_theory_5.Model.IRSystem
{
	static class IRSystem
	{
		static public IRMomoryViewModel momory = new IRMomoryViewModel();
		static public IRSourceViewModel source = new IRSourceViewModel();
		static public IRLexer lexer = new IRLexer();
		static public IRParser parser = new IRParser();
		static public IRInterpreter.IRInterpreter interpreter = new IRInterpreter.IRInterpreter();
		static public bool isRunning = false;
		static public IRCache cache = new IRCache();
		static public IRClock clock = new IRClock();
		static private bool IsInit = false;

		static public void Init(TextEditor source, DataGrid data, TextBox text)
		{
			IsInit = true;
			IRSystem.source.Init(source);
			momory.Init(data);
			clock.Actions += interpreter.Execute;
		}

		static public void Pause()
		{
			if (IsInit)
			{
				clock.Stop();
			}
		}

		static public void Stop()
		{
			if (IsInit)
			{
				clock.Stop();
				interpreter.Reset();
				cache.Reset();
				momory.Reset();
				source.UnlockEditor();
			}
		}

		static public void Start()
		{
			if (IsInit)
			{
				clock.Start();
				source.LockEditor();
			}
		}

		static public void OneStep()
		{
			if (IsInit)
			{
				interpreter.Execute();
				source.LockEditor();
			}
		}

		static public void changeInterval(int interval)
		{
			if (IsInit)
			{
				clock.Interval = interval;
			}
		}
	}
}
