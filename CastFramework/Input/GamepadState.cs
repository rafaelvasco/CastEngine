namespace CastFramework
{
    public struct GamepadState
    {
        public static readonly GamepadState Default = new GamepadState();

        public GamepadButton ButtonsState;

        private GamepadButton CompositeState
        {
            get
            {
                var result = ButtonsState;

                result |= Thumbsticks.DirectionalState;

                return result;
            }
        }

        public GamepadThumbsticks Thumbsticks;

        public GamePadTriggers Triggers;

        public bool this[GamepadButton button] => (CompositeState & button) == button;

        public GamepadState(GamepadThumbsticks thumbSticksState, GamePadTriggers triggersState,
            GamepadButton buttonsState)
            : this()
        {
            Thumbsticks = thumbSticksState;
            Triggers = triggersState;
            ButtonsState = buttonsState;
        }
    }
}
