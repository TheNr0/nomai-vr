using UnityEngine;
using Valve.VR;

namespace NomaiVR.Input.ActionInputs
{
    /// <summary>
    /// Uses the primary input when it's bound on the current controller, otherwise the fallback.
    /// E.g. the tool dpad is a vector2 on trackpads, but separate buttons on controllers with a real dpad.
    /// </summary>
    public class FallbackActionInput : IActionInput
    {
        private readonly IActionInput primary;
        private readonly IActionInput fallback;

        public FallbackActionInput(IActionInput primary, IActionInput fallback)
        {
            this.primary = primary;
            this.fallback = fallback;
        }

        public IActionInput Current => primary.Active ? primary : fallback;

        public Vector2 Value => Current.Value;
        public ISteamVR_Action_In Action => Current.Action;
        public bool Optional => primary.Optional && fallback.Optional;
        public bool Active => primary.Active || fallback.Active;
        public SteamVR_Input_Sources ActiveSource => Current.ActiveSource;
    }
}
