using System;
using UnityEngine;
using Valve.VR;

namespace NomaiVR.Input.ActionInputs
{
    public class BooleanActionInput : ActionInput<SteamVR_Action_Boolean>
    {
        private readonly bool isEitherHand;
        private readonly bool clickable;
        private readonly Func<SteamVR_Input_Sources> handSource;

        public override Vector2 Value
        {
            get
            {
                if (handSource != null) return new Vector2(SpecificAction.GetState(handSource()) ? 1f : 0f, 0f);
                var state = isEitherHand
                    ? SpecificAction.GetState(SteamVR_Input_Sources.LeftHand) ||
                      SpecificAction.GetState(SteamVR_Input_Sources.RightHand)
                    : SpecificAction.state;
                return new Vector2(state ? 1f : 0f, 0f);
            }
        }

        public override bool Active
        {
            get
            {
                if (handSource != null) return SpecificAction.GetActive(handSource());
                var state = isEitherHand
                    ? SpecificAction.GetActive(SteamVR_Input_Sources.LeftHand) ||
                      SpecificAction.GetActive(SteamVR_Input_Sources.RightHand)
                    : SpecificAction.active;
                return state;
            }
        }

        public override SteamVR_Input_Sources ActiveSource
        {
            get
            {
                if (handSource != null) return handSource();
                var state = isEitherHand
                    ? (SteamVR_Input_Sources)((int)SpecificAction.GetActiveDevice(SteamVR_Input_Sources.LeftHand) +
                                              (int)SpecificAction.GetActiveDevice(SteamVR_Input_Sources.RightHand))
                    : SpecificAction.activeDevice;
                return state;
            }
        }

        public bool Clickable => clickable;

        /// <param name="handSource">If set, only this hand is read (e.g. the hand holding the tool).</param>
        public BooleanActionInput(SteamVR_Action_Boolean action, bool eitherHand = false, bool clickable = true, bool optional = false, Func<SteamVR_Input_Sources> handSource = null) : base(action, optional)
        {
            this.handSource = handSource;
            isEitherHand = eitherHand;
            this.clickable = clickable;
        }
    }
}
