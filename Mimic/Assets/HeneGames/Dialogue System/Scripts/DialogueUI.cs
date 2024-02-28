using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace HeneGames.DialogueSystem
{
    public class DialogueUI : MonoBehaviour
    {
        #region Singleton

        public static DialogueUI instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        private DialogueManager currentDialogueManager;
        private bool typing;
        private string currentMessage;
        private float startDialogueDelayTimer;
        public InputAction pcButton;
        public InputAction psButton;
        public InputAction xboxButton;
        private string disableController;

        [Header("References")]
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameObject dialogueWindow;
        [SerializeField] private GameObject interactionUI;

        [Header("Settings")]
        [SerializeField] private bool animateText = true;

        [Range(0.1f, 1f)]
        [SerializeField] private float textAnimationSpeed = 0.5f;

        [Header("Next sentence input")]
        public InputAction actionInput;

        public enum InputType { PC, PlayStation, Xbox }
        public InputType currentInput = InputType.PC; // Default to PC

        [SerializeField] private Sprite pcSprite;
        [SerializeField] private Sprite psSprite;
        [SerializeField] private Sprite xbSprite;

        [SerializeField ]private Image buttonSprite;

        private void Start()
        {
            //Hide dialogue and interaction UI at start
            dialogueWindow.SetActive(false);
            interactionUI.SetActive(false);
            disableController = "DualShock4GamepadHID";
            pcButton.Enable();
            psButton.Enable();
            xboxButton.Enable();
        }

        private void Update()
        {
            SetSentenceInputButton();

            //Delay timer
            if(startDialogueDelayTimer > 0f)
            {
                startDialogueDelayTimer -= Time.deltaTime;
            }

            //Next dialogue input
            if (actionInput.WasPerformedThisFrame())
            {
                if(startDialogueDelayTimer <= 0f)
                {
                    if (!typing)
                    {
                        NextSentence();
                    }
                    else
                    {
                        StopAllCoroutines();
                        typing = false;
                        messageText.text = currentMessage;
                        // Parse sprite tags after setting the message text
                        messageText.text = ParseSpriteTags(messageText.text);
                    }
                }
            }
        }

        void SetSentenceInputButton()
        {
            switch (currentInput)
            {
                case InputType.PC:
                    buttonSprite.sprite = pcSprite;
                    actionInput = pcButton;
                    break;
                case InputType.PlayStation:
                    buttonSprite.sprite = psSprite;
                    actionInput = psButton;
                    break;
                case InputType.Xbox:
                    buttonSprite.sprite = xbSprite;
                    actionInput = xboxButton;
                    break;
            }

        var gamepad = Gamepad.current;

            if (gamepad == null)
            {
                currentInput = InputType.PC;
            } 
            if (gamepad is DualShockGamepad) 
            {
                currentInput = InputType.PlayStation;
                disableController = "DualShock4GamepadHID";
            }
            if (gamepad is XInputController && gamepad.name == disableController)
            {
                currentInput = InputType.Xbox;
            }
            else if (gamepad is XInputController)
            {
                disableController = "XInputControllerWindows";
            }
        }

        public void NextSentence()
        {
            //Continue only if we have dialogue
            if (currentDialogueManager == null)
                return;

            //Tell the current dialogue manager to display the next sentence. This function also gives information if we are at the last sentence
            currentDialogueManager.NextSentence(out bool lastSentence);

            //If last sentence remove current dialogue manager
            if (lastSentence)
            {
                currentDialogueManager = null;
            }
        }

        public void StartDialogue(DialogueManager _dialogueManager)
        {
            //Delay timer
            startDialogueDelayTimer = 0.1f;

            //Store dialogue manager
            currentDialogueManager = _dialogueManager;

            //Start displaying dialogue
            currentDialogueManager.StartDialogue();
        }

        public void ShowSentence(DialogueCharacter _dialogueCharacter, string _message)
        {
            StopAllCoroutines(); // Stop any ongoing text animation coroutine

            dialogueWindow.SetActive(true);

            portrait.sprite = _dialogueCharacter.characterPhoto;
            nameText.text = _dialogueCharacter.characterName;
            currentMessage = _message;

            if (animateText)
            {
                StartCoroutine(WriteTextToTextmesh(_message, messageText));
            }
            else
            {
                // If text animation is disabled, set the message text instantly
                messageText.text = _message;
            }
        }

        public void ClearText()
        {
            dialogueWindow.SetActive(false);
        }

        public void ShowInteractionUI(bool _value)
        {
            interactionUI.SetActive(_value);
        }

        public bool IsProcessingDialogue()
        {
            if(currentDialogueManager != null)
            {
                return true;
            }

            return false;
        }

        IEnumerator WriteTextToTextmesh(string _text, TextMeshProUGUI _textMeshObject)
        {
            typing = true;

            _textMeshObject.text = "";
            char[] _letters = _text.ToCharArray();

            float _speed = 1f - textAnimationSpeed;

            foreach (char _letter in _letters)
            {
                _textMeshObject.text += _letter;

                if (_textMeshObject.text.Length == _letters.Length)
                {
                    typing = false;
                }

                // Parse sprite tags here
                _textMeshObject.text = ParseSpriteTags(_textMeshObject.text);

                yield return new WaitForSeconds(0.1f * _speed);
            }
        }

        private string ParseSpriteTags(string message)
        {
            string parsedMessage = message;

            // First sprite tag (e.g., "*")
            string spriteTag1 = "*";
            parsedMessage = parsedMessage.Replace(spriteTag1, "<sprite=\"" + currentDialogueManager.spriteAssetName + "\" index=0>");

            // Second sprite tag (e.g., "$")
            string spriteTag2 = "$";
            parsedMessage = parsedMessage.Replace(spriteTag2, "<sprite=\"" + currentDialogueManager.secondSpriteAssetName + "\" index=0>");

            return parsedMessage;
        }

    }
}