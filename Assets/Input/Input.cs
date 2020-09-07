// GENERATED AUTOMATICALLY FROM 'Assets/Input/Input.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Input : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Input()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Input"",
    ""maps"": [
        {
            ""name"": ""BuildingMode"",
            ""id"": ""1a0dd22e-9eca-4c41-948e-22b89fd8ee06"",
            ""actions"": [
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""e3297c48-6ead-4b05-901f-d4f4df5d24e3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""75d4dd25-c446-44af-a0de-0e9f9a19893f"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""ViewMode"",
            ""id"": ""ebd91f5c-ac3a-4c56-b1b8-fda42d8666f9"",
            ""actions"": [
                {
                    ""name"": ""CallMenu"",
                    ""type"": ""Button"",
                    ""id"": ""36520d05-7459-46e2-bd93-82a820b2119c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""660798f8-813f-4102-a8b5-45a6175b8c52"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CallMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MovementMode"",
            ""id"": ""f805b270-e697-4412-aac2-4bc5dcaf4032"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""b53cc78d-2be0-42ae-9ee0-d2c0a703d9b0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""f1b6287e-e34e-4dd6-aa04-6b9164a7b409"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""f91dc548-80ab-4b2b-9858-2edcf6d42143"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""d1c73981-f308-4d4c-a0d9-5f3c571c92eb"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d10be780-2247-402e-98d0-f5ca48ce99eb"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c593035b-7217-44ec-9c65-cf889e21c7b4"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""38a20659-ef9c-42f0-96e4-235f3b4287bb"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""76f81ddb-738a-4460-be18-768519194ef7"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""26875de7-e8e2-4367-8e12-cc4644e6a877"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""e23dc0f2-317c-4a67-9852-52dd01a6ba4c"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""1970812c-4e34-41ac-a483-a562145075cb"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""42740d7e-928c-4608-8f96-b35c9af08bbc"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MenuMode"",
            ""id"": ""61faa959-c454-4bb9-8633-1ee35ecc8824"",
            ""actions"": [
                {
                    ""name"": ""CloseMenu"",
                    ""type"": ""Button"",
                    ""id"": ""ab8d97a8-02b6-4800-84e8-04176f159d02"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0c617744-eebc-4bda-b990-ae933238aed0"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CloseMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // BuildingMode
        m_BuildingMode = asset.FindActionMap("BuildingMode", throwIfNotFound: true);
        m_BuildingMode_Quit = m_BuildingMode.FindAction("Quit", throwIfNotFound: true);
        // ViewMode
        m_ViewMode = asset.FindActionMap("ViewMode", throwIfNotFound: true);
        m_ViewMode_CallMenu = m_ViewMode.FindAction("CallMenu", throwIfNotFound: true);
        // MovementMode
        m_MovementMode = asset.FindActionMap("MovementMode", throwIfNotFound: true);
        m_MovementMode_Move = m_MovementMode.FindAction("Move", throwIfNotFound: true);
        m_MovementMode_Rotate = m_MovementMode.FindAction("Rotate", throwIfNotFound: true);
        m_MovementMode_Zoom = m_MovementMode.FindAction("Zoom", throwIfNotFound: true);
        // MenuMode
        m_MenuMode = asset.FindActionMap("MenuMode", throwIfNotFound: true);
        m_MenuMode_CloseMenu = m_MenuMode.FindAction("CloseMenu", throwIfNotFound: true);
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

    // BuildingMode
    private readonly InputActionMap m_BuildingMode;
    private IBuildingModeActions m_BuildingModeActionsCallbackInterface;
    private readonly InputAction m_BuildingMode_Quit;
    public struct BuildingModeActions
    {
        private @Input m_Wrapper;
        public BuildingModeActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @Quit => m_Wrapper.m_BuildingMode_Quit;
        public InputActionMap Get() { return m_Wrapper.m_BuildingMode; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BuildingModeActions set) { return set.Get(); }
        public void SetCallbacks(IBuildingModeActions instance)
        {
            if (m_Wrapper.m_BuildingModeActionsCallbackInterface != null)
            {
                @Quit.started -= m_Wrapper.m_BuildingModeActionsCallbackInterface.OnQuit;
                @Quit.performed -= m_Wrapper.m_BuildingModeActionsCallbackInterface.OnQuit;
                @Quit.canceled -= m_Wrapper.m_BuildingModeActionsCallbackInterface.OnQuit;
            }
            m_Wrapper.m_BuildingModeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Quit.started += instance.OnQuit;
                @Quit.performed += instance.OnQuit;
                @Quit.canceled += instance.OnQuit;
            }
        }
    }
    public BuildingModeActions @BuildingMode => new BuildingModeActions(this);

    // ViewMode
    private readonly InputActionMap m_ViewMode;
    private IViewModeActions m_ViewModeActionsCallbackInterface;
    private readonly InputAction m_ViewMode_CallMenu;
    public struct ViewModeActions
    {
        private @Input m_Wrapper;
        public ViewModeActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @CallMenu => m_Wrapper.m_ViewMode_CallMenu;
        public InputActionMap Get() { return m_Wrapper.m_ViewMode; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ViewModeActions set) { return set.Get(); }
        public void SetCallbacks(IViewModeActions instance)
        {
            if (m_Wrapper.m_ViewModeActionsCallbackInterface != null)
            {
                @CallMenu.started -= m_Wrapper.m_ViewModeActionsCallbackInterface.OnCallMenu;
                @CallMenu.performed -= m_Wrapper.m_ViewModeActionsCallbackInterface.OnCallMenu;
                @CallMenu.canceled -= m_Wrapper.m_ViewModeActionsCallbackInterface.OnCallMenu;
            }
            m_Wrapper.m_ViewModeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CallMenu.started += instance.OnCallMenu;
                @CallMenu.performed += instance.OnCallMenu;
                @CallMenu.canceled += instance.OnCallMenu;
            }
        }
    }
    public ViewModeActions @ViewMode => new ViewModeActions(this);

    // MovementMode
    private readonly InputActionMap m_MovementMode;
    private IMovementModeActions m_MovementModeActionsCallbackInterface;
    private readonly InputAction m_MovementMode_Move;
    private readonly InputAction m_MovementMode_Rotate;
    private readonly InputAction m_MovementMode_Zoom;
    public struct MovementModeActions
    {
        private @Input m_Wrapper;
        public MovementModeActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_MovementMode_Move;
        public InputAction @Rotate => m_Wrapper.m_MovementMode_Rotate;
        public InputAction @Zoom => m_Wrapper.m_MovementMode_Zoom;
        public InputActionMap Get() { return m_Wrapper.m_MovementMode; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovementModeActions set) { return set.Get(); }
        public void SetCallbacks(IMovementModeActions instance)
        {
            if (m_Wrapper.m_MovementModeActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_MovementModeActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_MovementModeActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_MovementModeActionsCallbackInterface.OnMove;
                @Rotate.started -= m_Wrapper.m_MovementModeActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_MovementModeActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_MovementModeActionsCallbackInterface.OnRotate;
                @Zoom.started -= m_Wrapper.m_MovementModeActionsCallbackInterface.OnZoom;
                @Zoom.performed -= m_Wrapper.m_MovementModeActionsCallbackInterface.OnZoom;
                @Zoom.canceled -= m_Wrapper.m_MovementModeActionsCallbackInterface.OnZoom;
            }
            m_Wrapper.m_MovementModeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Zoom.started += instance.OnZoom;
                @Zoom.performed += instance.OnZoom;
                @Zoom.canceled += instance.OnZoom;
            }
        }
    }
    public MovementModeActions @MovementMode => new MovementModeActions(this);

    // MenuMode
    private readonly InputActionMap m_MenuMode;
    private IMenuModeActions m_MenuModeActionsCallbackInterface;
    private readonly InputAction m_MenuMode_CloseMenu;
    public struct MenuModeActions
    {
        private @Input m_Wrapper;
        public MenuModeActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @CloseMenu => m_Wrapper.m_MenuMode_CloseMenu;
        public InputActionMap Get() { return m_Wrapper.m_MenuMode; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuModeActions set) { return set.Get(); }
        public void SetCallbacks(IMenuModeActions instance)
        {
            if (m_Wrapper.m_MenuModeActionsCallbackInterface != null)
            {
                @CloseMenu.started -= m_Wrapper.m_MenuModeActionsCallbackInterface.OnCloseMenu;
                @CloseMenu.performed -= m_Wrapper.m_MenuModeActionsCallbackInterface.OnCloseMenu;
                @CloseMenu.canceled -= m_Wrapper.m_MenuModeActionsCallbackInterface.OnCloseMenu;
            }
            m_Wrapper.m_MenuModeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CloseMenu.started += instance.OnCloseMenu;
                @CloseMenu.performed += instance.OnCloseMenu;
                @CloseMenu.canceled += instance.OnCloseMenu;
            }
        }
    }
    public MenuModeActions @MenuMode => new MenuModeActions(this);
    public interface IBuildingModeActions
    {
        void OnQuit(InputAction.CallbackContext context);
    }
    public interface IViewModeActions
    {
        void OnCallMenu(InputAction.CallbackContext context);
    }
    public interface IMovementModeActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
    }
    public interface IMenuModeActions
    {
        void OnCloseMenu(InputAction.CallbackContext context);
    }
}
