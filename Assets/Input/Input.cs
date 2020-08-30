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
            ""name"": ""BuilderMode"",
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
            ""name"": ""ViewerMode"",
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
        }
    ],
    ""controlSchemes"": []
}");
        // BuilderMode
        m_BuilderMode = asset.FindActionMap("BuilderMode", throwIfNotFound: true);
        m_BuilderMode_Quit = m_BuilderMode.FindAction("Quit", throwIfNotFound: true);
        // ViewerMode
        m_ViewerMode = asset.FindActionMap("ViewerMode", throwIfNotFound: true);
        m_ViewerMode_CallMenu = m_ViewerMode.FindAction("CallMenu", throwIfNotFound: true);
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

    // BuilderMode
    private readonly InputActionMap m_BuilderMode;
    private IBuilderModeActions m_BuilderModeActionsCallbackInterface;
    private readonly InputAction m_BuilderMode_Quit;
    public struct BuilderModeActions
    {
        private @Input m_Wrapper;
        public BuilderModeActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @Quit => m_Wrapper.m_BuilderMode_Quit;
        public InputActionMap Get() { return m_Wrapper.m_BuilderMode; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BuilderModeActions set) { return set.Get(); }
        public void SetCallbacks(IBuilderModeActions instance)
        {
            if (m_Wrapper.m_BuilderModeActionsCallbackInterface != null)
            {
                @Quit.started -= m_Wrapper.m_BuilderModeActionsCallbackInterface.OnQuit;
                @Quit.performed -= m_Wrapper.m_BuilderModeActionsCallbackInterface.OnQuit;
                @Quit.canceled -= m_Wrapper.m_BuilderModeActionsCallbackInterface.OnQuit;
            }
            m_Wrapper.m_BuilderModeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Quit.started += instance.OnQuit;
                @Quit.performed += instance.OnQuit;
                @Quit.canceled += instance.OnQuit;
            }
        }
    }
    public BuilderModeActions @BuilderMode => new BuilderModeActions(this);

    // ViewerMode
    private readonly InputActionMap m_ViewerMode;
    private IViewerModeActions m_ViewerModeActionsCallbackInterface;
    private readonly InputAction m_ViewerMode_CallMenu;
    public struct ViewerModeActions
    {
        private @Input m_Wrapper;
        public ViewerModeActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @CallMenu => m_Wrapper.m_ViewerMode_CallMenu;
        public InputActionMap Get() { return m_Wrapper.m_ViewerMode; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ViewerModeActions set) { return set.Get(); }
        public void SetCallbacks(IViewerModeActions instance)
        {
            if (m_Wrapper.m_ViewerModeActionsCallbackInterface != null)
            {
                @CallMenu.started -= m_Wrapper.m_ViewerModeActionsCallbackInterface.OnCallMenu;
                @CallMenu.performed -= m_Wrapper.m_ViewerModeActionsCallbackInterface.OnCallMenu;
                @CallMenu.canceled -= m_Wrapper.m_ViewerModeActionsCallbackInterface.OnCallMenu;
            }
            m_Wrapper.m_ViewerModeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CallMenu.started += instance.OnCallMenu;
                @CallMenu.performed += instance.OnCallMenu;
                @CallMenu.canceled += instance.OnCallMenu;
            }
        }
    }
    public ViewerModeActions @ViewerMode => new ViewerModeActions(this);
    public interface IBuilderModeActions
    {
        void OnQuit(InputAction.CallbackContext context);
    }
    public interface IViewerModeActions
    {
        void OnCallMenu(InputAction.CallbackContext context);
    }
}
