using NomaiVR.Tools;
using Valve.VR;
using static Valve.VR.SteamVR_Actions;

namespace NomaiVR.Input.ActionInputs
{
    public static class ActionInputDefinitions
    {
        public static readonly EmptyActionInput Empty =
            new EmptyActionInput();

        public static readonly BooleanActionInput Autopilot =
            new BooleanActionInput(action: _default.Autopilot);

        public static readonly BooleanActionInput Back =
            new BooleanActionInput(action: _default.Back);

        public static readonly BooleanActionInput Grip =
            new BooleanActionInput(action: _default.Grip);

        public static readonly BooleanActionInput SecondaryInteract =
            new BooleanActionInput(action: _default.SecondaryInteract);

        public static readonly BooleanActionInput Interact =
            new BooleanActionInput(action: _default.Interact);

        public static readonly BooleanActionInput Jump =
            new BooleanActionInput(action: _default.Jump);

        public static readonly Vector2ActionInput Look =
            new Vector2ActionInput(action: _default.Look);

        public static readonly Vector2ActionInput LookY =
            new Vector2ActionInput(action: _default.Look, yOnly: true);

        public static readonly BooleanActionInput Map =
            new BooleanActionInput(action: _default.Map);

        public static readonly BooleanActionInput Menu =
            new BooleanActionInput(action: _default.Menu);

        public static readonly Vector2ActionInput Move =
            new Vector2ActionInput(action: _default.Move);

        public static readonly Vector2ActionInput MoveZ =
            new Vector2ActionInput(action: _default.Move, yOnly: true);

        public static readonly BooleanActionInput Recenter =
            new BooleanActionInput(action: _default.Recenter);

        public static readonly BooleanActionInput RollMode =
            new BooleanActionInput(action: _default.RollMode);

        public static readonly BooleanActionInput StationaryUse =
            new BooleanActionInput(action: _default.StationaryUse);

        public static readonly Vector2ActionInput StationaryUp =
            new Vector2ActionInput(action: _default.StationaryDpad, yOnly: true, clamp: true, yZero: true, textureModifier: "up");

        public static readonly Vector2ActionInput StationaryDown =
            new Vector2ActionInput(action: _default.StationaryDpad, yOnly: true, invert: true, clamp: true, yZero: true, textureModifier: "down");

        public static readonly Vector2ActionInput StationaryLeft =
            new Vector2ActionInput(action: _default.StationaryDpad, invert: true, clamp: true, yZero: true, textureModifier: "left");

        public static readonly Vector2ActionInput StationaryRight =
            new Vector2ActionInput(action: _default.StationaryDpad, clamp: true, textureModifier: "right");

        public static readonly SingleActionInput ThrustDown =
            new SingleActionInput(action: _default.ThrustDown);

        public static readonly SingleActionInput ThrustUp =
            new SingleActionInput(action: _default.ThrustUp);

        public static readonly Vector2ActionInput Up =
            new Vector2ActionInput(action: _default.UIDpad, yOnly: true, clamp: true, yZero: true, textureModifier: "up");

        public static readonly Vector2ActionInput Down =
            new Vector2ActionInput(action: _default.UIDpad, yOnly: true, invert: true, clamp: true, yZero: true, textureModifier: "down");

        public static readonly Vector2ActionInput Left =
            new Vector2ActionInput(action: _default.UIDpad, invert: true, clamp: true, yZero: true, textureModifier: "left");

        public static readonly Vector2ActionInput Right =
            new Vector2ActionInput(action: _default.UIDpad, clamp: true, yZero: true, textureModifier: "right");

        public static readonly BooleanActionInput UISelect =
            new BooleanActionInput(action: _default.UISelect);

        public static readonly BooleanActionInput UISubtabLeft =
            new BooleanActionInput(action: _default.UISubtabLeft, clickable: false);

        public static readonly BooleanActionInput UISubtabRight =
            new BooleanActionInput(action: _default.UISubtabRight, clickable: false);

        public static readonly BooleanActionInput UITabLeft =
            new BooleanActionInput(action: _default.UITabLeft, clickable: false);

        public static readonly BooleanActionInput UITabRight =
            new BooleanActionInput(action: _default.UITabRight, clickable: false);

        // Controllers with a grip click binding (Steam Frame) use the grip's click on the hand holding the tool.
        // It's checked first so a tools/use binding can't shadow it. Other controllers fall back to tools/use.
        public static readonly FallbackActionInput ToolUse = new FallbackActionInput(
            new BooleanActionInput(action: _default.GripClick, optional: true, handSource: () => InteractingHandSource),
            new BooleanActionInput(action: tools.Use, eitherHand: true));

        // Ship scout and signalscope on the Steam Frame: the tools set is active on the left hand, shadowing its bumper,
        // so the right bumper's secondary interact is used.
        public static readonly BooleanActionInput ShipToolUse =
            new BooleanActionInput(action: _default.SecondaryInteract, handSource: () => SteamVR_Input_Sources.RightHand);

        public static readonly FallbackActionInput ToolSecondary = new FallbackActionInput(
            new BooleanActionInput(action: tools.Secondary, eitherHand: true, optional: true),
            SecondaryInteract);

        public static readonly FallbackActionInput ToolUp = new FallbackActionInput(
            new Vector2ActionInput(action: tools.DPad, optional: true, yOnly: true, clamp: true, yZero: true, eitherHand: true, textureModifier: "up"),
            new BooleanActionInput(action: tools.DPadUp, eitherHand: true, clickable: false, optional: true));

        public static readonly FallbackActionInput ToolDown = new FallbackActionInput(
            new Vector2ActionInput(action: tools.DPad, optional: true, yOnly: true, invert: true, clamp: true, yZero: true, eitherHand: true, textureModifier: "down"),
            new BooleanActionInput(action: tools.DPadDown, eitherHand: true, clickable: false, optional: true));

        public static readonly FallbackActionInput ToolLeft = new FallbackActionInput(
            new Vector2ActionInput(action: tools.DPad, optional: true, invert: true, clamp: true, yZero: true, eitherHand: true, textureModifier: "left"),
            new BooleanActionInput(action: tools.DPadLeft, eitherHand: true, clickable: false, optional: true));

        public static readonly FallbackActionInput ToolRight = new FallbackActionInput(
            new Vector2ActionInput(action: tools.DPad, optional: true, clamp: true, yZero: true, eitherHand: true, textureModifier: "right"),
            new BooleanActionInput(action: tools.DPadRight, eitherHand: true, clickable: false, optional: true));

        private static SteamVR_Input_Sources InteractingHandSource =>
            VRToolSwapper.InteractingHand != null ? VRToolSwapper.InteractingHand.InputSource : SteamVR_Input_Sources.Any;
    }
}
