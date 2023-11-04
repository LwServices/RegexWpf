using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace RegexWpf
{
    public class FlowDocumentConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var FlowDocument = new FlowDocument();
            try
            {
                if (values[0] is not string InputText || values[1] is not ObservableCollection<Match> matches) throw new ArgumentOutOfRangeException("not all arguments are right");

                FlowDocument.Blocks.Add(new Paragraph(new Run(InputText)));
                TextPointer position = FlowDocument.ContentStart;
                var highLights = new[] { Brushes.Yellow, Brushes.Orange, Brushes.LightGreen, Brushes.LightBlue };
                var i = 0;
                foreach (Match match in matches)
                {
                    var word = match.Value;

                    while (position != null)
                    {
                        if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                        {
                            string textRun = position.GetTextInRun(LogicalDirection.Forward);

                            // Find the starting index of any substring that matches "word".
                            int indexInRun = textRun.IndexOf(word);
                            if (indexInRun >= 0)
                            {
                                TextPointer start = position.GetPositionAtOffset(indexInRun);
                                TextPointer end = start.GetPositionAtOffset(word.Length);
                                var textRange = new TextRange(start, end);
                                var color = (i++ % highLights.Length);
                                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, highLights[color]);
                                position = start.GetPositionAtOffset(word.Length);
                                break;
                            }
                        }

                        position = position.GetNextContextPosition(LogicalDirection.Forward);
                    }
                }
            }
            catch (Exception e)
            {
                FlowDocument.Blocks.Add(new Paragraph(new Run(e.Message)));
            }

            return FlowDocument;
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Implementation of IMultiValueConverter
    }
}