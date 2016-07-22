using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ContosoCabs.UWP.Custom_Triggers
{
    public class FontSizeTrigger : StateTriggerBase
    {
        private int _minFontSize = 8;
        private FrameworkElement _targetElement;
        private int _currentFontSize;

        public int MinFontSize
        {
            get
            {
                return _minFontSize;
            }
            set
            {
                _minFontSize = value;
            }
        }
        public FrameworkElement TargetElement
        {
            get
            {
                return _targetElement;
            }
            set
            {
                if (_targetElement != null)
                {
                    _targetElement.SizeChanged -= _targetElement_SizeChanged;
                }
                _targetElement = value;
                _targetElement.SizeChanged += _targetElement_SizeChanged;
            }
        }

        private void _targetElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
