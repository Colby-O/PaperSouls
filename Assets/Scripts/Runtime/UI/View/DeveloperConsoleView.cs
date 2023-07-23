using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.Console;

namespace PaperSouls.Runtime.UI.View
{
    [RequireComponent(typeof(PlayerInput))]
    internal sealed class DeveloperConsoleView : View
    {
        [Header("Console Settings")]
        [SerializeField] private List<ConsoleCommand> _commands;
        [SerializeField] private Color _defaultColor = Color.white;

        [Header("References")]
        [SerializeField] private TMP_InputField _commandInput;
        [SerializeField] private TMP_InputField _textAera;
        [SerializeField] private Button _submitButton;
        [SerializeField] private ScrollRect _scrollRect;

        [SerializeField] private PlayerInput _consoleInput;

        private InputAction _nextCommandAction;
        private InputAction _previousCommandAction;
        private InputAction _defaultCommand;

        private List<string> _commandHistory;
        private int _historySlot = 0;

        private static DeveloperConsoleView _instance;
        private DeveloperConsole _console;


        /// <summary>
        /// Converts a float color channel value from 0-1 to byte.
        /// </summary>
        private byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        /// <summary>
        /// Converts a Unity Color to a Hex value.
        /// </summary>
        private string ToRGBHex(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
        }

        /// <summary>
        /// Scroll console view to bottom at the end of the frame
        /// </summary>
        private IEnumerator ScrollToBottom()
        {
            // we need to wait for the current frame to end and all canvas activity
            // to finish before we scroll the view
            yield return new WaitForEndOfFrame();
            _scrollRect.verticalNormalizedPosition = 0;
        }

        /// <summary>
        /// Moves input cursor to the end at the end of the frame
        /// </summary>
        private IEnumerator InputCursorToEnd()
        {
            yield return new WaitForEndOfFrame();
            _commandInput.MoveTextEnd(false);
        }

        /// <summary>
        /// Prints a command to the Console Window.
        /// </summary>
        private void PrintToCommandWindow(string msg, Color? color = null)
        {
            _textAera.text += $" <color={ ToRGBHex(color ?? _defaultColor) }>{ msg }</color>\n";
            StartCoroutine(ScrollToBottom());
        }

        /// <summary>
        /// Display's a help message to the console
        /// </summary>
        private void Help()
        {
            string res = string.Empty;

            foreach (ConsoleCommand command in _commands)
            {
                res += " " + command.Description + "\n";
            }

            ConsoleResponse msg = new(res, ResponseType.Help);

            PrintToCommandWindow(msg.Message, msg.MessageColor);
        }

        /// <summary>
        /// Resets the input field in the console
        /// </summary>
        private void ResetInputField()
        {
            _commandInput.text = "";
            _historySlot = 0;
            _commandInput.ActivateInputField();
        }

        /// <summary>
        /// Clears all command history
        /// </summary>
        private void ClearCommandHistory()
        {
            _commandHistory = new();
            _historySlot = 0;
        }

        /// <summary>
        /// Process an inputed command.
        /// </summary>
        private void ProcessCommand(string val)
        {
            ConsoleResponse msg = _console.ProcessCommand(val);
            PrintToCommandWindow(val);
            if (_commandHistory.Count == 0 || !_commandHistory[^1].Equals(val, System.StringComparison.OrdinalIgnoreCase))
            {
                _commandHistory.Add(val);
            }

            if (msg.Type == ResponseType.Help) Help();
            else if (msg.Type == ResponseType.Clear) _textAera.text = string.Empty;
            else if (msg.Type == ResponseType.ClearHistory) ClearCommandHistory();
            else if (msg.Type != ResponseType.None && msg != null) PrintToCommandWindow(msg.Message, msg.MessageColor);
            ResetInputField();
        }

        /// <summary>
        /// Callback for the Submit button on the developer console.
        /// </summary>
        private void Submit() => ProcessCommand(_commandInput.text);

        public override void Init()
        {
            _commands = Resources.LoadAll<ConsoleCommand>("").ToList();
            ResetInputField();
            _console = new DeveloperConsole(_commands);
        }

        public override void Show()
        {
            base.Show();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            ResetInputField();
        }

        /// <summary>
        /// Puts the next commands in the history in the input box
        /// </summary>
        private void NextCommand(InputAction.CallbackContext e)
        {
            if (_historySlot < _commandHistory.Count) _historySlot++;
            if (_historySlot > _commandHistory.Count || _historySlot < 1) return;
            _commandInput.text = _commandHistory[^_historySlot];
            StartCoroutine(InputCursorToEnd());
        }

        /// <summary>
        /// Puts the previous commands in the history in the input box
        /// </summary>
        private void PreviousCommand(InputAction.CallbackContext e)
        {
            if (_historySlot > 0) _historySlot--;
            if (_historySlot < 1 || _historySlot > _commandHistory.Count)
            {
                _commandInput.text = "";
                return;
            }
            _commandInput.text = _commandHistory[^_historySlot];
            StartCoroutine(InputCursorToEnd());
        }

        private void BackToDefaultCommand(InputAction.CallbackContext e) => ResetInputField();

        private void AddListeners()
        {
            _commandInput.onSubmit.AddListener(ProcessCommand);
            _submitButton.onClick.AddListener(Submit);
        }

        private void RemoveListeners()
        {
            _commandInput.onSubmit.RemoveListener(ProcessCommand);
            _submitButton.onClick.RemoveListener(Submit);
        }

        private void OnEnable()
        {
            _nextCommandAction.performed += NextCommand;
            _previousCommandAction.performed += PreviousCommand;
            _defaultCommand.performed += BackToDefaultCommand;
        }

        private void OnDisable()
        {
            if (_nextCommandAction != null) _nextCommandAction.performed -= NextCommand;
            if (_previousCommandAction != null) _previousCommandAction.performed -= PreviousCommand;
            if (_defaultCommand != null) _defaultCommand.performed -= BackToDefaultCommand;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void Awake()
        {
            if (_instance)
            {
                Destroy(this.gameObject);
                return;
            }

            ClearCommandHistory();

            _nextCommandAction = _consoleInput.actions["HistoryNext"];
            _previousCommandAction = _consoleInput.actions["HistoryPrevious"];
            _defaultCommand = _consoleInput.actions["DefaultCommand"];

            AddListeners();

            _instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
