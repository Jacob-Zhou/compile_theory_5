using compile_theory_5.ViewModel;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace compile_theory_5.Model.IRSystem.IRViewModel
{
	class IRSourceViewModel
	{
		private int offset = -1;
		private TextDocument document;
		private TextEditor textEditor;
		private Encoding encoder = Encoding.Default;
		private byte[] sourceData;

		public Encoding Encoder
		{
			get
			{
				return encoder;
			}

			set
			{
				encoder = value;
				if (sourceData != null)
				{
					textEditor.Text = encoder.GetString(sourceData);
				}
			}
		}

		public byte[] SourceData
		{
			get
			{
				return sourceData;
			}

			set
			{
				sourceData = value;
				if (sourceData != null)
				{
					textEditor.Text = encoder.GetString(sourceData);
				}
			}
		}

		public void Init(TextEditor tEditor)
		{
			textEditor = tEditor;
			document = tEditor.Document;
			textEditor.TextChanged += TextChanged;
		}

		public void Colorize(int offset, int length, HLKind hlk)
		{
			textEditor.TextArea.TextView.LineTransformers.Add(new OffsetColorizer(offset, length, hlk));
		}

		private void TextChanged(object sender, EventArgs e)
		{
			Reset();
			textEditor.TextArea.TextView.LineTransformers.Clear();
			IRSystem.lexer.Highlighting();
			IRSystem.cache.Reset();
			textEditor.TextArea.TextView.Redraw();
		}

		public void SetText(string text)
		{
			textEditor.Text = text;
		}

		public void KeepOnlyRead()
		{
			textEditor.IsReadOnly = true;
		}

		public void UnkeepOnlyRead()
		{
			textEditor.IsReadOnly = false;
			Reset();
		}

		public void Reset()
		{
			offset = -1;
		}

		public int GetOffset(int line, int column)
		{
			return document.GetOffset(line, column);
		}

		public int GetOffset()
		{
			return offset;
		}

		public DocumentLine GetIRLine(int line)
		{
			return document.GetLineByNumber(line);
		}

		public int GetLine(int offset)
		{
			return document.GetLocation(offset).Line;
		}

		public int GetLineOffset(int offset)
		{
			return document.GetLocation(offset).Column;
		}

		public int GetLineCount()
		{
			return document.LineCount;
		}

		public int GetEndOffset()
		{
			return document.TextLength - 1;
		}

		public void SetOffset(int noffset)
		{
			offset = noffset;
		}

		public void putBack()
		{
			if (offset >= 0)
			{
				offset--;
			}
		}

		public char? GetChar(int offset)
		{
			if (offset < document.TextLength)
			{
				return document.GetCharAt(offset);
			}
			else
			{
				return null;
			}
		}

		public char? NextChar()
		{
			if (offset + 1 < document.TextLength)
			{
				offset++;
				return GetChar(offset);
			}
			else
			{
				return null;
			}
		}

		public void ShowIP()
		{
			if(IRSystem.interpreter.instructionPointer <= document.LineCount)
			{
				var l = document.GetLineByNumber(IRSystem.interpreter.instructionPointer);
				textEditor.Select(l.Offset, l.Length);
			}
		}

		public void LockEditor()
		{
			textEditor.IsEnabled = false;
		}

		public void UnlockEditor()
		{
			textEditor.IsEnabled = true;
		}
	}
}
