using System;

namespace CastFramework
{
     public class GuiSlider : GuiControl
    {
        public static Size DefaultSize => new Size(200, 30);

        public int Value
        {
            get => _value;
            set
            {
                _value = Calc.Clamp(value, _minValue, _maxValue);
                Gui.InvalidateVisual();
            }
        }

        public int MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;

                if (_minValue > _maxValue)
                {
                    _minValue = _maxValue;
                }

                if (_value < _minValue)
                {
                    _value = _minValue;
                }

                Gui.InvalidateVisual();
            }
        }

        public int MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;

                if (_maxValue < _minValue)
                {
                    _maxValue = _minValue;
                }

                if (_value > _maxValue)
                {
                    _value = _maxValue;
                }

                Gui.InvalidateVisual();
            }
        }

        public int Step
        {
            get => _step;
            set
            {
                _step = value;

                if (_step <= 1)
                {
                    _step = 1;
                }

                if (_step > _maxValue - _minValue)
                {
                    _step = _maxValue - _minValue;
                }

                Gui.InvalidateVisual();
            }
        }

        public Orientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;

                if (_orientation == Orientation.Vertical)
                {
                    Resize(H, W);
                    
                }

                Gui.InvalidateVisual();
            }
        }

        public override string Class => throw new NotImplementedException();

        public event EventHandler<int> OnValueChange; 

        private int _value;
        private int _minValue;
        private int _maxValue;
        private int _step;
        private Orientation _orientation;
        private bool _sliding;

        internal override void Update(GuiMouseState mouseState)
        {
            if (this.ContainsPoint(mouseState.MouseX, mouseState.MouseY))
            {
                if (mouseState.MouseLeftDown)
                {
                    if (!_sliding)
                    {
                        _sliding = true;
                        UpdateIndicator(mouseState.MouseX, mouseState.MouseY);
                    }
                }
            }

            if (!mouseState.MouseLeftDown && _sliding)
            {
                _sliding = false;
            }

            if (_sliding && mouseState.Moved)
            {
                UpdateIndicator(mouseState.MouseX, mouseState.MouseY);

            }

        }

        private void UpdateIndicator(int x, int y)
        {
            // Indicator area is offset by 2 pixels of origin , so offset x and y position by minus 2
            var factor = 
                _orientation == Orientation.Horizontal ? 
                Calc.Clamp((float) (x-2 - GlobalX) / W, 0.0f, 1.0f) : 
                Calc.Clamp((float)(y-2 - GlobalY) / H, 0.0f, 1.0f);

            _value = (int)(((_maxValue - _minValue) * factor + _minValue) / _step) * _step;
            OnValueChange?.Invoke(this, _value);

            Gui.InvalidateVisual();

        }

        internal override void Draw(Canvas canvas, GuiTheme theme)
        {
            theme.DrawSlider(canvas, this);
        }

        internal override void Draw(Canvas canvas, GuiStyle style)
        {
            throw new NotImplementedException();
        }

        internal GuiSlider(Gui gui, GuiContainer parent, int value, int minValue, int maxValue, int step, Orientation orientation) : base(gui, parent)
        {
            w = DefaultSize.W;
            h = DefaultSize.H;

            this._minValue = minValue;
            this._maxValue = maxValue;

            if (this._minValue > this._maxValue)
            {
                var temp = this._maxValue;
                this._maxValue = this._minValue;
                this._minValue = temp;
            }

            this._value = Calc.Clamp(value, this._minValue, this._maxValue);
            this._step = Calc.Clamp(step, 1, _maxValue);
            this._orientation = orientation;

            if (_orientation == Orientation.Vertical)
            {
                int temp = W;
                w = H;
                h = temp;
            }
        }
    }
}