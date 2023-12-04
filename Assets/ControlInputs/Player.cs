//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/ControlInputs/Player.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Player : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Player()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player"",
    ""maps"": [
        {
            ""name"": ""Ingame"",
            ""id"": ""28bfea44-60d3-470d-b5f0-007d0e977fdd"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""a46b89bd-e94a-4159-ba21-7808a6bf5b72"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""18581a48-3a05-4321-9204-82b975c3bd26"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Wield"",
                    ""type"": ""Value"",
                    ""id"": ""e4d2d8d3-84af-4cb1-a180-44fba93fd3c5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ShipRotation"",
                    ""type"": ""Value"",
                    ""id"": ""bb994fc3-b348-4c37-b23a-c7679b0481f2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MainShipPropulsion"",
                    ""type"": ""Value"",
                    ""id"": ""74e3b357-7825-4ef4-923a-a269b779c996"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TakeOffPropulsion"",
                    ""type"": ""Button"",
                    ""id"": ""34c8d049-a586-471a-ba1b-6cfa90d39652"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ShipYRightRotation"",
                    ""type"": ""Button"",
                    ""id"": ""49bb6075-0ce8-42b0-9abc-c2b4ea32df22"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ShipYLeftRotation"",
                    ""type"": ""Button"",
                    ""id"": ""78317723-9437-4a25-b03c-de2b5c953eb8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""BrakePropulsion"",
                    ""type"": ""Value"",
                    ""id"": ""130c0c6d-6b33-4e79-84d5-96b215c24f92"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LandingPropulsion"",
                    ""type"": ""Button"",
                    ""id"": ""62f7acc2-b1ae-491e-80b5-2c9c1d6706e0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Camera"",
                    ""type"": ""Value"",
                    ""id"": ""85edd53c-9bab-444d-9850-5517bd60136b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""005a2aca-00e2-42f5-9989-2dfe84edd06c"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""8552d042-5770-4e76-9f3d-a60a244f4329"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c46a2618-aa45-4cc1-a180-d3cac1eacda6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""019e9a0a-8f35-4488-8bc6-f0afef2b4bd5"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f7e4c121-1e33-44d2-af59-195f7eef4eec"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""dd51ec2b-dc79-4036-887a-1a9e4832449d"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""35a4f583-0996-4e5e-8713-da6fb8b0c8f6"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""db0b8f65-958c-4d78-84f1-38e2f9c5509c"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eba1273a-55b7-4255-ab14-aec74a2bf00f"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Wield"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e99e1c78-5a1c-4a43-a88a-303d1d53fcc4"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TakeOffPropulsion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b863e9c8-bcfb-48aa-8ddf-80bf5b8517a8"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TakeOffPropulsion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb85082a-b624-4987-bd69-32ad0387ae92"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipYRightRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""15864a15-94b6-4f8e-a685-540fbc5c5c28"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipYRightRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""560ba62f-b303-4c1a-9f3a-2ad2e75a8332"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipYLeftRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e8308e41-14fc-4486-abeb-6b26951dff61"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipYLeftRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""728b98c7-490f-4316-ad78-c3cb5877b1a0"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""2c49afdb-305c-445d-9e77-e35d8a08dac4"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipRotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6392f7fb-8c42-4cda-8b78-ff2e4d21592d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""104856b3-9bc6-496c-8fa7-c1d6c15c6670"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""853acdba-ed32-44bd-a686-8125960c3f1e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""da52e4d4-c44d-4a23-9e77-a0ef5a1c9f13"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShipRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a58d76af-e164-4639-89a2-c071abd10250"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MainShipPropulsion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1706637e-9966-4491-a411-6a535805fbe7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MainShipPropulsion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c2bf50d7-4a97-44e0-88a1-636418d8aa85"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BrakePropulsion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47a018bd-51d7-47fa-9bb2-a8e9466570fc"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BrakePropulsion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6eba6166-b05a-470b-a132-f1caf4a07a1a"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LandingPropulsion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fbc45444-348a-4002-bc77-449419c49693"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LandingPropulsion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""98c0fb4e-468f-4bc1-bd7e-b972738636d6"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82fb3a4c-235c-4fb4-b69f-ba6ea8da6352"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""28e77ffd-d288-4e8d-b2fb-0db07a5ef127"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""3520ada9-445f-479a-a0c1-a9620e3add49"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""fb3858b2-fbc1-41e9-bea7-837928aad175"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Ingame
        m_Ingame = asset.FindActionMap("Ingame", throwIfNotFound: true);
        m_Ingame_Movement = m_Ingame.FindAction("Movement", throwIfNotFound: true);
        m_Ingame_Jump = m_Ingame.FindAction("Jump", throwIfNotFound: true);
        m_Ingame_Wield = m_Ingame.FindAction("Wield", throwIfNotFound: true);
        m_Ingame_ShipRotation = m_Ingame.FindAction("ShipRotation", throwIfNotFound: true);
        m_Ingame_MainShipPropulsion = m_Ingame.FindAction("MainShipPropulsion", throwIfNotFound: true);
        m_Ingame_TakeOffPropulsion = m_Ingame.FindAction("TakeOffPropulsion", throwIfNotFound: true);
        m_Ingame_ShipYRightRotation = m_Ingame.FindAction("ShipYRightRotation", throwIfNotFound: true);
        m_Ingame_ShipYLeftRotation = m_Ingame.FindAction("ShipYLeftRotation", throwIfNotFound: true);
        m_Ingame_BrakePropulsion = m_Ingame.FindAction("BrakePropulsion", throwIfNotFound: true);
        m_Ingame_LandingPropulsion = m_Ingame.FindAction("LandingPropulsion", throwIfNotFound: true);
        m_Ingame_Camera = m_Ingame.FindAction("Camera", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Newaction = m_Menu.FindAction("New action", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Ingame
    private readonly InputActionMap m_Ingame;
    private IIngameActions m_IngameActionsCallbackInterface;
    private readonly InputAction m_Ingame_Movement;
    private readonly InputAction m_Ingame_Jump;
    private readonly InputAction m_Ingame_Wield;
    private readonly InputAction m_Ingame_ShipRotation;
    private readonly InputAction m_Ingame_MainShipPropulsion;
    private readonly InputAction m_Ingame_TakeOffPropulsion;
    private readonly InputAction m_Ingame_ShipYRightRotation;
    private readonly InputAction m_Ingame_ShipYLeftRotation;
    private readonly InputAction m_Ingame_BrakePropulsion;
    private readonly InputAction m_Ingame_LandingPropulsion;
    private readonly InputAction m_Ingame_Camera;
    public struct IngameActions
    {
        private @Player m_Wrapper;
        public IngameActions(@Player wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Ingame_Movement;
        public InputAction @Jump => m_Wrapper.m_Ingame_Jump;
        public InputAction @Wield => m_Wrapper.m_Ingame_Wield;
        public InputAction @ShipRotation => m_Wrapper.m_Ingame_ShipRotation;
        public InputAction @MainShipPropulsion => m_Wrapper.m_Ingame_MainShipPropulsion;
        public InputAction @TakeOffPropulsion => m_Wrapper.m_Ingame_TakeOffPropulsion;
        public InputAction @ShipYRightRotation => m_Wrapper.m_Ingame_ShipYRightRotation;
        public InputAction @ShipYLeftRotation => m_Wrapper.m_Ingame_ShipYLeftRotation;
        public InputAction @BrakePropulsion => m_Wrapper.m_Ingame_BrakePropulsion;
        public InputAction @LandingPropulsion => m_Wrapper.m_Ingame_LandingPropulsion;
        public InputAction @Camera => m_Wrapper.m_Ingame_Camera;
        public InputActionMap Get() { return m_Wrapper.m_Ingame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(IngameActions set) { return set.Get(); }
        public void SetCallbacks(IIngameActions instance)
        {
            if (m_Wrapper.m_IngameActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnJump;
                @Wield.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnWield;
                @Wield.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnWield;
                @Wield.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnWield;
                @ShipRotation.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnShipRotation;
                @ShipRotation.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnShipRotation;
                @ShipRotation.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnShipRotation;
                @MainShipPropulsion.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnMainShipPropulsion;
                @MainShipPropulsion.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnMainShipPropulsion;
                @MainShipPropulsion.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnMainShipPropulsion;
                @TakeOffPropulsion.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnTakeOffPropulsion;
                @TakeOffPropulsion.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnTakeOffPropulsion;
                @TakeOffPropulsion.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnTakeOffPropulsion;
                @ShipYRightRotation.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnShipYRightRotation;
                @ShipYRightRotation.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnShipYRightRotation;
                @ShipYRightRotation.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnShipYRightRotation;
                @ShipYLeftRotation.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnShipYLeftRotation;
                @ShipYLeftRotation.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnShipYLeftRotation;
                @ShipYLeftRotation.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnShipYLeftRotation;
                @BrakePropulsion.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnBrakePropulsion;
                @BrakePropulsion.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnBrakePropulsion;
                @BrakePropulsion.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnBrakePropulsion;
                @LandingPropulsion.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnLandingPropulsion;
                @LandingPropulsion.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnLandingPropulsion;
                @LandingPropulsion.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnLandingPropulsion;
                @Camera.started -= m_Wrapper.m_IngameActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_IngameActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_IngameActionsCallbackInterface.OnCamera;
            }
            m_Wrapper.m_IngameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Wield.started += instance.OnWield;
                @Wield.performed += instance.OnWield;
                @Wield.canceled += instance.OnWield;
                @ShipRotation.started += instance.OnShipRotation;
                @ShipRotation.performed += instance.OnShipRotation;
                @ShipRotation.canceled += instance.OnShipRotation;
                @MainShipPropulsion.started += instance.OnMainShipPropulsion;
                @MainShipPropulsion.performed += instance.OnMainShipPropulsion;
                @MainShipPropulsion.canceled += instance.OnMainShipPropulsion;
                @TakeOffPropulsion.started += instance.OnTakeOffPropulsion;
                @TakeOffPropulsion.performed += instance.OnTakeOffPropulsion;
                @TakeOffPropulsion.canceled += instance.OnTakeOffPropulsion;
                @ShipYRightRotation.started += instance.OnShipYRightRotation;
                @ShipYRightRotation.performed += instance.OnShipYRightRotation;
                @ShipYRightRotation.canceled += instance.OnShipYRightRotation;
                @ShipYLeftRotation.started += instance.OnShipYLeftRotation;
                @ShipYLeftRotation.performed += instance.OnShipYLeftRotation;
                @ShipYLeftRotation.canceled += instance.OnShipYLeftRotation;
                @BrakePropulsion.started += instance.OnBrakePropulsion;
                @BrakePropulsion.performed += instance.OnBrakePropulsion;
                @BrakePropulsion.canceled += instance.OnBrakePropulsion;
                @LandingPropulsion.started += instance.OnLandingPropulsion;
                @LandingPropulsion.performed += instance.OnLandingPropulsion;
                @LandingPropulsion.canceled += instance.OnLandingPropulsion;
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
            }
        }
    }
    public IngameActions @Ingame => new IngameActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Newaction;
    public struct MenuActions
    {
        private @Player m_Wrapper;
        public MenuActions(@Player wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Menu_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);
    public interface IIngameActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnWield(InputAction.CallbackContext context);
        void OnShipRotation(InputAction.CallbackContext context);
        void OnMainShipPropulsion(InputAction.CallbackContext context);
        void OnTakeOffPropulsion(InputAction.CallbackContext context);
        void OnShipYRightRotation(InputAction.CallbackContext context);
        void OnShipYLeftRotation(InputAction.CallbackContext context);
        void OnBrakePropulsion(InputAction.CallbackContext context);
        void OnLandingPropulsion(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
