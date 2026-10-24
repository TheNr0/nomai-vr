using NomaiVR.Assets;
using NomaiVR.Hands;
using NomaiVR.Helpers;
using NomaiVR.UI;
using UnityEngine;
using Valve.VR;

namespace NomaiVR.Ship
{
    internal class ShipTools : NomaiVRModule<ShipTools.Behaviour, ShipTools.Behaviour.Patch>
    {
        protected override bool IsPersistent => false;
        protected override OWScene[] Scenes => SolarSystemScene;

        public class Behaviour : MonoBehaviour
        {
            private static Transform mapGridRenderer;
            private bool isShipToolsetActive;

            internal void Awake()
            {
                mapGridRenderer = FindObjectOfType<MapController>()._gridRenderer.transform;
            }

            // Only the Steam Frame bindings rely on the tools set for ship tools.
            // Other controllers keep their default set cockpit bindings, which the tools set would shadow on the off hand.
            internal static bool UsesShipToolset => ToolHelper.IsUsingAnyTool(ToolGroup.Ship)
                                                    && InputPrompts.Instance != null
                                                    && InputPrompts.Instance.Platform == InputPrompts.ActiveVRPlatform.SteamFrame;

            internal void Update()
            {
                var shouldBeActive = UsesShipToolset;
                if (shouldBeActive != isShipToolsetActive) SetShipToolsetActive(shouldBeActive);
            }

            internal void OnDestroy()
            {
                if (isShipToolsetActive) SetShipToolsetActive(false);
            }

            // While the ship scout or signalscope is active, the left d-pad gets the tool controls, whatever the dominant hand.
            // It has no default set bindings on the Steam Frame, unlike the right hand's buttons (interact, back).
            private void SetShipToolsetActive(bool active)
            {
                isShipToolsetActive = active;
                if (active)
                {
                    SteamVR_Actions.tools.Activate(SteamVR_Input_Sources.LeftHand, 1);
                }
                else
                {
                    SteamVR_Actions.tools.Deactivate(SteamVR_Input_Sources.LeftHand);
                    SteamVR_Actions.tools.Deactivate(SteamVR_Input_Sources.RightHand);
                }
                InputPrompts.Behaviour.InvalidateButtonImages();
            }

            public class Patch : NomaiVRPatch
            {
                public override void ApplyPatches()
                {
                    Postfix<ShipBody>(nameof(ShipBody.Start), nameof(ShipStart));
                    Prefix<ReferenceFrameTracker>(nameof(ReferenceFrameTracker.FindReferenceFrameInLineOfSight), nameof(PreFindFrame));
                    Postfix<ReferenceFrameTracker>(nameof(ReferenceFrameTracker.FindReferenceFrameInLineOfSight), nameof(PostFindFrame));
                    Prefix<ReferenceFrameTracker>(nameof(ReferenceFrameTracker.FindReferenceFrameInMapView), nameof(PreFindFrame));
                    Postfix<ReferenceFrameTracker>(nameof(ReferenceFrameTracker.FindReferenceFrameInMapView), nameof(PostFindFrame));
                    Empty<PlayerCameraController>(nameof(PlayerCameraController.OnEnterLandingView));
                    Empty<PlayerCameraController>(nameof(PlayerCameraController.OnExitLandingView));
                    Empty<PlayerCameraController>(nameof(PlayerCameraController.OnEnterShipComputer));
                    Empty<PlayerCameraController>(nameof(PlayerCameraController.OnExitShipComputer));
                    Prefix<ShipCockpitController>(nameof(ShipCockpitController.EnterLandingView), nameof(PreEnterLandingView));
                    Prefix<ShipCockpitController>(nameof(ShipCockpitController.ExitLandingView), nameof(PreExitLandingView));
                    Postfix<ShipCockpitController>(nameof(ShipCockpitController.ExitFlightConsole), nameof(PostExitFlightConsole));
                    Prefix<ShipCockpitUI>(nameof(ShipCockpitUI.Update), nameof(PreCockpitUIUpdate));
                    Postfix<ShipCockpitUI>(nameof(ShipCockpitUI.Update), nameof(PostCockpitUIUpdate));
                    Prefix(typeof(ReferenceFrameTracker).GetMethod(nameof(ReferenceFrameTracker.UntargetReferenceFrame), new[] { typeof(bool) }), nameof(PreUntargetFrame));
                }

                private static void PreCockpitUIUpdate(ShipCockpitUI __instance)
                {
                    __instance._shipSystemsCtrlr._usingLandingCam = __instance._shipSystemsCtrlr._landingCam.enabled;
                }

                private static void PostCockpitUIUpdate(ShipCockpitController ____shipSystemsCtrlr)
                {
                    ____shipSystemsCtrlr._usingLandingCam = false;
                }

                private static bool PreEnterLandingView(ShipCockpitController __instance)
                {
                    __instance._landingCam.enabled = true;
                    __instance._landingLight.SetOn(true);

                    __instance._shipAudioController.PlayLandingCamOn(__instance._landingCamComponent.isDamaged
                        ? AudioType.ShipCockpitLandingCamStatic_LP
                        : AudioType.ShipCockpitLandingCamAmbient_LP);

                    return false;
                }

                private static bool PreExitLandingView(ShipCockpitController __instance)
                {
                    __instance._landingCam.enabled = false;
                    __instance._landingLight.SetOn(false);
                    __instance._shipAudioController.PlayLandingCamOff();

                    return false;
                }

                private static void PostExitFlightConsole(ShipCockpitController __instance)
                {
                    __instance.ExitLandingView();
                }

                private static void ShipStart(ShipBody __instance)
                {
                    var cockpitUI = __instance.transform.Find("Module_Cockpit/Systems_Cockpit/ShipCockpitUI");
                    var cockpitTech = __instance.transform.Find("Module_Cockpit/Geo_Cockpit/Cockpit_Tech/Cockpit_Tech_Interior");

                    cockpitUI.Find("ProbeScreen/ProbeScreenPivot").gameObject.AddComponent<ShipProbeInteract>();
                    cockpitUI.Find("SignalScreen/SignalScreenPivot").gameObject.AddComponent<ShipSignalscopeInteract>();
                    cockpitTech.Find("LandingCamScreen").gameObject.AddComponent<ShipLandingCamInteract>();
                    
                    var autopilotButton = Instantiate(AssetLoader.AutopilotButtonPrefab, cockpitTech, false);
                    autopilotButton.AddComponent<AutopilotButtonInteract>();
                }

                private static Vector3 cameraPosition;
                private static Quaternion cameraRotation;

                private static void PreFindFrame(ReferenceFrameTracker __instance)
                {
                    if (__instance._isLandingView)
                    {
                        return;
                    }

                    var activeCam = __instance._activeCam.transform;
                    cameraPosition = activeCam.position;
                    cameraRotation = activeCam.rotation;

                    if (__instance._isMapView)
                    {
                        var up = mapGridRenderer.up;
                        activeCam.position = mapGridRenderer.position + up * 10000;
                        activeCam.rotation = Quaternion.LookRotation(up * -1);
                    }
                    else
                    {
                        activeCam.position = LaserPointer.Behaviour.Laser.position;
                        activeCam.rotation = LaserPointer.Behaviour.Laser.rotation;
                    }
                }

                private static bool PreUntargetFrame()
                {
                    return !LaserPointer.Behaviour.HasFocusedInteractible();
                }

                private static ReferenceFrame PostFindFrame(ReferenceFrame __result, ReferenceFrameTracker __instance)
                {
                    if (__instance._isLandingView) return __result;

                    var activeCam = __instance._activeCam.transform;
                    activeCam.position = cameraPosition;
                    activeCam.rotation = cameraRotation;

                    return LaserPointer.Behaviour.HasFocusedInteractible() ? __instance._currentReferenceFrame : __result;
                }
            }
        }
    }
}
