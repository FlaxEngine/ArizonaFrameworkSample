using System.Collections.Generic;
using System.Linq;
using FlaxEditor.Content.Settings;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Networking;

namespace Game
{
    /// <summary>
    /// Simple main menu script.
    /// </summary>
    public class MainMenu : Script
    {
        public UIControl AddressTextBox;
        public UIControl PortTextBox;
        public UIControl ConnectButton;
        public UIControl StartServerButton;
        public UIControl HostButton;
        public UIControl DisconnectButton;
        public UIControl ExitButton;
        public Dictionary<NetworkConnectionState, Actor> ConnectionPanels;

        private string Address
        {
            get => AddressTextBox.Get<TextBox>().Text;
            set => AddressTextBox.Get<TextBox>().Text = value;
        }

        private ushort Port
        {
            get => ushort.Parse(PortTextBox.Get<TextBox>().Text);
            set => PortTextBox.Get<TextBox>().Text = value.ToString();
        }

        private void UpdateUI()
        {
            var hasFocus = Engine.HasGameViewportFocus;
            var state = NetworkManager.State;
            foreach (var panel in ConnectionPanels.Values.Distinct())
            {
                panel.IsActive = ConnectionPanels.Any(x => x.Key == state && x.Value == panel);
            }

            if (hasFocus)
                Engine.FocusGameViewport();
        }

        private void SetupOptions()
        {
            // Setup network connection settings
            var settings = GameSettings.LoadAsset<NetworkSettings>();
            var networkSettings = (NetworkSettings)settings.Instance;
            networkSettings.Address = Address;
            networkSettings.Port = Port;
            settings.SetInstance(networkSettings);
        }

        private void OnConnect()
        {
            SetupOptions();
            NetworkManager.StartClient();
        }

        private void OnStartServer()
        {
            SetupOptions();
            NetworkManager.StartServer();
        }

        private void OnHost()
        {
            SetupOptions();
            NetworkManager.StartHost();
        }

        private void OnDisconnect()
        {
            NetworkManager.Stop();
        }

        private void OnExit()
        {
            Engine.RequestExit();
        }

        /// <inheritdoc/>
        public override void OnEnable()
        {
            // Use project settings defaults
            var networkSettings = GameSettings.Load<NetworkSettings>();
            Address = networkSettings.Address;
            Port = networkSettings.Port;

            // Link for events
            ((Button)ConnectButton.Control).Clicked += OnConnect;
            ((Button)StartServerButton.Control).Clicked += OnStartServer;
            ((Button)HostButton.Control).Clicked += OnHost;
            ((Button)DisconnectButton.Control).Clicked += OnDisconnect;
            ((Button)ExitButton.Control).Clicked += OnExit;
            NetworkManager.StateChanged += UpdateUI;

            UpdateUI();
            Engine.FocusGameViewport();

            Screen.CursorLock = CursorLockMode.None;
            Screen.CursorVisible = true;

            // When running in headless mode start server by default
            if (Engine.IsHeadless)
            {
                Debug.Log("Auto-start server in headless mode");
                OnStartServer();
            }
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            // Unlink from events
            ((Button)ConnectButton.Control).Clicked -= OnConnect;
            ((Button)StartServerButton.Control).Clicked -= OnStartServer;
            ((Button)HostButton.Control).Clicked -= OnHost;
            ((Button)DisconnectButton.Control).Clicked -= OnDisconnect;
            ((Button)ExitButton.Control).Clicked -= OnExit;
            NetworkManager.StateChanged -= UpdateUI;
        }
    }
}
