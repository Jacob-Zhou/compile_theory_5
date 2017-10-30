using compile_theory_5.Model;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace compile_theory_5.ViewModel
{
	public enum HLKind
	{
		KEYWORD,
		ID,
		NUM,
		PUNCT,
		ERROR,
		OPERATOR,
		ANNO
	}

	public class OffsetColorizer : DocumentColorizingTransformer
	{
		public OffsetColorizer(int offset = 0, int length = 0, HLKind hlk = HLKind.ID)
		{
			this.offset = offset;
			this.length = length;
			this.hlk = hlk;
		}

		public int offset { get; set; }
		public int length { get; set; }
		public HLKind hlk { get; set; }

		protected override void ColorizeLine(DocumentLine line)
		{
			if (offset == 0 && length == 0)
				return;

			if (line.Length == 0)
				return;

			if (line.EndOffset < offset || line.Offset > offset + length)
				return;

			int start = line.Offset > offset ? line.Offset : offset;
			int end = offset + length > line.EndOffset ? line.EndOffset : offset + length;

			switch (hlk)
			{
				case HLKind.ID:
					ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(Brushes.Black));
					break;
				case HLKind.KEYWORD:
					ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(Brushes.Blue));
					break;
				case HLKind.NUM:
					ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(Brushes.SeaGreen));
					break;
				case HLKind.PUNCT:
					ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(Brushes.LimeGreen));
					break;
				case HLKind.ERROR:
					ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(Brushes.Red));
					break;
				case HLKind.ANNO:
					ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(Brushes.Green));
					break;
				case HLKind.OPERATOR:
					ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(Brushes.DarkGray));
					break;
			}
		}
	}

	class SourceViewModel
	{
		private static int offset = -1;
		private static TextDocument document;
		private static TextEditor textEditor;
		private static Encoding encoder = Encoding.Default;
		private static byte[] sourceData;
		private static bool errorMode = false;

		public static Encoding Encoder
		{
			get
			{
				return encoder;
			}

			set
			{
				encoder = value;
				if(sourceData != null)
				{
					textEditor.Text = encoder.GetString(sourceData);
				}
			}
		}

		public static byte[] SourceData
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

		public static void Init(TextEditor tEditor)
		{
			textEditor = tEditor;
			document = tEditor.Document;
			textEditor.TextChanged += TextChanged;
		}

		public static void Colorize(int offset, int length, HLKind hlk)
		{
			textEditor.TextArea.TextView.LineTransformers.Add(new OffsetColorizer(offset, length, hlk));
		}

		private static void TextChanged(object sender, EventArgs e)
		{
			Reset();
			textEditor.TextArea.TextView.LineTransformers.Clear();
			Lexer.Highlighting();
			textEditor.TextArea.TextView.Redraw();
		}

		public static void KeepOnlyRead()
		{
			textEditor.IsReadOnly = true;
		}

		public static void UnkeepOnlyRead()
		{
			textEditor.IsReadOnly = false;
			Reset();
		}

		public static void Reset()
		{
			offset = -1;
		}

		public static int GetOffset(int line, int column)
		{
			return document.GetOffset(line, column);
		}

		public static int GetOffset()
		{
			return offset;
		}

		public static int GetLine(int offset)
		{
			return document.GetLocation(offset).Line;
		}

		public static int GetLineOffset(int offset)
		{
			return document.GetLocation(offset).Column;
		}

		public static int GetLineCount()
		{
			return document.LineCount;
		}

		public static int GetEndOffset()
		{
			return document.TextLength - 1;
		}

		public static void SetOffset(int noffset)
		{
			offset = noffset;
		}

		public static void putBack()
		{
			if(offset >= 0)
			{
				offset--;
			}
		}

		public static char? NextChar()
		{
			if(offset + 1 < document.TextLength)
			{
				offset++;
				return document.GetCharAt(offset);
			}
			else
			{
				return null;
			}
		}

		public static void ChangeMode()
		{
			if (errorMode)
			{
				Grid.SetColumnSpan(textEditor, 3);
				ErrorViewModel.getInstance().clear();
				errorMode = false;
			}
			else
			{
				Grid.SetColumnSpan(textEditor, 1);
				errorMode = true;
			}
		}

		public static void ErrorMode()
		{
			Grid.SetColumnSpan(textEditor, 1);
			errorMode = true;
		}

		public static void NormalMode()
		{
			Grid.SetColumnSpan(textEditor, 3);
			ErrorViewModel.getInstance().clear();
			errorMode = false;
		}


		public static char? Forward()
		{
			if (offset + 1 < document.TextLength)
			{
				return document.GetCharAt(offset + 1);
			}
			else
			{
				return null;
			}
		}
	}
}
